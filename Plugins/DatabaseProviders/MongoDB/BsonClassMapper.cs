using IdentityServer.Models;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace IdentityServerPlus.Plugin.DatabaseProvider.MongoDB
{
    static class BsonClassMapper
    {
        internal static void MapClasses()
        {
            // No idea why this is needed............... AutoMap Dosnt work on postloaded assemblies apperently.
            BsonClassMap.RegisterClassMap<ApplicationUser>(cm =>
            {
                cm.AutoMap();
                cm.MapProperty(x => x.AccessFailedCount);
            });

            BsonClassMap.RegisterClassMap<Resource>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Name);
                cm.AddKnownType(typeof(ApiResource));
                cm.AddKnownType(typeof(IdentityResource));
            });

            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);

            });

            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.ClientId);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Key);
                cm.MapProperty(x => x.Expiration);
                cm.MapProperty(x => x.SubjectId);
                cm.MapProperty(x => x.Type);
                cm.MapProperty(x => x.ClientId);
                cm.MapProperty(x => x.CreationTime);
                cm.MapProperty(x => x.Data);
            });

            BsonClassMap.RegisterClassMap<UserLoginInfo>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(ApplicationProviderInfo));
                cm.MapProperty(x => x.LoginProvider);
                cm.MapProperty(x => x.ProviderDisplayName);
                cm.MapProperty(x => x.ProviderKey);
                cm.MapCreator(p => new UserLoginInfo(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName));
            });
            BsonClassMap.RegisterClassMap<ApplicationProviderInfo>(cm =>
            {
                cm.MapProperty(x => x.AccessToken);
                cm.MapProperty(x => x.AccessTokenExpiry);
                cm.MapProperty(x => x.IdToken);
                cm.MapProperty(x => x.IdTokenExpiry);
                cm.MapProperty(x => x.ProviderLinkedAt);
                cm.MapCreator(p => new ApplicationProviderInfo(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName));
            });

            BsonClassMap.RegisterClassMap<ApplicationClaim>(cm =>
            {
                cm.MapProperty(x => x.OriginalIssuer);
                //cm.MapProperty(x => x.Properties);
                //cm.MapProperty(x => x.ValueType);
                cm.MapProperty(x => x.Value);
                cm.MapProperty(x => x.Issuer);
                cm.MapProperty(x => x.Type);
                //cm.MapCreator(p => new ApplicationUserClaim() { Issuer = p.Issuer, Type = p.Type});
            });
        }
    }
}
