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

using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Steeltoe.Management.Endpoint.Metrics;

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
            const string Declaration = "Invoke";

            HttpRequest request = context.Request;

            _logger?.LogDebug2(Declaration, "Incoming path: {0}", request.Path.Value);

            string metricName = GetMetricName(request);
            if (!string.IsNullOrEmpty(metricName))
                // GET /metrics/{metricName}?tag=key:value&tag=key:value
                return _endpoint.Invoke(new MetricsRequest(metricName, ParseTags(request.Query)));

            // GET /metrics
            return _endpoint.Invoke(null);
        }
        #endregion

        #region Private Methods
        private string GetMetricName(HttpRequest request)
        {
            string path = _endpoint.Path;
            path = path.StartsWith(DelimiterSlash) ? path : DelimiterSlash + path;
            PathString epPath = new(path);
            if (request.Path.StartsWithSegments(epPath, out PathString remaining))
            {
                if (remaining.HasValue)
                    return remaining.Value.TrimStart(DelimiterSlashChar);
            }

            return null;
        }

        private static List<KeyValuePair<string, string>> ParseTags(IQueryCollection query)
        {
            List<KeyValuePair<string, string>> results = new();
            if (query == null)
                return results;

            foreach (var q in query)
            {
                if (!q.Key.Equals(KeyTag, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                KeyValuePair<string, string>? pair;
                foreach (var kvp in q.Value)
                {
                    pair = ParseTag(kvp);
                    if (pair == null)
                        continue;

                    if (!results.Contains(pair.Value))
                        results.Add(pair.Value);
                }
            }

            return results;
        }

        private static KeyValuePair<string, string>? ParseTag(string kvp)
        {
            string[] str = kvp.Split(new char[] { DelimiterColonChar }, 2);
            if ((str != null) && (str.Length == 2))
                return new KeyValuePair<string, string>(str[0], str[1]);

            return null;
        }
        #endregion

        #region Fields
        private readonly Steeltoe.Management.Endpoint.Metrics.MetricsEndpoint _endpoint;
        private readonly ILogger<MetricsContributor> _logger;
        #endregion

        #region Constants
        private const char DelimiterColonChar = ':';
        private const char DelimiterSlashChar = '/';
        private const string DelimiterSlash = "/";

        private const string KeyTag = "tag";
        #endregion
    }
}
