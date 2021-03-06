using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.ValueInjection.ValueInjectionUtility;

namespace Nuke.Components
{
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
                    .Apply(RestoreSettings));
            });

        Configure<DotNetRestoreSettings> RestoreSettings => _ => _;

        [Parameter("Ignore unreachable sources during " + nameof(Restore))]
        bool IgnoreFailedSources => TryGetValue(() => IgnoreFailedSources);
    }
}

