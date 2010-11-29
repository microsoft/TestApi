// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents an implication between two constraints.
    /// </summary>
    public class IfThenConstraint : Constraint
    {
        /// <summary>
        /// The condition to test.
        /// </summary>
        public ConditionConstraint If { get; set; }

        /// <summary>
        /// The implied result of the test.
        /// </summary>
        public ConditionConstraint Then { get; set; }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            if (CachedInteraction == null)
            {
                var ifInteraction = If.GetExcludedCombinations(model);
                var thenInteraction = Then.GetExcludedCombinations(model);

                CachedInteraction = ParameterInteraction.Merge(
                    model.Parameters,
                    ifInteraction,
                    thenInteraction,
                    (ifState, thenState) => ifState == ValueCombinationState.Covered ? thenState : ValueCombinationState.Covered);
            }

            return new ParameterInteraction(CachedInteraction);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination)
        {
            ConstraintSatisfaction ifSatisfaction = If.SatisfiesContraint(model, combination);

            if (ifSatisfaction == ConstraintSatisfaction.InsufficientData)
            {
                return ConstraintSatisfaction.InsufficientData;
            }

            if (ifSatisfaction == ConstraintSatisfaction.Unsatisfied)
            {
                return ConstraintSatisfaction.Satisfied;
            }

            return Then.SatisfiesContraint(model, combination);
        }

        internal override void ClearCache()
        {
            CachedInteraction = null;
            If.ClearCache();
            Then.ClearCache();
        }
    }
}
