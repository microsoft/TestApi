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


