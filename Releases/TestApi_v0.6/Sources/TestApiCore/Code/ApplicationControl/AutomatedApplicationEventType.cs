// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;

namespace Microsoft.Test.ApplicationControl
{
    /// <summary>
    /// Specifies the supported AutomatedApplication events.
    /// </summary>
    [Serializable]
    public enum AutomatedApplicationEventType
    {
        /// <summary>
        /// The test application's main window opened event.
        /// </summary>
        MainWindowOpenedEvent,

        /// <summary>
        /// The test application's closed event.
        /// </summary>
        ApplicationExitedEvent,

        /// <summary>
        /// The test application's main window's focus changed event.
        /// </summary>
        FocusChangedEvent
    }
}
