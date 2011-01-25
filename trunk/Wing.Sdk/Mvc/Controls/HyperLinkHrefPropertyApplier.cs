using System;
using System.Collections.Generic;


namespace Wing.Mvc.Controls
{
    public class HyperLinkHrefPropertyApplier : IControlPropertyApplier
    {
        public HyperLinkHrefPropertyApplier(ControlProperty getParamsProperty = null)
        {
            this.GetParamsProperty = getParamsProperty;
        }

        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            Object getParams = null;
            var urlStr = value.AsString();
            var uri = new Uri(urlStr, UriKind.RelativeOrAbsolute);
            //url.
            if (GetParamsProperty != null)
                getParams = target.GetValue(GetParamsProperty);
            if (getParams != null)
            {
                IDictionary<String, Object> dict = null;
                if (!(getParams is IDictionary<String, Object>))
                    dict = ReflectionHelper.AnonymousToDictionary(getParams);
                else
                    dict = (IDictionary<String, Object>)getParams;
                if (uri.Query.HasValue())
                    urlStr += urlStr + "&" + Utils.EncodeGetParams(dict);
                else
                    urlStr += uri.ToString() + "?" + Utils.EncodeGetParams(dict);
            }
            if (urlStr != null)
                result.Attributes[HtmlAttr.Href] = urlStr;
        }

        public ControlProperty GetParamsProperty { get; private set; }
    }
}
