using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Homesofts.MongoConfiguration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class EnsureIndexAttribute : Attribute
    {
        public EnsureIndexAttribute(params string[] keys) : this(IndexConstraints.Normal, keys) { }
        public EnsureIndexAttribute(IndexConstraints ic, params string[] keys)
        {
            this.Descending = ((ic & IndexConstraints.Descending) != 0);
            this.Unique = ((ic & IndexConstraints.Unique) != 0); ;
            this.Sparse = ((ic & IndexConstraints.Sparse) != 0); ;
            this.Keys = keys;
        }
        public bool Descending { get; private set; }
        public bool Unique { get; private set; }
        public bool Sparse { get; private set; }
        public string[] Keys { get; private set; }
    }
    public enum IndexConstraints
    {
        Normal = 0x00000001,
        Descending = 0x00000010,
        Unique = 0x00000100,
        Sparse = 0x00001000
    }
}
