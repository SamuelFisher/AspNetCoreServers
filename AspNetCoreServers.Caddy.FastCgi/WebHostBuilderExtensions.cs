using System;
using AspNetCoreServers.FastCgi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace AspNetCoreServers.Caddy.FastCgi
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseCaddyFastCgi(this IWebHostBuilder hostBuilder)
        {
            return UseCaddyFastCgi(hostBuilder, options => { });
        }

        public static IWebHostBuilder UseCaddyFastCgi(this IWebHostBuilder hostBuilder, Action<CaddyServerOptions> options)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.Configure(options);
                services.AddSingleton<FastCgiServer>();
                services.AddSingleton<IServer, CaddyFastCgiServer>();
            });
        }
    }
}
