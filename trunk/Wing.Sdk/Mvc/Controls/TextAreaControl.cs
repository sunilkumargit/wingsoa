using System;
using Wing.Mvc.Controls.Base;


namespace Wing.Mvc.Controls
{
    public class TextAreaControl : InputValueControlBase
    {
        public static readonly ControlProperty ColumnsProperty = ControlProperty.Register("Columns",
            typeof(int),
            typeof(TextAreaControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Cols, 0), 0);

        public static readonly ControlProperty RowsProperty = ControlProperty.Register("Rows",
            typeof(int),
            typeof(TextAreaControl),
            new HtmlAttributePropertyApplier(HtmlAttr.Rows, 0), 0);

        public TextAreaControl(String name = "") : base(HtmlTag.Textarea, name) { }
        public TextAreaControl() : this("") { }

        public int Columns
        {
            get { return GetValue<int>(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public int Rows
        {
            get { return GetValue<int>(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        protected override string ConvertRawValue(object rawValue)
        {
            return rawValue.AsString();
        }

        protected override void ApplyValueProperty(HtmlObject target, ControlProperty property, object value, ControlPropertyApplyResult result)
        {
            result.InnerText.Append(value.AsString());
        }
    }
}
