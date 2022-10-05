using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveDirectory.NET.Models
{
    public class ADGroup
    {
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string Name { get; set; }
        public string SamAccountName { get; set; }
        public string Mail { get; set; }
        public DateTime WhenCreated { get; set; }
        public int SamAccountType { get; set; }
        public string CN { get; set; }
        public int GroupType { get; set; }
        public int InstanceType { get; set; }
        public List<string> Members { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
