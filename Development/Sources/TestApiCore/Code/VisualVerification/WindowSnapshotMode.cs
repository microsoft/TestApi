// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VisualVerification
{
    /// <summary>
    /// WindowSnapshotMode determines if window border should be captured as part of Snapshot.
    /// </summary>
    public enum WindowSnapshotMode
    {
        /// <summary>
        /// Capture a snapshot of only the window client area. This mode excludes the window border.
        /// </summary>
        ExcludeWindowBorder = 0,
        /// <summary>
        /// Capture a snapshot of the entire window area. This mode includes the window border. 
        /// </summary>
        IncludeWindowBorder = 1
    }

}