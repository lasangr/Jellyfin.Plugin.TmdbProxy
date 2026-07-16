using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TmdbProxy.Configuration
{
    /// <summary>
    /// The kind of upstream proxy to use.
    /// </summary>
    public enum ProxyType
    {
        /// <summary>Plain HTTP proxy.</summary>
        Http = 0,

        /// <summary>HTTPS proxy.</summary>
        Https = 1,

        /// <summary>SOCKS5 proxy.</summary>
        Socks5 = 2
    }

    /// <summary>
    /// User-editable settings for the TMDB proxy plugin, exposed on the
    /// plugin's config page in the Jellyfin admin dashboard.
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether proxying is active.
        /// </summary>
        public bool Enabled { get; set; } = false;

        /// <summary>
        /// Gets or sets which kind of proxy <see cref="Host"/>/<see cref="Port"/> refer to.
        /// </summary>
        public ProxyType Type { get; set; } = ProxyType.Socks5;

        /// <summary>
        /// Gets or sets the proxy host (hostname or IP).
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the proxy port.
        /// </summary>
        public int Port { get; set; } = 1080;

        /// <summary>
        /// Gets or sets the username for proxy auth. Leave empty if the
        /// proxy doesn't require authentication.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for proxy auth.
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}
