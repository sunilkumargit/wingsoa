using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modularity
{
    public interface IModuleInfoBuilder
    {
        ModuleInfo BuildModuleInfo(Type type);
    }
}
