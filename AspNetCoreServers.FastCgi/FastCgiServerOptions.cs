using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AspNetCoreServers.FastCgi
{
    public class FastCgiServerOptions
    {
        public FastCgiServerOptions()
        {
            BindAddress = IPAddress.Loopback;
            BindPort = 9000;
        }

        /// <summary>
        /// Gets or sets the address to listen for FastCGI connections on.
        /// </summary>
        public IPAddress BindAddress { get; set; }

        /// <summary>
        /// Gets or sets the port to listen for FastCGI connections on.
        /// </summary>
        public int BindPort { get; set; }
    }
}
