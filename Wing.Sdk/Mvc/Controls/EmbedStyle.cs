using System;
using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class EmbedStyle : EmbedFileContentBase<EmbedStyle>
    {
        public EmbedStyle(String filePath = "")
            : base(HtmlTag.Style, filePath)
        {
            Attributes[HtmlAttr.Type] = "text/css";
        }
    }

}
