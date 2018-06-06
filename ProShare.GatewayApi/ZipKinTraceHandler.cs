using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using zipkin4net.Transport.Http;

namespace ProShare.GatewayApi
{
    public class ZipKinTraceHandler: TracingHandler
    {
        public ZipKinTraceHandler() : base("Gateway.Api") { }       
    }
}
