using System;

namespace Wing.Mvc.Controls.Base
{
    public abstract class CheckControlBase<TConcreteType> : InputValueControlBase
        where TConcreteType : CheckControlBase<TConcreteType>
    {
        [TemplatePart]
        public static readonly ControlProperty ContentTemplateProperty = ControlProperty.Register("ContentTemplate",
            typeof(HtmlObject),
            typeof(TConcreteType),
            (new TextBlock())
                .SetTemplateBinding(TextBlock.TextProperty, InputValueControlBase.TextProperty)
                .SetTemplateBinding(TextBlock.FontWeightProperty, InputValueControlBase.FontWeightProperty));


        public static readonly ControlProperty IsCheckedProperty = ControlProperty.Register("IsChecked",
            typeof(bool),
            typeof(TConcreteType),
            new HtmlBooleanAttributePropertyApplier(HtmlAttr.Checked, false, "checked", ""), false);

        public CheckControlBase(HtmlTag tag, String name)
            : base(tag, name)
        {
        }

        protected override void RenderBeginTag(string tagName, HtmlAttributeCollection attributes)
        {
            var ctx = CurrentContext;
            ctx.Document.AddStyleAttribute("white-space", "nowrap");
            ctx.Document.RenderBeginTag("span");
            base.RenderBeginTag(tagName, attributes);
        }

        protected override void RenderEndTag()
        {
            base.RenderEndTag();
            CurrentContext.Document.RenderEndTag();
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent(innerText, rawInnerText);
            if (ContentTemplate != null)
                ContentTemplate.Render(CurrentContext);
        }

        public bool IsChecked
        {
            get { return GetValue<bool>(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        public HtmlObject ContentTemplate
        {
            get { return GetValue<HtmlObject>(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        protected override string ConvertRawValue(object rawValue)
        {
            return rawValue.AsString();
        }

        protected override void ApplyValueProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            if (value.AsString().HasValue())
                result.Attributes[HtmlAttr.Value] = value.AsString();
        }

        public TConcreteType SetIsChecked(bool isChecked)
        {
            this.IsChecked = isChecked;
            return (TConcreteType)this;
        }
    }
}
