using System;


namespace Wing.Mvc.Controls
{
    public class HtmlAttributePropertyApplier : IControlPropertyApplier
    {
        public HtmlAttributePropertyApplier(HtmlAttr attribute, Object defaultValue = null)
        {
            this.Attribute = attribute;
            this.DefaultValue = defaultValue;
        }

        public HtmlAttr Attribute { get; private set; }
        public object DefaultValue { get; private set; }

        public void ApplyProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            if (value.AsString().IsEmpty() || value.AsString() == DefaultValue.AsString())
                return;

            result.Attributes[Attribute.ToString().ToLower()] = value.AsString();
        }
    }
}
