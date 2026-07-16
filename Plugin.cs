using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Jellyfin.Plugin.TmdbProxy.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.TmdbProxy
{
    /// <summary>
    /// Selectively routes requests to TMDB hosts through a configured
    /// HTTP/HTTPS/SOCKS5 proxy by setting the process-wide
    /// <c>HttpClient.DefaultProxy</c>.
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        private readonly TmdbSelectiveProxy _proxy = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;

            ApplyProxyCredentials();

            // HttpClient.DefaultProxy is consulted by any HttpClient/
            // SocketsHttpHandler that doesn't explicitly set its own
            // Proxy or UseProxy = false. This is process-wide, but
            // TmdbSelectiveProxy.IsBypassed() means only the four TMDB
            // hosts are ever actually routed through the configured
            // proxy -- everything else continues direct.
            HttpClient.DefaultProxy = _proxy;

            ConfigurationChanged += (_, _) => ApplyProxyCredentials();
        }

        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin? Instance { get; private set; }

        /// <inheritdoc />
        public override string Name => "TMDB Proxy";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("5074745e-45d8-40f7-bd43-2a54fa1df5a0");

        /// <inheritdoc />
        public override string Description =>
            "Routes api.tmdb.org, image.tmdb.org, images.tmdb.org and api.themoviedb.org " +
            "through an HTTP/HTTPS/SOCKS5 proxy; everything else goes direct.";

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "TmdbProxyConfig",
                    EmbeddedResourcePath = string.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "{0}.Configuration.configPage.html",
                        GetType().Namespace)
                }
            };
        }

        private void ApplyProxyCredentials()
        {
            if (!string.IsNullOrEmpty(Configuration.Username))
            {
                _proxy.Credentials = new NetworkCredential(
                    Configuration.Username, Configuration.Password);
            }
            else
            {
                _proxy.Credentials = null;
            }
        }
    }
}
