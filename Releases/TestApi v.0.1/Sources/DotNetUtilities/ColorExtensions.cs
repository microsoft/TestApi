using System;
using System.Drawing;

namespace Microsoft.Test
{
    /// <summary>
    /// Container for internal Extension methods on the Color type.
    /// </summary>
    internal static class ColorExtensions
    {
        #region Internal Static Members
        /// <summary>
        /// Compares colors by producing an absolute valued Color Difference object
        /// </summary>
        /// <param name="color1">The first color</param>
        /// <param name="color2">The second colar</param>
        /// <returns>The Color Difference of the two colors</returns>
        internal static ColorDifference CompareColors(this Color color1, Color color2)
        {
            ColorDifference diff = new ColorDifference();
            diff.A = (byte)Math.Abs(color1.A - color2.A);
            diff.R = (byte)Math.Abs(color1.R - color2.R);
            diff.G = (byte)Math.Abs(color1.G - color2.G);
            diff.B = (byte)Math.Abs(color1.B - color2.B);
            return diff;
        }

        /// <summary>
        /// Color differencing helper for snapshot comparisons.
        /// </summary>
        /// <param name="color1">The first color</param>
        /// <param name="color2">The second color</param>
        /// <param name="subtractAlpha">If set to false, the Alpha channel is overridden to full opacity, rather than the difference. 
        /// This is important for visualization, especially if both colors are fully opaque, as the difference produces a fully transparent difference.</param>
        /// <returns></returns>
        internal static Color SubtractColors(this Color color1, Color color2, bool subtractAlpha)
        {
            ColorDifference diff = CompareColors(color1, color2);
            if (!subtractAlpha)
            {
                diff.A = 255;
            }
            return Color.FromArgb(diff.A, diff.R, diff.G, diff.B);
        }
        #endregion
    }
}
