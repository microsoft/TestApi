// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Linq;

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// Represents a greater-than-or-equal condition between either a parameter and a value or 
    /// two parameters. 
    /// </summary>
    /// <remarks>
    /// All values must implement IComparable.
    /// </remarks>
    public class GreaterThanOrEqualConstraint : ConditionConstraint
    {
        /// <summary>
        /// Initializes a new instance of the GreaterThanOrEqualConstraint class using the specified parameter and value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public GreaterThanOrEqualConstraint(Parameter parameter, object value)
        {
            if (!(value is IComparable))
            {
                throw new ArgumentException("Value must implement IComparable.", "value");
            }

            if (parameter.Any((v) => !(v is IComparable)))
            {
                throw new ArgumentException("All parameter values must implement IComparable.", "parameter");
            }

            Value = (IComparable)value;
            LeftParameter = parameter;
            RightParameter = null;
        }

        /// <summary>
        /// Initializes a new instance of the GreaterThanOrEqualConstraint using two parameters.
        /// </summary>
        /// <param name="left"> The left parameter.</param>
        /// <param name="right">The right parameter.</param>
        public GreaterThanOrEqualConstraint(Parameter left, Parameter right)
        {
            if (left.Any((v) => !(v is IComparable)))
            {
                throw new ArgumentException("All parameter values must implement IComparable.", "left");
            }

            if (right.Any((v) => !(v is IComparable)))
            {
                throw new ArgumentException("All parameter values must implement IComparable.", "right");
            }

            Value = null;
            LeftParameter = left;
            RightParameter = right;
        }

        internal override ParameterInteraction GetExcludedCombinations(Model model)
        {
            if (CachedInteraction == null)
            {
                CachedInteraction = InternalConstraintHelpers.GetExcludedCombinations
                    (model, this, LeftParameter, RightParameter, Value, (value1, value2) => value1.CompareTo(value2) >= 0);
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

        Parameter LeftParameter { get; set; }
        Parameter RightParameter { get; set; }
        IComparable Value { get; set; }
    }
}
