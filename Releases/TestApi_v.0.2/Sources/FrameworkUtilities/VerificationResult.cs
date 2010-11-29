// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test
{
    /// <summary>
    /// Specifies values used to report the outcome of the SnapshotVerifier.Verify method.
    /// </summary>
    public enum VerificationResult
    {
        /// <summary>
        /// Object does not meet Verification criteria.
        /// </summary>
        Fail = 0,
        /// <summary>
        /// Object meets Verification criteria.
        /// </summary>
        Pass = 1
    }
}


