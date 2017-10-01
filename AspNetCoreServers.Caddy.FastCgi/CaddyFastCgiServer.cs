using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreServers.FastCgi;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace AspNetCoreServers.Caddy.FastCgi
{
    class CaddyFastCgiServer : IServer
    {
        private readonly IServer fastCgiServer;
        private readonly IOptions<FastCgiServerOptions> fastCgiOptions;
        private readonly IOptions<CaddyServerOptions> caddyOptions;
        private readonly IServerAddressesFeature serverAddresses;
        private readonly string caddyfilePath;

        private Process caddy;

        public CaddyFastCgiServer(
            FastCgiServer fastCgiServer,
            IOptions<FastCgiServerOptions> fastCgiOptions,
            IOptions<CaddyServerOptions> caddyOptions)
        {
            this.fastCgiServer = fastCgiServer;
            this.fastCgiOptions = fastCgiOptions;
            this.caddyOptions = caddyOptions;
            serverAddresses = new ServerAddressesFeature();
            Features.Set(serverAddresses);
            caddyfilePath = Path.Combine(Environment.CurrentDirectory, "Caddyfile");
        }

        public IFeatureCollection Features => fastCgiServer.Features;

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            // Configure and start Caddy

            var caddyfileContent = new CaddyfileBuilder()
                .WithUrls(serverAddresses.Addresses.ToArray())
                .WithFastCgiProxy("/", new IPEndPoint(fastCgiOptions.Value.BindAddress, fastCgiOptions.Value.BindPort))
                .Build();
            
            File.WriteAllText(caddyfilePath, caddyfileContent);
            caddy = Process.Start(caddyOptions.Value.CaddyExecutablePath, $"--conf \"{caddyfilePath}\"");

            return fastCgiServer.StartAsync(application, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop Caddy
            if (!caddy.HasExited)
                caddy.Kill();

            // Remove Caddyfile
            File.Delete(caddyfilePath);

            return fastCgiServer.StopAsync(cancellationToken);
        }

        public void Dispose()
        {
            caddy.Dispose();
            fastCgiServer.Dispose();
        }
    }
}
