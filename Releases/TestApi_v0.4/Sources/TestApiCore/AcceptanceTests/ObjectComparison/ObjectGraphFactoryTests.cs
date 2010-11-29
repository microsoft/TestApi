// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.ObjectComparison;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
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
            ObjectComparer comparer = new ObjectComparer(factory);

            Assert.True(comparer.Compare(leftObject, rightObject), "Custom compare failed");
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
            ObjectComparer comparer = new ObjectComparer(factory);

            Assert.False(comparer.Compare(leftObject, rightObject), "Custom compare passed when it should have failed");
        }

        #endregion
    }
}
