using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerPlus.Plugin.AuthenticationProvider.Microsoft.Models
{
    public class Provider
    {
        public string Scheme { get; set; }
        public string AuthorityTenant { get; set; }
        public string DirectoryTenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public List<string> Scopes { get; set; }
        public bool SaveTokens { get; set; }

        public List<string> AllowedDirectoryIds { get; set; }
    }

    public class MicrosoftAuthenticationConfig
    {
        public List<Provider> Providers { get; set; }
    }
}
