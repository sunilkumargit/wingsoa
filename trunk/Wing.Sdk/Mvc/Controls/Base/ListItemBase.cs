using System;

namespace Wing.Mvc.Controls.Base
{
    public abstract class ListItemBase : HtmlControl
    {
        public static ControlProperty LabelProperty = ControlProperty.Register("Label",
            typeof(String),
            typeof(ListItemBase),
            new HtmlAttributePropertyApplier(HtmlAttr.Label));

        public static ControlProperty ValueProperty = ControlProperty.Register("Value",
            typeof(String),
            typeof(ListItemBase),
            new HtmlAttributePropertyApplier(HtmlAttr.Value));

        public static ControlProperty IsSelectedProperty = ControlProperty.Register("IsSelected",
            typeof(bool),
            typeof(ListItemBase),
            new HtmlBooleanAttributePropertyApplier(HtmlAttr.Selected, false, "selected", ""),
            false,
            IsSelectedPropertyChanged);

        private IListItemContainer _parent;

        public ListItemBase(HtmlTag tag) : base(tag) { }

        internal void SetParentList(IListItemContainer parent)
        {
            _parent = parent;
        }

        public String Label
        {
            get { return GetValue<String>(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public String Value
        {
            get { return GetValue<String>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public bool IsSelected
        {
            get { return GetValue<bool>(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void IsSelectedPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var targetItem = args.Target as ListItemBase;
            if (targetItem != null && targetItem._parent != null)
                targetItem._parent.NotifyItemSelectedPropertyChanged(targetItem, (bool)args.NewValue);
        }

    }
}
