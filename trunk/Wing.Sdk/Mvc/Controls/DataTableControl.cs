using System;
using System.Collections;
using System.Collections.Generic;

using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class DataTableControl : HtmlControl
    {
        public static readonly ControlProperty EmptySourceTemplateProperty = ControlProperty.Register("EmptySourceTemplate",
            typeof(HtmlObject),
            typeof(DataTableControl),
            null,
            new PanelControl().SetText("sem dados para exibir"));

        public static readonly ControlProperty TitleTemplateProperty = ControlProperty.Register("TitleTemplateProperty",
            typeof(TableHeaderCell),
            typeof(DataTableControl));

        public static readonly ControlProperty TitleVisibilityProperty = ControlProperty.Register("TitleVisibility",
            typeof(Visibility),
            typeof(DataTableControl),
            null,
            Visibility.Visible);

        public static readonly ControlProperty HeaderVisibiliyProperty = ControlProperty.Register("HeaderVisibility",
            typeof(Visibility),
            typeof(DataTableControl),
            null,
            Visibility.Visible);

        public static readonly ControlProperty FooterVisibilityProperty = ControlProperty.Register("FooterVisibility",
            typeof(Visibility),
            typeof(DataTableControl),
            null,
            Visibility.None);

        public static readonly ControlProperty ItemsSourceProperty = ControlProperty.Register("ItemsSource",
            typeof(Object),
            typeof(DataTableControl));

        public static readonly ControlProperty DefaultHeaderCellClassProperty = ControlProperty.Register("DefaultHeaderCell",
            typeof(String),
            typeof(DataTableControl));


        private List<DataTableColumn> _columns = new List<DataTableColumn>();
        private bool _hasFooterCells;

        public DataTableControl()
            : base(HtmlTag.Div)
        {
        }

        public TableHeaderCell TitleTemplate
        {
            get { return GetValue<TableHeaderCell>(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }

        public HtmlObject EmptySourceTemplate
        {
            get { return GetValue<HtmlObject>(EmptySourceTemplateProperty); }
            set { SetValue(EmptySourceTemplateProperty, value); }
        }

        public Visibility TitleVisibility
        {
            get { return GetValue<Visibility>(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        public Visibility HeaderVisibility
        {
            get { return GetValue<Visibility>(HeaderVisibiliyProperty); }
            set { SetValue(HeaderVisibiliyProperty, value); }
        }

        public Visibility FooterVisibility
        {
            get { return GetValue<Visibility>(FooterVisibilityProperty); }
            set { SetValue(FooterVisibilityProperty, value); }
        }

        public Object ItemsSource
        {
            get { return GetValue<Object>(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public String DefaultHeaderCellClass
        {
            get { return GetValue<String>(DefaultHeaderCellClassProperty); }
            set { SetValue(DefaultHeaderCellClassProperty, value); }
        }

        public List<DataTableColumn> Columns { get { return _columns; } }

        protected override void RenderContent(String innerText, String rawInnerText)
        {
            var dataSource = ItemsSource as IEnumerable;
            var innerTable = new TableControl();

            innerTable.DataContext = dataSource;
            innerTable.Children.Clear();
            innerTable.HeaderRows.Clear();
            innerTable.FooterRows.Clear();

            if (dataSource != null && dataSource.GetEnumerator().MoveNext())
            {

                if (TitleVisibility != Visibility.None && TitleTemplate != null)
                {
                    innerTable.NewHeaderRow().Add(TitleTemplate
                        .SetColSpan(_columns.Count)
                        .SetProperty(HtmlControl.VisibilityProperty, TitleVisibility));
                }

                if (HeaderVisibility != Visibility.None)
                {
                    var rows = CreateRowSchema<TableHeaderCell>((col) => col.HeaderTemplate);
                    TableHeaderCell firstCol = null;
                    foreach (var rowSchema in rows)
                    {
                        var tableRow = innerTable.NewHeaderRow();
                        tableRow.Children.AddRange(rowSchema);
                        if (tableRow.Count > 0)
                            firstCol = tableRow[0];
                    }
                    if (firstCol != null)
                        firstCol.AddClass("first");
                }

                //criar o esquema das linhas e colunas da tabela
                var itemRowSchema = CreateRowSchema((col) => col.ItemTemplate);

                //por fim fazer um loop no datasource, considerando a paginação e renderizando as linhas
                var firstRow = true;
                foreach (var item in dataSource)
                {
                    foreach (var rowSchema in itemRowSchema)
                    {
                        var tableRow = innerTable.NewRow();
                        //renderiazar a linha.
                        if (firstRow)
                        {
                            tableRow.AddClass("first");
                            firstRow = false;
                        }
                        tableRow.DataContext = item;
                        tableRow.Children.AddRange(rowSchema);
                    }
                }

                if (FooterVisibility != Visibility.None && _hasFooterCells)
                {
                    var hrow = new TableHeaderRow();
                    TableHeaderCell first = null;
                    foreach (var column in _columns)
                    {
                        if (column.ColSpanBehavior == ColSpanBehavior.NoSpan)
                        {
                            hrow.Add(column.FooterTemplate);
                            first = first ?? column.FooterTemplate;
                        }
                    }
                    if (first != null)
                        first.AddClass("first");
                    innerTable.FooterRows.Add(hrow);
                }
            }
            else if (EmptySourceTemplate != null)
            {

                innerTable.NewCell()
                    .SetColSpan(Columns.Count)
                    .Add(EmptySourceTemplate);
            }

            innerTable.Render(CurrentContext);
            base.RenderContent("", rawInnerText);
        }

        private class RowSchema<TCellType> : List<List<TCellType>> where TCellType : TableCellBase<TCellType>
        {

        }

        private RowSchema<TCellType> CreateRowSchema<TCellType>(Func<DataTableColumn, TCellType> cellTemplateSelector)
            where TCellType : TableCellBase<TCellType>
        {
            var schema = new RowSchema<TCellType>();

            var newRow = true;
            List<List<DataTableColumn>> rows = new List<List<DataTableColumn>>();
            List<DataTableColumn> row = null;

            foreach (var column in _columns)
            {
                column.ItemTemplate.RowSpan = 0;
                column.ItemTemplate.ColSpan = 0;
                column.HeaderTemplate.RowSpan = 0;
                column.HeaderTemplate.ColSpan = 0;
                if (newRow)
                {
                    newRow = false;
                    row = new List<DataTableColumn>();
                    rows.Add(row);
                }
                if (column.ColSpanBehavior == ColSpanBehavior.Row)
                {
                    foreach (var col in row)
                    {
                        col.HeaderTemplate.RowSpan = Math.Max(2, col.HeaderTemplate.RowSpan + 1);
                        col.ItemTemplate.RowSpan = Math.Max(2, col.ItemTemplate.RowSpan + 1);
                    }
                    var colSpan = _columns.Count - row.Count - 1;
                    column.HeaderTemplate.ColSpan = colSpan;
                    column.ItemTemplate.ColSpan = colSpan;
                    newRow = true;
                }
                row.Add(column);
            }

            foreach (var rowItems in rows)
            {
                var tableRow = new List<TCellType>();
                schema.Add(tableRow);
                foreach (var col in rowItems)
                {
                    var cell = cellTemplateSelector(col);
                    tableRow.Add(cell);
                }
            }

            return schema;
        }

        protected override void PreRender()
        {
            base.PreRender();
            _hasFooterCells = false;
            var defaultHeaderCellClass = DefaultHeaderCellClass;
            // configurar as colunas e os templates
            foreach (var column in _columns)
            {
                //tem um template para o header?
                if (column.HeaderTemplate == null)
                    column.HeaderTemplate = new TableHeaderCell();

                var header = column.HeaderTemplate;
                if (column.Caption.HasValue() && header.Text.IsEmpty())
                    header.Text = column.Caption;
                if (column.HeaderCssClass.HasValue())
                    header.AddClass(column.HeaderCssClass);
                if (defaultHeaderCellClass.HasValue())
                    header.AddClass(defaultHeaderCellClass);

                //tem um template para a celula de dados?
                if (column.ItemTemplate == null)
                    column.ItemTemplate = new TableCell();

                if (column.CssClass.HasValue())
                    column.ItemTemplate.AddClass(column.CssClass);
                if (column.HorizontalAlign.HasValue)
                    column.ItemTemplate.SetHorizontalAlign(column.HorizontalAlign.Value);

                if (column.NoWrap.HasValue)
                    column.ItemTemplate.NoWrap = column.NoWrap.Value;


                //se não tiver verificar se o suário informou um nome de propriedade
                if (!String.IsNullOrEmpty(column.PropertyName))
                    column.ItemTemplate.SetBinding(HtmlControl.DataContextProperty, column.PropertyName);

                //tem um template para o footer?
                if (column.FooterTemplate == null)
                    column.FooterTemplate = new TableHeaderCell();
                else
                    _hasFooterCells = true;

            }
        }

        protected override void PostRender()
        {
            var headerCellClass = DefaultHeaderCellClass;
            if (headerCellClass != null)
            {
                foreach (var column in _columns)
                {
                    if (column.HeaderTemplate != null)
                        column.HeaderTemplate.RemoveClass(headerCellClass);
                }
            }
            base.PostRender();
        }
    }
}
