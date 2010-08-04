using System;

namespace Wing.Server.Core
{
    public interface IMvcControllerTypeLocator
    {
        Type GetControllerType(string controllerName);
        void RegisterControllerType(string controllerName, Type controllerType);
    }
}
