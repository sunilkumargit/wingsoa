using System;


namespace Wing.Mvc.Controls
{
    public class HtmlCustomAttributePropertyApplier : IControlPropertyApplier
    {
        public HtmlCustomAttributePropertyApplier(String attribute, Object defaultValue = null)
        {
            this.Attribute = attribute;
            this.DefaultValue = defaultValue;
        }

        public String Attribute { get; private set; }
        public object DefaultValue { get; private set; }

        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            if (value.AsString().IsEmpty() || value.AsString() == DefaultValue.AsString())
                return;

            result.Attributes[Attribute.ToString().ToLower()] = value.AsString();
        }
    }
}
