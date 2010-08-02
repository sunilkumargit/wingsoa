using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Server.Core;
using Wing.ServiceLocation;
using Wing.Logging;
using Wing.Modularity;
using System.Configuration;
using Wing.Events;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wing.Server.Bootstrap
{
    public class MvcControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var controllerTypeLocator = ServiceLocator.Current.GetInstance<IMvcControllerTypeLocator>();
            return controllerTypeLocator.GetControllerType(controllerName)
                ?? base.GetControllerType(requestContext, controllerName);
        }
    }
}