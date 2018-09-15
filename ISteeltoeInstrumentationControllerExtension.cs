/* ------------------------------------------------------------------------- *
thZero.NetCore.Library.Asp
Copyright (C) 2016-2018 thZero.com

<development [at] thzero [dot] com>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

	http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 * ------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Steeltoe.Common.HealthChecks;
using Steeltoe.Management.Endpoint.Mappings;
using Steeltoe.Management.Endpoint.Metrics;


namespace thZero.AspNetCore
{
    public interface ISteeltoeInstrumentationControllerExtension : IInstrumentationControllerExtension
    {
        HealthCheckResult GetHealth();
        IDictionary<string, object> GetInfo();
        ApplicationMappings GetMappings(HttpContext context);
        IMetricsResponse GetMetrics(HttpContext context);
    }
}
