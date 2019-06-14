using System;
using System.Collections.Generic;
using System.Text;

namespace SnekBot.Data
{
    public class DataObject
    {
        public List<ProtectedChannel> ProtectedChannels {get;set;}
        public List<Role> Roles {get;set;}
        public List<User> Users {get;set;}
        public List<string> WordBlacklist {get;set;}
        public List<string> LinkBlacklist {get;set;}
    }

    public enum ProtectionFlags
    {
        NoLink,
        WordBL,
        LinkBL
    }

    public enum RoleType
    {
        General,
        Timeout,
        Mute
    }

    public class ProtectedChannel
    {
        public ulong ChannelId {get; set;}

        public List<ProtectionFlags> ProtectionFlags {get; set;}

        public ProtectedChannel(ulong did, List<ProtectionFlags> flags)
        {
            ChannelId = did;
            ProtectionFlags = flags;
        }
    }

    public class Role
    {
        public ulong RoleId {get;set;}

        public RoleType RoleType {get; set;}

        public Role(ulong did, RoleType rType)
        {
            RoleId = did;
            RoleType = rType;
        }
    }

    public class User
    {
        public ulong UserId {get;set;}

        public bool IsAdministrative {get; set;}

        public User(ulong did, bool isAd)
        {
            UserId = did;
            IsAdministrative = isAd;
        }
    }
}
