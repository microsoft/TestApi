// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents one comparison mismatch.
    /// </summary>
    [DebuggerDisplay("{MismatchType}: LeftNodeName={LeftObjectNode.QualifiedName}")]
    public sealed class ObjectComparisonMismatch
    {
        #region Constructors

        /// <summary>
        /// Creates an instance of the ObjectComparisonMismatch class.
        /// </summary>
        /// <param name="leftObjectNode">The node from the left object.</param>
        /// <param name="rightObjectNode">The node from the right object.</param>
        /// <param name="mismatchType">Represents the type of mismatch.</param>
        public ObjectComparisonMismatch(GraphNode leftObjectNode, GraphNode rightObjectNode, ObjectComparisonMismatchType mismatchType)
        {
            this.leftObjectNode = leftObjectNode;
            this.rightObjectNode = rightObjectNode;
            this.mismatchType = mismatchType;
        }

        #endregion Public Members

        #region Public Members

        /// <summary>
        /// Gets the node in the left object.
        /// </summary>
        public GraphNode LeftObjectNode
        {
            get
            {
                return this.leftObjectNode;
            }
        }

        /// <summary>
        /// Gets the node in the right object.
        /// </summary>
        public GraphNode RightObjectNode
        {
            get
            {
                return this.rightObjectNode;
            }
        }

        /// <summary>
        /// Represents the type of mismatch.
        /// </summary>
        public ObjectComparisonMismatchType MismatchType 
        {
            get
            {
                return this.mismatchType;
            }
        }

        #endregion

        #region Private Data

        private GraphNode leftObjectNode;
        private GraphNode rightObjectNode;
        private ObjectComparisonMismatchType mismatchType;

        #endregion
    }
}
