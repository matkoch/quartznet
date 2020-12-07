// Copyright 2020 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.Linq;
using Nuke.Common.Tooling;

namespace Nuke.Components
{
    public static class ToolSettingsExtensions
    {
        public static TSettings WhenNotNull<TSettings, TObject>(this TSettings settings, TObject obj, Func<TSettings, TObject, TSettings> configurator)
            where TSettings : ToolSettings
        {
            return obj != null ? configurator.Invoke(settings, obj) : settings;
        }

    }
}

