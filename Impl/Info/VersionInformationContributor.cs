﻿/* ------------------------------------------------------------------------- *
thZero.NetCore.Library.Asp.Instrumentation.Steeltoe
Copyright (C) 2016-2021 thZero.com

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

using Steeltoe.Management.Info;

using thZero.Services;

namespace thZero.AspNetCore
{
    public sealed class VersionInformationContributor : IInfoContributor
    {
        public VersionInformationContributor(IServiceVersionInformation version)
        {
            Version = version;
        }

        #region Public Methods
        public void Contribute(IInfoBuilder builder)
        {
            if (Version == null)
                return;

            builder.WithInfo("version", new
            {
                full = Version.VersionFormatted,
                major = Version.Version.Major,
                minor = Version.Version.Minor,
                revesion = Version.Version.Revision,
                build = Version.Version.Build,
                buildDate = Version.BuildDate
            });
        }
        #endregion

        #region Private Properties
        private IServiceVersionInformation Version { get; set; }
        #endregion
    }
}
