using System;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class SelectOption : ListItemBase
    {
        public static readonly ControlProperty GroupProperty = ControlProperty.Register("Group",
            typeof(String),
            typeof(SelectOption));

        public SelectOption() : base(HtmlTag.Option) { }

        public string Group
        {
            get { return GetValue<String>(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }
    }
}
