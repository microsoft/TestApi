// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Test;
using Xunit;

namespace Microsoft.Test.AcceptanceTests
{
    public class ThemeTests : IDisposable
    {
        #region Private Fields

        private Theme _currentTheme;

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
            var curTheme = Theme.GetCurrent();
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
            var availableThemes = Theme.GetAvailableSystemThemes();
            foreach (var theme in availableThemes)
            {
                Debug.WriteLine("set theme: " + theme.Path.FullName);
                Theme.SetCurrent(theme);
                WaitForThemeSet(theme.Path.FullName);

                Debug.WriteLine("Verifying theme: " + theme.Path);
                var curTheme = Theme.GetCurrent();
                VerifyThemes(theme, curTheme);
            }
        }

        [Fact]
        public void SetCurrentTest2()
        {
            var availableThemes = Theme.GetAvailableSystemThemes();
            foreach (var theme in availableThemes)
            {
                Debug.WriteLine("set theme: " + theme.Path.FullName);
                Theme.SetCurrent(new FileInfo(theme.Path.FullName));
                WaitForThemeSet(theme.Path.FullName);
                
                Debug.WriteLine("Verifying theme: " + theme.Path);
                var curTheme = Theme.GetCurrent();
                VerifyThemes(theme, curTheme);
            }
        }

        [Fact]
        public void GetAvailableSystemThemesTest()
        {
            var systemThemes = Theme.GetAvailableSystemThemes();
            Assert.NotNull(systemThemes);
            Assert.True(systemThemes.Length > 0);
        }

        #endregion Tests

        #region Helpers

        private void Init()
        {
            _currentTheme = Theme.GetCurrent();
        }

        private void CleanUp()
        {
            // set theme back to original 
            var theme = Theme.GetCurrent();
            if (theme.Path != _currentTheme.Path)
            {
                Theme.SetCurrent(_currentTheme);
            }
        }

        private void WaitForThemeSet(string themeToBeSet)
        {
            int counter = 0;
            int max = 20;

            do
            {
                Thread.Sleep(1500);
                counter++;
            } while (counter < max && Theme.GetCurrent().Path.FullName.ToLower() != themeToBeSet.ToLower());
        }

        private void VerifyThemes(Theme theme1, Theme theme2)
        {
            Assert.True(theme1 == null ? theme2 == null : true);
            Assert.True(theme2 == null ? theme1 == null : true);

            Assert.Equal(theme1.Path.FullName.ToLower(), theme2.Path.FullName.ToLower());
            Assert.Equal(theme1.Style.ToLower(), theme2.Style.ToLower());
            Assert.Equal(theme1.Name.ToLower(), theme2.Name.ToLower());
            Assert.Equal(theme1.IsEnabled, theme2.IsEnabled);
        }

        #endregion Helpers
    }
}
