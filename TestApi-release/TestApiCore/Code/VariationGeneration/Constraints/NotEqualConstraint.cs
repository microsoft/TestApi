// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents an inequality condition between either a parameter and a value or two parameters.
    /// </summary>
    public class NotEqualConstraint : ConditionConstraint
    {
        /// <summary>
        /// Initializes a new instance of the NotEqualConstraint class using the specified parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public NotEqualConstraint(Parameter parameter, object value) 
        {
            InnerConstraint = new NotConstraint { Condition = new EqualConstraint(parameter, value) };
        }

        /// <summary>
        /// Initializes a new instance of the NotEqualConstraint class using the specified parameters.
        /// </summary>
        /// <param name="first"> The first parameter.</param>
        /// <param name="second">The second parameter.</param>
        public NotEqualConstraint(Parameter first, Parameter second) 
        {
            InnerConstraint = new NotConstraint { Condition = new EqualConstraint(first, second) };
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
