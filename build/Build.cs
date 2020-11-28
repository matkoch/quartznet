using System.Collections.Generic;

using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;

[CheckBuildProjectConfigurations]
// [GitHubActions(
//     "continues",
//     GitHubActionsImage.MacOsLatest,
//     On = new[] {GitHubActionsTrigger.Push},
//     InvokedTargets = new[] {nameof(Publish)},
//     ImportGitHubTokenAs = nameof(GitHubToken))]
class Build : NukeBuild,
    IHazGitVersion,
    IHazGitRepository,
    IHazChangelog,
    IRestore,
    ICompile,
    ITest,
    IPush,
    ICreateGitHubRelease
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => ((IPack) x).Pack);

    AbsolutePath BinaryDirectory => ((IHazArtifacts) this).ArtifactsDirectory / "bin";
    AbsolutePath SourceDirectory => RootDirectory / "src";

    Target Clean => _ => _
        .Before<IRestore>(x => x.Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(BinaryDirectory);
        });

    public IEnumerable<Project> TestProjects => ((IHazSolution) this).Solution.GetProjects("*.Tests");
}
