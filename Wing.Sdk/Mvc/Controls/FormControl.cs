using System;

namespace Wing.Mvc.Controls
{
    public class FormControl : ContainerControl<FormControl>
    {
        public static readonly ControlProperty MethodProperty = ControlProperty.Register("Method", typeof(HtmlFormMethod), typeof(FormControl), new HtmlAttributePropertyApplier(HtmlAttr.Method), HtmlFormMethod.Post);


        public static readonly ControlProperty ActionProperty = ControlProperty.Register("Action", typeof(String), typeof(FormControl), new HtmlAttributePropertyApplier(HtmlAttr.Action), "");

        public FormControl()
            : base(HtmlTag.Form)
        {
            Method = HtmlFormMethod.Post;
            Action = "/";
        }

        public string Action
        {
            get { return GetValue<String>(ActionProperty); }
            set { SetValue(ActionProperty, value); }
        }

        public HtmlFormMethod Method
        {
            get { return GetValue<HtmlFormMethod>(MethodProperty); }
            set { SetValue(MethodProperty, value); }
        }
    }
}
