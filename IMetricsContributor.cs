using System;

using Microsoft.AspNetCore.Http;

using Steeltoe.Management.Endpoint.Metrics;

namespace thZero.AspNetCore
{
    public interface IMetricsContributor
    {
        IMetricsResponse Invoke(HttpContext context);
    }
}
