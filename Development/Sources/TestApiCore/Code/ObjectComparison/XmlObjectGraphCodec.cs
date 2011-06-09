// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents a codec for encoding object graphs to and decoding them from XML.
    /// </summary>
    /// <remarks>
    /// Please note that there is a possible data loss associated with encoding an object 
    /// graph into an XML format. E.g., <see cref="XmlObjectGraphCodec"/> does not
    /// persist information about data types. In particular it affects graph node comparison.
    /// Consider two object graphs: the first one, OG1, was built from a 'live' object, the
    /// second one, OG2, was decoded from XML. Before comparing those two object graphs you
    /// might need to 'round-trip' OG1 through <see cref="XmlObjectGraphCodec"/> to simulate
    /// the same data loss as the one occurred to OG2.
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to use <see cref="XmlObjectGraphCodec"/>
    /// to encode an object graph into XML.
    /// <c>
    /// class Person
    /// {
    ///     public Person(string name)
    ///     {
    ///         Name = name;
    ///         Children = new Collection&lt;Person&gt;();
    ///     }
    ///     public string Name { get; set; }
    ///     public Collection&lt;Person&gt; Children { get; private set; }
    /// }
    /// </c>
    /// <c>
    /// var p1 = new Person("John");
    /// p1.Children.Add(new Person("Peter"));
    /// p1.Children.Add(new Person("Mary"));
    ///
    /// // Create an object graph
    /// var factory = new PublicPropertyObjectGraphFactory();
    /// var graph1 = factory.CreateObjectGraph(p1);
    ///
    /// // Encode a constructed object graph into XML
    /// var stream = new MemoryStream();
    /// new XmlObjectGraphCodec().EncodeObjectGraph(graph1, stream);
    ///
    /// // Output the resulting XML into the Console window
    /// stream.Position = 0;
    /// using (var reader = new StreamReader(stream))
    /// {
    ///     Trace.WriteLine(reader.ReadToEnd());
    /// }
    /// </c>
    /// <para/>
    /// The example below shows to perform a round-trip mentioned before to simulate
    /// encoding-decoding data loss.
    /// <c>
    /// var p1 = new Person("John");
    /// p1.Children.Add(new Person("Peter"));
    /// p1.Children.Add(new Person("Mary"));
    /// 
    /// // Create an object graph
    /// var factory = new PublicPropertyObjectGraphFactory();
    /// var graph1 = factory.CreateObjectGraph(p1);
    /// 
    /// // Simulate round-trip data loss
    /// {
    ///     var roundtripStream = new MemoryStream();
    ///     var xmlCodec = new XmlObjectGraphCodec();
    ///     xmlCodec.EncodeObjectGraph(graph1, roundtripStream);
    ///     graph1 = xmlCodec.DecodeObjectGraph(stream);
    /// }
    /// 
    /// // Decode a baseline graph from the earlier prepared stream
    /// var graph2 = new XmlObjectGraphCodec().DecodeObjectGraph(stream);
    /// 
    /// var result = new ObjectGraphComparer().Compare(graph1, graph2);
    /// Trace.WriteLine("{0}", result ? "Object graphs are equal!" : "Object graphs are NOT equal!");
    /// </c>
    /// </example>
    public class XmlObjectGraphCodec : ObjectGraphCodec
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlObjectGraphCodec"/> class.
        /// </summary>
        public XmlObjectGraphCodec()
            : this(new HashSet<ObjectGraphComparisonStrategy>()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlObjectGraphCodec"/> class with
        /// the collection of comparison strategies.
        /// </summary>
        /// <param name="strategies">
        /// The set of strategies. This set is used during decoding of object graphs
        /// from an XML stream to restore comparison strategies originally attached to object graph
        /// nodes. See the <see cref="EncodeObjectGraph"/> method for more details.
        /// </param>
        public XmlObjectGraphCodec(HashSet<ObjectGraphComparisonStrategy> strategies)
        {
            if (strategies == null)
            {
                throw new ArgumentNullException("strategies");
            }

            // Build the dictionary of strategies
            this.strategies = new Dictionary<string, ObjectGraphComparisonStrategy>();
            foreach (var s in strategies)
            {
                this.strategies.Add(GetStrategyName(s), s);
            }

            ThrowExceptionOnUnknownComparisonStrategy = true;
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether an exception should be thrown 
        /// during decoding of an object graph, if XML data for some node of the graph
        /// specifies a comparison strategy unknown to the <see cref="XmlObjectGraphCodec"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if an exception should be thrown; otherwise, <c>false</c>. 
        /// 	The default is <c>true</c>.
        /// </value>
        /// <remarks>
        /// Comparison strategies can be specified during construction time using the
        /// <see cref="XmlObjectGraphCodec(HashSet{ObjectGraphComparisonStrategy})"/>
        /// constructor.
        /// </remarks>
        public bool ThrowExceptionOnUnknownComparisonStrategy { get; set; }

        /// <summary>
        /// Decodes an object graph from XML data provided in a <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream containing XML data to decode an object graph from.</param>
        /// <returns>The decoded object graph.</returns>
        /// <remarks>
        /// The collection of comparison strategies passed into the 
        /// <see cref="XmlObjectGraphCodec(HashSet{ObjectGraphComparisonStrategy})"/>
        /// constructor is used to restore comparison strategies used to be attached to graph nodes.
        /// </remarks>
        /// <exception cref="XmlException">
        /// XML data for a graph node specifies a strategy which name is empty or whitespaces
        /// -or-
        /// XML data for a graph node specifies a strategy that cannot be found in comparison strategies 
        /// collection and the value of the <see cref="ThrowExceptionOnUnknownComparisonStrategy"/> 
        /// property is <see lang="true"/>.
        /// </exception>
        public override GraphNode DecodeObjectGraph(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            
            stream.Position = 0;

            // We might be deserializing culture sensitive information, so set invariant culture
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(stream, new XmlReaderSettings { CheckCharacters = false });

                // Move to the first content
                reader.MoveToContent();
                return CreateObjectGraphFromXml(reader);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                if (reader != null) reader.Close();
            }
        }

        /// <summary>
        /// Encodes a given object graph into XML representation and saves it 
        /// into a <paramref name="stream"/>.
        /// </summary>
        /// <param name="root">The root of an object graph to be encoded.</param>
        /// <param name="stream">The stream to save the encoded object graph to.</param>
        /// <remarks>
        /// If nodes of the object graph have comparison strategy attached, an additional
        /// XML attribute is created for those nodes to store type names of comparison
        /// strategies. The name of this attribute can be retrieved from 
        /// <see cref="ComparisonStrategyXmlAttribute"/>.
        /// <para/><see cref="System.Text.Encoding.UTF8"/> encoding is used for XML.
        /// <para/>Please note that <see cref="XmlObjectGraphCodec"/> encodes into XML 
        /// only leaf node values. That is if <see cref="GraphNode.ObjectValue"/> of a non-leaf 
        /// node is not <see lang="null"/> it is ignored.
        /// <para/>Cyclic object graphs are encoded in the depth-first order.
        /// </remarks>
        public override void EncodeObjectGraph(GraphNode root, Stream stream)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            // We might be serializing culture sensitive information, so set invariant culture
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true, CheckCharacters = false });
                SaveObjectGraphToXml(root, writer, new HashSet<GraphNode>());
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = currentCulture;
                if (writer != null) writer.Close();
            }
        }

        #region Serialization Helpers

        /// <summary>
        /// Creates the object graph from XML. Recursively calls itself if needed.
        /// </summary>
        private GraphNode CreateObjectGraphFromXml(XmlReader reader)
        {
            Debug.Assert(reader != null);

            if (reader.NodeType != XmlNodeType.Element)
            {
                throw new ArgumentException("Cannot find an XML node for deserialization.");
            }

            var root = new GraphNode { Name = reader.LocalName };
            root.ComparisonStrategy = GetStrategy(reader);
            if (IsEmptyElement(reader)) return root;

            // Iterate through XML content of a current node
            var moveCursor = true;
            while (true)
            {
                if (moveCursor) MoveToNextContent(reader);

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        var child = CreateObjectGraphFromXml(reader);
                        child.Parent = root;
                        root.Children.Add(child);
                        // Previous graph node already moved an XML cursor to the next node
                        moveCursor = false;
                        break;

                    case XmlNodeType.EndElement:
                        MoveToNextContent(reader);
                        return root; // We are done!!

                    case XmlNodeType.None:
                        throw new ArgumentException("XML stream ended unexpectedly.");

                    case XmlNodeType.Text:
                        root.ObjectValue = reader.Value;
                        break;

                    default:
                        // XmlReader.MoveToContent skips the following node types:
                        //    XmlNodeType.ProcessingInstruction
                        //    XmlNodeType.SignificantWhitespace
                        //    XmlNodeType.Whitespace
                        //    XmlNodeType.DocumentType
                        //    XmlNodeType.Comment
                        // XmlNodeType.XmlDeclaration and XmlNodeType.Document should be 
                        // skipped over in the very beginning.
                        // The following node types are not supported:
                        //    XmlNodeType.Notation
                        //    XmlNodeType.EntityReference
                        //    XmlNodeType.Entity
                        //    XmlNodeType.EndEntity
                        //    XmlNodeType.CDATA
                        //    XmlNodeType.DocumentFragment
                        throw new NotSupportedException(
                            String.Format("{0} type of XML content is not supported", reader.NodeType));
                }
            }
        }

        /// <summary>
        /// Exports the object graph into XML.
        /// </summary>
        /// <param name="root">The root of the object graph.</param>
        /// <param name="writer">The XML writer used to export the object graph.</param>
        /// <param name="visited">The visited nodes to deal with cyclic object graphs.</param>
        private static void SaveObjectGraphToXml(GraphNode root, XmlWriter writer, HashSet<GraphNode> visited)
        {
            Debug.Assert(root != null);
            Debug.Assert(writer != null);

            if (!visited.Add(root)) return;

            if (root.Name == null)
            {
                throw new InvalidOperationException("In order to serialize a graph node it must have a name");
            }

            writer.WriteStartElement(root.Name);

            if (root.ComparisonStrategy != null)
            {
                writer.WriteAttributeString(
                    ComparisonStrategyXmlAttribute,
                    GetStrategyName(root.ComparisonStrategy));
            }

            if (HasNonvisitedChildren(root, visited))
            {
                foreach (var c in root.Children)
                {
                    SaveObjectGraphToXml(c, writer, visited);
                }
            }
            else
            {
                if (root.ObjectValue != null)
                {
                    if ((root.ObjectValue.GetType().IsPrimitive &&
                         root.ObjectValue.GetType() != typeof(Char)) ||
                        root.ObjectValue.GetType() == typeof(String) ||
                        root.ObjectValue.GetType() == typeof(DateTime))
                    {
                        writer.WriteValue(root.ObjectValue);
                    }
                    else
                    {
                        writer.WriteValue(root.ObjectValue.ToString());
                    }
                }
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Determines whether the current XML <paramref name="reader"/> element it empty and moves to the
        /// next content element, if it is.
        /// </summary>
        private static bool IsEmptyElement(XmlReader reader)
        {
            Debug.Assert(reader != null);

            if (reader.NodeType != XmlNodeType.Element) return false;
            if (!reader.IsEmptyElement) return false;

            MoveToNextContent(reader);
            return true;
        }

        /// <summary>
        /// Determines whether the specified node has nonvisited children.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="visited">The visited.</param>
        private static bool HasNonvisitedChildren(GraphNode node, HashSet<GraphNode> visited)
        {
            foreach (var c in node.Children)
            {
                if (!visited.Contains(c))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Moves XML <paramref name="reader"/> cursor to the next content element.
        /// </summary>
        private static void MoveToNextContent(XmlReader reader)
        {
            Debug.Assert(reader != null);

            reader.Read();
            reader.MoveToContent();
        }

        /// <summary>
        /// Gets the name of the strategy.
        /// </summary>
        private static string GetStrategyName(ObjectGraphComparisonStrategy strategy)
        {
            Debug.Assert(strategy != null);
            return strategy.GetType().Name;
        }

        /// <summary>
        /// Tries to find the strategy by its name.
        /// </summary>
        private bool TryFindStrategy(string name, out ObjectGraphComparisonStrategy strategy)
        {
            Debug.Assert(name != null);

            if (string.IsNullOrEmpty(name.Trim()))
            {
                throw new XmlException("Comparison strategy name is empty or whitespaces.");
            }

            if (!strategies.TryGetValue(name, out strategy))
            {
                if (ThrowExceptionOnUnknownComparisonStrategy)
                {
                    throw new XmlException(
                        string.Format("Comparison strategy with an unknown name is found: {0}.", name));
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the comparison strategy for the current XML node.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        private ObjectGraphComparisonStrategy GetStrategy(XmlReader reader)
        {
            ObjectGraphComparisonStrategy strategy = null;

            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    if (string.Equals(
                        reader.Name, ComparisonStrategyXmlAttribute,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (strategy != null)
                        {
                            throw new XmlException("XML node can have only one comparison strategy attribute");
                        }

                        TryFindStrategy(reader.Value, out strategy);
                    }
                }

                reader.MoveToElement();
            }

            return strategy;
        }

        #endregion

        #region Public and Private Data

        /// <summary>
        /// Name of an XML attribute used to store a type name of a comparison strategy of a node.
        /// </summary>
        public const string ComparisonStrategyXmlAttribute = "comparison";

        private IDictionary<string, ObjectGraphComparisonStrategy> strategies;

        #endregion
    }
}
