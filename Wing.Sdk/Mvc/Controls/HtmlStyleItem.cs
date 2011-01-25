using System;

namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public class HtmlStyleItem
    {
        public CssProperty Style { get; set; }
        public String StyleTag { get { return CssConfig.GetItemNameFromEnum(typeof(CssProperty), Style); } }
        public String Value { get; set; }

        public HtmlStyleItem(CssProperty style, string value)
        {
            Style = style;
            Value = value;
        }
    }
}
