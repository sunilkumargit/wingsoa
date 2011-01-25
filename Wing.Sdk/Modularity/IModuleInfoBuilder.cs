using System;

namespace Wing.Modularity
{
    public interface IModuleInfoBuilder
    {
        ModuleInfo BuildModuleInfo(Type type);
    }
}
