// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents an "and" relationship between two conditions.
    /// </summary>
    public class AndConstraint : ConditionConstraint
    {
        /// <summary>
        /// The first condition.
        /// </summary>
        public ConditionConstraint First { get; set; }

        /// <summary>
        /// The second condition.
        /// </summary>
        public ConditionConstraint Second { get; set; }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            if (CachedInteraction == null)
            {
                ParameterInteraction firstInteraction = First.GetExcludedCombinations(model);
                ParameterInteraction secondInteraction = Second.GetExcludedCombinations(model);

                CachedInteraction = ParameterInteraction.Merge(model.Parameters,
                    firstInteraction,
                    secondInteraction,
                    (state1, state2) => state1 == ValueCombinationState.Covered && state2 == ValueCombinationState.Covered ?
                        ValueCombinationState.Covered :
                        ValueCombinationState.Excluded);
            }

            return new ParameterInteraction(CachedInteraction);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination)
        {
            ConstraintSatisfaction first = First.SatisfiesContraint(model, combination);
            ConstraintSatisfaction second = Second.SatisfiesContraint(model, combination);

            if (first == ConstraintSatisfaction.Unsatisfied || second == ConstraintSatisfaction.Unsatisfied)
            {
                return ConstraintSatisfaction.Unsatisfied;
            }

            if (first == ConstraintSatisfaction.Satisfied && second == ConstraintSatisfaction.Satisfied)
            {
                return ConstraintSatisfaction.Satisfied;
            }

            return ConstraintSatisfaction.InsufficientData;
        }

        internal override void ClearCache()
        {
            CachedInteraction = null;
            First.ClearCache();
            Second.ClearCache();
        }
    }
}