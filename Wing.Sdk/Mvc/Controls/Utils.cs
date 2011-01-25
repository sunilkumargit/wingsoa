using System;
using System.Collections.Generic;


namespace Wing.Mvc.Controls
{
    internal static class Utils
    {
        public static string EncodeGetParams(IDictionary<String, Object> paramList)
        {
            String result = "";
            foreach (KeyValuePair<String, Object> param in paramList)
            {
                if (!String.IsNullOrEmpty((param.Value ?? "").ToString()))
                    result += String.Format("{0}{1}={2}",
                        (result.Length == 0 ? "" : "&"),
                        param.Key.ToString(),
                        System.Web.HttpContext.Current.Server.UrlEncode(param.Value.ToString()));
            }
            return result;
        }

        public static string EncodeGetParams(Object paramList)
        {
            return EncodeGetParams(ReflectionHelper.AnonymousToDictionary(paramList));
        }
    }
}
