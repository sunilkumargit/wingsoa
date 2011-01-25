using System;


namespace Wing.Mvc.Controls
{
    public class HtmlBooleanAttributePropertyApplier : IControlPropertyApplier
    {
        public HtmlBooleanAttributePropertyApplier(HtmlAttr attribute, bool defaultValue, String valueIfTrue, String valueIfFalse)
        {
            this.Attribute = attribute;
            this.DefaultValue = defaultValue;
            this.ValueIfTrue = valueIfTrue;
            this.ValueIfFalse = valueIfFalse;
        }

        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            var boolValue = ConversionHelper.ToBoolean(value);
            if (boolValue == DefaultValue)
                return;
            result.Attributes[Attribute] = boolValue ? ValueIfTrue : ValueIfFalse;
        }

        public HtmlAttr Attribute { get; private set; }

        public bool DefaultValue { get; private set; }

        public string ValueIfTrue { get; private set; }

        public string ValueIfFalse { get; private set; }
    }
}
