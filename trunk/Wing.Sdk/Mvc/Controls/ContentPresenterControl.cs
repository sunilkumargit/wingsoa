using System;

namespace Wing.Mvc.Controls
{
    public class ContentPresenterControl : HtmlObject
    {
        public static readonly ControlProperty ContentProperty = ControlProperty.Register("Content",
            typeof(Object),
            typeof(ContentPresenterControl));

        public ContentPresenterControl() : base(HtmlTag.Unknown) { }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            var content = Content;
            if (content != null)
            {
                var control = content as HtmlObject;
                if (control != null)
                    control.Render(CurrentContext);
                else if (content is IHtmlControlCollection)
                    foreach (HtmlObject c in (IHtmlControlCollection)content)
                        c.Render(CurrentContext);
                else
                    innerText += content.ToString();
            }
            base.RenderContent(innerText, rawInnerText);
        }

        public Object Content
        {
            get { return GetValue<Object>(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}
