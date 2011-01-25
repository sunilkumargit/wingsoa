using System;
using System.Collections.Generic;

namespace Wing.Mvc.Controls
{
    public class Link : ContainerControl<Link>
    {
        public static readonly ControlProperty GetParamsProperty = ControlProperty.Register("GetParams",
            typeof(IDictionary<String, Object>),
            typeof(Link));

        public static readonly ControlProperty HrefProperty = ControlProperty.Register("Href",
            typeof(String),
            typeof(Link),
            new HyperLinkHrefPropertyApplier(GetParamsProperty));

        public Link()
            : base(HtmlTag.A)
        {

        }

        public IDictionary<String, Object> GetParams
        {
            get
            {
                var result = GetValue<Dictionary<String, Object>>(GetParamsProperty);
                if (result == null)
                {
                    result = new Dictionary<string, Object>();
                    GetParams = result;
                }
                return result;
            }
            set { SetValue(GetParamsProperty, value); }
        }

        public String HRef
        {
            get { return GetValue<String>(HrefProperty); }
            set { SetValue(HrefProperty, value); }
        }
    }
}
