using Nuke.Common.Tools.GitVersion;

using static Nuke.Common.ValueInjection.ValueInjectionUtility;

public interface IHazGitVersion
{
    [GitVersion]
    GitVersion GitVersion => TryGetValue(() => GitVersion);

    string Version => GitVersion.NuGetVersionV2;
}
