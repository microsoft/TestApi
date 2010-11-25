// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents the type of mismatch.
    /// </summary>
    public enum ObjectComparisonMismatchType
    {
        /// <summary>
        /// The node is missing in the right graph.
        /// </summary>
        MissingRightNode = 0,

        /// <summary>
        /// The node is missing in the left graph.
        /// </summary>
        MissingLeftNode = 1,

        /// <summary>
        /// The right node has fewer children than the left node.
        /// </summary>
        RightNodeHasFewerChildren = 2,

        /// <summary>
        /// The left node has fewer children than the right node.
        /// </summary>
        LeftNodeHasFewerChildren = 3,

        /// <summary>
        /// The node types do not match.
        /// </summary>
        ObjectTypesDoNotMatch = 4,

        /// <summary>
        /// The node values do not match.
        /// </summary>
        ObjectValuesDoNotMatch = 5
    }
}
