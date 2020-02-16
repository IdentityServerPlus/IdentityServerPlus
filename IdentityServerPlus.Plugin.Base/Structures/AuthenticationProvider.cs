using System;
using System.Threading.Tasks;
using IdentityServer.Models;
using IdentityServerPlus.Plugin.Base.Interfaces;
using Microsoft.AspNetCore.Authentication;

namespace IdentityServerPlus.Plugin.Base.Structures
{
    public abstract class AuthenticationProvider : IAuthenticationProvider
    {

        public string Name => "Microsoft logins provider";

        public abstract string Description { get; }

        protected AuthenticationProvider() { }

        public abstract AuthenticationBuilder Build(AuthenticationBuilder builder);
        public abstract string GetProviderId(string scheme, AuthenticateResult authResult);
        public abstract ApplicationUser ProvisionUser(string scheme, AuthenticateResult result);
        public virtual Task UpdateUserAsync(string scheme, ApplicationUser user, AuthenticateResult result)
        {
            return Task.CompletedTask;
        }

        public abstract string GetFriendlyName(string scheme);

        public abstract bool HostsScheme(string scheme);
    }
}