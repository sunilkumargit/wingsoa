
using System;
namespace Wing.Mvc.Controls
{
    [Flags]
    public enum ExtensionStage
    {
        PreInit = 2,
        PostInit = 4,
        PreRender = 8,
        Render = 16,
        PostRender = 32,
        All = PreInit | PostInit | PreRender | Render | PostRender
    }
}
