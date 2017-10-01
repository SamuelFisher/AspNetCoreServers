using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace AspNetCoreServers.Caddy
{
    public class CaddyfileBuilder
    {
        private readonly IReadOnlyList<string> listenUrls;
        private readonly IReadOnlyList<string> directives;

        public CaddyfileBuilder()
        {
            listenUrls = new List<string>();
            directives = new List<string>();
        }

        private CaddyfileBuilder(IEnumerable<string> listenUrls, IEnumerable<string> directives)
        {
            this.listenUrls = listenUrls.ToList();
            this.directives = directives.ToList();
        }

        public CaddyfileBuilder WithUrls(params string[] listenUrls)
        {
            return new CaddyfileBuilder(listenUrls, directives);
        }

        public CaddyfileBuilder WithFastCgiProxy(string path, IPEndPoint endPoint)
        {
            return WithProxy("fastcgi", path, endPoint);
        }

        public CaddyfileBuilder WithHttpProxy(string path, IPEndPoint endPoint)
        {
            return WithProxy("proxy", path, endPoint);
        }

        private CaddyfileBuilder WithProxy(string proxyType, string path, IPEndPoint endPoint)
        {
            return new CaddyfileBuilder(
                listenUrls.Append($"{proxyType} {path} {endPoint.Address}:{endPoint.Port}"),
                directives
            );
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            foreach (var url in listenUrls)
                stringBuilder.AppendLine(url);

            stringBuilder.AppendLine();

            foreach (var directive in directives)
                stringBuilder.AppendLine(directive);

            return stringBuilder.ToString();
        }

    }
}
