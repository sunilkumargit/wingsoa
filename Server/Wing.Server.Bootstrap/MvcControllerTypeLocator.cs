using System;
using System.Collections.Generic;
using Wing.Server.Core;

namespace Wing.Server.Bootstrap
{
    public class MvcControllerTypeLocator : IMvcControllerTypeLocator
    {
        private Dictionary<String, Type> _registeredTypes = new Dictionary<string, Type>();

        #region IMvcControllerTypeLocator Members

        public Type GetControllerType(string controllerName)
        {
            controllerName = NormalizeControllerName(controllerName);
            if (_registeredTypes.ContainsKey(controllerName))
                return _registeredTypes[controllerName];
            return null;
        }

        public void RegisterControllerType(string controllerName, Type controllerType)
        {
            if (controllerName == null)
                controllerName = controllerType.Name;
            controllerName = NormalizeControllerName(controllerName);
            _registeredTypes.Add(controllerName, controllerType);
        }

        #endregion

        private String NormalizeControllerName(String controllerName)
        {
            if (controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            return controllerName.ToLower();
        }
    }
}
