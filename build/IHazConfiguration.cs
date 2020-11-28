using Nuke.Common;
using static Nuke.Common.ValueInjection.ValueInjectionUtility;

public interface IHazConfiguration : INukeBuild
{
    [Parameter]
    Configuration Configuration => TryGetValue(() => Configuration)
                                   ?? (IsLocalBuild ? Configuration.Debug : Configuration.Release);
}
