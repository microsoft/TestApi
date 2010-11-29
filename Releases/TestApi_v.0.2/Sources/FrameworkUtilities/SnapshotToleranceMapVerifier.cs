// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Drawing;

namespace Microsoft.Test
{
    /// <summary>
    /// Verifies that all pixels in a Snapshot are within the tolerance range, defined by the tolerance map.
    /// </summary>
    /// <example>
    /// This sample demonstrates the use of SnapshotToleranceMapVerifier.
    /// <code>
    /**
        Snapshot expected = Shapshot.FromFile("expectedImage.png");
        Shapshot actual = Snapshot.FromRectangle(new Rectangle(10, 10, 200, 100));
        Snapshot diff = actual.CompareTo(master);
        
        Snapshot toleranceMap = Snapshot.FromFile("expectedImageTolerances.png");

        SnapshotVerifier v = new SnapshotToleranceMapVerifier(toleranceMap);
        if (v.Verify() == VerificationResult.Pass)
        {
            // Log success
        }
        else
        {
            // Log failure. Store the actual snapshot and the diff for investigation
            actual.ToFile("actualImage.png");
            diff.ToFile("diffImage.png");
        }
    */
    /// </code>
    /// </example>
    public class SnapshotToleranceMapVerifier : SnapshotVerifier
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SnapshotToleranceMapVerifier class, using the specified tolerance map. 
        /// </summary>
        /// <param name="toleranceMap">
        /// A Snapshot instance defining the tolerance map, used by the verifier.
        /// A black tolerance map (a snapshot, where all pixels are with zero values) means zero tolerance. 
        /// A white tolerance map (a snapshot, where all pixels are with value 0xFF) means infinitely high tolerance.
        /// </param>
        public SnapshotToleranceMapVerifier(Snapshot toleranceMap)
        {
            this.ToleranceMap = toleranceMap;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Ensures that the image colors are all with smaller values than the image colors of the tolerance map.
        /// </summary>
        /// <param name="image">The actual image being verified.</param>
        /// <returns>A VerificationResult enumeration value based on the image, and the tolerance map.</returns>
        public override VerificationResult Verify(Snapshot image)
        {
            if (image.Width != ToleranceMap.Width || image.Height != ToleranceMap.Height)
            {
                throw new InvalidOperationException("image size must match expected size.");
            }

            for (int row = 0; row < image.Height; row++)
            {
                for (int column = 0; column < image.Width; column++)
                {
                    if (image[row, column].A > ToleranceMap[row, column].A ||
                        image[row, column].R > ToleranceMap[row, column].R ||
                        image[row, column].G > ToleranceMap[row, column].G ||
                        image[row, column].B > ToleranceMap[row, column].B)
                    {
                        //Exit early as we have a counter-example to prove failure.
                        return VerificationResult.Fail;
                    }
                }
            }
            return VerificationResult.Pass;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A Snapshot defining the tolerance map used by the verifier.
        /// A black tolerance map (a snapshot, where all pixels are with zero values) means zero tolerance. 
        /// A white tolerance map (a snapshot, where all pixels are with value 0xFF) means infinitely high tolerance.
        /// </summary>
        public Snapshot ToleranceMap { get; set; }

        #endregion
    }
}
