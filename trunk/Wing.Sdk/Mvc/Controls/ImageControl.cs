using System;

namespace Wing.Mvc.Controls
{
    public class ImageControl : HtmlControl
    {
        public static readonly ControlProperty SrcProperty = ControlProperty.Register("Src",
            typeof(String),
            typeof(ImageControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Src));

        public static readonly ControlProperty AltProperty = ControlProperty.Register("Alt",
            typeof(String),
            typeof(ImageControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Alt));

        public ImageControl(String src = "", String alt = "")
            : base(HtmlTag.Img)
        {
            this.Src = src;
            this.Alt = alt;
        }

        public String Src
        {
            get { return GetValue<String>(SrcProperty); }
            set { SetValue(SrcProperty, value); }
        }

        public String Alt
        {
            get { return GetValue<String>(AltProperty); }
            set { SetValue(AltProperty, value); }
        }
    }
}
