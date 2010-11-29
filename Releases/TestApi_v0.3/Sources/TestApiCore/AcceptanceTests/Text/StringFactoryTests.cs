// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using Microsoft.Test.Text;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests
{
    public class StringFactoryTests
    {
        [Fact]
        public void GenerateRandomString()
        {
            StringProperties sp = new StringProperties();

            Assert.Throws<NotImplementedException>(
                delegate
                { 
                    string s1 = StringFactory.GenerateRandomString(sp, 75);
                    string s2 = StringFactory.GenerateRandomString(sp, 75);

                    Assert.NotNull(s1);
                    Assert.NotNull(s2);
                    Assert.Equal<string>(s1, s2);
                });
        }
    }
}
