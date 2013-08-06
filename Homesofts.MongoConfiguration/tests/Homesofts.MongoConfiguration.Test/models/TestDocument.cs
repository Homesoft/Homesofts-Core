using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Homesofts.MongoConfiguration.Test.models
{
    public class TestDocument
    {
        public string Id { get; set; }

        [EnsureIndex]
        public string Name { get; set; }

        public int Value { get; set; }
    }
}
