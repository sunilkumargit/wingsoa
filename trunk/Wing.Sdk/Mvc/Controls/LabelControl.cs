using System;

namespace Wing.Mvc.Controls
{
    public class LabelControl : HtmlControl
    {
        public static readonly ControlProperty ForProperty = ControlProperty.Register("For",
            typeof(String),
            typeof(LabelControl),
            new HtmlAttributePropertyApplier(HtmlAttr.For));

        public LabelControl() : base(HtmlTag.Label) { }

        public String For
        {
            get { return GetValue<String>(ForProperty); }
            set { SetValue(ForProperty, value); }
        }
    }
}
