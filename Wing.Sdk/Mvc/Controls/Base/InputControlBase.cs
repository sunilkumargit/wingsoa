using System;

namespace Wing.Mvc.Controls.Base
{
    public abstract class InputControlBase : HtmlControl, IFormField
    {
        public static readonly ControlProperty NameProperty = ControlProperty.Register("Name",
            typeof(String),
            typeof(InputControlBase),
            new HtmlAttributePropertyApplier(HtmlAttr.Name));

        public static readonly ControlProperty IsDisabledProperty = ControlProperty.Register("IsDisabled",
            typeof(bool),
            typeof(InputControlBase),
            new HtmlBooleanAttributePropertyApplier(HtmlAttr.Disabled, false, "disabled", ""), false);

        public InputControlBase(HtmlTag tag, String name)
            : base(tag)
        {
            Name = name;
        }

        public String Name
        {
            get { return GetValue<String>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }


        public bool IsDisabled
        {
            get { return GetValue<bool>(IsDisabledProperty); }
            set { SetValue(IsDisabledProperty, value); }
        }

        internal bool SettingValueFromRaw = false;

        protected override void PreRender()
        {
            base.PreRender();
            if (String.IsNullOrEmpty(this.Name) && String.IsNullOrEmpty(this.Id))
                this.Id = HtmlControl.CreateUniqueId();
            if (String.IsNullOrEmpty(this.Name))
                this.Name = this.Id;
            else if (String.IsNullOrEmpty(this.Id))
                this.Id = this.Name;
        }
    }
}
