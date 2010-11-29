// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.Test.ObjectComparison;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    /// <summary>
    /// Factory that Extracts properties which have 'ExtractProperty' attribute
    /// set on them.
    /// </summary>
    class ExtractAttributeObjectGraphFactory : ObjectGraphFactory
    {
        public override GraphNode CreateObjectGraph(object value)
        {
            // Queue of pending nodes 
            Queue<GraphNode> pendingQueue = new Queue<GraphNode>();

            // Dictionary of < object hashcode, node > - to lookup already visited objects 
            Dictionary<int, GraphNode> visitedObjects = new Dictionary<int, GraphNode>();

            // Build the root node and enqueue it 
            GraphNode root = new GraphNode()
            {
                Name = "RootObject",
                ObjectValue = value,
            };

            pendingQueue.Enqueue(root);

            while (pendingQueue.Count != 0)
            {
                GraphNode currentNode = pendingQueue.Dequeue();
                object nodeData = currentNode.ObjectValue;
                Type nodeType = currentNode.ObjectType;

                // If we have reached a leaf node -
                // no more processing is necessary
                if (nodeData == null || nodeData.GetType().IsPrimitive)
                {
                    continue;
                }

                // Handle loops by checking the visted objects 
                if (visitedObjects.Keys.Contains(nodeData.GetHashCode()))
                {
                    // Caused by a cycle - we have alredy seen this node so
                    // use the existing node instead of creating a new one
                    GraphNode prebuiltNode = visitedObjects[nodeData.GetHashCode()];
                    currentNode.Children.Add(prebuiltNode);
                    continue;
                }
                else
                {
                    visitedObjects.Add(nodeData.GetHashCode(), currentNode);
                }

                // Extract and add child nodes for current object //
                Collection<GraphNode> childNodes = GetChildNodes(nodeData);
                foreach (GraphNode childNode in childNodes)
                {
                    childNode.Parent = currentNode;
                    currentNode.Children.Add(childNode);

                    pendingQueue.Enqueue(childNode);
                }
            }

            return root;
        }

        private Collection<GraphNode> GetChildNodes(object nodeData)
        {
            Collection<GraphNode> childNodes = new Collection<GraphNode>();

            // Primitive has no children
            if (nodeData.GetType().IsPrimitive)
            {
                return childNodes;
            }

            // Get all properties with the [ExtractProperty] attribute on it 
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(
                nodeData, 
                new Attribute[] { new ExtractPropertyAttribute() });

            // Add the properties to the children collection
            foreach (PropertyDescriptor property in properties)
            {
                childNodes.Add(new GraphNode()
                {
                    Name = property.Name,
                    ObjectValue = property.GetValue(nodeData),
                });
            }

            return childNodes;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ExtractPropertyAttribute : Attribute { }

    public class TypeWithAttributedProperty
    {
        [ExtractProperty]
        public string PropertyWithTestAttribute { get; set; }

        public string PropertyWithoutTestAttribute { get; set; }
    }
}
