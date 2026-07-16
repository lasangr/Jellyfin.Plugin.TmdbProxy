# Jellyfin TMDB Proxy Plugin

Routes requests to these four hosts through a configurable HTTP,
HTTPS, or SOCKS5 proxy (with optional username/password auth):

- `api.tmdb.org`
- `image.tmdb.org`
- `images.tmdb.org`
- `api.themoviedb.org`

All other outbound traffic from Jellyfin is unaffected.

## How it works

The plugin sets .NET's process-wide `HttpClient.DefaultProxy` to a
custom `IWebProxy` (`TmdbSelectiveProxy`) at startup. That proxy's
`IsBypassed()` check returns `true` (i.e. "don't proxy this") for
every host except the four listed above, so only TMDB traffic is
ever actually routed through your configured proxy.

`HttpClient.DefaultProxy` is honored by any `HttpClient` /
`SocketsHttpHandler` that doesn't explicitly set its own `Proxy` or
`UseProxy = false`. Jellyfin's built-in TMDB metadata/image-fetching
code, as of the versions this was written against, creates its
HttpClients without overriding proxy settings, so this should
actually intercept it — unlike a named-`HttpClient`-based approach,
which the built-in provider never resolves.

**Caveat I can't fully rule out:** I don't have a compiler or a live
Jellyfin instance in front of me to verify this against your exact
server version. If a future/different Jellyfin version explicitly
disables the default proxy on its internal clients, this plugin's
proxy target simply won't be hit and TMDB calls will go direct. If
that happens after installing, check Dashboard → Logs — TMDB timeouts/
connection errors with no indication the proxy was used would be the
symptom, and it'd mean falling back to the OS-level `HTTP_PROXY`
env var approach we discussed earlier (which is layer-independent and
guaranteed to work regardless of how Jellyfin structures its clients).

## SOCKS5 auth

.NET's built-in SOCKS4/4a/5 client support reads username/password
from the proxy URI's userinfo (`socks5://user:pass@host:port`), which
is how this plugin passes SOCKS5 credentials through.

## HTTP/HTTPS proxy auth

For HTTP/HTTPS proxy types, credentials are set via
`IWebProxy.Credentials` (`NetworkCredential`), which .NET uses for
Basic/Digest proxy auth challenges.

## Build

Requires the .NET 9 SDK.

```bash
dotnet build -c Release
```

Match `Jellyfin.Controller`/`Jellyfin.Model` package versions in the
`.csproj` to your server's Jellyfin version before building.

## Install

Copy the built DLL(s) from `bin/Release/net9.0/` into:

```
/var/lib/jellyfin/plugins/TmdbProxy_1.0.0.0/
```

then:

```bash
sudo systemctl restart jellyfin
```

Configure proxy type, host, port, and optional credentials under
**Dashboard → Plugins → TMDB Proxy**.

## Security note

The proxy password is stored in the plugin's config XML on disk in
plain text (standard for Jellyfin plugin configs, not specific to
this one) — use a dedicated proxy credential, not one reused
elsewhere.

## Files

- `Plugin.cs` — sets `HttpClient.DefaultProxy` at startup and on config changes
- `TmdbSelectiveProxy.cs` — the `IWebProxy` implementation: proxies the 4 TMDB hosts, bypasses everything else
- `Configuration/PluginConfiguration.cs` — proxy type, host, port, credentials, enabled flag
- `Configuration/configPage.html` — admin dashboard settings UI
