// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

namespace Microsoft.Test.Text
{
    /// <summary>
    /// sub Group DataStructure
    /// </summary>
    internal class SubGroup
    {
        /// <summary>
        /// Define LineBreakDatabase class,
        /// <a href="http://www.unicode.org/charts/">Newline</a>
        /// </summary>
        public SubGroup(UnicodeRange range, string name, string ids, UnicodeChart chart)
        {
            UnicodeRange = new UnicodeRange(range);
            SubGroupName = name;
            SubIds = ids;
            UnicodeChart = chart;
        }

        /// <summary>
        /// SubGroupRange property
        /// </summary>
        public UnicodeRange UnicodeRange { get; set; }

        /// <summary>
        /// SubGroupName property
        /// </summary>
        public string SubGroupName { get; set; }

        /// <summary>
        /// Enum Chart
        /// </summary>
        public UnicodeChart UnicodeChart { get; set; }

        /// <summary>
        /// SubIds property
        /// </summary>
        public string SubIds { get; set; }
    }
}


