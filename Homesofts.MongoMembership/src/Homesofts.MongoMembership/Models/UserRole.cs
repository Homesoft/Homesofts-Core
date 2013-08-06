using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Homesofts.Extension;

namespace Homesofts.MongoMembership.Models
{
    [BsonIgnoreExtraElements]
    public class UserRole : BaseSecurityModel
    {
        protected UserRole() { }
        public UserRole(User user, Role role)
        {
            assertValidUserAndRole(user, role);
            UserId = user.Id;
            UserName = user.Name;
            RoleId = role.Id;
            RoleName = role.Name;
            this.ApplicationName = role.ApplicationName;
        }
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public Guid RoleId { get; private set; }
        public string RoleName { get; private set; }
        public override string ApplicationName { get; set; }
        [BsonIgnore]
        protected override object Key
        {
            get { return String.Concat(UserId.ToString(), ".", RoleId.ToString(), ".", ApplicationName); }
        }

        private void assertValidUserAndRole(User user, Role role)
        {
            user.ReportIfNull("User cannot be null");
            role.ReportIfNull("Role cannot be null");
            if (user.ApplicationName != role.ApplicationName)
                throw new Exception(String.Format("Cannot add user {0} of {1} to {2} of {3}, because different application name", user, user.ApplicationName, role, role.ApplicationName));
        }
    }
}
