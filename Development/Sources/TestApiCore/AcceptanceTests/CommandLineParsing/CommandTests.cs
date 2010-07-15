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
    public class CommandTests
    {
        [Theory]
        [InlineData("/Verbose", "/RunId=10", true, 10)]
        [InlineData("/Verbose=true", "/RunId=10", true, 10)]
        [InlineData("/Verbose=True", "/RunId=10", true, 10)]
        [InlineData("/RunId=10", "/Verbose=true", true, 10)]
        public void TestCommonUsage(string arg1, string arg2, bool? expectedVerbose, int? expectedRunId)
        {
            string[] args = new string[] { arg1, arg2 };
            Command c = new RunCommand();
            c.ParseArguments(args);
            c.Execute();

            Assert.Equal<bool?>(expectedVerbose, (c as RunCommand).Verbose);
            Assert.Equal<int?>(expectedRunId, (c as RunCommand).RunId);
            Assert.Equal<bool?>(true, (c as RunCommand).GotExecuted);
        }

        [Fact]
        public void ConfirmThatCommandWithProtectedPropertiesThrows()
        {
            string[] args = new string[] { "/Verbose", "/RunId=18" };
            Command c = new CommandWithProtectedProperties();

            Assert.Throws<ArgumentException>(
                delegate
                {
                    c.ParseArguments(args);
                });
        }

        [Fact]
        public void ConfirmThatCommandWithPrivatePropertiesThrows()
        {
            string[] args = new string[] { "/Verbose", "/RunId=18" };
            Command c = new CommandWithPrivateProperties();

            Assert.Throws<ArgumentException>(
                delegate
                {
                    c.ParseArguments(args);
                });
        }



        //
        // Supporting classes
        //
        protected class RunCommand : Command
        {
            public bool? Verbose { get; set; }
            public int? RunId { get; set; }
            public bool? GotExecuted { get; private set; }

            public override void Execute()
            {
                GotExecuted = true;
            }
        }

        protected class CommandWithProtectedProperties : Command
        {
            protected bool? Verbose { get; set; }
            protected int? RunId { get; set; }

            public override void Execute()
            {
                // nothing
            }
        }

        protected class CommandWithPrivateProperties : Command
        {
            private bool? Verbose { get; set; }
            private int? RunId { get; set; }

            public override void Execute()
            {
                // nothing
            }
        }
    }
}
