using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Homesofts.MongoConfiguration.Section;
using System.Configuration;
using MongoDB.Driver;

namespace Homesofts.MongoConfiguration
{
    public class MongoConfig
    {
        private MongoConfig()
        {
            MongoSection mongodbSection = (MongoSection)ConfigurationManager.GetSection(MongoSection.DEFAULT_SECTION);

            MongoClientSettings settings = new MongoClientSettings();
            settings.ConnectionMode = mongodbSection.ConnectionMode;
            settings.Credentials = new[] { MongoCredential.CreateMongoCRCredential(mongodbSection.Database, mongodbSection.UserName, mongodbSection.Password) };
            settings.WriteConcern = WriteConcern.Acknowledged;
            settings.ReadPreference = new ReadPreference(ReadPreferenceMode.Primary);

            if (mongodbSection.ReplicaSetName.IsNotNullAndEmpty())
                settings.ReplicaSetName = mongodbSection.ReplicaSetName;

            if (mongodbSection.ServerAddresses.Count == 0)
                throw new ConfigurationErrorsException("No server has been define in configuration");

            var servers = new List<MongoServerAddress>();
            foreach (ServerAddress serverAddr in mongodbSection.ServerAddresses)
            {
                servers.Add(new MongoServerAddress(serverAddr.Server, serverAddr.Port));
            }
            settings.Servers = servers;
            Client = new MongoClient(settings);
            Server = Client.GetServer();
            Database = Server.GetDatabase(mongodbSection.Database);
        }

        public static MongoConfig Instance { get { return new MongoConfig(); } }
        public MongoClient Client { get; private set; }
        public MongoServer Server { get; private set; }
        public MongoDatabase Database { get; private set; }
    }
    static class ObjectExtension
    {
        public static bool IsNotNullAndEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str);
        }
    }
}
