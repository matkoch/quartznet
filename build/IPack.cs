﻿using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

public interface IPack : ICompile, IHazArtifacts
{
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetOutputDirectory(PackagesDirectory)
                .WhenNotNull(this as IHazGitVersion, (_, o) => _
                    .SetVersion(o.GitVersion.NuGetVersionV2))
                .WhenNotNull(this as IHazChangelog, (_, o) => _
                    .SetPackageReleaseNotes(o.ReleaseNotes))
                .Apply(PackSettings));
        });

    Configure<DotNetPackSettings> PackSettings => _ => _;
}
