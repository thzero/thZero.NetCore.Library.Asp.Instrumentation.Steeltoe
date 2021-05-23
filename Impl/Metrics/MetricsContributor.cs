// Copyright 2017 the original author or authors.
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
using Microsoft.Extensions.Logging;
using Steeltoe.Management.Endpoint.Metrics;
using System;
using System.Collections.Generic;

namespace thZero.AspNetCore
{
    public sealed class MetricsContributor : IMetricsContributor
    {
        public MetricsContributor(
            Steeltoe.Management.Endpoint.Metrics.MetricsEndpoint endpoint,
            ILogger<MetricsContributor> logger = null)
        {
            _endpoint = endpoint;
            _logger = logger;
        }

        #region Public Methods
        public IMetricsResponse Invoke(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            _logger?.LogDebug("Incoming path: {0}", request.Path.Value);

            string metricName = GetMetricName(request);
            if (!string.IsNullOrEmpty(metricName))
            {
                // GET /metrics/{metricName}?tag=key:value&tag=key:value
                var tags = ParseTags(request.Query);
                var metricRequest = new MetricsRequest(metricName, tags);
                var serialInfo = _endpoint.Invoke(metricRequest);
                return serialInfo;
            }
            else
            {
                // GET /metrics
                var serialInfo = _endpoint.Invoke(null);
                return serialInfo;
            }
        }
        #endregion

        #region Private Methods
        private string GetMetricName(HttpRequest request)
        {
            PathString epPath = new PathString(_endpoint.Path);
            if (request.Path.StartsWithSegments(epPath, out PathString remaining))
            {
                if (remaining.HasValue)
                {
                    return remaining.Value.TrimStart('/');
                }
            }

            return null;
        }

        private List<KeyValuePair<string, string>> ParseTags(IQueryCollection query)
        {
            List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();
            if (query == null)
            {
                return results;
            }

            foreach (var q in query)
            {
                if (q.Key.Equals("tag", StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var kvp in q.Value)
                    {
                        var pair = ParseTag(kvp);
                        if (pair != null)
                        {
                            if (!results.Contains(pair.Value))
                            {
                                results.Add(pair.Value);
                            }
                        }
                    }
                }
            }

            return results;
        }

        private KeyValuePair<string, string>? ParseTag(string kvp)
        {
            var str = kvp.Split(new char[] { ':' }, 2);
            if (str != null && str.Length == 2)
            {
                return new KeyValuePair<string, string>(str[0], str[1]);
            }

            return null;
        }
        #endregion

        #region Fields
        private Steeltoe.Management.Endpoint.Metrics.MetricsEndpoint _endpoint;
        private ILogger<MetricsContributor> _logger;
        #endregion
    }
}
