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
using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Endpoint.Mappings;
using Steeltoe.Management.Endpoint.Metrics;

namespace thZero.AspNetCore
{
    public class SteeltoeInstrumentationControllerExtension : ISteeltoeInstrumentationControllerExtension
    {
        public SteeltoeInstrumentationControllerExtension(HealthEndpoint endpointHealth, InfoEndpoint endpointInfo, MappingsEndpoint endpointMappings, MetricsEndpoint endpointMetrics)
        {
            EndpointHealth = endpointHealth;
            EndpointInfo = endpointInfo;
            EndpointMappings = endpointMappings;
            EndpointMetrics = endpointMetrics;
        }

        #region Public Methods
        public HealthCheckResult GetHealth()
        {
            if (EndpointHealth == null)
                return null;

            var results = EndpointHealth.Invoke();
            return results;
        }

        public IDictionary<string, object> GetInfo()
        {
            if (EndpointInfo == null)
                return null;

            var results = EndpointInfo.Invoke();
            return results;
        }

        public ApplicationMappings GetMappings(HttpContext context)
        {
            if (EndpointMappings == null)
                return null;

            var results = EndpointMappings.Invoke(context);
            return results;
        }

        public IMetricsResponse GetMetrics(HttpContext context)
        {
            if (EndpointMetrics == null)
                return null;

            var results = EndpointMetrics.Invoke(context);
            return results;
        }
        #endregion

        #region Protected Properties
        protected HealthEndpoint EndpointHealth { get; private set; }
        protected InfoEndpoint EndpointInfo { get; private set; }
        protected MappingsEndpoint EndpointMappings { get; private set; }
        protected MetricsEndpoint EndpointMetrics { get; private set; }
        #endregion
    }
}
