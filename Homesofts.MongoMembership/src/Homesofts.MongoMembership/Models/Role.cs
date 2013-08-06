using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using Homesofts.Extension;

namespace Homesofts.MongoMembership.Models
{
    public class Role : BaseSecurityNameModel
    {
        public const string ADMINISTRATOR = "Administrators";

        readonly List<UserRole> _userRoles = new List<UserRole>();

        protected Role() { }
        public Role(string rolename, string applicationname)
            : base(Guid.Empty, rolename, applicationname)
        { }
        public UserRole AddUser(User user)
        {
            if (user.IsNull())
                throw new Exception("User cannot be null");
            if (_userRoles.Contains(ur => ur.UserId.Equals(user.Id)))
                throw new Exception(String.Format("User {0} is already in {1}", user.Name, this.Name));

            var userRole = new UserRole(user, this);
            _userRoles.Add(userRole);
            return userRole;
        }
        internal void AddUserRole(UserRole userRole)
        {
            _userRoles.Add(userRole);
        }
        public UserRole RemoveUser(User user)
        {
            if (user.IsNull())
                throw new Exception("User cannot be null");
            if (!_userRoles.Contains(ur => ur.UserId.Equals(user.Id)))
                throw new Exception(String.Format("User {0} is not in {1}", user.Name, this.Name));
            if (user.Name.Equals(User.ADMINISTRATOR_USER) && this.Name.Equals(ADMINISTRATOR))
                throw new Exception(String.Format("Cannot remove {0} from {1}", user, this));
            foreach (var ur in _userRoles)
            {
                if (ur.UserId.Equals(user.Id))
                {
                    _userRoles.Remove(ur);
                    return ur;
                }
            }
            return null;
        }
        [BsonIgnore]
        public IList<UserRole> UserRoles { get { return _userRoles.AsReadOnly(); } }
        [BsonIgnore]
        protected override object Key
        {
            get { return String.Concat(Name, ApplicationName); }
        }
        public override string ToString()
        {
            return this.Name;
        }
    }
}
