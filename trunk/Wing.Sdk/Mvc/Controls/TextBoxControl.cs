using System;

using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class TextBoxControl : InputValueControlBase
    {
        public static readonly ControlProperty UserMaskProperty = ControlProperty.Register("UserMask",
            typeof(UserMask),
            typeof(TextBoxControl),
            UserMask.Default);

        public static readonly ControlProperty CustomMaskProperty = ControlProperty.Register("CustomMask",
            typeof(String),
            typeof(TextBoxControl),
            null,
            CustomMaskPropertyChanged);

        public static readonly ControlProperty IsReadOnlyProperty = ControlProperty.Register("IsReadOnly",
            typeof(bool),
            typeof(TextBoxControl),
            new HtmlBooleanAttributePropertyApplier(HtmlAttr.ReadOnly, false, "readonly", ""), false);

        public static readonly ControlProperty IsHiddenProperty = ControlProperty.Register("IsHidden",
            typeof(bool),
            typeof(TextBoxControl), false);

        public static readonly ControlProperty IsPasswordProperty = ControlProperty.Register("IsPassword",
            typeof(bool),
            typeof(TextBoxControl), false);

        public static readonly ControlProperty SizeProperty = ControlProperty.Register("Size",
            typeof(int),
            typeof(TextBoxControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Size, 0), 0);

        public static readonly ControlProperty MaxLengthProperty = ControlProperty.Register("MaxLength",
            typeof(int),
            typeof(TextBoxControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Maxlength, 0), 0);

        public TextBoxControl() : base(HtmlTag.Input, null) { }
        public TextBoxControl(String name) : base(HtmlTag.Input, name) { }

        public UserMask UserMask
        {
            get { return GetValue<UserMask>(UserMaskProperty); }
            set { SetValue(UserMaskProperty, value); }
        }

        public String CustomMask
        {
            get { return GetValue<String>(CustomMaskProperty); }
            set { SetValue(CustomMaskProperty, value); }
        }

        private static void CustomMaskPropertyChanged(ControlPropertyChangedEventArgs args)
        {
            if (args.NewValue.AsString().HasValue())
                args.Target.SetValue(UserMaskProperty, UserMask.Custom);
            else if (((UserMask)args.Target.GetValue(UserMaskProperty)) == UserMask.Custom)
                args.Target.SetValue(UserMaskProperty, UserMask.Default);
        }

        public bool IsReadOnly
        {
            get { return GetValue<bool>(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public int Size
        {
            get { return GetValue<int>(SizeProperty); }
            set { SetValue(SizeProperty, value); }

        }

        public int MaxLength
        {
            get { return GetValue<int>(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public bool IsHidden
        {
            get { return GetValue<bool>(IsHiddenProperty); }
            set { SetValue(IsHiddenProperty, value); }
        }

        public bool IsPassword
        {
            get { return GetValue<bool>(IsPasswordProperty); }
            set { SetValue(IsPasswordProperty, value); }
        }

        protected override void DataBind()
        {
            base.DataBind();
        }

        protected override void PreRender()
        {
            base.PreRender();
            if (!IsHidden && !IsPassword)
                Attributes[HtmlAttr.Type] = "text";
            else if (IsHidden)
                Attributes[HtmlAttr.Type] = "hidden";
            else
                Attributes[HtmlAttr.Type] = "password";
        }

        protected override string ConvertRawValue(object rawValue)
        {
            if (rawValue == null)
                return "";
            if (ConversionHelper.IsFloatingPointType(rawValue.GetType()))
                return String.Format("{0:N}", rawValue);
            return rawValue.ToString();
        }

        protected override void ApplyValueProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            if (value.AsString().HasValue())
                result.Attributes[HtmlAttr.Value] = value.AsString();
        }
    }
}
