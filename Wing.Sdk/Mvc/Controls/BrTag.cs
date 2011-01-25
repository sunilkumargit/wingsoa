
namespace Wing.Mvc.Controls
{
    public class BrTag : HtmlControl
    {
        public BrTag()
            : base(HtmlTag.Unknown)
        {

        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent(innerText, rawInnerText);
            CurrentContext.Document.Write("<br/>");
        }
    }
}
