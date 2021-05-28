using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.OutboundHttpProxy.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string Proxy { get; set; }

        public PluginConfiguration()
        {
            Proxy = "";
        }
    }
}
