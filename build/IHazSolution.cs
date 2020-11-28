using Nuke.Common.ProjectModel;
using static Nuke.Common.ValueInjection.ValueInjectionUtility;

public interface IHazSolution
{
    [Solution]
    Solution Solution => TryGetValue(() => Solution);
}
