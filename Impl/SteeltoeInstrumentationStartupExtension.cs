﻿/* ------------------------------------------------------------------------- *
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

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Endpoint.Mappings;
using Steeltoe.Management.Endpoint.Metrics;

namespace thZero.AspNetCore
{
    public class SteeltoeInstrumentationStartupExtension : IStartupExtension
    {
        #region Public Methods
        public void InitializeMvcPost(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        public void InitializeMvcPre(IServiceCollection services, IConfigurationRoot configuration)
        {
            InitiaalizeHealthContributors(services, configuration);

            services.AddHealthActuator(configuration);

            services.AddSingleton<IInfoContributor, VersionInformationContributor>();
            InitiaalizeInfoContributors(services, configuration);

            // Add custom info contributor
            services.AddInfoActuator(configuration);

            services.AddSingleton<MappingsEndpoint, MappingsEndpoint>();
            services.AddSingleton<IMappingsContributor, StandardMappingsContributor>(); 
            InitiaalizeMappingsContributors(services, configuration);

            services.AddMappingsActuator(configuration);

            services.AddSingleton<ISteeltoeInstrumentationControllerExtension, SteeltoeInstrumentationControllerExtension>();

            services.AddSingleton<MetricsEndpoint, MetricsEndpoint>();
            services.AddSingleton<IMetricsContributor, MetricsContributor>();
            services.AddMetricsActuator(configuration);
        }

        public void InitializeSsl(IApplicationBuilder app)
        {
        }

        public void InitializeStaticPost(IApplicationBuilder app)
        {
        }

        public void InitializeStaticPre(IApplicationBuilder app)
        {
        }
        #endregion

        #region Protected Methods
        protected virtual void InitiaalizeHealthContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected virtual void InitiaalizeInfoContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected virtual void InitiaalizeMappingsContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }
        #endregion
    }
}
