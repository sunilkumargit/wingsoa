using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wing.Client;

namespace Wing.Host.AppCode.Helpers
{
    public static class ViewHelper
    {
        public static String GetResourceMapString()
        {
            var result = "";
            foreach (var resource in ServiceLocator.GetInstance<IResourceMapService>().Mappings)
            {
                result += String.Format("Loader.mapResource('{0}', '{1}', '{2}'); \n",
                                    resource.Name,
                                    resource.Type == ResourceType.Script ? "js" : "css",
                                    resource.Url);
            }
            return result;
        }

        public static string GetResourceInitString(ResourceLoadMode loadMode)
        {
            var resources = ServiceLocator.GetInstance<IResourceMapService>().GetResourcesByMode(loadMode);
            var result = "";
            foreach (var resource in resources)
                result += String.Format("Loader.dependsOn('{0}'); \n", resource.Name);
            return result;

        }
    }
}