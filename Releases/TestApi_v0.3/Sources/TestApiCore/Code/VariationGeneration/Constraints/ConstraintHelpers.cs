// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration.Constraints
{
    /// <summary>
    /// A collection of methods to help create constraints.
    /// </summary>
    public static class ConstraintHelpers
    {
        /// <summary>
        /// Creates an AndConstraint between two ConditionConstraints.
        /// </summary>
        /// <param name="first">The first condition.</param>
        /// <param name="second">The second condition.</param>
        /// <returns>The constraint.</returns>
        public static AndConstraint And(this ConditionConstraint first, ConditionConstraint second)
        {
            return new AndConstraint { First = first, Second = second };
        }

        /// <summary>
        /// Creates an EqualConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static EqualConstraint Equal(this Parameter parameter, object value)
        {
            return new EqualConstraint(parameter, value);
        }

        /// <summary>
        /// Creates an EqualConstraint between two parameters.
        /// </summary>
        /// <param name="first"> The first parameter.</param>
        /// <param name="second">The second parameter.</param>
        /// <returns>The constraint.</returns>
        public static EqualConstraint Equal(this Parameter first, Parameter second)
        {
            return new EqualConstraint(first, second);
        }

        /// <summary>
        /// Creates an EqualConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static GreaterThanConstraint GreaterThan(this Parameter parameter, object value)
        {
            return new GreaterThanConstraint(parameter, value);
        }

        /// <summary>
        /// Creates a GreaterThanConstraint between two parameters.
        /// </summary>
        /// <param name="left"> The left parameter.</param>
        /// <param name="right">The right parameter.</param>
        /// <returns>The constraint.</returns>
        public static GreaterThanConstraint GreaterThan(this Parameter left, Parameter right)
        {
            return new GreaterThanConstraint(left, right);
        }

        /// <summary>
        /// Creates an GreaterThanOrEqualConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static GreaterThanOrEqualConstraint GreaterThanOrEqual(this Parameter parameter, object value)
        {
            return new GreaterThanOrEqualConstraint(parameter, value);
        }

        /// <summary>
        /// Creates a GreaterThanOrEqualConstraint between two parameters.
        /// </summary>
        /// <param name="left"> The left parameter.</param>
        /// <param name="right">The right parameter.</param>
        /// <returns>The constraint.</returns>
        public static GreaterThanOrEqualConstraint GreaterThanOrEqual(this Parameter left, Parameter right)
        {
            return new GreaterThanOrEqualConstraint(left, right);
        }

        /// <summary>
        /// Creates a LessThanConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static LessThanConstraint LessThan(this Parameter parameter, object value)
        {
            return new LessThanConstraint(parameter, value);
        }

        /// <summary>
        /// Creates a LessThanConstraint between two parameters.
        /// </summary>
        /// <param name="left"> The left parameter.</param>
        /// <param name="right">The right parameter.</param>
        /// <returns>The constraint.</returns>
        public static LessThanConstraint LessThan(this Parameter left, Parameter right)
        {
            return new LessThanConstraint(left, right);
        }

        /// <summary>
        /// Creates a LessThanOrEqualConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static LessThanOrEqualConstraint LessThanOrEqual(this Parameter parameter, object value)
        {
            return new LessThanOrEqualConstraint(parameter, value);
        }

        /// <summary>
        /// Creates an EqualConstraint between two parameters.
        /// </summary>
        /// <param name="left"> The left parameter.</param>
        /// <param name="right">The right parameter.</param>
        /// <returns>The constraint.</returns>
        public static LessThanOrEqualConstraint LessThanOrEqual(this Parameter left, Parameter right)
        {
            return new LessThanOrEqualConstraint(left, right);
        }

        /// <summary>
        /// Creates a NotConstraint on the given condition.
        /// </summary>
        /// <param name="condition">The condition to negate.</param>
        /// <returns>The constraint.</returns>
        public static NotConstraint Not(this ConditionConstraint condition)
        {
            return new NotConstraint { Condition = condition };
        }

        /// <summary>
        /// Creates a NotEqualConstraint between a parameter and a value.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        /// <returns>The constraint.</returns>
        public static NotEqualConstraint NotEqual(this Parameter parameter, object value)
        {
            return new NotEqualConstraint(parameter, value);
        }

        /// <summary>
        /// Creates a NotEqualConstraint between 2 parameters.
        /// </summary>
        /// <param name="first"> The first parameter.</param>
        /// <param name="second">The second parameter.</param>
        /// <returns>The constraint.</returns>
        public static NotEqualConstraint NotEqual(this Parameter first, Parameter second)
        {
            return new NotEqualConstraint(first, second);
        }

        /// <summary>
        /// Creates an OrConstraint between two ConditionConstraints.
        /// </summary>
        /// <param name="first">The first condition.</param>
        /// <param name="second">The second condition.</param>
        /// <returns>The constraint.</returns>
        public static OrConstraint Or(this ConditionConstraint first, ConditionConstraint second)
        {
            return new OrConstraint { First = first, Second = second };
        }
    }
}
