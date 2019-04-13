// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Test.Theming;
using Xunit;

namespace Microsoft.Test.AcceptanceTests.Theming
{
    public class ThemeTests : IDisposable
    {
        #region Private Fields

        private Theme currentTheme;

        #endregion Private Fields

        public ThemeTests()
        {
            Init();
        }

        public void Dispose()
        {
            CleanUp();
        }

        #region Tests

        [Fact]
        public void GetCurrentTest()
        {
            Theme curTheme = Theme.GetCurrent();
            Assert.NotNull(curTheme);
            Assert.True(!string.IsNullOrEmpty(curTheme.Name));
            Assert.True(!string.IsNullOrEmpty(curTheme.Style));
            Assert.NotNull(curTheme.Path);
            Assert.True(!string.IsNullOrEmpty(curTheme.Path.FullName));
            Assert.True(curTheme.IsEnabled);
        }

        [Fact]
        public void SetCurrentTest1()
        {
            Theme[] availableThemes = Theme.GetAvailableSystemThemes();
            foreach (var theme in availableThemes)
            {
                Debug.WriteLine("set theme: " + theme.Path.FullName);
                Theme.SetCurrent(theme);

                Debug.WriteLine("Verifying theme: " + theme.Path);
                var curTheme = Theme.GetCurrent();
                VerifyThemes(theme, curTheme);
            }
        }

        [Fact]
        public void SetCurrentTest2()
        {
            Theme[] availableThemes = Theme.GetAvailableSystemThemes();
            foreach (var theme in availableThemes)
            {
                Debug.WriteLine("set theme: " + theme.Path.FullName);
                Theme.SetCurrent(new FileInfo(theme.Path.FullName));

                Debug.WriteLine("Verifying theme: " + theme.Path);
                var curTheme = Theme.GetCurrent();
                VerifyThemes(theme, curTheme);
            }
        }

        [Fact]
        public void GetAvailableSystemThemesTest()
        {
            Theme[] systemThemes = Theme.GetAvailableSystemThemes();
            Assert.NotNull(systemThemes);
            Assert.True(systemThemes.Length > 0);
        }

        #endregion Tests

        #region Helpers

        private void Init()
        {
            currentTheme = Theme.GetCurrent();
        }

        private void CleanUp()
        {
            // set theme back to original
            Theme theme = Theme.GetCurrent();
            if (theme.Path != currentTheme.Path)
            {
                Theme.SetCurrent(currentTheme);
            }
        }

        private void VerifyThemes(Theme theme1, Theme theme2)
        {
            Assert.True(theme1 == null ? theme2 == null : true);
            Assert.True(theme2 == null ? theme1 == null : true);

            Assert.Equal(theme1.Path.FullName.ToLower(), theme2.Path.FullName.ToLower());
            Assert.Equal(theme1.Name.ToLower(), theme2.Name.ToLower());

            if(theme1.Name.ToLower() != "windows classic")
            {
                Assert.Equal(theme1.Style.ToLower(), theme2.Style.ToLower());
                Assert.Equal(theme1.IsEnabled, theme2.IsEnabled);
            }
        }

        #endregion Helpers
    }
}
