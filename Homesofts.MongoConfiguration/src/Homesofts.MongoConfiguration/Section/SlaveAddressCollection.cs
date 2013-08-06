using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Homesofts.MongoConfiguration.Section
{
    public class SlaveAddressCollection : ConfigurationElementCollection
    {
        public SlaveAddress this[int index]
        {
            get { return this.Cast<SlaveAddress>().ToList()[index]; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SlaveAddress();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            SlaveAddress serverAddrs = (SlaveAddress)element;
            return String.Concat(serverAddrs.Server, ":", serverAddrs.Port);
        }
    }
}
