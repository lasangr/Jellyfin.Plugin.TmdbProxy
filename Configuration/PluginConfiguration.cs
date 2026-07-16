using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.TmdbProxy.Configuration
{
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
        /// Plain string ("http", "https", or "socks5") rather than an enum,
        /// deliberately -- enums can round-trip inconsistently between the
        /// XML config persisted to disk and the JSON the dashboard API
        /// sends/receives, which is what caused the dropdown to silently
        /// fail to save. A string avoids that class of bug entirely.
        /// </summary>
        public string Type { get; set; } = "socks5";

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
