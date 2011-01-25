using System;

namespace Wing.Mvc.Controls.Base
{
    public abstract class InputValueControlBase : InputControlBase, IValuePropertyApplier
    {
        public static readonly ControlProperty ValueProperty = ControlProperty.Register("Value",
            typeof(String),
            typeof(InputValueControlBase),
            new DelegateControlPropertyApplier(ApplyValuePropertyCallback),
            null,
            ValuePropertyChanged);

        public static readonly ControlProperty RawValueProperty = ControlProperty.Register("RawValue",
            typeof(Object),
            typeof(InputValueControlBase),
            null,
            RawValuePropertyChanged);

        public InputValueControlBase(HtmlTag tag, String name)
            : base(tag, name)
        {

        }

        public String Value
        {
            get { return GetValue<String>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public Object RawValue
        {
            get { return GetValue(RawValueProperty); }
            set { SetValue(RawValueProperty, value); }
        }

        private static void ValuePropertyChanged(ControlPropertyChangedEventArgs args)
        {
            if (!((InputControlBase)args.Target).SettingValueFromRaw)
                args.Target.SetValue(RawValueProperty, args.NewValue);
        }

        private static void RawValuePropertyChanged(ControlPropertyChangedEventArgs args)
        {
            var target = args.Target as InputValueControlBase;
            var strValue = target.ConvertRawValueInternal(args.NewValue);

            ((InputControlBase)args.Target).SettingValueFromRaw = true;
            args.Target.SetValue(ValueProperty, strValue);
            ((InputControlBase)args.Target).SettingValueFromRaw = false;
        }

        internal String ConvertRawValueInternal(Object rawValue)
        {
            return this.ConvertRawValue(rawValue);
        }

        private static void ApplyValuePropertyCallback(HtmlObject target, ControlProperty property, Object value, ControlPropertyApplyResult result)
        {
            var applierTarget = target as IValuePropertyApplier;
            if (applierTarget != null)
                applierTarget.ApplyValueProperty(target, property, value, result);
            else
                throw new Exception("The target control must implement IValuePropertyApplier in order to accepted ValueProperty value apply.");
        }

        void IValuePropertyApplier.ApplyValueProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            this.ApplyValueProperty(target, property, value, result);
        }

        protected abstract String ConvertRawValue(Object rawValue);
        protected abstract void ApplyValueProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result);

    }
}
