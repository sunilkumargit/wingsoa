using System;
using System.Web.Mvc;
using System.Web.Routing;
using Wing.Server.Core;
using Wing.ServiceLocation;

namespace Wing.Server.Bootstrap
{
    public class MvcControllerFactory : DefaultControllerFactory
    {
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            var controllerTypeLocator = ServiceLocator.GetInstance<IMvcControllerTypeLocator>();
            return controllerTypeLocator.GetControllerType(controllerName)
                ?? base.GetControllerType(requestContext, controllerName);
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return base.GetControllerInstance(requestContext, controllerType);
        }
    }
}