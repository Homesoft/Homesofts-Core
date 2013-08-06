using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Homesofts.MongoConfiguration.Section
{
    public class ServerAddressCollection : ConfigurationElementCollection
    {
        public ServerAddress this[int index]
        {
            get { return this.Cast<ServerAddress>().ToList()[index]; }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerAddress();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            ServerAddress serverAddr = (ServerAddress)element;
            return String.Concat(serverAddr.Server, ":", serverAddr.Port);
        }
    }
}
