/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Olap.Mdx;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Providers.MemberActions
{
    public class HistoryItem4MdxQuery : IHistoryItem<HistoryItem4MdxQuery>
    {
        internal readonly AxisInfo ColumnsActionChain;
        internal readonly AxisInfo RowsActionChain;
        public bool RotateAxes { get; set; }

        //public Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject = null;

        public HistoryItem4MdxQuery()
        {
            ColumnsActionChain = new AxisInfo();
            RowsActionChain = new AxisInfo();
        }
        HistoryItem4MdxQuery(HistoryItem4MdxQuery old)
        {
            ColumnsActionChain = old.ColumnsActionChain.Clone();
            RowsActionChain = old.RowsActionChain.Clone();
            RotateAxes = old.RotateAxes;
            //ConcretizeMdxObject=old.ConcretizeMdxObject;
        }
        public HistoryItem4MdxQuery Clone()
        {
            return new HistoryItem4MdxQuery(this);
        }
        public void AddMemberAction(int axisIndex, MemberAction Action)
        {
            AxisInfo ai = (axisIndex == 0) ^ RotateAxes
             ? ColumnsActionChain
             : RowsActionChain;

            ai.AddMemberAction(Action);
        }
        internal void SortByValue(int axisIndex, PerformMemberActionArgs args)
        {
            AxisInfo ai = (axisIndex == 0) ^ RotateAxes
             ? RowsActionChain
             : ColumnsActionChain;

            ai.SortByValue(args);
        }
        public MdxSelectStatement CreateWrappedStatement(MdxSelectStatement original)
        {
            if (original == null)
                return null;

            try
            {
                MdxSelectStatement select = (MdxSelectStatement)original.Clone();

                if (select.Axes.Count <= 0)
                    return select;

                var axis0_actions = ColumnsActionChain;
                if (select.Axes[0].Name.ToLower() == "1" || select.Axes[0].Name.ToLower() == "rows")
                    axis0_actions = RowsActionChain;

                select.Axes[0] = axis0_actions.GetWrappedAxis(select.Axes[0]/*, this.ConcretizeMdxObject*/);

                if (select.Axes.Count <= 1)
                    return select;

                var axis1_actions = RowsActionChain;
                if (select.Axes[1].Name.ToLower() == "0" || select.Axes[1].Name.ToLower() == "columns")
                    axis1_actions = ColumnsActionChain;

                select.Axes[1] = axis1_actions.GetWrappedAxis(select.Axes[1]/*, this.ConcretizeMdxObject*/);

                // Переворот осей
                if (RotateAxes)
                {
                    if (select.Axes.Count > 1)
                    {
                        var axis0 = select.Axes[0];
                        select.Axes[0] = select.Axes[1];
                        select.Axes[0].Name = "0";
                        select.Axes[1] = axis0;
                        axis0.Name = "1";
                    }
                }
                return select;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
