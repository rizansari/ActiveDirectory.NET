using ActiveDirectory.NET.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;

namespace ActiveDirectory.NET
{
    public class AD
    {
        private string _ldapAddress;
        private string _userName;
        private string _password;
        private string _domain;
        private string _domainAndUser;

        public AD(string LdapAddress)
        {
            _ldapAddress = string.Format("LDAP://{0}", LdapAddress);
        }

        public AD(string LdapAddress, string UserName, string Password, string Domain)
        {
            _ldapAddress = string.Format("LDAP://{0}", LdapAddress);

            _userName = UserName;
            _password = Password;
            _domain = Domain;
            _domainAndUser = Domain + @"\" + UserName;
        }

        private DirectoryEntry CreateDirectoryEntry()
        {
            if (string.IsNullOrWhiteSpace(_userName))
            {
                return new DirectoryEntry(_ldapAddress);
            }
            else
            {
                return new DirectoryEntry(_ldapAddress, _domainAndUser, _password);
            }
        }

        public List<ADGroup> GetGroups()
        {
            var groups = new List<ADGroup>();

            DirectoryEntry sr = CreateDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(sr);
            search.Filter = "(&(objectClass=group))";
            SearchResultCollection results = search.FindAll();

            foreach (var r in results)
            {
                var group = ToAdGroup(r);

                if (group != null)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        public List<ADGroup> GetGroups(string GroupName)
        {
            var groups = new List<ADGroup>();

            DirectoryEntry sr = CreateDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(sr);
            search.Filter = "(&(objectClass=group))";
            SearchResultCollection results = search.FindAll();

            foreach (var r in results)
            {
                var group = ToAdGroup(r);

                if (group != null && group.Name == GroupName)
                {
                    groups.Add(group);
                }
            }

            return groups;
        }

        public bool Authenticate(string UserName, string Password, string Domain)
        {
            bool ret = false;
            try
            {
                String domainAndUsername = Domain + @"\" + UserName;
                
                DirectoryEntry sr = new DirectoryEntry(_ldapAddress, domainAndUsername, Password);
                DirectorySearcher search = new DirectorySearcher(sr);
                search.Filter = "(&(objectClass=user)(objectCategory=person)(samaccountname=" + UserName +"))";
                SearchResult result = search.FindOne();

                ret = true;
            }
            catch (Exception ex)
            {
                ret = false;
            }

            return ret;
        }

        public List<ADUser> GetUsers()
        {
            var users = new List<ADUser>();

            DirectoryEntry sr = CreateDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(sr);
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            SearchResultCollection results = search.FindAll();

            foreach (var r in results)
            {
                var user = ToAdUser(r);

                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public List<ADUser> GetUsersByGroupName(string GroupName)
        {
            var groups = GetGroups(GroupName);

            var users = new List<ADUser>();

            if (groups == null || groups.Count == 0)
            {
                return users;
            }

            DirectoryEntry sr = CreateDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(sr);
            search.Filter = "(&(objectClass=user)(objectCategory=person))";
            SearchResultCollection results = search.FindAll();

            foreach (var r in results)
            {
                var user = ToAdUser(r);

                if (user != null && user.Groups != null && GroupsIntersect(user.Groups, groups))
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public List<ADUser> GetUsersByUserName(string UserName, bool Partial = false) //, string Domain = "")
        {
            var users = new List<ADUser>();

            DirectoryEntry sr = CreateDirectoryEntry();
            DirectorySearcher search = new DirectorySearcher(sr);

            string filter = "(objectClass=user)(objectCategory=person)";

            if (Partial)
            {
                filter += "(samaccountname=*" + UserName + "*)";
            }
            else
            {
                filter += "(samaccountname=" + UserName + ")";
            }

            //if (!string.IsNullOrWhiteSpace(Domain))
            //{
            //    //filter += "(dc=" + Domain + ")";
            //    filter += "(DC=traffic-tech,DC=local)";
            //}
            
            search.Filter = "(&" + filter +")";
            
            SearchResultCollection results = search.FindAll();

            foreach (var r in results)
            {
                var user = ToAdUser(r);

                if (user != null)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        private bool GroupsIntersect(List<string> groups1, List<ADGroup> groups2)
        {
            foreach (var group in groups2)
            {
                if (groups1.Contains(group.DistinguishedName))
                {
                    return true;
                }
            }

            return false;
        }

        private ADGroup ToAdGroup(object r)
        {
            var result = r as SearchResult;

            if (!result.Properties.Contains("displayname"))
            {
                return null;
            }

            var group = new ADGroup();

            if (result.Properties.Contains("samaccountname"))
            {
                group.SamAccountName = (string)result.Properties["samaccountname"][0];
            }

            if (result.Properties.Contains("displayname"))
            {
                group.DisplayName = (string)result.Properties["displayname"][0];
            }

            if (result.Properties.Contains("name"))
            {
                group.Name = (string)result.Properties["name"][0];
            }

            if (result.Properties.Contains("mail"))
            {
                group.Mail = (string)result.Properties["mail"][0];
            }

            if (result.Properties.Contains("whencreated"))
            {
                group.WhenCreated = (DateTime)result.Properties["whencreated"][0];
            }

            if (result.Properties.Contains("distinguishedname"))
            {
                group.DistinguishedName = (string)result.Properties["distinguishedname"][0];
            }

            if (result.Properties.Contains("samaccounttype"))
            {
                group.SamAccountType = (int)result.Properties["samaccounttype"][0];
            }

            if (result.Properties.Contains("cn"))
            {
                group.CN = (string)result.Properties["cn"][0];
            }

            if (result.Properties.Contains("grouptype"))
            {
                group.GroupType = (int)result.Properties["grouptype"][0];
            }

            if (result.Properties.Contains("instancetype"))
            {
                group.InstanceType = (int)result.Properties["instancetype"][0];
            }

            if (result.Properties.Contains("member"))
            {
                group.Members = new List<string>();
                foreach (var member in result.Properties["member"])
                {
                    group.Members.Add((string)member);
                }
            }

            return group;
        }

        private ADUser ToAdUser(object r)
        {
            var result = r as SearchResult;

            if (!result.Properties.Contains("displayname"))
            {
                return null;
            }

            var user = new ADUser();

            if (result.Properties.Contains("samaccountname"))
            {
                user.SamAccountName = (string)result.Properties["samaccountname"][0];
            }

            if (result.Properties.Contains("mail"))
            {
                user.Mail = (string)result.Properties["mail"][0];
            }

            if (result.Properties.Contains("displayname"))
            {
                user.DisplayName = (string)result.Properties["displayname"][0];
            }

            if (result.Properties.Contains("description"))
            {
                user.Description = (string)result.Properties["description"][0];
            }

            if (result.Properties.Contains("name"))
            {
                user.Name = (string)result.Properties["name"][0];
            }

            if (result.Properties.Contains("memberof"))
            {
                user.Groups = new List<string>();
                foreach (var group in result.Properties["memberof"])
                {
                    user.Groups.Add((string)group);
                }
            }

            if (result.Properties.Contains("cn"))
            {
                user.CN = (string)result.Properties["cn"][0];
            }

            if (result.Properties.Contains("lastlogon"))
            {
                user.LastLogon = (long)result.Properties["lastlogon"][0];
            }

            if (result.Properties.Contains("accountexpires"))
            {
                user.AccountExpires = (long)result.Properties["accountexpires"][0];
            }

            if (result.Properties.Contains("samaccounttype"))
            {
                user.SamAccountType = (int)result.Properties["samaccounttype"][0];
            }

            if (result.Properties.Contains("lastlogontimestamp"))
            {
                user.LastLogonTimestamp = (long)result.Properties["lastlogontimestamp"][0];
            }

            if (result.Properties.Contains("distinguishedname"))
            {
                user.DistinguishedName = (string)result.Properties["distinguishedname"][0];
            }

            if (result.Properties.Contains("userprincipalname"))
            {
                user.UserPrincipalName = (string)result.Properties["userprincipalname"][0];
            }

            if (result.Properties.Contains("objectguid"))
            {
                user.ObjectGuid = new Guid((byte[])result.Properties["objectguid"][0]);
            }

            return user;
        }

    }
}
