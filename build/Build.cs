using System.Linq;

using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Utilities.Collections;
using Nuke.GitHub;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.GitHub.GitHubTasks;

[CheckBuildProjectConfigurations]
[GitHubActions(
    "continues",
    GitHubActionsImage.MacOsLatest,
    On = new[] {GitHubActionsTrigger.Push},
    InvokedTargets = new[] {nameof(Publish)},
    ImportGitHubTokenAs = nameof(GitHubToken))]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    [GitRepository] readonly GitRepository GitRepository;
    // [GitVersion(UpdateBuildNumber = true)] readonly GitVersion GitVersion;

    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath BinaryDirectory => ArtifactsDirectory / "bin";
    AbsolutePath SourceDirectory => RootDirectory / "src";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(BinaryDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild());
        });

    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    Target Pack => _ => _
        .DependsOn(Compile)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .SetOutputDirectory(PackagesDirectory));
        });

    readonly string NuGetSource = "https://api.nuget.org/v3/index.json";
    [Parameter] readonly string NuGetApiKey;
    [Parameter] readonly string GitHubToken;

    Target Publish => _ => _
        .DependsOn(Pack)
        // .DependsOn(Test)
        // .Requires(() => NuGetApiKey)
        .Executes(async () =>
        {
            await PublishRelease(_ => _
                .SetName("vBla")
                .SetTag("v3.2.3")
                .SetReleaseNotes(ChangelogTasks.GetNuGetReleaseNotes(RootDirectory / "Changelog.md"))
                .SetArtifactPaths(PackagesDirectory.GlobFiles("*.nupkg").Select(x => x.ToString()).ToArray())
                .SetToken(GitHubToken));
            // DotNetNuGetPush(_ => _
            //         .SetSource(NuGetSource)
            //         .SetApiKey(NuGetApiKey)
            //         .CombineWith(PackagesDirectory.GlobFiles("*.nupkg"), (_, v) => _
            //             .SetTargetPath(v)),
            //     completeOnFailure: true,
            //     degreeOfParallelism: 5);
        });
}
