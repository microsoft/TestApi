// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Test.VariationGeneration.Constraints;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Code shared between the conditional constraints(==,!=,>, etc).
    /// </summary>
    internal static class InternalConstraintHelpers
    {
        // helper to implement Constraint.GetExcludeCombinations
        internal static ParameterInteraction GetExcludedCombinations<T>(Model model, Constraint constraint, Parameter first, Parameter second, T value, Func<T, T, bool> comparison)
        {
            Debug.Assert(first != null && model != null && constraint != null && comparison != null);
            List<int> parameterIndices = new List<int>();
            parameterIndices.Add(model.Parameters.IndexOf(first));
            if (second != null)
            {
                parameterIndices.Add(model.Parameters.IndexOf(second));
            }
            parameterIndices.Sort();

            ParameterInteraction interaction = new ParameterInteraction(parameterIndices);
            List<int[]> valueTable = ParameterInteractionTable.GenerateValueTable(model.Parameters, interaction);

            foreach (var valueIndices in valueTable)
            {
                T value1, value2;

                value1 = first[valueIndices[0]] is ParameterValue ? (T)((ParameterValue)first[valueIndices[0]]).Value : (T)first[valueIndices[0]];
                if (second == null)
                {
                    value2 = value;
                }
                else
                {
                    value2 = first[valueIndices[1]] is ParameterValue ? (T)((ParameterValue)second[valueIndices[1]]).Value : (T)second[valueIndices[1]];
                }

                ValueCombinationState comboState = comparison(value1, value2) ? ValueCombinationState.Covered : ValueCombinationState.Excluded;
                interaction.Combinations.Add(new ValueCombination(valueIndices, interaction) { State = comboState });
            }

            return interaction;
        }

        // helper to implement Constraint.SatisfiesConstraint
        internal static ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination, ParameterInteraction interaction)
        {
            Debug.Assert(model != null && combination != null && interaction != null);

            var parameterMap = combination.ParameterToValueMap;
            for(int i = 0; i < interaction.Parameters.Count; i++)
            {
                if (!parameterMap.ContainsKey(interaction.Parameters[i]))
                {
                    return ConstraintSatisfaction.InsufficientData;
                }
            }

            for(int i = 0; i < interaction.Combinations.Count; i++)
            {
                if (ParameterInteractionTable.MatchCombination(interaction.Combinations[i], combination))
                {
                    if (interaction.Combinations[i].State == ValueCombinationState.Excluded)
                    {
                        return ConstraintSatisfaction.Unsatisfied;
                    }
                }
            }

            return ConstraintSatisfaction.Satisfied;
        }        
    }
}
