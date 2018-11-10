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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Steeltoe.Management.Endpoint.Health;
using Steeltoe.Management.Endpoint.Info;
using Steeltoe.Management.Endpoint.Mappings;
using Steeltoe.Management.Endpoint.Metrics;

namespace thZero.AspNetCore
{
    public class SteeltoeInstrumentationStartupExtension : IStartupExtension
    {
        #region Public Methods
        public virtual void ConfigureInitializePost(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
        }

        public virtual void ConfigureInitializePre(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
        }

        public virtual void ConfigureInitializeFinalPre(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
        }

        public virtual void ConfigureInitializeFinalPost(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
        {
        }

        public virtual void ConfigureInitializeRoutesBuilderPost(IRouteBuilder routes)
        {
        }

        public virtual void ConfigureInitializeRoutesBuilderPre(IRouteBuilder routes)
        {
        }

        public virtual void ConfigureInitializeSsl(IApplicationBuilder app)
        {
        }

        public virtual void ConfigureInitializeStaticPost(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
        }

        public virtual void ConfigureInitializeStaticPre(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
        }

        public virtual void ConfigureServicesMvcPost(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        public virtual void ConfigureServicesMvcPre(IServiceCollection services, IConfigurationRoot configuration)
        {
            ConfigureInitializeHealthContributors(services, configuration);

            services.AddHealthActuator(configuration);

            services.AddSingleton<IInfoContributor, VersionInformationContributor>();
            ConfigureInitializeInfoContributors(services, configuration);

            // Add custom info contributor
            services.AddInfoActuator(configuration);

            services.AddSingleton<MappingsEndpoint, MappingsEndpoint>();
            services.AddSingleton<IMappingsContributor, StandardMappingsContributor>();
            ConfigureInitializeMappingsContributors(services, configuration);

            services.AddMappingsActuator(configuration);

            services.AddSingleton<MetricsEndpoint, MetricsEndpoint>();
            services.AddSingleton<IMetricsContributor, MetricsContributor>();
            services.AddMetricsActuator(configuration);

            services.AddSingleton<ISteeltoeInstrumentationControllerExtension, SteeltoeInstrumentationControllerExtension>();
        }
        #endregion

        #region Protected Methods
        protected virtual void ConfigureInitializeHealthContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected virtual void ConfigureInitializeInfoContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }

        protected virtual void ConfigureInitializeMappingsContributors(IServiceCollection services, IConfigurationRoot configuration)
        {
        }
        #endregion
    }
}
