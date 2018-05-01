// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.ObjectComparison;
using Xunit;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Microsoft.Test.AcceptanceTests.ObjectComparison
{
    public class ObjectGraphFactoryTests
    {
        #region CustomFactory tests

        [Fact]
        public void CustomFactoryMatch()
        {
            TypeWithAttributedProperty leftObject = new TypeWithAttributedProperty()
            {
                PropertyWithoutTestAttribute = "Should not be compared",
                PropertyWithTestAttribute = "Should be compared",
            };

            TypeWithAttributedProperty rightObject = new TypeWithAttributedProperty()
            {
                PropertyWithoutTestAttribute = "Should not be compared - so this is different",
                PropertyWithTestAttribute = "Should be compared",
            };

            ExtractAttributeObjectGraphFactory factory = new ExtractAttributeObjectGraphFactory();
            ObjectGraphComparer comparer = new ObjectGraphComparer();
            var left = factory.CreateObjectGraph(leftObject);
            var right = factory.CreateObjectGraph(rightObject);

            Assert.True(comparer.Compare(left, right), "Custom compare failed");
        }

        [Fact]
        public void CustomFactoryMismatch()
        {
            TypeWithAttributedProperty leftObject = new TypeWithAttributedProperty()
            {
                PropertyWithoutTestAttribute = "Should not be compared",
                PropertyWithTestAttribute = "Should be compared",
            };

            TypeWithAttributedProperty rightObject = new TypeWithAttributedProperty()
            {
                PropertyWithoutTestAttribute = "Should not be compared - so this is different",
                PropertyWithTestAttribute = "Should be compared - and should fail because its different",
            };

            ExtractAttributeObjectGraphFactory factory = new ExtractAttributeObjectGraphFactory();
            ObjectGraphComparer comparer = new ObjectGraphComparer();
            var left = factory.CreateObjectGraph(leftObject);
            var right = factory.CreateObjectGraph(rightObject);

            Assert.False(comparer.Compare(left, right), "Custom compare passed when it should have failed");
        }

        #endregion

        #region PublicPropertyObjectGraphFactory tests

        [Fact]
        public void CallsFactoriesFromTheFactoryMap()
        {
            var o = new NamedTypeWithAttributedProperty
            {
                Name = "Ralph",
                Value = new TypeWithAttributedProperty
                {
                    PropertyWithTestAttribute = "TestValue",
                    PropertyWithoutTestAttribute = "ShouldBeIgnored"
                }
            };

            var map = new ObjectGraphFactoryMap(true);
            map[typeof(TypeWithAttributedProperty)] = new ExtractAttributeObjectGraphFactory();

            var factory = new PublicPropertyObjectGraphFactory();
            var graph = factory.CreateObjectGraph(o, map);

            var expected =
@"RootObjectValue = 'Microsoft.Test.AcceptanceTests.ObjectComparison.ObjectGraphFactoryTests+NamedTypeWithAttributedProperty' Type=Microsoft.Test.AcceptanceTests.ObjectComparison.ObjectGraphFactoryTests+NamedTypeWithAttributedProperty
    ValueValue = 'Microsoft.Test.AcceptanceTests.ObjectComparison.TypeWithAttributedProperty' Type=Microsoft.Test.AcceptanceTests.ObjectComparison.TypeWithAttributedProperty
        PropertyWithTestAttributeValue = 'TestValue' Type=System.String
    NameValue = 'Ralph' Type=System.String";
            var actual = TestHelpers.StringFromGraph(graph);
            Assert.Equal(expected, actual.Trim());
        }

        class NamedTypeWithAttributedProperty
        {
            public string Name { get; set; }

            public TypeWithAttributedProperty Value { get; set; }
        }

        [Fact]
        public void HandlesPropertiesWithNullValues()
        {
            // There was a bug that PublicPropertyObjectGraphFactory threw
            // NullReferenceException when
            //   1. factoryMap is not null
            //   2. Property value is null
            // This test case is to make sure that this bug has been fixed and
            // does not come back

            var left = typeof(string);
            var right = typeof(string);

            var fac = new PublicPropertyObjectGraphFactory();
            var comparer = new ObjectGraphComparer();

            var factoryMap = new ObjectGraphFactoryMap(false);
            factoryMap.Add(typeof(MethodBase), new StubGraphFactory());
            factoryMap.Add(typeof(Assembly), new StubGraphFactory());

            var leftNode = fac.CreateObjectGraph(left, factoryMap); // With StubFactory
            var rightNode = fac.CreateObjectGraph(right); // Without StubFactory

            bool noDifferences = comparer.Compare(leftNode, rightNode);
            Assert.False(noDifferences);

            var leftChildrenCount = leftNode.GetNodesInDepthFirstOrder().Count();
            var rightChildrenCount = rightNode.GetNodesInDepthFirstOrder().Count();
            // Make sure that we reduced size of the object graph by using StubFactory
            Assert.True(leftChildrenCount < rightChildrenCount);
        }

        class StubGraphFactory : ObjectGraphFactory
        {
            public override GraphNode CreateObjectGraph(object value, ObjectGraphFactoryMap factoryMap = null)
            {
                return new GraphNode
                {
                    Name = value == null ? "null" : value.GetType().Name,
                    ObjectValue = value
                };
            }
        }

        #endregion
    }
}
