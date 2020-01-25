using System;
using System.Collections.Generic;
using IdentityServerPlus.Plugin.Base.Interfaces;
using IdentityServerPlus.Plugin.Base.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerPlus.Plugin.Base.Structures
{
    public abstract class PluginBase
    {

        /// <summary>The Display Name of this plugin</summary>
        public string Name { get; }

        /// <summary>The globally unique ID for this plugin to not get mixed up with others</summary>
        public abstract Guid Id { get; }

        /// <summary>When this plugin was last updated (compiled)</summary>
        public abstract DateTime LastUpdated { get; }

        /// <summary>The Version Code of this plugin for easy updating</summary>
        public string Version { get; }

        protected PluginBase(string name, string version)
        {
            this.Name = name;
            this.Version = version;
        }


        /// <summary>
        /// List all providers that this plugin will expose.
        /// </summary>
        public abstract IEnumerable<ProviderItem> GetProviderTypesAndArguments(IConfiguration configuration);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public abstract IEnumerable<PluginConfigurationItem> GetPluginConfigurations();

        /// <summary>
        /// Create an instance of provider from type, with DI enabled
        /// </summary>
        public virtual IPluginProvider BuildProvider(Type type, params object[] parameters)
        {
            if (!type.IsSubclassOf(typeof(IPluginProvider)))
            {
                throw new InvalidOperationException("Can not create provider " + type.Name + " as it does not implement IPluginProvider");
            }
            return (IPluginProvider)Activator.CreateInstance(type, parameters);

        }
    }
}