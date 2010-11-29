// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;

namespace Microsoft.Test.VisualVerification
{
    /// <summary>
    /// WPF type centric helper, on top of the general purpose Snapshot.
    /// </summary>
    public static class SnapshotHelper
    {
        /// <summary>
        /// Creates a Snapshot instance from a Wpf Window.
        /// </summary>
        /// <param name="window">The Wpf Window, identifying the window to capture from.</param>
        /// <param name="windowSnapshotMode">Determines if window border region should captured as part of Snapshot.</param>
        /// <returns>A Snapshot instance of the pixels captured.</returns>
        public static Snapshot SnapshotFromWindow(Visual window, WindowSnapshotMode windowSnapshotMode)
        {
            Snapshot result;

            HwndSource source = (HwndSource)PresentationSource.FromVisual(window);
            if (source == null)
            {
                throw new InvalidOperationException("The specified Window is not being rendered.");
            }

            IntPtr windowHandle = source.Handle;

            result = Snapshot.FromWindow(windowHandle, windowSnapshotMode);

            return result;
        }
    }
}
