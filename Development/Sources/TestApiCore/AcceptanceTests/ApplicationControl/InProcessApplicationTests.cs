// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using Microsoft.Test.ApplicationControl;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.ApplicationControl
{
    public class InProcessApplicationTests : IDisposable
    {
        public const int DefaultTimeoutInMS = 60000;

        public InProcessApplicationTests()
        {
            TestApplicationPath = "Fake path";
            TestWindowClassName = "Fake window name";
            TestAutomatedApp = new InProcessApplication(new InProcessApplicationSettings
            {
                Path = TestApplicationPath,
                InProcessApplicationType = InProcessApplicationType.InProcessSameThread,
                ApplicationImplementationFactory = new MockApplicationFactory()
            });
            TestAutomatedApp.Start();
        }

        protected virtual void MyTestCleanup()
        {
        }

        public void Dispose()
        {
            MyTestCleanup();
            GC.SuppressFinalize(this);
        }

        protected AutomatedApplication TestAutomatedApp { get; set; }
        protected string TestApplicationPath { get; set; }
        protected string TestWindowClassName { get; set; }

        [Fact]
        public void CreateTest()
        {
            Assert.NotNull(TestAutomatedApp);
            Assert.NotNull((TestAutomatedApp as InProcessApplication).ApplicationSettings);
            Assert.Equal<InProcessApplicationType>(InProcessApplicationType.InProcessSameThread, (TestAutomatedApp as InProcessApplication).ApplicationSettings.InProcessApplicationType);
            Assert.Equal<string>(TestApplicationPath, (TestAutomatedApp as InProcessApplication).ApplicationSettings.Path);
        }

        [Fact]
        public void MainWindowTest()
        {
            Assert.Throws<MockApplicationException>(
                () =>
                {
                    var temp = TestAutomatedApp.MainWindow;
                    Debug.WriteLine(temp);
                });
        }

        [Fact]
        public void WaitForMainWindowTest()
        {
            Assert.Throws<MockApplicationException>(
                () =>
                {
                    TestAutomatedApp.WaitForMainWindow(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
                });
        }

        [Fact]
        public void WaitForWindowTest()
        {
            Assert.Throws<MockApplicationException>(
                () =>
                {
                    TestAutomatedApp.WaitForWindow("TestDialog", TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
                });
        }

        [Fact]
        public void WaitForInputIdleTest()
        {
            Assert.Throws<MockApplicationException>(
                 () =>
                 {
                     TestAutomatedApp.WaitForInputIdle(TimeSpan.FromMilliseconds(DefaultTimeoutInMS));
                 });
        }

        [Fact]
        public void CloseTest()
        {
            Assert.Throws<MockApplicationException>(
                 () =>
                 {
                     TestAutomatedApp.Close();
                 });
        }

        [Fact]
        public void ApplicationDriverTest()
        {
            Assert.Throws<MockApplicationException>(
                 () =>
                 {
                     var temp = (TestAutomatedApp as InProcessApplication).ApplicationDriver;
                     Debug.WriteLine(temp);
                 });
        }
    }
}
