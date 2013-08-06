using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Homesofts.MongoConfiguration.Section
{
    public class ServerAddress : ConfigurationElement
    {
        [ConfigurationProperty("server")]
        public string Server
        {
            get { return (string)this["server"]; }
            set { this["server"] = value; }
        }
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }
    }
}
