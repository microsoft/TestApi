// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.VisualVerification
{
    /// <summary>
    /// Base class for all Snapshot verifier types.
    /// This establishes a single method contract: Verify(Snapshot).
    /// </summary>
    public abstract class SnapshotVerifier
    {
        /// <summary>
        /// Verifies the specified Snapshot instance against the current settings of the SnapshotVerifier instance.
        /// </summary>
        /// <param name="image">The image to be verified.</param>
        /// <returns>The verification result based on the supplied image and the current settings of the SnapshotVerifier instance.</returns>
        public abstract VerificationResult Verify(Snapshot image);
    }
}
