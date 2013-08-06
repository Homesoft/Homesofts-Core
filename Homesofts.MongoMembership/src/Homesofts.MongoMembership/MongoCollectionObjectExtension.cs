using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace Homesofts.MongoMembership
{
    static class MongoCollectionObjectExtension
    {
        public static bool IsExist(this MongoCollection collection, string name, string applicationName)
        {
            return collection.IsExist(Query.And(Query.EQ("Name", name), Query.EQ("ApplicationName", applicationName)));
        }
        public static bool IsExist(this MongoCollection collection, IMongoQuery predicate)
        {
            return collection.Count(predicate) > 0;
        }
        public static IEnumerable<T> FindByApplicationName<T>(this MongoCollection<T> collection, string applicationName, int startIndex, int pageSize)
        {
            var cursor = collection.Find(Query.EQ("ApplicationName", applicationName)).SetSortOrder("Name");
            if (pageSize > 0)
                cursor.SetSkip(startIndex).SetLimit(pageSize);
            return cursor;
        }
        public static IEnumerable<T> FindLike<T>(this MongoCollection<T> collection, string propertyName, string value, string applicationName, int startIndex, int pageSize)
        {
            var cursor = collection.Find(Query.And(Query.Matches(propertyName,
                BsonRegularExpression.Create(new Regex(value))), Query.EQ("ApplicationName", applicationName)));
            if (pageSize > 0)
                cursor.SetSkip(startIndex).SetLimit(pageSize);
            return cursor;
        }
        public static T FindByName<T>(this MongoCollection<T> collection, string name, string applicationName)
        {
            return collection.FindOne(Query.And(Query.EQ("Name", name), Query.EQ("ApplicationName", applicationName)));
        }
    }
}
