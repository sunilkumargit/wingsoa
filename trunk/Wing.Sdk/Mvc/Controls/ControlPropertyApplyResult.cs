using System.Text;

namespace Wing.Mvc.Controls
{
    public class ControlPropertyApplyResult
    {
        internal ControlPropertyApplyResult(HtmlObject target)
        {
            this.Target = target;
            this.Attributes = new HtmlAttributeCollection();
            this.Styles = new HtmlStyleCollection();
        }

        public HtmlObject Target { get; private set; }

        public HtmlAttributeCollection Attributes { get; private set; }

        public HtmlStyleCollection Styles { get; private set; }

        public StringBuilder InnerText { get; private set; }

        public StringBuilder RawInnerText { get; private set; }

        internal void Begin()
        {
            Attributes.Clear();
            Styles.Clear();
            InnerText = new StringBuilder();
            RawInnerText = new StringBuilder();
        }
    }
}
