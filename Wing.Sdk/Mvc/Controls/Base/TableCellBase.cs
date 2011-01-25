
namespace Wing.Mvc.Controls.Base
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class TableCellBase<TConcreteType> : ContainerControl<TConcreteType> where TConcreteType : TableCellBase<TConcreteType>
    {
        public static readonly ControlProperty ColSpanProperty = ControlProperty.Register("ColSpan", typeof(int), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Colspan, 0), 0);

        public static readonly ControlProperty RowSpanProperty = ControlProperty.Register("RowSpan", typeof(int), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Rowspan, 0), 0);

        public static readonly ControlProperty HorizontalContentAlignProperty = ControlProperty.Register("HorizontalContentAlign", typeof(HorizontalAlignment), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Align, HorizontalAlignment.inherit), HorizontalAlignment.inherit);

        [System.Diagnostics.DebuggerStepThrough]
        public TableCellBase(HtmlTag tag)
            : base(tag)
        {
        }

        public int ColSpan
        {
            get { return GetValue<int>(ColSpanProperty); }
            set { SetValue(ColSpanProperty, value); }
        }

        public int RowSpan
        {
            get { return GetValue<int>(RowSpanProperty); }
            set { SetValue(RowSpanProperty, value); }
        }

        public HorizontalAlignment HorizontalContentAlign
        {
            get { return GetValue<HorizontalAlignment>(HorizontalContentAlignProperty); }
            set { SetValue(HorizontalContentAlignProperty, value); }
        }

        public bool NoWrap
        {
            get { return WhiteSpaceBehavior == CssWhiteSpace.NoWrap; }
            set { WhiteSpaceBehavior = value ? CssWhiteSpace.NoWrap : CssWhiteSpace.Normal; }
        }

        public TConcreteType SetColSpan(int colSpan)
        {
            ColSpan = colSpan;
            return This;
        }

        public TConcreteType SetRowSpan(int rowSpan)
        {
            RowSpan = rowSpan;
            return This;
        }

        public TConcreteType SetHorizontalAlign(HorizontalAlignment align)
        {
            HorizontalContentAlign = align;
            return This;
        }

        public TConcreteType SetNoWrap(bool nowrap)
        {
            NoWrap = nowrap;
            return This;
        }
    }
}
