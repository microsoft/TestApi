// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents a less-than-or-equal condition between either a parameter and a value or two parameters.
    /// </summary>
    /// <remarks>
    /// All values must implement IComparable.
    /// </remarks>
    public class LessThanOrEqualConstraint : ConditionConstraint
    {
        /// <summary>
        /// Initializes a new instance of the LessThanOrEqualConstraint class the specified parameter and value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public LessThanOrEqualConstraint(Parameter parameter, object value)
        {
            InnerConstraint = new NotConstraint { Condition = new GreaterThanConstraint(parameter, value) };
        }

        /// <summary>
        /// Initializes a new instance of the LessThanOrEqualConstraint class the specified parameters.
        /// </summary>
        /// <param name="left"> The first parameter</param>
        /// <param name="right">The second parameter</param>
        public LessThanOrEqualConstraint(Parameter left, Parameter right) 
        {
            InnerConstraint = new NotConstraint { Condition = new GreaterThanConstraint(left, right) };
        }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            return InnerConstraint.GetExcludedCombinations(model);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination)
        {
            return InnerConstraint.SatisfiesContraint(model, combination);
        }

        internal override void ClearCache()
        {
            InnerConstraint.ClearCache();
        }

        Constraint InnerConstraint { get; set; }
    }
}
