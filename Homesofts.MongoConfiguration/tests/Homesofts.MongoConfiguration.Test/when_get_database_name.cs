using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Specifications;

namespace Homesofts.MongoConfiguration.Test
{
    [Subject("Get Database Name")]
    public class when_get_database_name
    {
        It should_get_database_name = () =>
            {
                MongoConfig.Instance.Database.Name.ShouldEqual("homesofts");
            };
    }
}
