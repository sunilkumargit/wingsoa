using System.Collections;

namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class DataListControl : HtmlControl
    {
        public static readonly ControlProperty ColumnsProperty = ControlProperty.Register("Columns",
            typeof(int),
            typeof(DataListControl),
            1);

        public static readonly ControlProperty ItemTemplateProperty = ControlProperty.Register("ItemTemplate",
            typeof(HtmlControl),
            typeof(DataListControl));

        public static readonly ControlProperty ItemsSourceProperty = ControlProperty.Register("ItemsSource",
            typeof(object),
            typeof(DataListControl));

        public DataListControl() : base(HtmlTag.Div) { }

        public int Columns
        {
            get { return GetValue<int>(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public HtmlControl ItemTemplate
        {
            get { return GetValue<HtmlControl>(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public object ItemsSource
        {
            get { return GetValue<object>(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        protected override void RenderContent(string innerText, string rawInnerText)
        {
            base.RenderContent("", "");
            if (ItemTemplate == null || ItemsSource == null)
                return;

            var dataSource = ItemsSource as IEnumerable;

            if (dataSource == null)
                return;

            bool tr = false;
            int col = 0;
            var ctx = CurrentContext;

            ctx.Document.RenderBeginTag("table");
            foreach (var item in dataSource)
            {
                if (!tr)
                {
                    ctx.Document.RenderBeginTag("tr");
                    tr = true;
                }
                ctx.Document.RenderBeginTag("td");
                ItemTemplate.DataContext = item;
                ItemTemplate.Render(ctx);
                ctx.Document.RenderEndTag();
                if (++col >= Columns)
                {
                    col = 0;
                    ctx.Document.RenderEndTag();
                    tr = false;
                }
            }
            if (tr)
                ctx.Document.RenderEndTag();
            ctx.Document.RenderEndTag();
        }


    }
}
