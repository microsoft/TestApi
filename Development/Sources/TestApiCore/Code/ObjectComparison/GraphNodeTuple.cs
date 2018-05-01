// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents a helper (<see cref="GraphNode"/> x <see cref="System.Boolean"/>) tuple
    /// used by <see cref="PublicPropertyObjectGraphFactory"/>.
    /// </summary>
    /// <remarks>
    /// This type can be replaced by a BCL tuple type, when TestApi is migrated to .NET 4.0.
    /// </remarks>
    struct GraphNodeTuple
    {
        /// <summary>
        /// Gets the graph node.
        /// </summary>
        public GraphNode Node { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the node was created externally.
        /// </summary>
        public bool CreatedExternally { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphNodeTuple"/> struct.
        /// </summary>
        /// <param name="node">The graph node.</param>
        /// <param name="createdExternally">Whether the graph node was created externally.</param>
        public GraphNodeTuple(GraphNode node, bool createdExternally)
            : this()
        {
            Node = node;
            CreatedExternally = createdExternally;
        }
    }
}
