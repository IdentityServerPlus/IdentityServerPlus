using IdentityServer.Models;
using IdentityServer4.Models;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Models;
using IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Services;
using IdentityServerPlus.Plugin.DatabaseProvider.MongoDB.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB
{
    class MongoDBDatabaseProvider : IPluginProvider, IIdentityProvider, IIdentityServerProvider
    {
        public string Name => "MongoDB Database Provider";

        public string Description => "Connect your identity server with MongoDB";

        public IdentityProviderType Type => IdentityProviderType.Database;

        private MongoDBConfiguration _config { get; }

        public MongoDBDatabaseProvider(MongoDBConfiguration config)
        {
            _config = config;
            BsonClassMapper.MapClasses();
        }

        public IdentityBuilder Build(IdentityBuilder builder)
        {

            var client = new MongoClient(_config.ConnectionString);
            builder.Services.AddSingleton<IMongoDatabase>(client.GetDatabase(_config.Database));
            builder.Services.AddScoped<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>();
            builder.Services.AddScoped<IRoleStore<ApplicationRole>, RoleStore<ApplicationRole>>();

            return builder;
        }

        public IIdentityServerBuilder Build(IIdentityServerBuilder builder)
        {
            return builder.AddClientStore<ResourceStore>()
            .AddResourceStore<ResourceStore>()
            .AddPersistedGrantStore<UserResourceStore>()
            .AddCorsPolicyService<CorsPolicyService>();
        }
    }
}
