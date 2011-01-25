using System;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class EmbedScript : EmbedFileContentBase<EmbedScript>
    {
        public EmbedScript(String filePath = "")
            : base(HtmlTag.Script, filePath)
        {
            Attributes[HtmlAttr.Type] = "text/javascript";
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent(innerText, rawInnerText);
        }
    }
}
