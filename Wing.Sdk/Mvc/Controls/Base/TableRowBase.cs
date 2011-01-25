using System;

namespace Wing.Mvc.Controls.Base
{
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class TableRowBase<TConcreteType, TCellType> : ContainerControl<TConcreteType, TCellType>
        where TConcreteType : TableRowBase<TConcreteType, TCellType>
        where TCellType : TableCellBase<TCellType>, new()
    {
        public TableRowBase(HtmlTag tag) : base(tag) { }

        public TCellType NewCell()
        {
            var cell = new TCellType();
            Add(cell);
            return cell;
        }

        public TCellType NewCell(params HtmlObject[] innerControls)
        {
            var cell = NewCell();
            foreach (var control in innerControls)
                cell.Add(control);
            return cell;
        }

        public TCellType NewCell(String text)
        {
            return NewCell().SetText(text);
        }

        public TConcreteType AddCell(TypedControlDelegate<TCellType> setDelegate)
        {
            return AddNew<TCellType>(setDelegate);
        }

        public TConcreteType AddCell(String text)
        {
            return AddCell(c => c.SetText(text));
        }

        public TConcreteType AddCell(params HtmlObject[] innerControls)
        {
            NewCell(innerControls);
            return This;
        }
    }

}
