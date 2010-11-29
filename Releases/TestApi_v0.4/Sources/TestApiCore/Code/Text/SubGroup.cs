using System;


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


