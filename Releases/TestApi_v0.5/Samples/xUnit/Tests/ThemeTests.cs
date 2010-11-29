using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using System.Threading;
using Microsoft.Test.Theming;
using System.Diagnostics;

namespace Tests
{
    public class ThemeTests
    {
        /// <summary>
        /// Example of using the Theme helper to change to the available themes.
        /// </summary>
        [Fact]
        public void ChangeThemes()
        {
            Thread.Sleep(500);

            var startingTheme = Theme.GetCurrent();

            try
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
            finally
            {
                Theme.SetCurrent(startingTheme);
            }
        }

        private void VerifyThemes(Theme theme1, Theme theme2)
        {
            Assert.True(theme1 == null ? theme2 == null : true);
            Assert.True(theme2 == null ? theme1 == null : true);

            Assert.Equal(theme1.Path.FullName.ToLower(), theme2.Path.FullName.ToLower());
            Assert.Equal(theme1.Name.ToLower(), theme2.Name.ToLower());
        }
    }
}
