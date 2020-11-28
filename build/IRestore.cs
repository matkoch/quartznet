﻿using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.ValueInjection.ValueInjectionUtility;

public interface IRestore : IHazSolution, INukeBuild
{
    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution)
                .SetIgnoreFailedSources(IgnoreFailedSources)
                .When(IsServerBuild, _ => _
                    .SetProperty("ContinuesIntegrationBuild", true))
                    // RestorePackagesWithLockFile
                    // .SetProperty("RestoreLockedMode", true))
                .Apply(RestoreSettings));
        });

    Configure<DotNetRestoreSettings> RestoreSettings => _ => _;

    [Parameter("Ignore unreachable sources during " + nameof(Restore))]
    bool IgnoreFailedSources => TryGetValue(() => IgnoreFailedSources);
}
