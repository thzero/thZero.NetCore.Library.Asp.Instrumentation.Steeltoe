using System;

using Microsoft.AspNetCore.Http;

using Steeltoe.Management.Endpoint.Mappings;

namespace thZero.AspNetCore
{
    public interface IMappingsContributor
    {
        ApplicationMappings Invoke(HttpContext context);
    }
}
