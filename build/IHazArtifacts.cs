using Nuke.Common;
using Nuke.Common.IO;

public interface IHazArtifacts : INukeBuild
{
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
}
