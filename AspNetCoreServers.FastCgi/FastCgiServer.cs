using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Owin;
using Microsoft.Extensions.Options;
using Fos;

namespace AspNetCoreServers.FastCgi
{
    public class FastCgiServer : IServer
    {
        private FosSelfHost cgiServer;

        public FastCgiServer(IOptions<FastCgiServerOptions> options)
        {
            var serverAddressesFeature = new ServerAddressesFeature();
            serverAddressesFeature.Addresses.Add($"fastcgi://{options.Value.BindAddress}:{options.Value.BindPort}");
            Features.Set<IServerAddressesFeature>(serverAddressesFeature);
            Features.Set<IHttpRequestFeature>(new HttpRequestFeature());
            Features.Set<IHttpResponseFeature>(new HttpResponseFeature());
        }

        public IFeatureCollection Features { get; } = new FeatureCollection();

        public Task StartAsync<TContext>(IHttpApplication<TContext> application, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                Func<IDictionary<string, object>, Task> appFunc = async env =>
                {
                    var owinFeatures = new OwinFeatureCollection(env);
                    var features = new FeatureCollection(owinFeatures);

                    var owinHttpResponse = features.Get<IHttpResponseFeature>();
                    features.Set<IHttpResponseFeature>(new NoOnStartingHttpResponseFeature(owinHttpResponse));

                    var context = application.CreateContext(features);
                    try
                    {
                        await application.ProcessRequestAsync(context);
                    }
                    catch (Exception ex)
                    {
                        application.DisposeContext(context, ex);
                        throw;
                    }

                    application.DisposeContext(context, null);
                };

                // Convert OWIN WebSockets to ASP.NET Core WebSockets
                appFunc = OwinWebSocketAcceptAdapter.AdaptWebSockets(appFunc);

                // Wrap this into a middleware handler that Fos can accept
                Func<IDictionary<string, object>, Func<IDictionary<string, object>, Task>, Task> middlewareHandler = async (env, next) =>
                {
                    await appFunc(env);

                    if (next != null)
                        await next(env);
                };

                cgiServer = new FosSelfHost(builder =>
                {
                    builder.Use(middlewareHandler);
                });

                cgiServer.Bind(IPAddress.Loopback, 9000);
                cgiServer.Start(true);
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                cgiServer.Stop();
            }, cancellationToken);
        }

        public void Dispose()
        {
            cgiServer.Dispose();
        }
    }
}
