// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Negates the inner condition.
    /// </summary>
    public class NotConstraint : ConditionConstraint
    {
        /// <summary>
        /// The condition to negate.
        /// </summary>
        public ConditionConstraint Condition { get; set; }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            if (CachedInteraction == null)
            {
                ParameterInteraction interaction = Condition.GetExcludedCombinations(model);

                foreach (var combination in interaction.Combinations)
                {
                    if (combination.State == ValueCombinationState.Excluded)
                    {
                        combination.State = ValueCombinationState.Covered;
                    }
                    else
                    {
                        combination.State = ValueCombinationState.Excluded;
                    }
                }

                CachedInteraction = interaction;
            }
            return new ParameterInteraction(CachedInteraction);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination)
        {
            var satisfaction = Condition.SatisfiesContraint(model, combination);

            if (satisfaction == ConstraintSatisfaction.InsufficientData)
            {
                return ConstraintSatisfaction.InsufficientData;
            }

            if (satisfaction == ConstraintSatisfaction.Satisfied)
            {
                return ConstraintSatisfaction.Unsatisfied;
            }
            else
            {
                return ConstraintSatisfaction.Satisfied;
            }
        }

        internal override void ClearCache()
        {
            CachedInteraction = null;
            Condition.ClearCache();
        }
    }
}
