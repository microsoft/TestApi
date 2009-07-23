// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Microsoft.Test.VariationGeneration
{
    

    /// <summary>
    /// A single value in the model
    /// </summary>
    internal class ValueCombination
    {
        public ValueCombination(ValueCombination combination)
        {
            this.parameterToValueMap = new Dictionary<int, int>(combination.ParameterToVaueMap);
            this.State = combination.State;
        }

        public ValueCombination(IList<int> values, ParameterInteraction interaction)
        {
            if (values.Count != interaction.Parameters.Count)
            {
                throw new ArgumentOutOfRangeException("values", "values and interaction must be the same length.");
            }

            for (int i = 0; i < values.Count; i++)
            {
                parameterToValueMap[interaction.Parameters[i]] = values[i];
            }
            State = ValueCombinationState.Uncovered;
        }

        public ValueCombinationState State { get; set; }

        private Dictionary<int, int> parameterToValueMap = new Dictionary<int,int>();
        /// <summary>
        /// Dictionary that maps a parameter to its value
        /// </summary>
        public IDictionary<int, int> ParameterToVaueMap { get { return parameterToValueMap; } }
    }
}
