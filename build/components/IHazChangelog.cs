using System;
using System.Linq;
using Nuke.Common;
using static Nuke.Common.ChangeLog.ChangelogTasks;

namespace Nuke.Components
{
    public interface IHazChangelog : INukeBuild
    {
        string ChangelogFile => RootDirectory / "CHANGELOG.md";

        string ReleaseNotes => GetNuGetReleaseNotes(
            ChangelogFile,
            (this as IHazGitRepository)?.GitRepository);
    }
}

