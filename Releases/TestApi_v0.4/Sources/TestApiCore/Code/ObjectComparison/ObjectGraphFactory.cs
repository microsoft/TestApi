// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Creates a graph for the 
    /// provided object.
    /// </summary>
    public abstract class ObjectGraphFactory
    {
        /// <summary>
        /// Creates a graph for the given object.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <returns>The root node of the created graph.</returns>
        public virtual GraphNode CreateObjectGraph(object value)
        {
            throw new NotSupportedException("Please provide a behavior for this method in a derived class");
        }
    }
}
