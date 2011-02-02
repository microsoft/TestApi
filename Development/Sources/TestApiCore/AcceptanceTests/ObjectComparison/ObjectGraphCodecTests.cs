// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    /// <summary>
    /// Tests for the <see cref="ObjectGraphCodec"/>.
    /// </summary>
    public class ObjectGraphCodecTests
    {
        #region XmlObjectGraphCodec tests

        [Fact]
        public void XmlObjectGraphCodecEncodingCommentSample()
        {
            var actualStream = new MemoryStream();
            var listener = new TextWriterTraceListener(actualStream);
            Trace.Listeners.Add(listener);

            // Example code starts here //

            var p1 = new Person("John");
            p1.Children.Add(new Person("Peter"));
            p1.Children.Add(new Person("Mary"));

            // Create an object graph
            var factory = new PublicPropertyObjectGraphFactory();
            var graph1 = factory.CreateObjectGraph(p1);

            // Encode a constructed object graph into XML
            var stream = new MemoryStream();
            new XmlObjectGraphCodec().EncodeObjectGraph(graph1, stream);

            // Output the resulting XML into the Console window
            stream.Position = 0;
            using (var reader = new StreamReader(stream))
            {
                Trace.WriteLine(reader.ReadToEnd());
            }

            // Example code ends here //

            var expectedXml =
"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
@"<RootObject>
    <Name>John</Name>
    <Children>
        <Count>2</Count>
        <IEnumerable0>
            <Name>Peter</Name>
            <Children>
                <Count>0</Count>
            </Children>
        </IEnumerable0>
        <IEnumerable1>
            <Name>Mary</Name>
            <Children>
                <Count>0</Count>
            </Children>
        </IEnumerable1>
    </Children>
</RootObject>";

            // 'stream' is disposed, that is why we are using TraceListener

            Trace.Flush();
            Trace.Listeners.Remove(listener);

            var actual = new XmlObjectGraphCodec().DecodeObjectGraph(actualStream);
            var expected = new XmlObjectGraphCodec().DecodeObjectGraph(
                new MemoryStream(Encoding.UTF8.GetBytes(expectedXml)));
            var result = new ObjectGraphComparer().Compare(actual, expected);
            Assert.True(result);
        }

        [Fact]
        public void XmlObjectGraphCodecRoundtripCommentSample()
        {
            var expectedXml =
"<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
@"<RootObject>
    <Name>John</Name>
    <Children>
        <Count>2</Count>
        <IEnumerable0>
            <Name>Peter</Name>
            <Children>
                <Count>0</Count>
            </Children>
        </IEnumerable0>
        <IEnumerable1>
            <Name>Mary</Name>
            <Children>
                <Count>0</Count>
            </Children>
        </IEnumerable1>
    </Children>
</RootObject>";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(expectedXml));

            // Example code starts here //

            var p1 = new Person("John");
            p1.Children.Add(new Person("Peter"));
            p1.Children.Add(new Person("Mary"));

            // Create an object graph
            var factory = new PublicPropertyObjectGraphFactory();
            var graph1 = factory.CreateObjectGraph(p1);

            // Simulate round-trip data loss
            {
                var roundtripStream = new MemoryStream();
                var xmlCodec = new XmlObjectGraphCodec();
                xmlCodec.EncodeObjectGraph(graph1, roundtripStream);
                graph1 = xmlCodec.DecodeObjectGraph(stream);
            }

            // Decode a baseline graph from the earlier prepared stream
            var graph2 = new XmlObjectGraphCodec().DecodeObjectGraph(stream);

            var result = new ObjectGraphComparer().Compare(graph1, graph2);
            Trace.WriteLine("{0}", result ? "Object graphs are equal!" : "Object graphs are NOT equal!");

            // Example code ends here //

            Assert.True(result);
        }

        [Fact]
        public void EncodesAndComparesCyclicObjectGraphs()
        {
            var root1 = new GraphNode { Name = "RootNode" };
            {
                var child1 = new GraphNode { Name = "Name", Parent = root1, ObjectValue = "Peter" };
                var child2 = new GraphNode { Name = "Value", Parent = root1 };
                var child3 = new GraphNode { Name = "Value", Parent = root1, ObjectValue = "Milk" };
                var child21 = new GraphNode { Name = "Grandchild", Parent = child2, ObjectValue = "test" };

                root1.Children.Add(child1);
                root1.Children.Add(child2);
                root1.Children.Add(child3);
                child2.Children.Add(child21);

                child21.Children.Add(root1);
            }

            var root2 = new GraphNode { Name = "RootNode" };
            {
                var child1 = new GraphNode { Name = "Name", Parent = root2, ObjectValue = "Peter" };
                var child3 = new GraphNode { Name = "Value", Parent = root2, ObjectValue = "Milk" };
                var child2 = new GraphNode { Name = "Value", Parent = root2 };
                var child21 = new GraphNode { Name = "Grandchild", Parent = child2, ObjectValue = "test" };

                root2.Children.Add(child1);
                root2.Children.Add(child3);
                root2.Children.Add(child2);
                child2.Children.Add(child21);

                child21.Children.Add(root2);
            }

            // Normally those object graphs are equal
            Assert.True(new ObjectGraphComparer().Compare(root1, root2));

            var decoded1 = TestHelpers.XmlCodecRoundtrip(root1);
            var decoded2 = TestHelpers.XmlCodecRoundtrip(root2);

            Assert.True(new ObjectGraphComparer().Compare(decoded1, decoded2));
        }

        [Fact]
        public void RestoresComparisonStrategies()
        {
            var root1 = new GraphNode { Name = "RootNode" };
            {
                var child1 = new GraphNode { Name = "Name", Parent = root1, ObjectValue = "Peter" };
                var child2 = new GraphNode { Name = "Value", Parent = root1 };
                var child3 = new GraphNode { Name = "Value", Parent = root1, ObjectValue = "MILK" };
                var child21 = new GraphNode { Name = "Grandchild", Parent = child2, ObjectValue = "TEst" };

                root1.Children.Add(child1);
                root1.Children.Add(child2);
                root1.Children.Add(child3);
                child2.Children.Add(child21);

                child3.ComparisonStrategy = new IgnoreCaseComparisonStrategy();
                child21.ComparisonStrategy = new IgnoreCaseComparisonStrategy();
            }

            var root2 = new GraphNode { Name = "RootNode" };
            {
                
                var child1 = new GraphNode { Name = "Name", Parent = root2, ObjectValue = "Peter" };
                var child3 = new GraphNode { Name = "Value", Parent = root2, ObjectValue = "Milk" };
                var child2 = new GraphNode { Name = "Value", Parent = root2 };
                var child21 = new GraphNode { Name = "Grandchild", Parent = child2, ObjectValue = "test" };

                root2.Children.Add(child1);
                root2.Children.Add(child3);
                root2.Children.Add(child2);
                child2.Children.Add(child21);

                child3.ComparisonStrategy = new IgnoreCaseComparisonStrategy();
                child21.ComparisonStrategy = new IgnoreCaseComparisonStrategy();
            }

            // Normally those object graphs are equal
            Assert.True(new ObjectGraphComparer().Compare(root1, root2));

            var strategies= new HashSet<ObjectGraphComparisonStrategy>();
            strategies.Add(new IgnoreCaseComparisonStrategy());
            var decoded1 = TestHelpers.XmlCodecRoundtrip(root1, strategies);
            Assert.True(new ObjectGraphComparer().Compare(decoded1, root2));
        }

        [Fact]
        public void ByDefaultThrowsOnUnknownComparisonStrategy()
        {
            var root = new GraphNode { Name = "Root", ComparisonStrategy = new IgnoreCaseComparisonStrategy() };
            Assert.Throws<XmlException>(() => TestHelpers.XmlCodecRoundtrip(root));
        }

        [Fact]
        public void IfSetDoesNotThrowOnUnknownComparisonStrategy()
        {
            var root = new GraphNode { Name = "Root", ComparisonStrategy = new IgnoreCaseComparisonStrategy() };
            var decoded = TestHelpers.XmlCodecRoundtrip(root, false);

            IEnumerable<ObjectComparisonMismatch> mismatches;
            var result = new ObjectGraphComparer().Compare(root, decoded, out mismatches);
            Assert.False(result);
            Assert.Equal(1, mismatches.Count());

            var expected = "ComparisonStrategiesDoNotMatch:Left=Root(Null) Right=Root(Null)";
            var actual = TestHelpers.StringFromMismatches(mismatches)[0];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WorksWithFileStream()
        {
            var p1 = new Person("John");
            p1.Children.Add(new Person("Peter"));
            p1.Children.Add(new Person("Mary"));

            var factory = new PublicPropertyObjectGraphFactory();
            var graph1 = factory.CreateObjectGraph(p1);

            // Save the object graph into a file
            var codec = new XmlObjectGraphCodec();
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                codec.EncodeObjectGraph(graph1, file);
            }

            // Read the object graph from the file
            GraphNode actual;
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                actual = codec.DecodeObjectGraph(file);
            }

            File.Delete(filePath);

            var stream = new MemoryStream();
            codec.EncodeObjectGraph(graph1, stream);
            var expected = codec.DecodeObjectGraph(stream);

            Assert.True(new ObjectGraphComparer().Compare(expected, actual));
        }

        class IgnoreCaseComparisonStrategy : ObjectGraphComparisonStrategy
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

        #region ObjectGraphCodec tests

        class SimpleCodec : ObjectGraphCodec
        {
            public override GraphNode DecodeObjectGraph(Stream stream)
            {
                stream.Position = 0;
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                var reader = new StringReader(Encoding.UTF8.GetString(bytes));
                var line = reader.ReadLine();
                var root = DecodeGraphNode(line);
                while ((line = reader.ReadLine()) != null)
                {
                    var child = DecodeGraphNode(line);
                    child.Parent = root;
                    root.Children.Add(child);
                }

                return root;
            }

            public override void EncodeObjectGraph(GraphNode root, Stream stream)
            {
                var builder = new StringBuilder();
                EncodeGraphNode(root, builder);
                foreach (var c in root.Children)
                {
                    EncodeGraphNode(c, builder);
                }

                var bytes = Encoding.UTF8.GetBytes(builder.ToString());
                stream.Write(bytes, 0, bytes.Length);
            }

            private void EncodeGraphNode(GraphNode node, StringBuilder builder)
            {
                builder.AppendFormat("[{0}]:{1}", node.Name, node.ObjectValue);
                builder.AppendLine();
            }

            private GraphNode DecodeGraphNode(string line)
            {
                var match = Regex.Match(line, @"^\[(.*)\]:(.*)");
                Debug.Assert(match.Success);
                var node = new GraphNode
                {
                    // Group #0 is the string itself, skip it
                    Name = match.Groups[1].Value,
                    ObjectValue = match.Groups[2].Value
                };

                return node;
            }
        }

        [Fact]
        public void ObjectGraphCodecExampleCodecCommentSample()
        {
            // Example code begins here //

            var p1 = new Person("John");
            p1.Children.Add(new Person("Peter"));
            p1.Children.Add(new Person("Mary"));

            // Create a sample object graph
            var factory = new PublicPropertyObjectGraphFactory();
            var graph1 = factory.CreateObjectGraph(p1);

            // Encode that object graph
            var stream = new MemoryStream();
            var codec = new SimpleCodec();
            codec.EncodeObjectGraph(graph1, stream);
            
            // Print the result
            Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer()));

            // Decode the object graph
            var graph2 = codec.DecodeObjectGraph(stream);

            // Result should be false, because we encoded only tree top nodes
            // from the whole object graph
            var result = new ObjectGraphComparer().Compare(graph1, graph2);
            Trace.WriteLine("{0}", result ? "Object graphs are equal!" : "Object graphs are NOT equal!");

            // Example code ends here //

            Assert.False(result);
        }

        [Fact]
        public void ObjectGraphCodecExampleCodecCommentSampleRoundtrip()
        {
            var p1 = new Person("John");
            p1.Children.Add(new Person("Peter"));
            p1.Children.Add(new Person("Mary"));

            var factory = new PublicPropertyObjectGraphFactory();
            var graph1 = factory.CreateObjectGraph(p1);

            var stream = new MemoryStream();
            var codec = new SimpleCodec();
            codec.EncodeObjectGraph(graph1, stream);

            // Round trip original object graph
            graph1 = codec.DecodeObjectGraph(stream);
            var graph2 = codec.DecodeObjectGraph(stream);

            var result = new ObjectGraphComparer().Compare(graph1, graph2);
            Assert.True(result);
        }

        #endregion

        class Person
        {
            public Person(string name)
            {
                Name = name;
                Children = new Collection<Person>();
            }
            public string Name { get; set; }
            public Collection<Person> Children { get; private set; }
        }
    }
}
