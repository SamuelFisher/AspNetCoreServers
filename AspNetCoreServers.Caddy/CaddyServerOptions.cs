using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreServers.Caddy
{
    public class CaddyServerOptions
    {
        public CaddyServerOptions()
        {
            CaddyExecutablePath = "caddy";
        }

        public string CaddyExecutablePath { get; set; }
    }
}
