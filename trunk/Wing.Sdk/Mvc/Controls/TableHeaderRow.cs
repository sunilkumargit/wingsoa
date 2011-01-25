using Wing.Mvc.Controls.Base;

namespace Wing.Mvc.Controls
{
    public class TableHeaderRow : TableRowBase<TableHeaderRow, TableHeaderCell>
    {
        [System.Diagnostics.DebuggerStepThrough]
        public TableHeaderRow() : base(HtmlTag.Tr) { }
    }
}
