using Nuke.Common.Git;

using static Nuke.Common.ValueInjection.ValueInjectionUtility;

public interface IHazGitRepository
{
    [GitRepository]
    GitRepository GitRepository => TryGetValue(() => GitRepository);
}
