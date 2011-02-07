// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    public class ObjectGraphFactoryMapTests
    {
        [Fact]
        public void CanAddFactoryExactMatch()
        {
            var map = new ObjectGraphFactoryMap(true);
            map.Add(typeof(Type), new PublicPropertyObjectGraphFactory());

            var factory = map[typeof(Type)];
            Assert.Equal(typeof(PublicPropertyObjectGraphFactory), factory.GetType());
        }

        [Fact]
        public void CanAddNonGenericTypeDefinitionFactoryExactMatch()
        {
            var map = new ObjectGraphFactoryMap(true);
            map.Add(typeof(Func<int>), new PublicPropertyObjectGraphFactory());

            Assert.True(map.ContainsKey(typeof(Func<>)));
            Assert.True(map.ContainsKey(typeof(Func<Type>)));
            Assert.Equal(1, map.Values.Count);

            var factory = map[typeof(Func<>)];
            Assert.Equal(typeof(PublicPropertyObjectGraphFactory), factory.GetType());
        }

        [Fact]
        public void CanRemoveFactoryExactMatch()
        {
            var map = new ObjectGraphFactoryMap(true);
            map.Add(typeof(Type), new PublicPropertyObjectGraphFactory());
            Assert.True(map.ContainsKey(typeof(Type)));

            map.Remove(typeof(Type));
            Assert.False(map.ContainsKey(typeof(Type)));
        }

        [Fact]
        public void CanRemoveNonGenericTypeDefinitionFactoryExactMatch()
        {
            var map = new ObjectGraphFactoryMap(true);
            map.Add(typeof(List<string>), new PublicPropertyObjectGraphFactory());
            Assert.True(map.ContainsKey(typeof(List<>)));

            map.Remove(typeof(List<>));
            Assert.False(map.ContainsKey(typeof(Type)));
            Assert.Equal(0, map.Values.Count);
        }

        [Fact]
        public void CanAddFactoryNonExactMatch()
        {
            var map = new ObjectGraphFactoryMap(false);
            map.Add(typeof(MemberInfo), new PublicPropertyObjectGraphFactory());
            map.Add(typeof(object), new GraphFactory2());
            map.Add(typeof(MethodInfo), new GraphFactory1());

            Assert.Equal(3, map.Keys.Count);

            var f = map[typeof(MemberInfo)];
            Assert.Equal(typeof(PublicPropertyObjectGraphFactory), f.GetType());
            f = map[typeof(ConstructorInfo)];
            Assert.Equal(typeof(PublicPropertyObjectGraphFactory), f.GetType());
            f = map[typeof(MethodInfo)];
            Assert.Equal(typeof(GraphFactory1), f.GetType());
            f = map[typeof(AttributeTargets)];
            Assert.Equal(typeof(GraphFactory2), f.GetType());
            f = map[typeof(Predicate<object>)];
            Assert.Equal(typeof(GraphFactory2), f.GetType());
        }

        [Fact]
        public void CanAddFactoryForInterfaceNonExactMatch()
        {
            var map = new ObjectGraphFactoryMap(false);
            map.Add(typeof(IEnumerable), new GraphFactory1());

            Assert.Equal(1, map.Keys.Count);

            var f = map[typeof(IEnumerable)];
            Assert.Equal(typeof(GraphFactory1), f.GetType());

            f = map[typeof(ICollection<string>)];
            Assert.Equal(typeof(GraphFactory1), f.GetType());

            f = map[typeof(IDictionary<,>)];
            Assert.Equal(typeof(GraphFactory1), f.GetType());

            Assert.Throws<KeyNotFoundException>(() => map[typeof(object)]);
        }

        [Fact]
        public void ThrowsOnAmbiguousMatch()
        {
            var map = new ObjectGraphFactoryMap(false);
            map.Add(typeof(ICollection), new GraphFactory1());
            map.Add(typeof(ISerializable), new GraphFactory2());

            Assert.Equal(2, map.Keys.Count);
            Assert.Throws<ArgumentException>(() => map[typeof(Dictionary<,>)]);
        }

        [Fact]
        public void DoesNotThrowOnMultipleNonAmbiguousMatch()
        {
            var map = new ObjectGraphFactoryMap(false);
            map.Add(typeof(ICollection), new GraphFactory1());
            map.Add(typeof(ISerializable), new GraphFactory1());

            Assert.Equal(2, map.Keys.Count);
            Assert.Equal(typeof(GraphFactory1), map[typeof(Dictionary<,>)].GetType());
        }

        class GraphFactory1 : ObjectGraphFactory
        {
            public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
            {
                throw new NotImplementedException();
            }
        }

        class GraphFactory2 : ObjectGraphFactory
        {
            public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
            {
                throw new NotImplementedException();
            }
        }
    }
}
