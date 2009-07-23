// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents a less-than condition between either a parameter and a value or two parameters. 
    /// </summary>
    /// <remarks>
    /// All values must implement IComparable.
    /// </remarks>
    public class LessThanConstraint : ConditionConstraint
    {
        /// <summary>
        /// Initializes a new instance of the LessThanConstraint class using the specified parameter and value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public LessThanConstraint(Parameter parameter, object value)
        {
            InnerConstraint = new NotConstraint { Condition = new GreaterThanOrEqualConstraint(parameter, value) };
        }

        /// <summary>
        /// Initializes a new instance of the LessThanConstraint class using the specified parameters.
        /// </summary>
        /// <param name="left"> The left side</param>
        /// <param name="right">The right side</param>
        public LessThanConstraint(Parameter left, Parameter right)
        {
            InnerConstraint = new NotConstraint { Condition = new GreaterThanOrEqualConstraint(left, right) };
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
