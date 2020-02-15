﻿using IdentityModel;
using IdentityServer.Models;
using IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft
{
    internal class MicrosoftAuthenticationProvider : Base.Structures.AuthenticationProvider
    {
        private const string ObjectIdentfier = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        private MicrosoftAuthenticationConfig _config { get; }

        public MicrosoftAuthenticationProvider(MicrosoftAuthenticationConfig configuration) : base("Microsoft", "microsoft")
        {
            _config = configuration;
        }

        public override string Description => "A login provider for Microft Social and Enterprise (Office 365) Connections";

        public override AuthenticationBuilder Build(AuthenticationBuilder builder)
        {

            foreach (var provider in _config.Providers)
            {
                builder = builder.AddOpenIdConnect(provider.Scheme, options =>
                {
                    options.SaveTokens = true;
                    foreach (var scope in provider.Scopes)
                    {
                        options.Scope.Add(scope);
                    }
                    options.ResponseType = "id_token token";
                    options.ClientId = provider.ClientId;
                    options.ClientSecret = provider.ClientSecret;
                    options.MetadataAddress = "https://login.microsoftonline.com/" + provider.AuthorityTenant + "/v2.0/.well-known/openid-configuration";
                    options.Authority = "https://login.microsoftonline.com/" + provider.AuthorityTenant;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudience = provider.ClientId,
                        IssuerValidator = (issuer, securityToken, validationParameters) =>
                        {
                            // This is the directory ID of the logged in user
                            var allowedDirectories = provider.AllowedDirectoryIds;
                            if (allowedDirectories.Count == 1 && allowedDirectories[0] == "*")
                            {
                                if (issuer.StartsWith("https://login.microsoftonline.com/") && issuer.EndsWith("/v2.0"))
                                    return issuer;
                            }
                            return null;
                        }
                    };
                });
            }
            return builder;
        }

        public override string GetProviderId(AuthenticateResult authResult)
        {
            var claims = authResult.Principal.Claims;
            var id = claims.SingleOrDefault(x => x.Type == ObjectIdentfier);
            return id.Value;
        }



        public override ApplicationUser ProvisionUser(AuthenticateResult authResult)
        {
            var claims = authResult.Principal.Claims;
            var email = claims.SingleOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName);
            var fullName = claims.SingleOrDefault(x => x.Type == JwtClaimTypes.Name);
            return new ApplicationUser
            {
                Email = email.Value,
                Username = Guid.NewGuid().ToString(),
                Claims = new List<ApplicationClaim>()
                {
                    new ApplicationClaim(new System.Security.Claims.Claim(JwtClaimTypes.Name, fullName.Value, null, Scheme, Scheme))
                }
            };
        }

        public override async Task UpdateUserAsync(ApplicationUser user, AuthenticateResult result)
        {


            //TODO: Add options for this to happen, and when
            try
            {
                var graphClient = new GraphServiceClient(new GraphAuthenticationProvider(user.Providers.SingleOrDefault(x => x.LoginProvider == Scheme).AccessToken));
                var me = await graphClient.Me.Request().GetAsync();
                user.PhoneNumber = me.MobilePhone;
                user.PhoneNumberConfirmed = true;

                user.Email = me.UserPrincipalName;
                user.EmailVerified = true; // we know its confirmed if we got this far from the microsoft account.
            }
            catch (Exception) { }


            var provider = user.Providers.SingleOrDefault(x => x.LoginProvider == Scheme);
            if (provider != null)
            {


                if (!string.IsNullOrWhiteSpace(result.Properties.GetTokenValue("access_token")))
                {
                    provider.AccessToken = result.Properties.GetTokenValue("access_token");

                    provider.AccessTokenExpiry = DateTime.Parse(result.Properties.GetTokenValue("expires_at"));
                }
                if (!string.IsNullOrWhiteSpace(result.Properties.GetTokenValue("id_token")))
                {
                    provider.IdToken = result.Properties.GetTokenValue("id_token");
                    provider.IdTokenExpiry = DateTime.Parse(result.Properties.GetTokenValue("expires_at"));
                }
            }
            await base.UpdateUserAsync(user, result);
        }
    }
}