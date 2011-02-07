// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents a base class for all comparison strategies.
    /// </summary>
    /// <remarks>
    /// Comparison strategies are used to specify such things like ordered/unordered 
    /// collection comparison, case sensitive/insensitive string match, non-exact match and so on. 
    /// Each <see cref="GraphNode"/> has strategy associated (if a node does not have a strategy 
    /// associated a default one will be used). Those strategies are useful for a general 
    /// object comparison scenario. Each strategy is uniquely identified by its type name. 
    /// Usually, the name is saved into XML (or whatever format a chosen codec uses) to be able to 
    /// restore that back during decoding.
    /// <para/>This class is not thread safe.
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to create a custom comparison strategy. Since 
    /// comparison strategies are assigned to nodes by a graph factory, we will need a 
    /// graph factory as well.
    /// </example>
    /// <code>
    /// class Sample
    /// {
    ///     public string Value { get; set; }
    /// 
    ///     public Sample Child { get; set; }
    /// 
    ///     public Sample(string value, Sample sample)
    ///     {
    ///         Value = value;
    ///         Child = sample;
    ///     }
    /// }
    /// </code>
    /// <code>
    /// class SampleFactory : ObjectGraphFactory
    /// {
    ///     private static readonly ObjectGraphComparisonStrategy strategy = new SampleComparisonStrategy();
    /// 
    ///     public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
    ///     {
    ///         created = new Dictionary&lt;Sample, GraphNode&gt;();
    ///         return CreateObjectGraph((Sample)value, null);
    ///     }
    /// 
    ///     // Visits all nodes recursively
    ///     private GraphNode CreateObjectGraph(Sample value, GraphNode parent)
    ///     {
    ///         if (created.ContainsKey(value))
    ///         {
    ///             return created[value];
    ///         }
    /// 
    ///         var root = new GraphNode();
    ///         root.ObjectValue = value.Value;
    ///         root.ComparisonStrategy = strategy;
    ///         root.Parent = parent;
    ///         root.Name = value.GetType().Name;
    ///         created.Add(value, root);
    ///         if (value.Child != null)
    ///         {
    ///             var child = CreateObjectGraph(value.Child, root);
    ///             root.Children.Add(child);
    ///         }
    /// 
    ///         return root;
    ///     }
    /// 
    ///     // Keeps track of visited nodes
    ///     private Dictionary&lt;Sample, GraphNode&gt; created;
    /// }
    /// </code>
    /// <code>
    /// class SampleComparisonStrategy : ObjectGraphComparisonStrategy
    /// {
    ///     protected override IEnumerable&lt;ObjectComparisonMismatch&gt; Compare(GraphNode left, GraphNode right)
    ///     {
    ///         // This comparison strategy assumes that it is attached to correct nodes
    /// 
    ///         var leftString = (string)left.ObjectValue;
    ///         var rightString = (string)right.ObjectValue;
    ///         var mismatches = new List&lt;ObjectComparisonMismatch&gt;();
    ///         if (!string.Equals(leftString, rightString, StringComparison.OrdinalIgnoreCase))
    ///         {
    ///             mismatches.Add(new ObjectComparisonMismatch(
    ///                 left, right, ObjectComparisonMismatchType.ObjectValuesDoNotMatch));
    ///         }
    /// 
    ///         // Mark nodes as visited
    ///         MarkVisited(left);
    ///         MarkVisited(right);
    /// 
    ///         // Compare inner nodes if any
    ///         CompareChildNodes(left, right);
    ///         return mismatches;
    ///     }
    /// }
    /// </code>
    public abstract class ObjectGraphComparisonStrategy
    {
        #region Protected and Public Members

        /// <summary>
        /// Compares child nodes of the given object graphs.
        /// </summary>
        /// <param name="left">The left node to compare inner nodes of.</param>
        /// <param name="right">The right node to compare inner nodes of.</param>
        protected void CompareChildNodes(GraphNode left, GraphNode right)
        {
            if (left == null)
            {
                throw new ArgumentNullException("left");
            }

            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            ComparisonResult &= Comparer.CompareChildNodes(left, right);
        }

        /// <summary>
        /// Marks the node as visited.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <remarks>
        /// This method allows custom comparison strategies to perform comparison of
        /// cyclic object graphs.
        /// </remarks>
        protected void MarkVisited(GraphNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            Comparer.MarkVisited(node);
        }

        /// <summary>
        /// Determines whether the specified node is visited.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is visited; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method allows custom comparison strategies to perform comparison of
        /// cyclic object graphs.
        /// </remarks>
        protected bool IsVisited(GraphNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return Comparer.IsVisited(node);
        }

        /// <summary>
        /// Performs a comparison of two object graph nodes.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        /// <returns>The collection of the occurred mismatches.</returns>
        /// <remarks>
        /// The strategy returns only mismatches which it records. The comparison
        /// of any child nodes which the strategy does not work with should be
        /// done using the <see cref="ObjectGraphComparisonStrategy.CompareChildNodes"/>.
        /// <para/>The strategy is responsible to marking all nodes it compares as visited
        /// by using <see cref="MarkVisited"/> method. That prevents possible infinite loops
        /// while comparing cyclic object graphs. The strategy can use <see cref="IsVisited"/>
        /// to determine whether the given node has been visited already.
        /// <para/>If a strategy compares non-lead nodes, it might need to call 
        /// <see cref="CompareChildNodes"/> method for corresponding nodes to compare their
        /// child nodes.
        /// </remarks>
        protected abstract IEnumerable<ObjectComparisonMismatch> Compare(GraphNode left, GraphNode right);

        #endregion

        #region Inner Members

        /// <summary>
        /// Compares the specified object graphs. It is called by <see cref="ObjectGraphComparer"/>.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="comparer">
        /// The calling comparer to compare object sub-graphs, which the given strategy does 
        /// not work with and perform other interactions.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified nodes are equal; otherwise, <c>false</c>.
        /// </returns>
        internal bool Compare(GraphNode left, GraphNode right, ObjectGraphComparer comparer)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);
            Debug.Assert(comparer != null);

            EnterCompare(comparer);

            try
            {
                var mismatches = Compare(left, right);
                if (mismatches != null)
                {
                    ComparisonResult &= !mismatches.Any();
                    foreach (var m in mismatches)
                    {
                        Comparer.AddMismatch(m);
                    }
                }

                return ComparisonResult;
            }
            finally
            {
                ExitCompare();
            }
        }

        #endregion

        #region Private Comparison Helpers

        private void EnterCompare(ObjectGraphComparer comparer)
        {
            comparisons.Push(new ComparerResultTuple(comparer));
        }

        private void ExitCompare()
        {
            comparisons.Pop();
        }

        private ObjectGraphComparer Comparer
        {
            get { return comparisons.Peek().Comparer; }
        }

        private bool ComparisonResult
        {
            get
            {
                return comparisons.Peek().ComparisonResult;
            }
            set
            {
                comparisons.Peek().ComparisonResult = value;
            }
        }

        class ComparerResultTuple
        {
            public ComparerResultTuple(ObjectGraphComparer comparer)
            {
                Comparer = comparer;
                ComparisonResult = true;
            }

            public ObjectGraphComparer Comparer { get; private set; }

            public bool ComparisonResult { get; set; }
        }

        #endregion

        #region Private Data

        // The same strategy object can be attached to multiple graph nodes
        // with multiple comparers, thus we need to maintain the current one
        private Stack<ComparerResultTuple> comparisons = new Stack<ComparerResultTuple>();

        #endregion
    }
}
