using System;

namespace Wing.Mvc.Controls
{
    [System.Diagnostics.DebuggerStepThrough]
    public class DataTableColumn
    {
        public DataTableColumn(DataTableControl table)
        {
            DataTable = table;
            ColSpanBehavior = ColSpanBehavior.NoSpan;
        }

        public String Name { get; set; }
        public String Caption { get; set; }
        public String PropertyName { get; set; }
        public string CssClass { get; set; }
        public String HeaderCssClass { get; set; }
        public HorizontalAlignment? HorizontalAlign { get; set; }
        public int MaxLength { get; set; }
        public bool? NoWrap { get; set; }
        public ColSpanBehavior ColSpanBehavior { get; set; }
        public int ColSpan { get; set; }
        public TableHeaderCell HeaderTemplate { get; set; }
        public TableHeaderCell FooterTemplate { get; set; }
        public TableCell ItemTemplate { get; set; }
        public DataTableControl DataTable { get; private set; }

        public DataTableColumn SetCaption(String caption)
        {
            this.Caption = caption;
            return this;
        }

        public DataTableColumn SetCssClass(String cssClass)
        {
            this.CssClass = cssClass;
            return this;
        }

        public DataTableColumn SetHeaderCssClass(String cssClass)
        {
            this.HeaderCssClass = cssClass;
            return this;
        }

        public DataTableColumn SetHorizontalAlign(HorizontalAlignment? align)
        {
            this.HorizontalAlign = align;
            return this;
        }

        public DataTableColumn SetNoWrap(bool? noWrap)
        {
            this.NoWrap = noWrap;
            return this;
        }

        public DataTableColumn SetColSpanBehavior(ColSpanBehavior behavior)
        {
            this.ColSpanBehavior = behavior;
            return this;
        }

        public DataTableColumn SetColSpan(int colSpan)
        {
            this.ColSpan = colSpan;
            return this;
        }

        public DataTableColumn SetItemTemplate(TableCell itemTemplate)
        {
            this.ItemTemplate = itemTemplate;
            return this;
        }

        public DataTableColumn SetItemTemplate(HtmlControl innerControlTemplate)
        {
            this.ItemTemplate = new TableCell().Add(innerControlTemplate);
            return this;
        }

        public DataTableColumn SetFooterTemplate(TableHeaderCell footerTemplate)
        {
            this.FooterTemplate = footerTemplate;
            return this;
        }

        public DataTableColumn SetFooterTemplate(HtmlControl innerControlTemplate)
        {
            this.FooterTemplate = new TableHeaderCell().Add(innerControlTemplate);
            return this;
        }

        public DataTableColumn SetHeaderTemplate(TableHeaderCell headerTemplate)
        {
            this.HeaderTemplate = headerTemplate;
            return this;
        }

        public DataTableColumn SetHeaderTemplate(HtmlControl innerControlTemplate)
        {
            this.HeaderTemplate = new TableHeaderCell().Add(innerControlTemplate);
            return this;
        }

    }
}