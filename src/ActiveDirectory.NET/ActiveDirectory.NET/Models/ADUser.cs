using System;
using System.Collections.Generic;
using System.Text;

namespace ActiveDirectory.NET.Models
{
    public class ADUser
    {
        public bool AccountEnabled { get; set; }
        public long AccountExpires { get; set; }
        public string CN { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public List<string> Groups { get; set; }
        public long LastLogon { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public Guid ObjectGuid { get; set; }
        public string SamAccountName { get; set; }
        public string UserPrincipalName { get; set; }
        public DateTime WhenCreated { get; set; }
        public int SamAccountType { get; set; }
        public long LastLogonTimestamp { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
