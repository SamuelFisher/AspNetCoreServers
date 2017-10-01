using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreServers.Caddy.FastCgi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCoreServers.Examples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return new WebHostBuilder()
                .UseCaddyFastCgi()
                .UseStartup<Startup>()
                .ConfigureLogging(logging => logging.AddConsole())
                .UseUrls("http://localhost:5000/")
                .Build();
        }
    }
}
