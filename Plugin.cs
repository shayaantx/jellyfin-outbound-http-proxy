using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Jellyfin.Plugin.OutboundHttpProxy.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.OutboundHttpProxy
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private IWebProxy oldProxy;
        public override string Name => "Outbound HTTP Proxy";

        public override Guid Id => Guid.Parse("396bc3b1-6ad2-447f-914e-7c13a45c93d0");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            // save the old proxy if we need to unset our custom proxy
            oldProxy = HttpClient.DefaultProxy;
            this.setProxy();
        }

        public static Plugin Instance { get; private set; }

        public override string Description => base.Description;

        public override string ConfigurationFileName => base.ConfigurationFileName;

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }

        private void setProxy()
        {
            if (!string.IsNullOrEmpty(Instance.Configuration.Proxy)) {
                string[] proxyUrlAndPort = Instance.Configuration.Proxy.Split(":");
                if (proxyUrlAndPort.Length != 2) {
                    throw new Exception("Proxy address should be in following format <hostname>:<port>");
                }
                
                WebProxy proxy = new WebProxy(proxyUrlAndPort[0], Int32.Parse(proxyUrlAndPort[1]));
                proxy.BypassProxyOnLocal = true;
                HttpClient.DefaultProxy = proxy;
            } else {
                this.unsetProxy();
            }
        }

        private void unsetProxy()
        {
            //no params provided means the proxy will be bypassed
            HttpClient.DefaultProxy = oldProxy;
        }

        public override void OnUninstalling()
        {
            this.unsetProxy();
            base.OnUninstalling();
        }

        public override void SaveConfiguration(PluginConfiguration config)
        {
            base.SaveConfiguration(config);
            this.setProxy();
        }
    }
}
