using System;

namespace Wing.Mvc.Controls
{
    public class ScriptRef : HtmlObject
    {
        public static readonly ControlProperty SrcProperty = ControlProperty.Register("Src",
            typeof(String),
            typeof(ScriptRef),
            new HtmlAttributePropertyApplier(HtmlAttr.Src));

        public ScriptRef(String src = "")
            : base(HtmlTag.Script)
        {
            Attributes[HtmlAttr.Type] = "text/javascript";
            Src = src;
        }

        public String Src
        {
            get { return GetValue<String>(SrcProperty); }
            set { SetValue(SrcProperty, value); }
        }
    }
}
