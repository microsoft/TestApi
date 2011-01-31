// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.ObjectComparison;
using Xunit;

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

        #endregion
    }
}
