using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using Homesofts.MongoConfiguration.Test.models;
using MongoDB.Driver;

namespace Homesofts.MongoConfiguration.Test
{
    class when_search_text
    {
        static string id = Guid.NewGuid().ToString();
        static MongoCollection collection;

        Establish context = () =>
        {
            collection = MongoConfig.Instance.Database.GetCollection<TestDocument>();
        };

        Because of = () =>
        {
            TestDocument model = new TestDocument { Id = id, Name = "Denny", Value = 15 };
            IMongoIndexKeys keys = new IndexKeysDocument(new MongoDB.Bson.BsonElement("Name","text"));
            collection.EnsureIndex(keys);
            collection.Save(model);
        };

        It test = () =>
        {
            var commandDocument = new CommandDocument
            {
                {"text", "TestDocuments" },
                {"search", "Den" },
                {"limit", 10}
            };
            var result = MongoConfig.Instance.Database.RunCommand(commandDocument);
            var doc = result.Response["results"][0]["obj"];
            doc.ShouldNotBeNull();
        };
    }
}
