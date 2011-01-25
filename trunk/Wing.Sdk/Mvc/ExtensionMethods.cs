using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Wing.Mvc
{
    public static class ExtensionMethods
    {
        public static bool TryGetValue(this System.Web.Mvc.IValueProvider valueProvider, string key, out ValueProviderResult result)
        {
            try
            {
                result = valueProvider.GetValue(key);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }
    }
}
