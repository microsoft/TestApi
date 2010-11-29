// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using Microsoft.Test.CommandLineParsing;
using Xunit;
using Xunit.Extensions;

namespace Microsoft.Test.AcceptanceTests.CommandLineParsing
{
    public class CommandLineParserTests
    {
        [Theory]
        [InlineData("/Verbose", "/RunId=10", true, 10)]      // bug!
        [InlineData("/Verbose=true", "/RunId=10", true, 10)]
        [InlineData("/Verbose=True", "/RunId=10", true, 10)]
        [InlineData("/RunId=10", "/Verbose=true", true, 10)]  
        public void TestCommonUsage(string arg1, string arg2, bool? expectedVerbose, int? expectedRunId)
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { arg1, arg2 };
            a.ParseArguments(args);

            Assert.Equal<bool?>(expectedVerbose, a.Verbose);
            Assert.Equal<int?>(expectedRunId, a.RunId);
        }

        [Theory]
        [InlineData("/Verbose", "/RUNID=10", true, 10)]
        [InlineData("/Verbose", "/RunID=10", true, 10)]
        [InlineData("/VERBOSE", "/RunId=10", true, 10)]
        public void CasingScenarios(string arg1, string arg2, bool? expectedVerbose, int? expectedRunId)
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { arg1, arg2 };
            a.ParseArguments(args);

            Assert.Equal<bool?>(expectedVerbose, a.Verbose);
            Assert.Equal<int?>(expectedRunId, a.RunId);            
        }

        [Theory]
        [InlineData("/Verbose", "")]
        [InlineData("", "/RunId=10")]
        [InlineData("/sample", "/howMany=10")]
        public void ExceptionsUponMismatchedFields(string arg1, string arg2)
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { arg1, arg2 };
            Assert.Throws<ArgumentException>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }

        [Theory]
        [InlineData("/Verbose", "/RunId=true")]  // bug in docs
        [InlineData("/Verbose", "/RunId=10.1")]  // bug in docs
        public void ExceptionsUponMismatchedFields2(string arg1, string arg2)
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { arg1, arg2 };
            Assert.Throws<Exception>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }

        [Theory]
        [InlineData("/Verbose=0", "/RunId=10")]  // bug in docs
        public void ExceptionsUponMismatchedFields3(string arg1, string arg2)
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { arg1, arg2 };
            Assert.Throws<FormatException>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }

        [Fact]
        public void MoreArgsThanExpectedInTheParsedIntoClass()
        {
            CommandLineArguments a = new CommandLineArguments();
            string[] args = new string[] { "/Verbose", "/TestId=3", "/RunId=10" };
            Assert.Throws<ArgumentException>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }

        [Fact]
        public void ExceptionWhenTryingToParseInProtectedFields()
        {
            CommandLineArgumentsWithProtectedFields a = new CommandLineArgumentsWithProtectedFields();
            string[] args = new string[] { "/Verbose", "RunId=15" };
            Assert.Throws<ArgumentException>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }

        [Fact]
        public void ExceptionWhenTryingToParseInPrivateFields()
        {
            CommandLineArgumentsWithPrivateFields a = new CommandLineArgumentsWithPrivateFields();
            string[] args = new string[] { "/Verbose", "RunId=15" };
            Assert.Throws<ArgumentException>(
                delegate
                {
                    a.ParseArguments(args);
                });
        }


        //
        // Supporting classes
        //
        protected class CommandLineArguments
        {
            public bool? Verbose { get; set; }
            public int? RunId { get; set; }
        }

        protected class CommandLineArgumentsWithProtectedFields
        {
            protected bool? Verbose { get; set; }
            protected int? RunId { get; set; }
        }

        protected class CommandLineArgumentsWithPrivateFields
        {
            private bool? Verbose { get; set; }
            private int? RunId { get; set; }
        }
    }
}

