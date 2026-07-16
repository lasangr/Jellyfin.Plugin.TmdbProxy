using System;
using System.Collections.Generic;
using System.Net;
using Jellyfin.Plugin.TmdbProxy.Configuration;

namespace Jellyfin.Plugin.TmdbProxy
{
    /// <summary>
    /// An <see cref="IWebProxy"/> that routes requests to a fixed set of
    /// TMDB hosts through a configured HTTP/HTTPS/SOCKS5 proxy, and
    /// bypasses the proxy (direct connection) for every other host.
    ///
    /// Assigned to the process-wide <c>HttpClient.DefaultProxy</c> so it
    /// applies to any HttpClient (including Jellyfin's built-in TMDB
    /// provider's) that doesn't explicitly override its own Proxy /
    /// UseProxy settings. See README for the caveat on this.
    /// </summary>
    public class TmdbSelectiveProxy : IWebProxy
    {
        private static readonly HashSet<string> ProxiedHosts = new(StringComparer.OrdinalIgnoreCase)
        {
            "api.tmdb.org",
            "image.tmdb.org",
            "images.tmdb.org",
            "api.themoviedb.org"
        };

        /// <inheritdoc />
        public ICredentials? Credentials { get; set; }

        /// <inheritdoc />
        public Uri GetProxy(Uri destination)
        {
            var config = Plugin.Instance?.Configuration;

            var scheme = config?.Type?.ToLowerInvariant() switch
            {
                "socks5" => "socks5",
                "https" => "https",
                _ => "http"
            };

            var host = config?.Host ?? string.Empty;
            var port = config?.Port ?? 0;

            if (!string.IsNullOrEmpty(config?.Username))
            {
                // Embedding credentials in the proxy URI's userinfo is the
                // form .NET's built-in SOCKS4/4a/5 support reads auth
                // from. For HTTP/HTTPS proxies this is a secondary path;
                // the Credentials property below is the primary one used
                // for HTTP proxy Basic/Digest auth challenges.
                var userInfo = Uri.EscapeDataString(config.Username) + ":" +
                                Uri.EscapeDataString(config.Password ?? string.Empty);
                return new Uri($"{scheme}://{userInfo}@{host}:{port}");
            }

            return new Uri($"{scheme}://{host}:{port}");
        }

        /// <inheritdoc />
        public bool IsBypassed(Uri host)
        {
            var config = Plugin.Instance?.Configuration;

            if (config is not { Enabled: true } || string.IsNullOrWhiteSpace(config.Host))
            {
                return true; // proxying disabled or unconfigured -> bypass everything
            }

            return !ProxiedHosts.Contains(host.Host);
        }
    }
}
