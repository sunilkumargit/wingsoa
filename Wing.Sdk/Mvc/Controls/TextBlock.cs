
using System;

namespace Wing.Mvc.Controls
{
    /// <summary>
    /// Render a <span></span> tag with text or raw html,
    /// converts linebreak (\n\r) charactes to <br/> tag element.
    /// </summary>
    public class TextBlock : HtmlControl
    {
        public TextBlock()
            : base(HtmlTag.Span)
        {
        }

        public TextBlock(String text = "")
            : this()
        {
            Text = text;
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            var isFirst = true;
            foreach (var line in StringHelper.SplitLines(innerText))
            {
                if (!isFirst)
                    CurrentContext.Document.Write("<br/>");
                CurrentContext.Document.WriteEncodedText(line);
                isFirst = false;
            }
            base.RenderContent("", rawInnerText);
        }
    }
}
