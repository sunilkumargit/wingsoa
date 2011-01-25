using System;

namespace Wing.Mvc.Controls
{
    public class ContentControl : HtmlControl
    {
        public static readonly ControlProperty ContentProperty = ControlProperty.Register("Content",
            typeof(Object),
            typeof(ContentControl));

        [TemplatePart]
        public static readonly ControlProperty ContentTemplateProperty = ControlProperty.Register("ContentTemplate",
            typeof(HtmlObject),
            typeof(ContentControl));

        private static ContentPresenterControl _defaultContentTemplate = new ContentPresenterControl()
            .SetTemplateBinding(ContentPresenterControl.ContentProperty, ContentProperty);

        public ContentControl() : base(HtmlTag.Div) { }

        public HtmlObject Content
        {
            get { return GetValue<HtmlObject>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public HtmlObject ContentTemplate
        {
            get { return GetValue<HtmlObject>(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent(innerText, rawInnerText);
            if (ContentTemplate != null)
                ContentTemplate.Render(CurrentContext);
            else
            {
                _defaultContentTemplate.ApplyTemplate(this);
                _defaultContentTemplate.Render(CurrentContext);
            }
        }
    }
}
