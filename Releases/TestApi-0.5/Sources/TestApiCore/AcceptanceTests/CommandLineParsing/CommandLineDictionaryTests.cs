// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Globalization;
using Microsoft.Test.CommandLineParsing;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.CommandLineParsing
{
    public class CommandLineDictionaryTests
    {
        [Fact]
        public void TestCommonUsage()
        {
            string[] args = new string[] { "/verbose", "/runid=10" };

            CommandLineDictionary d = CommandLineDictionary.FromArguments(args);

            Assert.True(d.ContainsKey("verbose"));
            Assert.True(d.ContainsKey("runid"));
            Assert.Equal<int>(10, Int32.Parse(d["runid"], CultureInfo.InvariantCulture));
            Assert.Equal<string>("/verbose /runid=10", d.ToString());
            Assert.Equal<int>(2, d.Count);
        }

        [Fact]
        public void TestCommonUsageWithCustomKeyAndValueCharacters()
        {
            string[] args = new string[] { "-verbose", "-runid:10" };

            CommandLineDictionary d = CommandLineDictionary.FromArguments(args, '-', ':');

            Assert.True(d.ContainsKey("verbose"));
            Assert.True(d.ContainsKey("runid"));
            Assert.Equal<int>(10, Int32.Parse(d["runid"], CultureInfo.InvariantCulture));
            Assert.Equal<string>("-verbose -runid:10", d.ToString()); // bug!
            Assert.Equal<int>(2, d.Count);
        }

        [Theory]
        [InlineData("/verbose", "/anotherFlag")]
        [InlineData("/verbose", "/anotherFlag=true")]
        [InlineData("/verbose", "/runId=5")]
        [InlineData("/verbose", "/runId = 10")]
        [InlineData("/verbose=true", "/runId=10")]
        [InlineData("/runId=10", "/runId = 10")]           // bug! this passes, instead it should throw
        public void TestValidArguments(string arg1, string arg2)
        {
            string[] args = new string[] { arg1, arg2 };
            CommandLineDictionary d = CommandLineDictionary.FromArguments(args);
            Assert.Equal<int>(2, d.Count);
        }


        [Theory]
        [InlineData("/verbose", "/verbose")]
        [InlineData("/verbose", "/verbose=true")]
        [InlineData("/runId=10", "/runId=10")]
        [InlineData("/runId=10", "/runId=20")]
        [InlineData("/runId=10", "/RUNID=20")]
        public void TestDuplicatedArguments(string arg1, string arg2)
        {
            string[] args = new string[] { arg1, arg2 };
            Assert.Throws<ArgumentException>(
                delegate 
                {
                    CommandLineDictionary d = CommandLineDictionary.FromArguments(args);
                });
        }
    }
}

