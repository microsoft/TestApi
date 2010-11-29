using System;


namespace Microsoft.Test.Text
{
    internal class Group
    {
        public Group(UnicodeRange range, string groupName, string name, string ids, UnicodeChart chart)
        {
            UnicodeRange = new UnicodeRange(range);
            GroupName = groupName;
            Name = name;
            Ids = ids;
            UnicodeChart = chart;
            SubGroups = null;
        }

        public UnicodeRange UnicodeRange { get; set; }
        
        public string GroupName { get; set; }
        
        public string Name { get; set; }
        
        public string Ids { get; set; }
        
        public UnicodeChart UnicodeChart { get; set; }
        
        public SubGroup [] SubGroups { get; set; }
    }
}

