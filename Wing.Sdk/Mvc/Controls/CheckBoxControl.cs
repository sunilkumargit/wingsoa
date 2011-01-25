using System;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class CheckBoxControl : CheckControlBase<CheckBoxControl>
    {
        public CheckBoxControl(String name = "") : base(HtmlTag.Input, name) { }

        protected override void PreRender()
        {
            base.PreRender();
            Attributes[HtmlAttr.Type] = "checkbox";
        }
    }
}
