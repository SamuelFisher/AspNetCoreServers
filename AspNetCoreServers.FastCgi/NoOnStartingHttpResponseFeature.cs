using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace AspNetCoreServers.FastCgi
{
    public class NoOnStartingHttpResponseFeature : IHttpResponseFeature
    {
        private readonly IHttpResponseFeature underlying;

        public NoOnStartingHttpResponseFeature(IHttpResponseFeature underlying)
        {
            this.underlying = underlying;
        }

        public void OnStarting(Func<object, Task> callback, object state)
        {
            // Do nothing
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {
            underlying.OnCompleted(callback, state);
        }

        public int StatusCode
        {
            get { return underlying.StatusCode; }
            set { underlying.StatusCode = value; }
        }

        public string ReasonPhrase
        {
            get { return underlying.ReasonPhrase; }
            set { underlying.ReasonPhrase = value; }
        }

        public IHeaderDictionary Headers
        {
            get { return underlying.Headers; }
            set { underlying.Headers = value; }
        }

        public Stream Body
        {
            get { return underlying.Body; }
            set { underlying.Body = value; }
        }

        public bool HasStarted
        {
            get { return underlying.HasStarted; }
        }
    }
}
