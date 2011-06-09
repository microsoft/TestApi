// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Test.ObjectComparison;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    static class TestHelpers
    {
        public static string[] StringFromMismatches(IEnumerable<ObjectComparisonMismatch> mismatches)
        {
            List<string> outputLines = new List<string>();
            foreach (ObjectComparisonMismatch mismatch in mismatches)
            {
                string message = String.Format(CultureInfo.InvariantCulture,
                    "{0}:Left={1}({2}) Right={3}({4})",
                    mismatch.MismatchType,
                    mismatch.LeftObjectNode == null ? "Null" : mismatch.LeftObjectNode.QualifiedName,
                    mismatch.LeftObjectNode == null ? "Null" : mismatch.LeftObjectNode.ObjectValue ?? "Null",
                    mismatch.RightObjectNode == null ? "Null" : mismatch.RightObjectNode.QualifiedName,
                    mismatch.RightObjectNode == null ? "Null" : mismatch.RightObjectNode.ObjectValue ?? "Null");

                outputLines.Add(message);
            }

            return outputLines.ToArray();
        }

        public static string StringFromGraph(GraphNode graph)
        {
            var stringBuilder = new StringBuilder();
            IEnumerable<GraphNode> nodes = graph.GetNodesInDepthFirstOrder();
            foreach (GraphNode node in nodes)
            {
                string type = "Null";
                if (node.ObjectValue != null)
                {
                    type = node.ObjectType.FullName;
                }

                stringBuilder.AppendLine("".PadLeft(node.Depth * 4) + node.Name + "Value = '" + node.ObjectValue + "'" + " Type=" + type);
            }
            return stringBuilder.ToString();
        }

        public static void ObjectToConsole(object value, ObjectGraphFactory factory)
        {
            GraphNode root = factory.CreateObjectGraph(value);
            Console.WriteLine(StringFromGraph(root));
        }

        public static void StreamToConsole(MemoryStream stream)
        {
            var s = Encoding.UTF8.GetString(stream.GetBuffer());
            Console.WriteLine(s);
        }

        public static GraphNode XmlCodecRoundtrip(GraphNode root)
        {
            return XmlCodecRoundtrip(root, new XmlObjectGraphCodec());
        }

        public static GraphNode XmlCodecRoundtrip(GraphNode root, bool throwOnUnknownStrategy)
        {
            var codec = new XmlObjectGraphCodec();
            codec.ThrowExceptionOnUnknownComparisonStrategy = throwOnUnknownStrategy;
            return XmlCodecRoundtrip(root, codec);
        }

        public static GraphNode XmlCodecRoundtrip(GraphNode root, HashSet<ObjectGraphComparisonStrategy> strategies)
        {
            return XmlCodecRoundtrip(root, new XmlObjectGraphCodec(strategies));
        }

        public static GraphNode XmlCodecRoundtrip(GraphNode root, XmlObjectGraphCodec codec)
        {
            var stream = new MemoryStream();
            codec.EncodeObjectGraph(root, stream);
            return codec.DecodeObjectGraph(stream);
        }

        public static GraphNode XmlCodecFileRoundtrip(GraphNode graph)
        {
            // Save the object graph into a file
            var codec = new XmlObjectGraphCodec();
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                codec.EncodeObjectGraph(graph, file);
            }

            // Read the object graph from the file
            GraphNode actual;
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                actual = codec.DecodeObjectGraph(file);
            }

            File.Delete(filePath);

            return actual;
        }
    }
}
