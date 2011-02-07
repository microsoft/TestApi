// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    /// <summary>
    /// Tests for the <see cref="ObjectGraphComparisonStrategy"/> API.
    /// </summary>
    public class ObjectGraphComparisonStrategyTests
    {
        #region Sample comparison strategy tests

        [Fact]
        public void ObjectGraphComparisonStrategyCommentSample()
        {
            var l1 = new Sample("SAmplE", null);
            var l2 = new Sample("1", l1);
            var l3 = new Sample("TeeST", l2);
            var l4 = new Sample("2", l3);
            l1.Child = l4; // l4 -> l3 -> l2 -> l1 -> l4

            var r1 = new Sample("sample", null);
            var r2 = new Sample("1", r1);
            var r3 = new Sample("TeeeST", r2);
            var r4 = new Sample("2", r3);
            r1.Child = r4;

            var factory = new SampleFactory();
            var graph1 = factory.CreateObjectGraph(l4);
            var graph2 = factory.CreateObjectGraph(r4);

            IEnumerable<ObjectComparisonMismatch> mismatches;
            var result = new ObjectGraphComparer().Compare(graph1, graph2, out mismatches);
            Assert.False(result);
            Assert.Equal(1, mismatches.Count());

            var expected = "ObjectValuesDoNotMatch:Left=Sample.Sample(TeeST) Right=Sample.Sample(TeeeST)";
            var actual = TestHelpers.StringFromMismatches(mismatches)[0];
            Assert.Equal(expected, actual);
        }

        class Sample
        {
            public string Value { get; set; }

            public Sample Child { get; set; }

            public Sample(string value, Sample sample)
            {
                Value = value;
                Child = sample;
            }
        }

        class SampleFactory : ObjectGraphFactory
        {
            private static readonly ObjectGraphComparisonStrategy strategy = new SampleComparisonStrategy();

            public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
            {
                created = new Dictionary<Sample, GraphNode>();
                return CreateObjectGraph((Sample)value, null);
            }
            
            // Visits all nodes recursively
            private GraphNode CreateObjectGraph(Sample value, GraphNode parent)
            {
                if (created.ContainsKey(value))
                {
                    return created[value];
                }

                var root = new GraphNode();
                root.ObjectValue = value.Value;
                root.ComparisonStrategy = strategy;
                root.Parent = parent;
                root.Name = value.GetType().Name;
                created.Add(value, root);
                if (value.Child != null)
                {
                    var child = CreateObjectGraph(value.Child, root);
                    root.Children.Add(child);
                }

                return root;
            }

            // Keeps track of visited nodes
            private Dictionary<Sample, GraphNode> created;
        }

        class SampleComparisonStrategy : ObjectGraphComparisonStrategy
        {
            protected override IEnumerable<ObjectComparisonMismatch> Compare(GraphNode left, GraphNode right)
            {
                // This comparison strategy assumes that it is attached to correct nodes

                var leftString = (string)left.ObjectValue;
                var rightString = (string)right.ObjectValue;
                var mismatches = new List<ObjectComparisonMismatch>();
                if (!string.Equals(leftString, rightString, StringComparison.OrdinalIgnoreCase))
                {
                    mismatches.Add(new ObjectComparisonMismatch(
                        left, right, ObjectComparisonMismatchType.ObjectValuesDoNotMatch));
                }

                // Mark nodes as visited
                MarkVisited(left);
                MarkVisited(right);

                // Compare inner nodes if any
                CompareChildNodes(left, right);
                return mismatches;
            }
        }

        #endregion
    }
}
