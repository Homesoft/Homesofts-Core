using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homesofts.MongoConfiguration;
using MongoDB.Bson.Serialization.Attributes;
using System.Web.Security;
using Homesofts.Extension;

namespace Homesofts.MongoMembership.Models
{
    public class User : BaseSecurityNameModel
    {
        public const string ADMINISTRATOR_USER = "Administrator";

        protected User() { }
        private User(Guid id, string name, string applicationName) : base(id, name, applicationName) { }
        public User(string username, string applicationname, string email, string password)
            : base(Guid.Empty, username, applicationname)
        {
            Email = email;
            Password = ASCIIEncoding.ASCII.GetBytes(password);
        }
        public string ProviderName { get; set; }
        public string EmployeeName { get; set; }
        [EnsureIndex(IndexConstraints.Unique)]
        public string Email { get; set; }
        public string Comment { get; set; }
        public bool ResetPasswordOnFirstLogin { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateCreated { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastActivityDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastLoginDate { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastPasswordChangedDate { get; set; }
        public bool IsLockedOut { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastLockedOutDate { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }

        public override string ToString()
        {
            return String.Format("{0}({1})", this.Name, Email);
        }
        protected override object Key { get { return Email; } }
        public static implicit operator MembershipUser(User user)
        {
            if (user.IsNull()) return null;
            return new MembershipUser(user.ProviderName, user.Name, user, user.Email, user.PasswordQuestion,
                user.Comment, user.IsApproved, user.IsLockedOut, user.DateCreated, user.LastLoginDate, user.LastActivityDate,
                user.LastPasswordChangedDate, user.LastLockedOutDate);
        }
        public static implicit operator User(MembershipUser user)
        {
            if (user.IsNull()) return null;
            return user.ProviderUserKey as User;
        }
    }
}
