using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using zipkin4net.Transport.Http;

namespace ProShare.GatewayApi
{
    public class ZipKinTraceHandler: DelegatingHandler
    {
        public ZipKinTraceHandler() : base(new TracingHandler("Gateway.Api")) { }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken);
        }
    }
}
