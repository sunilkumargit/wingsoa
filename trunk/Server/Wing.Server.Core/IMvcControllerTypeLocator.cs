using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wing.Server.Core
{
    public interface IMvcControllerTypeLocator
    {
        Type GetControllerType(string controllerName);
        void RegisterControllerType(string controllerName, Type controllerType);
    }
}
