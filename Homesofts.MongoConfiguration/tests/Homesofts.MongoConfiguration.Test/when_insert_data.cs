using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;
using MongoDB.Driver;
using Builder = MongoDB.Driver.Builders;
using MongoDB.Bson;
using Homesofts.MongoConfiguration.Test.models;

namespace Homesofts.MongoConfiguration.Test
{
    [Subject("Insert Data")]
    public class when_insert_data
    {
        static MongoCollection<TestDocument> testCollection;
        static string id;

        Establish context = () =>
            {
                id = Guid.NewGuid().ToString();
                testCollection = MongoConfig.Instance.Database.GetCollection<TestDocument>();
            };

        Because of = () =>
            {
                TestDocument model = new TestDocument { Id = id, Name = "Denny", Value = 13 };
                testCollection.Insert(model);
            };

        It should_be_inserted = () =>
            {
                var result = testCollection.FindOneById(id);
                result.ShouldNotBeNull();
            };

        It should_be_index_created = () =>
            {
                var result = testCollection.IndexExists("Name");
                result.ShouldBeTrue();
            };

        It should_be_not_saved_with_same_id = () =>
            {
                TestDocument model = new TestDocument { Id = id, Name = "Denny", Value = 13 };
                testCollection.Insert(model);
            };

        Cleanup remove_data = () =>
            {
                testCollection.RemoveById(id);
            };
    }
}
