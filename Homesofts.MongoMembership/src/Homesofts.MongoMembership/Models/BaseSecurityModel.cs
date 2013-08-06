using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homesofts.MongoConfiguration;
using Homesofts.Extension;

namespace Homesofts.MongoMembership.Models
{
    public abstract class BaseSecurityModel
    {
        public BaseSecurityModel() { }
        public BaseSecurityModel(Guid id, string applicationName)
        {
            Id = id;
            ApplicationName = applicationName;
        }
        public Guid Id { get; private set; }
        [EnsureIndex]
        public virtual string ApplicationName { get; set; }

        public override bool Equals(object obj)
        {
            BaseSecurityModel model = obj as BaseSecurityModel;
            if (model.IsNull()) return false;
            return model.Key.Equals(Key);
        }
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
        protected abstract object Key { get; }
        public override string ToString()
        {
            return ApplicationName;
        }
    }
}
