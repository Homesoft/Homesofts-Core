using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using MongoDB.Driver;
using Homesofts.MongoConfiguration;
using Homesofts.MongoMembership.Models;
using System.Collections.Specialized;
using Homesofts.Extension;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace Homesofts.MongoMembership
{
    public sealed class MongoDBRoleProvider : RoleProvider
    {
        private MongoCollection<User> UsersCollection { get { return MongoConfig.Instance.Database.GetCollection<User>(); } }
        private MongoCollection<Role> RolesCollection { get { return MongoConfig.Instance.Database.GetCollection<Role>(); } }
        private MongoCollection<UserRole> UserRolesCollection { get { return MongoConfig.Instance.Database.GetCollection<UserRole>(); } }

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config.IsNull())
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = "NHRoleProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "MongoDB Role provider");
            }

            base.Initialize(name, config);

            if (String.IsNullOrEmpty(config["applicationName"]) || config["applicationName"].Trim() == "")
            {
                ApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                ApplicationName = config["applicationName"];
            }
        }

        public override string ApplicationName { get; set; }

        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            var users = findUsersByName(usernames);
            var roles = findRolesByName(rolenames);

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    UserRolesCollection.Save(role.AddUser(user));
                }
            }
        }

        public override void CreateRole(string rolename)
        {
            if (RoleExists(rolename))
                throw new Exception(String.Format("Role {0} already exists.", rolename));

            var role = new Role(rolename, ApplicationName);
            RolesCollection.Save(role);
        }

        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            if (!RoleExists(rolename))
                throw new Exception(String.Format("Role name {0} not found.", rolename));

            if (throwOnPopulatedRole && GetUsersInRole(rolename).IsNotEmpty())
                throw new Exception("Cannot delete a populated role.");

            RolesCollection.Remove(Query.And(Query.EQ("Name", rolename), Query.EQ("ApplicationName", ApplicationName)));
            UserRolesCollection.Remove(Query.And(Query.EQ("RoleName", rolename), Query.EQ("ApplicationName", ApplicationName)));
            return true;
        }

        public override string[] GetAllRoles()
        {
            return RolesCollection.FindByApplicationName(ApplicationName, 0, 0).Select(r => r.Name).ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            var cursor = UserRolesCollection.Find(Query.And(Query.EQ("UserName", username), Query.EQ("ApplicationName", ApplicationName))).SetSortOrder("RoleName");
            return cursor.Select(ur => ur.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string rolename)
        {
            var cursor = UserRolesCollection.Find(Query.And(Query.EQ("RoleName", rolename), Query.EQ("ApplicationName", ApplicationName))).SetSortOrder("UserName");
            return cursor.Select(ur => ur.UserName).ToArray();
        }

        public override bool IsUserInRole(string username, string rolename)
        {
            return UserRolesCollection.IsExist(Query.And(Query.EQ("UserName", username), Query.EQ("RoleName", rolename), Query.EQ("ApplicationName", ApplicationName)));
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            var users = findUsersByName(usernames);
            var roles = findRolesByName(rolenames);

            List<BsonValue> deleteUserRolesIds = new List<BsonValue>();
            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    var removedUserRole = role.RemoveUser(user);
                    if (removedUserRole.IsNotNull())
                        deleteUserRolesIds.Add(removedUserRole.Id);
                }
            }
            if (deleteUserRolesIds.IsNotEmpty())
                UserRolesCollection.Remove(Query.In("_id", deleteUserRolesIds));
        }

        public override bool RoleExists(string rolename)
        {
            return RolesCollection.IsExist(rolename, ApplicationName);
        }

        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            var cursor = UserRolesCollection.Find(Query.And(Query.EQ("RoleName", rolename), Query.Matches("UserName",
                BsonRegularExpression.Create(new Regex(usernameToMatch))), Query.EQ("ApplicationName", ApplicationName))).
                SetSortOrder("UserName");
            return cursor.Select(ur => ur.UserName).ToArray();
        }

        private IList<Role> findRolesByName(string[] rolenames)
        {
            var roles = findByNames<Role>(RolesCollection, rolenames, "Role {0} are not found.");
            foreach (var role in roles)
            {
                loadUserRolesFor(role);
            }
            return roles;
        }
        private void loadUserRolesFor(Role role)
        {
            var userRoles = UserRolesCollection.Find(Query.EQ("RoleId", role.Id)).SetSortOrder("RoleName").ToList();
            userRoles.ForEach(ur => role.AddUserRole(ur));
        }
        private IList<User> findUsersByName(string[] usernames)
        {
            return findByNames<User>(UsersCollection, usernames, "User {0} are not found.");
        }
        private IList<T> findByNames<T>(MongoCollection<T> collection, string[] names, string formatedErrorMessage) where T : BaseSecurityNameModel
        {
            var providers = collection.Find(Query.In("Name", names.Select(n => (BsonValue)n))).SetSortOrder("Name");

            IEnumerable<string> providersNotFound = (from name in names where !providers.Contains(p => p.Name.Equals(name)) select name);

            if (providersNotFound.IsNotEmpty())
                throw new Exception(String.Format(String.Format(formatedErrorMessage, createMessage(providersNotFound))));
            return providers.ToList();
        }
        private string createMessage(IEnumerable<string> tokenNames)
        {
            return String.Join(", ", tokenNames.ToArray());
        }
    }
}
