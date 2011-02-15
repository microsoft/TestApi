// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Creates a graph by extracting public instance properties in the object. If the
    /// property is an IEnumerable, extract the items. If an exception is thrown
    /// when accessing a property on the left object, it is considered a match if 
    /// the same exception type is thrown when accessing the property on the right
    /// object.
    /// </summary>
    ///
    /// <example>
    /// For examples, refer to <see cref="ObjectGraphComparer"/>.
    /// </example>
    public sealed class PublicPropertyObjectGraphFactory : ObjectGraphFactory
    {
        #region Public Members

        /// <summary>
        /// Creates a graph for the given object by extracting public properties.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="factoryMap">
        /// If this parameter is not equal <see lang="null"/>, the map is used
        /// for each inner object to find out whether some other factory wants 
        /// to process it.
        /// </param>
        /// <returns>The root node of the created graph.</returns>
        public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            // Queue of pending nodes 
            Queue<GraphNode> pendingQueue = new Queue<GraphNode>();

            // Dictionary of < object hashcode, node > - to lookup already visited objects 
            Dictionary<int, GraphNode> visitedObjects = new Dictionary<int, GraphNode>();

            GraphNode root;
            if (TryInvokeForeignFactory(value, factoryMap, out root))
            {
                // Everything is done for us
                return root;
            }

            // Build the root ourselves and enqueue it
            root = new GraphNode()
            {
                Name = "RootObject",
                ObjectValue = value,
            };

            pendingQueue.Enqueue(root);

            while (pendingQueue.Count != 0)
            {
                var currentNode = pendingQueue.Dequeue();
                var nodeData = currentNode.ObjectValue;
                var nodeType = currentNode.ObjectType;

                // If we have reached a leaf node -
                // no more processing is necessary
                if (IsLeafNode(nodeData, nodeType))
                {
                    continue;
                }

                // Handle loops by checking the visited objects 
                if (visitedObjects.Keys.Contains(nodeData.GetHashCode()))
                {
                    // Caused by a cycle - we have already seen this node so
                    // use the existing node instead of creating a new one
                    var prebuiltNode = visitedObjects[nodeData.GetHashCode()];
                    currentNode.Children.Add(prebuiltNode);
                    continue;
                }
                else
                {
                    visitedObjects.Add(nodeData.GetHashCode(), currentNode);
                }

                // Extract and add child nodes for current object //
                var childNodes = GetChildNodes(nodeData, factoryMap);
                foreach (var childNode in childNodes)
                {
                    childNode.Node.Parent = currentNode;
                    currentNode.Children.Add(childNode.Node);

                    // If a node was created externally, we do not need to
                    // process it any longer. It is supposed to be fully built
                    if (!childNode.CreatedExternally)
                    {
                        pendingQueue.Enqueue(childNode.Node);
                    }
                }
            }

            return root;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Given an object, get a list of tuples of the immediate child nodes.
        /// </summary>
        /// <param name="nodeData">The object whose child nodes need to be extracted</param>
        /// <param name="factoryMap">The factory map.</param>
        private Collection<GraphNodeTuple> GetChildNodes(object nodeData, ObjectGraphFactoryMap factoryMap)
        {
            var childNodes = new Collection<GraphNodeTuple>();

            // Extract and add properties 
            foreach (var child in ExtractProperties(nodeData, factoryMap))
            {
                childNodes.Add(child);
            }

            // Extract and add IEnumerable content 
            if (IsIEnumerable(nodeData))
            {
                foreach (var child in GetIEnumerableChildNodes(nodeData, factoryMap))
                {
                    childNodes.Add(child);
                }
            }

            return childNodes;
        }

        private List<GraphNodeTuple> ExtractProperties(object nodeData, ObjectGraphFactoryMap factoryMap)
        {
            var childNodes = new List<GraphNodeTuple>();

            var properties = nodeData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                object value = null;

                var parameters = property.GetIndexParameters();
                // Skip indexed properties and properties that cannot be read
                if (property.CanRead && parameters.Length == 0)
                {
                    try
                    {
                        value = property.GetValue(nodeData, null);
                    }
                    catch (Exception ex)
                    {
                        // If accessing the property threw an exception
                        // then make the type of exception as the child.
                        // Do we want to validate the entire exception object 
                        // here ? - currently not doing to improve perf.
                        value = ex.GetType().ToString();
                    }

                    var tuple = CreateGraphNode(value, factoryMap);
                    // Set a child name in any case
                    tuple.Node.Name = property.Name;
                    childNodes.Add(tuple);
                }
            };

            return childNodes;
        }

        private static List<GraphNodeTuple> GetIEnumerableChildNodes(object nodeData, ObjectGraphFactoryMap factoryMap)
        {
            var childNodes = new List<GraphNodeTuple>();

            var enumerableData = nodeData as IEnumerable;
            var enumerator = enumerableData.GetEnumerator();

            int count = 0;
            while (enumerator.MoveNext())
            {
                var tuple = CreateGraphNode(enumerator.Current, factoryMap);
                // Set a child name in any case
                tuple.Node.Name = "IEnumerable" + count++;
                childNodes.Add(tuple);
            }

            return childNodes;
        }

        private static bool IsIEnumerable(object nodeData)
        {
            IEnumerable enumerableData = nodeData as IEnumerable;
            if (enumerableData != null &&
                enumerableData.GetType().IsPrimitive == false &&
                nodeData.GetType() != typeof(System.String))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool IsLeafNode(object nodeData, Type nodeType)
        {
            return nodeData == null ||
                               nodeType.IsPrimitive ||
                               nodeType == typeof(string);
        }

        private static bool TryInvokeForeignFactory(object value, ObjectGraphFactoryMap factoryMap, out GraphNode root)
        {
            root = null;

            ObjectGraphFactory foreignFactory;
            if (value != null && factoryMap != null && factoryMap.TryGetValue(value.GetType(), out foreignFactory))
            {
                root = foreignFactory.CreateObjectGraph(value, factoryMap);
            }

            return root != null;
        }

        private static GraphNodeTuple CreateGraphNode(object value, ObjectGraphFactoryMap factoryMap)
        {
            GraphNode node;
            bool createdExternally = true;
            if (!TryInvokeForeignFactory(value, factoryMap, out node))
            {
                node = new GraphNode { ObjectValue = value };
                createdExternally = false;
            }

            return new GraphNodeTuple(node, createdExternally);
        }

        #endregion
    }
}
