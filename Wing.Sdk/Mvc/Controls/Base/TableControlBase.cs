using System;

namespace Wing.Mvc.Controls.Base
{
    public abstract class TableControlBase<TConcreteType, THeaderRowType, THeaderCellType, TRowType, TCellType> : ContainerControl<TConcreteType, TRowType>
        where TConcreteType : TableControlBase<TConcreteType, THeaderRowType, THeaderCellType, TRowType, TCellType>
        where THeaderRowType : TableRowBase<THeaderRowType, THeaderCellType>, new()
        where TRowType : TableRowBase<TRowType, TCellType>, new()
        where THeaderCellType : TableCellBase<THeaderCellType>, new()
        where TCellType : TableCellBase<TCellType>, new()
    {
        public TableControlBase()
            : base(HtmlTag.Table)
        {
            HeaderRows = new HtmlControlCollection<THeaderRowType>();
            FooterRows = new HtmlControlCollection<THeaderRowType>();
            RegisterChildrenCollection(HeaderRows, FooterRows);
        }

        public static readonly ControlProperty TableBorderProperty = ControlProperty.Register("TableBorder", typeof(int), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Border), 0);
        public static readonly ControlProperty CellSpacingProperty = ControlProperty.Register("CellSpacing", typeof(int), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Cellspacing), 0);
        public static readonly ControlProperty CellPaddingProperty = ControlProperty.Register("CellPadding", typeof(int), typeof(TConcreteType), new HtmlAttributePropertyApplier(HtmlAttr.Cellpadding), 0);

        public HtmlControlCollection<THeaderRowType> HeaderRows { get; private set; }
        public HtmlControlCollection<THeaderRowType> FooterRows { get; private set; }

        public int CellPadding
        {
            get { return GetValue<int>(CellPaddingProperty); }
            set { SetValue(CellPaddingProperty, value); }
        }

        public int CellSpacing
        {
            get { return GetValue<int>(CellSpacingProperty); }
            set { SetValue(CellSpacingProperty, value); }
        }

        public int TableBorder
        {
            get { return GetValue<int>(TableBorderProperty); }
            set { SetValue(TableBorderProperty, value); }
        }

        public bool HasHeader { get { return HeaderRows.Count > 0; } }
        public bool HasFooter { get { return FooterRows.Count > 0; } }

        protected override void RenderContent(String innerText, String rawInnerText)
        {
            if (HasHeader)
            {
                CurrentContext.Document.RenderBeginTag("thead");
                HeaderRows.RenderChildren(CurrentContext);
                CurrentContext.Document.RenderEndTag();
            }
            CurrentContext.Document.RenderBeginTag("tbody");
            base.RenderContent("", "");
            if (HasFooter)
            {
                CurrentContext.Document.RenderBeginTag("tfooter");
                FooterRows.RenderChildren(CurrentContext);
                CurrentContext.Document.RenderEndTag();
            }
            CurrentContext.Document.RenderEndTag();
        }

        public TConcreteType SetBorder(int border)
        {
            TableBorder = border;
            return This;
        }

        public TConcreteType SetCellPadding(int cellPadding)
        {
            CellPadding = cellPadding;
            return This;
        }

        public TConcreteType SetCellSpacing(int cellSpacing)
        {
            CellSpacing = cellSpacing;
            return This;
        }

        public TRowType NewRow()
        {
            var row = new TRowType();
            Add(row);
            return row;
        }

        public THeaderRowType NewHeaderRow()
        {
            var row = new THeaderRowType();
            HeaderRows.Add(row);
            return row;
        }

        public TCellType NewCell()
        {
            if (Children.Count == 0)
                return NewRow().NewCell();
            return (Children[Children.Count - 1]).NewCell();
        }

        public TCellType NewCell(String text)
        {
            return NewCell().SetText(text);
        }

        public TCellType NewCell(params HtmlControl[] innerControls)
        {
            return NewCell().Add(innerControls);
        }

        public THeaderCellType NewHeaderCell()
        {
            if (HeaderRows.Count == 0)
                return NewHeaderRow().NewCell();
            return HeaderRows[HeaderRows.Count - 1].NewCell();
        }

        public TConcreteType AddHeader(THeaderCellType headerCell)
        {
            if (HeaderRows.Count == 0)
                NewHeaderRow().Add(headerCell);
            else
                HeaderRows[HeaderRows.Count - 1].Add(headerCell);
            return This;
        }

        public TConcreteType AddCell(params HtmlControl[] innerControls)
        {
            NewCell().Add(innerControls);
            return This;
        }

        public TConcreteType AddRow()
        {
            NewRow();
            return This;
        }

        public TConcreteType AddCell(string text)
        {
            NewCell().SetText(text);
            return This;
        }

        public TConcreteType AddHeader(string text)
        {
            NewHeaderCell().SetText(text);
            return This;
        }
    }
}
