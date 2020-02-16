using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.Base.Interfaces
{
    public interface IAuthenticationProvider : IPluginProvider
    {

        string GetFriendlyName(string scheme);
        bool HostsScheme(string scheme);

        AuthenticationBuilder Build(AuthenticationBuilder builder);

        string GetProviderId(string scheme, AuthenticateResult authResult);

        ApplicationUser ProvisionUser(string scheme, AuthenticateResult authResult);
        Task UpdateUserAsync(string scheme, ApplicationUser user, AuthenticateResult result);

    }
}
