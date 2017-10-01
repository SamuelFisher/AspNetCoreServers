using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreServers.FastCgi
{
    public static class WebHostBuilderExtensions
    {
        public static IWebHostBuilder UseFastCgi(this IWebHostBuilder hostBuilder)
        {
            return hostBuilder.UseFastCgi(options => { });
        }

        public static IWebHostBuilder UseFastCgi(this IWebHostBuilder hostBuilder, Action<FastCgiServerOptions> options)
        {
            return hostBuilder.ConfigureServices(services =>
            {
                services.Configure(options);
                services.AddSingleton<IServer, FastCgiServer>();
            });
        }
    }
}
