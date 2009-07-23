// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// The core generation engine. Takes in a model and transforms it into a table of all possible values that
    /// need to be covered or excluded. Uses that table to create variations and transforms that into the public
    /// <see cref="Variation"/>
    /// </summary>
    internal static class VariationGenerator
    {
        /// <summary>
        /// Entry point to VariationGenerator
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="order">The order of the parameters (pairwise, 3-wise, etc)</param>
        /// <param name="seed">Random seed to use</param>
        /// <returns>Generated Variations</returns>
        public static IEnumerable<Variation> GenerateVariations(Model model, int order, int seed)
        {
            // calculate the number variations to exhaustively test the model, 
            // useful to determine if something is wrong during generation
            int maxVariations = model.Parameters.Aggregate(1, (total, next) => total * next.Count);
            var variationIndices = GenerateVariationIndices(Prepare(model, order),model.Parameters.Count, seed, maxVariations);

            return from v in variationIndices
                   select IndicesToVariation(model, v);
        }

        // generate all the values to cover or exclude
        private static ParameterInteractionTable Prepare(Model model, int order)
        {
            return new ParameterInteractionTable(model, order);
        }

        // this is the actual generation function
        // returns a list of indices that allow lookup of the actual value in the model
        private static IList<int[]> GenerateVariationIndices(ParameterInteractionTable interactions, int variationSize, int seed, int maxVariations)
        {
            Random random = new Random(seed);
            List<int[]> variations = new List<int[]>();

            // while there a uncovered values
            while (!interactions.IsCovered())
            {
                int[] candidate = new int[variationSize];
                for (int i = 0; i < candidate.Length; i++)
                {
                    // -1 indicates an empty slot
                    candidate[i] = -1;
                }

                // while there are empty slots
                while (candidate.Any((i) => i == -1))
                {
                    // if all the slots are empty
                    if (candidate.All((i) => i == -1))
                    {
                        // then pick the first uncovered combination from the most uncovered parameter interaction
                        int mostUncovered =
                            interactions.Interactions.Max((i) => i.GetUncoveredCombinationsCount());

                        var interaction = interactions.Interactions.First((i) => i.GetUncoveredCombinationsCount() == mostUncovered);
                        var combination = interaction.Combinations.First((c) => c.State == ValueCombinationState.Uncovered);

                        foreach (var valuePair in combination.ParameterToVaueMap)
                        {
                            candidate[valuePair.Key] = valuePair.Value;
                        }

                        combination.State = ValueCombinationState.Covered;
                    }
                    else
                    {
                        // find interactions that aren't covered by the current candidate variation
                        var incompletelyCoveredInteractions =
                            from interaction in interactions.Interactions
                            where interaction.Parameters.Any((i) => candidate[i] == -1)
                            select interaction;

                        // find values that can be added to the current candidate
                        var compatibleValues =
                            from interaction in incompletelyCoveredInteractions
                            from combination in interaction.Combinations
                            where IsCompatibleValue(combination, candidate)
                            select combination;

                        // get the uncovered values
                        var uncoveredValues = compatibleValues.Where((v) => v.State == ValueCombinationState.Uncovered).ToList();

                        // calculate what the candidate will look like if add an uncovered value
                        var proposedCandidates =
                            from value in uncoveredValues
                            select new
                            {
                                Value = value,
                                Candidate = CreateProposedCandidate(value, candidate)
                            };
                        
                        // if any of the proposed candidates isn't exclude
                        if (proposedCandidates.Any((a) => !IsExcluded(interactions.ExcludedCombinations(), a.Candidate)))
                        {
                            // find the value that will cover the most combinations
                            int maxCovered = proposedCandidates.Max(
                                (a) => uncoveredValues.Count(
                                    (v) => IsCovered(v, a.Candidate) &&
                                        !IsExcluded(interactions.ExcludedCombinations(), a.Candidate)));

                            ValueCombination proposedValue = proposedCandidates.First(
                                (a) => uncoveredValues.Count(
                                    (v) => IsCovered(v, a.Candidate) &&
                                        !IsExcluded(interactions.ExcludedCombinations(), a.Candidate)) == maxCovered).Value;

                            // add this value to candidate and mark all values as such

                            foreach (var valuePair in proposedValue.ParameterToVaueMap)
                            {
                                candidate[valuePair.Key] = valuePair.Value;
                            }

                            // get the newly covered values so they can be marked
                            var newlyCoveredValue = uncoveredValues.Where((v) => IsCovered(v, candidate)).ToList();

                            foreach(var value in newlyCoveredValue)
                            {
                                value.State = ValueCombinationState.Covered;
                            }
                        }
                        else
                        {
                            // no uncovered values can be added with violating a constraint, add a random covered value
                            int count = compatibleValues.Count();
                            int attempts = 0;
                            ValueCombination value;
                            int[] proposedCandidate;
                            do
                            {
                                value = compatibleValues.ElementAt(random.Next(count - 1));
                                proposedCandidate = CreateProposedCandidate(value, candidate);

                                // this is a heuristic, since we're pulling random values just going to count probably
                                // means we've attempted duplicates, going to 2 * count means we've probably tried
                                // everything at least once
                                if (attempts > count * 2)
                                {
                                    throw new InternalVariationGenerationException("Unable to find candidate with no exclusions.");
                                }

                                attempts++;
                            }
                            while (interactions.ExcludedCombinations().Any((c) => IsCovered(c, CreateProposedCandidate(value, candidate))));

                            // add this value to candidate and mark all values as such

                            foreach (var valuePair in value.ParameterToVaueMap)
                            {
                                candidate[valuePair.Key] = valuePair.Value;
                            }
                        }
                    }
                }

                variations.Add(candidate);

                // more variations than are need to exhaustively test the model have been adde
                if (variations.Count > maxVariations)
                {
                    throw new InternalVariationGenerationException("More variations than an exhaustive suite produced.");
                }
            }

            return variations;
        }

        // is this value covered by the candidate
        private static bool IsCovered(ValueCombination value, int[] candidate)
        {
            return value.ParameterToVaueMap.Keys
                .All((i) => candidate[i] == value.ParameterToVaueMap[i]);
        }

        // does this candidate violate any constraints
        private static bool IsExcluded(IEnumerable<ValueCombination> excludedValues, int[] candidate)
        {
            return excludedValues.Any((c) => IsCovered(c, candidate));
        }

        // add this value to the candidate
        private static int[] CreateProposedCandidate(ValueCombination value, int[] baseCandidate)
        {
            int[] proposed = new int[baseCandidate.Length];
            baseCandidate.CopyTo(proposed, 0);

            foreach (var valuePair in value.ParameterToVaueMap)
            {
                proposed[valuePair.Key] = valuePair.Value;
            }

            return proposed;
        }

        // can this value be added to this candidate
        private static bool IsCompatibleValue(ValueCombination value, int[] candidate)
        {
            return value.ParameterToVaueMap.Keys
                .All((i) => candidate[i] == value.ParameterToVaueMap[i] || candidate[i] == -1);
        }

        // map value indices to actual values and create a Variation
        private static Variation IndicesToVariation(Model model, IList<int> indices)
        {
            Variation v = new Variation();

            for (int i = 0; i < indices.Count; i++)
            {
                v.Add(model.Parameters[i].Name, model.Parameters[i][indices[i]]);
            }

            return v;
        }
    }
}
