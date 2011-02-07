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
    /// Represents a generic object graph comparer. This class compares object 
    /// graphs produced by an <see cref="ObjectGraphFactory"/>.
    /// </summary>
    /// <remarks>
    /// Comparing two objects for equivalence is a relatively common task during test validation. 
    /// One example would be to test whether a type follows the rules required by a particular 
    /// serializer by saving and loading the object and comparing the two. A deep object 
    /// comparison is one where all the properties and its properties are compared repeatedly 
    /// until primitives are reached. The .NET Framework provides mechanisms to perform such comparisons but 
    /// requires the types in question to implement part of the comparison logic 
    /// (IComparable, .Equals). However, there are often types that do not follow 
    /// these mechanisms. This API provides a mechanism to deep compare two objects using 
    /// reflection. 
    /// </remarks>
    /// 
    /// <example>
    /// The following example demonstrates how to compare two objects using a general-purpose object 
    /// graph factory (represented by <see cref="PublicPropertyObjectGraphFactory"/>).
    /// 
    /// <code>
    /// Person p1 = new Person("John");
    /// p1.Children.Add(new Person("Peter"));
    /// p1.Children.Add(new Person("Mary"));
    ///
    /// Person p2 = new Person("John");
    /// p2.Children.Add(new Person("Peter"));
    /// 
    /// ObjectGraphFactory factory = new PublicPropertyObjectGraphFactory();
    /// GraphNode left = factory.CreateObjectGraph(p1);
    /// GraphNode right = factory.CreateObjectGraph(p2);
    /// ObjectComparer comparer = new ObjectGraphComparer();
    /// Console.WriteLine(
    ///     "Objects p1 and p2 {0}", 
    ///     comparer.Compare(left, right) ? "match!" : "do NOT match!");
    /// </code>
    ///
    /// where Person is declared as follows:
    ///
    /// <code>
    /// class Person
    /// {
    ///     public Person(string name) 
    ///     { 
    ///         Name = name;
    ///         Children = new Collection&lt;Person&gt;();
    ///     }
    ///     public string Name { get; set; }
    ///     public Collection&lt;Person&gt; Children { get; private set;  }
    /// }
    /// </code>
    /// </example>
    ///
    /// <example>
    /// In addition, the object comparison API allows the user to get back a list of comparison mismatches. 
    /// For an example, see <see cref="ObjectComparisonMismatch"/> objects. 
    /// </example>
    public sealed class ObjectGraphComparer
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectGraphComparer"/> class.
        /// </summary>
        public ObjectGraphComparer()
        {
            this.probing = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectGraphComparer"/> class 
        /// in the probing mode.
        /// </summary>
        /// <param name="visited">The nodes already visited.</param>
        /// <remarks>
        /// When in the probing mode, the comparer returns a comparison verdict
        /// as soon as it is known. It modifies its visited nodes, thus in case of
        /// a positive result it represents an accurate set of all visited nodes.
        /// That is needed for an unordered comparison, when we do not know apriori 
        /// which node should match which.
        /// </remarks>
        private ObjectGraphComparer(HashSet<GraphNode> visited)
        {
            this.probing = true;
            this.visited = new HashSet<GraphNode>(visited);
            this.mismatches = new List<ObjectComparisonMismatch>();
        }

        #endregion

        #region Public and Protected Members

        /// <summary>
        /// Performs a deep comparison of two object graphs.
        /// </summary>
        /// <param name="left">The left object graph.</param>
        /// <param name="right">The right object graph.</param>
        /// <returns>true if the object graphs match.</returns>
        /// <remarks>
        /// If a graph node does not have a comparison strategy attached the default 
        /// strategy is used which is unordered comparison for child nodes and
        /// case-sensitive, exact match for node values.
        /// </remarks>
        public bool Compare(GraphNode left, GraphNode right)
        {
            IEnumerable<ObjectComparisonMismatch> mismatches;
            return Compare(left, right, out mismatches);
        }

        /// <summary>
        /// Performs a deep comparison of two object graphs and provides 
        /// a list of mismatching nodes.
        /// </summary>
        /// <param name="left">The left object graph.</param>
        /// <param name="right">The right object graph.</param>
        /// <param name="mismatches">The list of mismatches.</param>
        /// <returns>true if the object graphs match.</returns>
        /// <remarks>
        /// Only nodes with equal names are compared with each other.
        /// <para/>If a graph node does not have a comparison strategy attached the default 
        /// strategy is used which is unordered comparison for child nodes and
        /// case-sensitive, exact match for node values.
        /// <para/>Nodes with equal names must have the same comparison strategies attached.
        /// If the strategies are different, then those nodes and all their children are not
        /// compared and skipped alltogether, because there is no way to identify which
        /// strategy should be preferred.
        /// </remarks>
        public bool Compare(GraphNode left, GraphNode right, out IEnumerable<ObjectComparisonMismatch> mismatches)
        {
            if (visiting)
            {
                mismatches = null;
                return CompareObjectGraphs(left, right);
            }

            bool isMatch = false;
            try
            {
                this.visiting = true;
                this.mismatches = new List<ObjectComparisonMismatch>();
                this.visited = new HashSet<GraphNode>();
                isMatch = CompareObjectGraphs(left, right);
                mismatches = this.mismatches;
            }
            finally
            {
                this.visiting = false;
                this.visited = null;
                this.mismatches = null;
            }

            return isMatch;
        }

        #endregion

        // Those members are needed for a seamless integration between 
        // comparison strategies and comparers
        #region Internal Members

        /// <summary>
        /// Determines whether the specified node is visited.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is visited; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsVisited(GraphNode node)
        {
            Debug.Assert(node != null);
            return visited != null && visited.Contains(node);
        }

        /// <summary>
        /// Marks the node as visited.
        /// </summary>
        /// <param name="node">The node.</param>
        internal void MarkVisited(GraphNode node)
        {
            Debug.Assert(node != null);
            visited.Add(node);
        }

        /// <summary>
        /// Compares the child nodes of the given nodes.
        /// </summary>
        /// <param name="left">The left node to compare children of.</param>
        /// <param name="right">The right node to compare children of.</param>
        /// <returns></returns>
        internal bool CompareChildNodes(GraphNode left, GraphNode right)
        {
            Debug.Assert(left != null);
            Debug.Assert(right != null);

            visited.Add(left);
            visited.Add(right);
            return CompareObjectGraphs(left.Children, right.Children);
        }

        /// <summary>
        /// Adds the mismatch to the mismatch collection.
        /// </summary>
        /// <param name="mismatch">The mismatch.</param>
        internal void AddMismatch(ObjectComparisonMismatch mismatch)
        {
            Debug.Assert(mismatch != null);
            if (!probing)
            {
                mismatches.Add(mismatch);
            }
        }

        #endregion

        #region Private Members

        private bool FirstGroupKeysAreEqual(
            List<IGrouping<string, GraphNode>> leftNodeGroups,
            List<IGrouping<string, GraphNode>> rightNodeGroups,
            List<ObjectComparisonMismatch> mismatches)
        {
            // Note that groups are sorted
            var lg = leftNodeGroups.First();
            var rg = rightNodeGroups.First(); 
            var keyCompare = string.Compare(lg.Key, rg.Key, StringComparison.Ordinal);
            
            if (keyCompare == 0) return true;

            if (mismatches != null)
            {
                if (keyCompare > 0)
                {
                    // If the left group key is bigger than the left node is missing some children
                    AddMissingNodeMismatches(rg, false);
                    rightNodeGroups.RemoveAt(0);
                }
                else if (keyCompare < 0)
                {
                    // If the right group key is bigger than the right node is missing some children
                    AddMissingNodeMismatches(lg, true);
                    leftNodeGroups.RemoveAt(0);
                }
            }

            return false;
        }

        /// <summary>
        /// Compares the object graphs.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        private bool CompareObjectGraphs(GraphNode left, GraphNode right)
        {
            // TODO : it makes sense to replace recursion in this method with iterations.
            // Perhaps next TestApi release?

            // Check if some node has been already visited
            var alreadyVisited = CheckNodesNotVisited(left, right);
            if (alreadyVisited.HasValue)
            {
                return alreadyVisited.Value;
            }

            // Check if a custom comparison strategy is involved
            if (left.ComparisonStrategy != null || right.ComparisonStrategy != null)
            {
                // Strategies recursively invoke the comparer to compare any child nodes
                return CompareObjectGraphsWithStrategies(left, right);
            }

            // Add root nodes to prevent recursion
            visited.Add(left);
            visited.Add(right);

            // Initial number of mismatches
            var mismatchesCount = mismatches.Count;

            // Compare root nodes themselves
            var mismatch = CompareNodes(left, right);
            if (mismatch != null)
            {
                // If we are just probing for equality of nodes, we already 
                // know the answer
                if (probing) return false;

                mismatches.Add(mismatch);
            }

            // Get children, group and order them by names
            var leftNodeGroups = left.Children
                .GroupBy(n => n.Name, StringComparer.Ordinal)
                .OrderBy(g => g.Key, StringComparer.Ordinal).ToList();
            var rightNodeGroups = right.Children
                .GroupBy(n => n.Name, StringComparer.Ordinal)
                .OrderBy(g => g.Key, StringComparer.Ordinal).ToList();

            // For each group of left nodes find a corresponding group of
            // right nodes (group key is the same, i.e. names of elements are
            // the same) and compare them
            while (leftNodeGroups.Any() && rightNodeGroups.Any())
            {
                // Keys of the first groups in the group lists are not equal
                // Move on to the next groups
                if (!FirstGroupKeysAreEqual(leftNodeGroups, rightNodeGroups, probing ? null : mismatches))
                {
                    // If we are just probing for equality of nodes, we already 
                    // know the answer
                    if (probing) return false;

                    continue;
                }

                var lg = leftNodeGroups.First();
                leftNodeGroups.RemoveAt(0);
                var rg = rightNodeGroups.First();
                rightNodeGroups.RemoveAt(0);

                if (!CompareObjectGraphs(lg, rg))
                {
                    // If we are just probing for equality of nodes, we already 
                    // know the answer
                    if (probing) return false;
                }
            }

            if (leftNodeGroups.Any() || rightNodeGroups.Any())
            {
                // If we are just probing for equality of nodes, we already 
                // know the answer
                if (probing) return false;

                AddMissingNodeMismatches(leftNodeGroups.SelectMany(g => g), true);
                AddMissingNodeMismatches(rightNodeGroups.SelectMany(g => g), false);
            }

            return mismatches.Count == mismatchesCount;
        }

        /// <summary>
        /// Performs and unordered comparison of the given object graph collections.
        /// </summary>
        /// <param name="leftNodes">The left nodes.</param>
        /// <param name="rightNodes">The right nodes.</param>
        private bool CompareObjectGraphs(IEnumerable<GraphNode> leftNodes, IEnumerable<GraphNode> rightNodes)
        {
            var left = leftNodes.ToList();
            var right = rightNodes.ToList();

            if (left.Count == 1 && right.Count == 1)
            {
                return CompareObjectGraphs(left.First(), right.First());
            }

            var mismatchesCount = mismatches.Count;

            // One of the groups has more than 1 element, since we are
            // doing unordered comparison by default, we need to
            // go through elements in both groups and find matches
            var leftIndex = 0;
            while (leftIndex < left.Count)
            {
                var found = false;
                for (int rightIndex = 0; rightIndex < right.Count; rightIndex++)
                {
                    if (probing)
                    {
                        // If we are already in the probing mode, there is no need to
                        // create a yet new comparer
                        found = CompareObjectGraphs(left[leftIndex], right[rightIndex]);
                        if (!found) return false;
                    }
                    else
                    {
                        var probingComparer = new ObjectGraphComparer(visited);
                        found = probingComparer.CompareObjectGraphs(left[leftIndex], right[rightIndex]);
                        // If nodes match, need to remember visited sub-nodes
                        if (found) visited.UnionWith(probingComparer.visited);
                    }

                    if (found)
                    {
                        left.RemoveAt(leftIndex);
                        right.RemoveAt(rightIndex);
                        break;
                    }
                }

                if (!found) leftIndex++;
            }

            // Now left and right contain elements without a match
            // Go through them and get mismatches...
            while (left.Any() && right.Any())
            {
                // ...but, if we are just probing for equality of nodes, we already 
                // know the answer
                if (probing) return false;

                CompareObjectGraphs(left.First(), right.First());
                left.RemoveAt(0);
                right.RemoveAt(0);
            }

            if (left.Any() || right.Any())
            {
                // If we are just probing for equality of nodes, we already 
                // know the answer
                if (probing) return false;

                AddMissingNodeMismatches(left, true);
                AddMissingNodeMismatches(right, false);
            }

            return mismatches.Count == mismatchesCount;
        }

        private static ObjectComparisonMismatch CompareNodes(GraphNode leftNode, GraphNode rightNode)
        {
            // check if one of the nodes is null while the other is not
            if ((leftNode.ObjectValue == null && rightNode.ObjectValue != null) ||
                (leftNode.ObjectValue != null && rightNode.ObjectValue == null))
            {
                var mismatch = new ObjectComparisonMismatch(
                    leftNode,
                    rightNode,
                    ObjectComparisonMismatchType.ObjectValuesDoNotMatch);
                return mismatch;
            }

            if (leftNode.ObjectValue != null && rightNode.ObjectValue != null)
            {
                // compare type names //
                if (!leftNode.ObjectType.Equals(rightNode.ObjectType))
                {
                    var mismatch = new ObjectComparisonMismatch(
                        leftNode,
                        rightNode,
                        ObjectComparisonMismatchType.ObjectTypesDoNotMatch);
                    return mismatch;
                }

                // compare primitives, strings, datatimes, guids
                if (leftNode.ObjectType.IsPrimitive ||
                    leftNode.ObjectType == typeof(string) ||
                    leftNode.ObjectType == typeof(DateTime) ||
                    leftNode.ObjectType == typeof(Guid))
                {
                    if (!leftNode.ObjectValue.Equals(rightNode.ObjectValue))
                    {
                        var mismatch = new ObjectComparisonMismatch(
                            leftNode,
                            rightNode,
                            ObjectComparisonMismatchType.ObjectValuesDoNotMatch);
                        return mismatch;
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            // compare the child count 
            if (leftNode.Children.Count != rightNode.Children.Count)
            {
                var type = leftNode.Children.Count > rightNode.Children.Count
                    ? ObjectComparisonMismatchType.RightNodeHasFewerChildren
                    : ObjectComparisonMismatchType.LeftNodeHasFewerChildren;

                var mismatch = new ObjectComparisonMismatch(
                    leftNode,
                    rightNode,
                    type);
                return mismatch;
            }

            // No mismatch //
            return null;
        }

        private void AddMissingNodeMismatches(IEnumerable<GraphNode> nodes, bool rightIsMissing)
        {           
            foreach (var n in nodes)
            {
                var rightNode = rightIsMissing ? null : n;
                var leftNode = rightIsMissing ? n : null;
                var mismatchType = rightIsMissing 
                    ? ObjectComparisonMismatchType.MissingRightNode 
                    : ObjectComparisonMismatchType.MissingLeftNode;
                mismatches.Add(new ObjectComparisonMismatch(leftNode, rightNode, mismatchType));
            }
        }

        private bool? CheckNodesNotVisited(GraphNode left, GraphNode right)
        {
            var leftHasBeenVisited = visited.Contains(left);
            var rightHasBeenVisited = visited.Contains(right);
            if (leftHasBeenVisited || rightHasBeenVisited)
            {
                if (leftHasBeenVisited && rightHasBeenVisited) return true;

                // If we are just probing for equality of nodes, we already 
                // know the answer
                if (probing) return false;

                // If the left node has been already visited, but the right not, then
                // the right node is missing from the left graph
                if (leftHasBeenVisited) AddMissingNodeMismatches(new[] { right }, false);
                else AddMissingNodeMismatches(new[] { left }, true);

                // If some the nodes has been visited then there is no reason to compare
                // children of those nodes
                return false;
            }

            return null;
        }

        private bool CompareObjectGraphsWithStrategies(GraphNode left, GraphNode right)
        {
            // Comparison strategy is responsible for recording all occurred
            // mismatches with the calling comparer
            if (CheckComparisonStrategiesSame(left, right))
            {
                return left.ComparisonStrategy.Compare(left, right, this);
            }

            mismatches.Add(
                new ObjectComparisonMismatch(left, right,
                        ObjectComparisonMismatchType.ComparisonStrategiesDoNotMatch));

            return false;
        }

        private static bool CheckComparisonStrategiesSame(GraphNode left, GraphNode right)
        {
            if ((left.ComparisonStrategy == null && right.ComparisonStrategy != null) ||
                (left.ComparisonStrategy != null && right.ComparisonStrategy == null))
            {
                return false;
            }

            return left.ComparisonStrategy.GetType() == right.ComparisonStrategy.GetType();
        }

        #endregion

        #region Private Data

        // The same comparer is invoked recursively by comparison
        // strategies, thus need to keep a state of comparison in progress
        private bool visiting;
        private HashSet<GraphNode> visited;
        private List<ObjectComparisonMismatch> mismatches;

        // When in a probing mode, the comparer returns a comparison verdict
        // as soon as it is known. It modifies its visited nodes, so in case of
        // a positive result it represents a set of all visited nodes
        // That is needed for an unordered comparison, when we do not know apriori 
        // which node should match which
        private readonly bool probing;

        #endregion
    }
}
