// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents an equality condition between either a parameter and a value or two parameters.
    /// </summary>
    public class EqualConstraint : ConditionConstraint
    {
        /// <summary>
        /// Initializes a new instance of the EqualConstraint class using the specified parameter and value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public EqualConstraint(Parameter parameter, object value) 
        {
            FirstParameter = parameter;
            SecondParameter = null;
            Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the EqualConstraint class using the specified parameters.
        /// </summary>
        /// <param name="first"> The first parameter.</param>
        /// <param name="second">The second parameter.</param>
        public EqualConstraint(Parameter first, Parameter second) 
        {
            FirstParameter = first;
            SecondParameter = second;
            Value = null;
        }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            if (CachedInteraction == null)
            {
                CachedInteraction = InternalConstraintHelpers.GetExcludedCombinations
                    (model, this, FirstParameter, SecondParameter, Value, (value1, value2) => value1.Equals(value2));
            }

            return new ParameterInteraction(CachedInteraction);
        }

        internal override ConstraintSatisfaction SatisfiesContraint(Model model, ValueCombination combination)
        {
            if (CachedInteraction == null)
            {
                GetExcludedCombinations(model);
            }

            return InternalConstraintHelpers.SatisfiesContraint(model, combination, CachedInteraction);
        }

        internal override void ClearCache()
        {
            CachedInteraction = null;
        }

        Parameter FirstParameter { get; set; }
        Parameter SecondParameter { get; set; }
        object Value { get; set; }
    }
}
