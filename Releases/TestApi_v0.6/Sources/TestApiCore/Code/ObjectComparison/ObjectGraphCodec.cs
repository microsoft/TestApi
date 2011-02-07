// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.IO;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Represents a base class for object graph codecs.
    /// </summary>
    /// <remarks>
    /// Object graph codecs are used for encoding object graphs into 
    /// and decoding them from data streams. In other words a codec 
    /// knows how to serialize object graphs into a specific data
    /// format (e.g., XML) and how to deserialize them from that 
    /// format.
    /// </remarks>
    /// <example>
    /// The following example shows how to implement a simple codec.
    /// <c>
    /// class SimpleCodec : ObjectGraphCodec
    /// {
    ///     public override GraphNode DecodeObjectGraph(Stream stream)
    ///     {
    ///         stream.Position = 0;
    ///         var bytes = new byte[stream.Length];
    ///         stream.Read(bytes, 0, bytes.Length);
    ///         var reader = new StringReader(Encoding.UTF8.GetString(bytes));
    ///         var line = reader.ReadLine();
    ///         var root = DecodeGraphNode(line);
    ///         while ((line = reader.ReadLine()) != null)
    ///         {
    ///             var child = DecodeGraphNode(line);
    ///             child.Parent = root;
    ///             root.Children.Add(child);
    ///         }
    /// 
    ///         return root;
    ///     }
    /// 
    ///     public override void EncodeObjectGraph(GraphNode root, Stream stream)
    ///     {
    ///         var builder = new StringBuilder();
    ///         EncodeGraphNode(root, builder);
    ///         foreach (var c in root.Children)
    ///         {
    ///             EncodeGraphNode(c, builder);
    ///         }
    /// 
    ///         var bytes = Encoding.UTF8.GetBytes(builder.ToString());
    ///         stream.Write(bytes, 0, bytes.Length);
    ///     }
    /// 
    ///     private void EncodeGraphNode(GraphNode node, StringBuilder builder)
    ///     {
    ///         builder.AppendFormat("[{0}]:{1}", node.Name, node.ObjectValue);
    ///         builder.AppendLine();
    ///     }
    /// 
    ///     private GraphNode DecodeGraphNode(string line)
    ///     {
    ///         var match = Regex.Match(line, @"^\[(.*)\]:(.*)");
    ///         Debug.Assert(match.Success);
    ///         var node = new GraphNode
    ///         {
    ///             // Group #0 is the string itself, skip it
    ///             Name = match.Groups[1].Value,
    ///             ObjectValue = match.Groups[2].Value
    ///         };
    /// 
    ///         return node;
    ///     }
    /// }
    /// </c>
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
    /// // Create a sample object graph
    /// var factory = new PublicPropertyObjectGraphFactory();
    /// var graph1 = factory.CreateObjectGraph(p1);
    /// 
    /// // Encode that object graph
    /// var stream = new MemoryStream();
    /// var codec = new SimpleCodec();
    /// codec.EncodeObjectGraph(graph1, stream);
    ///         
    /// // Print the result
    /// Console.WriteLine(Encoding.UTF8.GetString(stream.GetBuffer()));
    /// 
    /// // Decode the object graph
    /// var graph2 = codec.DecodeObjectGraph(stream);
    /// 
    /// // Result should be false, because we encoded only tree top nodes
    /// // from the whole object graph
    /// var result = new ObjectGraphComparer().Compare(graph1, graph2);
    /// Trace.WriteLine("{0}", result ? "Object graphs are equal!" : "Object graphs are NOT equal!");
    /// </c>
    /// </example>
    public abstract class ObjectGraphCodec
    {
        #region Public Members

        /// <summary>
        /// Decodes an object graph from a stream.
        /// </summary>
        /// <param name="stream">The stream containing an encoded object graph.</param>
        /// <returns>The decoded object graph.</returns>
        public abstract GraphNode DecodeObjectGraph(Stream stream);

        /// <summary>
        /// Encodes an object graph and saves it into a stream.
        /// </summary>
        /// <param name="root">The root of an object graph to be encoded.</param>
        /// <param name="stream">The stream to save the encoded object graph to.</param>
        public abstract void EncodeObjectGraph(GraphNode root, Stream stream);

        #endregion
    }
}
