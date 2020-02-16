using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Models
{
    public class MongoDBConfiguration
    {
        public string ConnectionString { get; set; }

        public string Database { get; set; }
    }
}
