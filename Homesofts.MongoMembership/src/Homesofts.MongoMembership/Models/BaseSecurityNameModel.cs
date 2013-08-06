using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homesofts.MongoConfiguration;
using Homesofts.Extension;

namespace Homesofts.MongoMembership.Models
{
    public abstract class BaseSecurityNameModel : BaseSecurityModel
    {
        string _name;

        public BaseSecurityNameModel() { }
        public BaseSecurityNameModel(Guid id, string name, string applicationName)
            : base(id, applicationName)
        {
            Name = name;
        }
        [EnsureIndex(IndexConstraints.Unique)]
        public virtual string Name
        {
            get { return _name; }
            set
            {
                AssertValidName(value);
                _name = value;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} at ({1})", Name, ApplicationName);
        }

        protected virtual void AssertValidName(string name)
        {
            var invalidChar = new List<string>();
            if (name.Contains(","))
                invalidChar.Add("','");
            //if (name.Contains(" "))
            //    invalidChar.Add("' '");
            if (name.Contains("*"))
                invalidChar.Add("'*'");
            if (name.Contains("?"))
                invalidChar.Add("'?'");
            if (invalidChar.IsNotEmpty())
                throw new Exception(String.Format("{0} name cannot contain {1}.", this.GetType().Name, String.Join(",", invalidChar.ToArray())));
            if (name.IsNullOrEmpty())
                throw new Exception(String.Format("{0} name cannot empty.", this.GetType().Name));
        }
    }
}
