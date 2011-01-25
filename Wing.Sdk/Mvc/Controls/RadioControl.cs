using System;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class RadioControl : CheckControlBase<RadioControl>
    {
        public RadioControl(String name = "") : base(HtmlTag.Input, name) { }

        protected override void PreRender()
        {
            base.PreRender();
            Attributes[HtmlAttr.Type] = "radio";
        }
    }
}
