// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Represents a single value in a parameter.
    /// </summary>
    public class ParameterValue
    {
        /// <summary>
        /// Initializes a new value with the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public ParameterValue(object value) : this(value, null, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new value with the specified value and an expected result.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="tag">A user defined tag.</param>
        public ParameterValue(object value, object tag) : this(value, tag, 1.0)
        {
        }

        /// <summary>
        /// Initializes a new value with the specified value and weight.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="weight">The weight of the value.</param>
        public ParameterValue(object value, double weight) : this(value, null, weight)
        {
        }

        /// <summary>
        /// Initializes a new value with the specified value, a tag, and weight.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="tag">A user defined tag.</param>
        /// <param name="weight">The weight of the value.</param>
        public ParameterValue(object value, object tag, double weight)
        {
            Value = value;
            Tag = tag;
            Weight = weight;
        }

        /// <summary>
        /// The actual value.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Tags the value with a user defined expected result.  At most one tagged value will appear in a variation.  The default is null.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// A value indicating whether this value should be chosen more or less frequently.  Larger values will be chosen more often.  The default is 1.0.
        /// </summary>
        /// <remarks>
        /// Weighting creates preferences for certain values.  Due to the nature of the algorithm used, the actual weight value has no intrinsic meaning 
        /// (weighting one value at 10.0 and the others at 1.0 doesn’t mean it will appear 10x more often).  The primary goal of the algorithm is
        /// to cover all the combinations with fewest test cases possible which often runs counter to honoring the weight.  Weight acts a tie breaker when 
        /// candidate values cover the same number of combinations.  
        /// </remarks>
        public double Weight { get; set; }
    }
}
