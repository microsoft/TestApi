// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ObjectComparison
{
    /// <summary>
    /// Creates a graph for the provided object.
    /// </summary>
    /// <example>
    /// The following example demonstrates the use of a simple factory to do shallow comparison of two objects.
    /// <code lang="C#" >
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
    /// </code>
    /// <code lang="C#" >
    /// class SimpleObjectGraphFactory : ObjectGraphFactory
    /// {
    ///     public override GraphNode CreateObjectGraph(object o, ObjectGraphFactoryMap factoryMap = null)
    ///     {
    ///         // Build the object graph with nodes that need to be compared.
    ///         // in this particular case, we only pick up the object itself
    ///         GraphNode node = new GraphNode();
    ///         node.Name = "PersonObject";
    ///         node.ObjectValue = (o as Person).Name;
    ///         return node;
    ///     }
    /// }
    /// </code>
    /// <code lang="C#" >
    /// Person p1 = new Person("John");
    /// p1.Children.Add(new Person("Peter"));
    /// p1.Children.Add(new Person("Mary"));
    ///
    /// Person p2 = new Person("John");
    /// p2.Children.Add(new Person("Peter"));
    ///
    /// ObjectGraphFactory factory = new SimpleObjectGraphFactory();
    /// ObjectComparer comparer = new ObjectComparer(factory);
    /// Console.WriteLine(
    ///     "Objects p1 and p2 {0}",
    ///     comparer.Compare(p1, p2) ? "match!" : "do NOT match!");
    /// </code>
    /// </example>
    public abstract class ObjectGraphFactory
    {
        /// <summary>
        /// Creates a graph for the given object.
        /// </summary>
        /// <param name="value">The object to convert.</param>
        /// <param name="factoryMap">
        /// The factory map is used when the currect factory encounters an object of a type
        /// it does not work with. In this case the factory should use the map to find a factory
        /// for that type and use it.
        /// </param>
        /// <returns>The root node of the created graph.</returns>
        public abstract GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null);
    }
}
