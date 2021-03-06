﻿// Copyright 2017 the original author or authors.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Logging;
using Steeltoe.Management.Endpoint.Mappings;
using System.Collections.Generic;

namespace thZero.AspNetCore
{
    public sealed class StandardMappingsContributor : IMappingsContributor
    {
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
        private readonly IEnumerable<IApiDescriptionProvider> _apiDescriptionProviders;
        private readonly IMappingsOptions _options;
        private readonly IRouteMappings _routeMappings;

        public StandardMappingsContributor(
            IMappingsOptions options,
            IRouteMappings routeMappings = null,
            IActionDescriptorCollectionProvider actionDescriptorCollectionProvider = null,
            IEnumerable<IApiDescriptionProvider> apiDescriptionProviders = null,
            ILogger<StandardMappingsContributor> logger = null)
        {
            _logger = logger;
            _options = options;
            _routeMappings = routeMappings;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _apiDescriptionProviders = apiDescriptionProviders;
        }

        #region Public Methods
        public ApplicationMappings Invoke(HttpContext context)
        {
            ApplicationMappings result = GetApplicationMappings(context);
            return result;
        }
        #endregion

        #region Private Methods
        private ApplicationMappings GetApplicationMappings(HttpContext context)
        {
            IDictionary<string, IList<MappingDescription>> desc = new Dictionary<string, IList<MappingDescription>>();
            if (_actionDescriptorCollectionProvider != null)
            {
                ApiDescriptionProviderContext apiContext = GetApiDescriptions(_actionDescriptorCollectionProvider?.ActionDescriptors?.Items);
                desc = GetMappingDescriptions(apiContext);
            }

            if (_routeMappings != null)
            {
                AddRouteMappingsDescriptions(_routeMappings, desc);
            }

            var contextMappings = new ContextMappings(desc);
            return new ApplicationMappings(contextMappings);
        }

        private bool IsMappingsRequest(HttpContext context)
        {
            if (!context.Request.Method.Equals("GET"))
            {
                return false;
            }

            PathString path = new PathString(_options.Path);
            return context.Request.Path.Equals(path);
        }

        private IDictionary<string, IList<MappingDescription>> GetMappingDescriptions(ApiDescriptionProviderContext apiContext)
        {
            IDictionary<string, IList<MappingDescription>> mappingDescriptions = new Dictionary<string, IList<MappingDescription>>();
            foreach (var desc in apiContext.Results)
            {
                var cdesc = desc.ActionDescriptor as ControllerActionDescriptor;
                var details = GetRouteDetails(desc);
                mappingDescriptions.TryGetValue(cdesc.ControllerTypeInfo.FullName, out IList<MappingDescription> mapList);

                if (mapList == null)
                {
                    mapList = new List<MappingDescription>();
                    mappingDescriptions.Add(cdesc.ControllerTypeInfo.FullName, mapList);
                }

                var mapDesc = new MappingDescription(cdesc.MethodInfo, details);
                mapList.Add(mapDesc);
            }

            return mappingDescriptions;
        }

        private IRouteDetails GetRouteDetails(ApiDescription desc)
        {
            var routeDetails = new AspNetCoreRouteDetails();

            routeDetails.HttpMethods = GetHttpMethods(desc);

            if (desc.ActionDescriptor.AttributeRouteInfo?.Template != null)
            {
                routeDetails.RouteTemplate = desc.ActionDescriptor.AttributeRouteInfo.Template;
            }
            else
            {
                ControllerActionDescriptor cdesc = desc.ActionDescriptor as ControllerActionDescriptor;
                routeDetails.RouteTemplate = $"/{cdesc.ControllerName}/{cdesc.ActionName}";
            }

            List<string> produces = new List<string>();
            foreach (var respTypes in desc.SupportedResponseTypes)
            {
                foreach (var format in respTypes.ApiResponseFormats)
                {
                    produces.Add(format.MediaType);
                }
            }

            routeDetails.Produces = produces;

            List<string> consumes = new List<string>();
            foreach (var reqTypes in desc.SupportedRequestFormats)
            {
                consumes.Add(reqTypes.MediaType);
            }

            routeDetails.Consumes = consumes;

            return routeDetails;
        }

        private void AddRouteMappingsDescriptions(IRouteMappings routeMappings, IDictionary<string, IList<MappingDescription>> desc)
        {
            if (routeMappings == null)
            {
                return;
            }

            foreach (var router in routeMappings.Routers)
            {
                var route = router as Route;
                if (route != null)
                {
                    var details = GetRouteDetails(route);
                    desc.TryGetValue("CoreRouteHandler", out IList<MappingDescription> mapList);

                    if (mapList == null)
                    {
                        mapList = new List<MappingDescription>();
                        desc.Add("CoreRouteHandler", mapList);
                    }

                    var mapDesc = new MappingDescription("CoreRouteHandler", details);
                    mapList.Add(mapDesc);
                }
            }
        }

        private IRouteDetails GetRouteDetails(Route route)
        {
            var routeDetails = new AspNetCoreRouteDetails();

            routeDetails.HttpMethods = GetHttpMethods(route);
            routeDetails.RouteTemplate = route.RouteTemplate;

            return routeDetails;
        }

        private IList<string> GetHttpMethods(ApiDescription desc)
        {
            if (!string.IsNullOrEmpty(desc.HttpMethod))
            {
                return new List<string>() { desc.HttpMethod };
            }

            return null;
        }

        private IList<string> GetHttpMethods(Route route)
        {
            var constraints = route.Constraints;
            if (constraints.TryGetValue("httpMethod", out IRouteConstraint routeConstraint))
            {
                var methodConstraint = routeConstraint as HttpMethodRouteConstraint;
                if (methodConstraint != null)
                {
                    return methodConstraint.AllowedMethods;
                }
            }

            return null;
        }

        private ApiDescriptionProviderContext GetApiDescriptions(IReadOnlyList<ActionDescriptor> actionDescriptors)
        {
            if (actionDescriptors == null)
            {
                return new ApiDescriptionProviderContext(new List<ActionDescriptor>());
            }

            foreach (var action in actionDescriptors)
            {
                // This is required in order for OnProvidersExecuting() to work
                var apiExplorerActionData = new ApiDescriptionActionData()
                {
                    GroupName = "Steeltoe"
                };

                action.SetProperty(apiExplorerActionData);
            }

            var context = new ApiDescriptionProviderContext(actionDescriptors);

            foreach (var provider in _apiDescriptionProviders)
            {
                provider.OnProvidersExecuting(context);
            }

            return context;
        }
        #endregion

        #region Fields
        private ILogger<StandardMappingsContributor> _logger;
        #endregion
    }
}
