using System;
using System.IO;

namespace Wing.Mvc.Controls.Base
{
    public abstract class EmbedFileContentBase<TConcreteType> : HtmlObject where TConcreteType : EmbedFileContentBase<TConcreteType>
    {
        public static readonly ControlProperty FilePathProperty = ControlProperty.Register("FilePath",
            typeof(String),
            typeof(TConcreteType));

        public EmbedFileContentBase(HtmlTag tag, String filePath = "")
            : base(tag)
        {
            FilePath = filePath;
        }

        public string FilePath
        {
            get { return GetValue<String>(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            var path = FilePath;
            var data = "";
            if (!String.IsNullOrEmpty(path))
            {
                if (File.Exists(path))
                    data = File.ReadAllText(FilePath);
                else if (File.Exists(CurrentContext.MapPath(path)))
                    data = File.ReadAllText(CurrentContext.MapPath(path));
                else
                    data = "File not found: " + path;
                data = " " + data;
            }
            base.RenderContent(innerText, rawInnerText + data);
        }
    }
}
