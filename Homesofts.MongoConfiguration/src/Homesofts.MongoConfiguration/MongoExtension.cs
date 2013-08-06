using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace Homesofts.MongoConfiguration
{
    public static class MongoExtension
    {
        public static bool Exists(this MongoCollection collection, IMongoQuery query)
        {
            return collection.Count(query) > 0;
        }
        public static void RemoveById(this MongoCollection collection, Guid id)
        {
            collection.Remove(Query.EQ("_id", id));
        }
        public static void RemoveById(this MongoCollection collection, string id)
        {
            collection.Remove(Query.EQ("_id", id));
        }
        public static MongoCollection GetCollection(this MongoDatabase database, Type type)
        {
            var collection = database.GetCollection(type.Name + "s");
            ensureIndex(type, collection);
            return collection;
        }
        public static MongoCollection<T> GetCollection<T>(this MongoDatabase database)
        {
            var collection = database.GetCollection<T>(typeof(T).Name + "s");
            Type type = typeof(T);
            ensureIndex(type, collection);
            return collection;
        }

        private static void ensureIndex(Type type, MongoCollection collection)
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(EnsureIndexAttribute), true) as EnsureIndexAttribute[];
                foreach (var attribute in attributes)
                {
                    var keyNames = attribute.Keys.Length > 0 ? attribute.Keys : new string[] { property.Name };
                    if (!collection.IndexExists(keyNames))
                    {
                        var keys = IndexKeys.Ascending(keyNames);
                        if (attribute.Descending)
                            keys = IndexKeys.Descending(keyNames);
                        var options = IndexOptions.SetUnique(attribute.Unique).SetSparse(attribute.Sparse);
                        collection.EnsureIndex(keys, options);
                    }
                }
            }
        }
    }
}
