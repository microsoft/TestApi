// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Test.VariationGeneration
{
    /// <summary>
    /// Represents a single variable and its values in the model. Combinations of these values
    /// are used in the combinatorial generation of variations by the <see cref="Model"/>.
    /// </summary>
    /// <remarks>
    /// Exhaustively testing all possible inputs to any nontrivial software component is generally impossible
    /// due to the enormous number of variations. Combinatorial testing is one approach to achieve high coverage
    /// with a much smaller set of variations. Pairwise, the most common combinatorial strategy, tests every possible 
    /// pair of values. Higher orders of combinations (3-wise, 4-wise, etc.) can also be used for higher coverage
    /// at the expense of more variations. See <a href="http://pairwise.org">Pairwise Testing</a> and 
    /// <a href="http://www.pairwise.org/docs/pnsqc2006/PNSQC%20140%20-%20Jacek%20Czerwonka%20-%20Pairwise%20Testing%20-%20BW.pdf">
    /// Pairwise Testing in Real World</a> for more resources.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Microsoft.Test.VariationGeneration.Parameter", Justification="The suggested name ParameterCollection is confusing.")]
    public class Parameter : IList<object>
    {       

        /// <summary>
        /// Initializes a new instance of the parameter class using the specified name.
        /// </summary>
        /// <param name="name">The name of the parameter</param>
        public Parameter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Adds a value to the parameter.
        /// </summary>
        /// <param name="item">The value to add</param>
        public void Add(object item)
        {
            values.Add(item);
        }

        /// <summary>
        /// Removes all values from the parameter.
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }

        /// <summary>
        /// Checks whether the parameter has the specified value.
        /// </summary>
        /// <param name="item">The value to search for.</param>
        /// <returns>Whether the value was found.</returns>
        public bool Contains(object item)
        {
            return values.Contains(item);
        }

        /// <summary>
        /// Copies the values into the specified array.
        /// </summary>
        /// <param name="array">The target array.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(object[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The number of values in the parameter.
        /// </summary>
        public int Count
        {
            get { return values.Count; }
        }

        /// <summary>
        /// Whether values can be added or removed from the parameter.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified value from the parameter if found.
        /// </summary>
        /// <param name="item">The value to remove.</param>
        /// <returns>Whether the value was found and removed.</returns>
        public bool Remove(object item)
        {
            return values.Remove(item);
        }

        /// <summary>
        /// Gets an enumerator over the values in the parameter.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<object> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        /// <summary>
        /// Determines the index of the value in the parameter.
        /// </summary>
        /// <param name="item">The value to find.</param>
        /// <returns>The index of the value if found, otherwise -1.</returns>
        public int IndexOf(object item)
        {
            return values.IndexOf(item);
        }

        /// <summary>
        /// Inserts a value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index where the value should be inserted.</param>
        /// <param name="item">The value to insert.</param>
        public void Insert(int index, object item)
        {
            values.Insert(index, item);
        }

        /// <summary>
        /// Removes a value at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the value to remove.</param>
        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        /// <summary>
        /// The value at the specified index.
        /// </summary>
        /// <param name="index">The index of the value.</param>
        /// <returns>The value at the index.</returns>
        public object this[int index]
        {
            get
            {
                return values[index];
            }
            set
            {
                values[index] = value;
            }
        }

        private List<object> values = new List<object>();
    }
}
