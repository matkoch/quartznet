using System;
using System.Collections.Generic;

using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;

using static Nuke.Common.Tools.DotNet.DotNetTasks;

public interface ICompile : IRestore, IHazConfiguration
{
    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                // .SetNoRestore(InvokedTargets.Contains(Restore))
                .WhenNotNull(this as IHazGitRepository, (_, o) => _
                    .SetRepositoryUrl(o.GitRepository.HttpsUrl))
                .WhenNotNull(this as IHazGitVersion, (_, o) => _
                    .SetAssemblyVersion(o.GitVersion.AssemblySemVer)
                    .SetFileVersion(o.GitVersion.AssemblySemFileVer)
                    .SetInformationalVersion(o.GitVersion.InformationalVersion))
                .When(IsServerBuild, _ => _
                    .SetProperty("ContinuesIntegrationBuild", true))
                .Apply(CompileSettings));

            DotNetPublish(_ => _
                    .SetConfiguration(Configuration)
                    .EnableNoBuild()
                    .WhenNotNull(this as IHazGitRepository, (_, o) => _
                        .SetRepositoryUrl(o.GitRepository.HttpsUrl))
                    .WhenNotNull(this as IHazGitVersion, (_, o) => _
                        .SetAssemblyVersion(o.GitVersion.AssemblySemVer)
                        .SetFileVersion(o.GitVersion.AssemblySemFileVer)
                        .SetInformationalVersion(o.GitVersion.InformationalVersion))
                    .When(IsServerBuild, _ => _
                        .SetProperty("ContinuesIntegrationBuild", true))
                    .CombineWith(PublishConfigurations, (_, v) => _
                        .SetProject(v.Project)
                        .SetFramework(v.Framework)),
                degreeOfParallelism: PublishDegreeOfParallelism);
        });

    Configure<DotNetBuildSettings> CompileSettings => _ => _;

    IEnumerable<(Project Project, string Framework)> PublishConfigurations
        => new (Project Project, string Framework)[0];

    int PublishDegreeOfParallelism => 10;
}


public static class ToolSettingsExtensions
{
    public static TSettings WhenNotNull<TSettings, TObject>(this TSettings settings, TObject obj, Func<TSettings, TObject, TSettings> configurator)
        where TSettings : ToolSettings
    {
        return obj != null ? configurator.Invoke(settings, obj) : settings;
    }

    // public static TSettings[] When<TSettings, TObject>(this TSettings[] settings, Func<TSettings, TObject> obj, Configure<TSettings> configurator)
    //     where TSettings : ToolSettings
    // {
    //     return settings.Select(x => condition(x) ? x.Apply(configurator) : x).ToArray();
    // }
}
