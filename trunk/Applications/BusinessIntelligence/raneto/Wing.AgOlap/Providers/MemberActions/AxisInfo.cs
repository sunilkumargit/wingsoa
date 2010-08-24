/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Olap.Mdx;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.PivotGrid;

namespace Ranet.AgOlap.Providers.MemberActions
{
    internal class AxisInfo
    {
        readonly List<MemberAction> Actions = new List<MemberAction>();
        public bool HideEmpty { get; set; }
        public SortDescriptor MeasuresSort;

        public AxisInfo()
        {
            HideEmpty = false;
            MeasuresSort = null;
        }

        AxisInfo(AxisInfo old)
        {
            HideEmpty = old.HideEmpty;
            if (old.MeasuresSort != null)
                MeasuresSort = old.MeasuresSort.Clone();

            foreach (var cont in old.Actions)
            {
                Actions.Add(cont.Clone());
            }
        }
        //internal static string GetSortExpr(PerformMemberActionArgs args)
        //{
        //  List<string> sortExpr = new List<string>();

        //  string lasthier = args.Member.HierarchyUniqueName;
        //  for (int i = 0; i < args.Ascendants.Count; i++)
        //  {
        //    var member = args.Ascendants[i];

        //    if (lasthier != member.HierarchyUniqueName)
        //    {
        //      lasthier = member.HierarchyUniqueName;
        //      sortExpr.Insert(0, member.UniqueName);
        //    }
        //  }
        //  sortExpr.Add(args.Member.UniqueName);
        //  return "("+string.Join(",",sortExpr.ToArray())+")";
        //}
        internal void SortByValue(PerformMemberActionArgs args)
        {
            var tuple = args.Member.GetAxisTuple();
            var measuresSort = MeasuresSort as SortByValueDescriptor;
            if (measuresSort == null)
            {
                measuresSort = new SortByValueDescriptor();
                measuresSort.Tuple = tuple;
                MeasuresSort = measuresSort;

            }
            else if (!measuresSort.CompareByTuple(tuple))
            {
                measuresSort.Tuple = tuple;
                measuresSort.Type = SortTypes.None;
            }

            if (MeasuresSort.Type == SortTypes.None)
                MeasuresSort.Type = SortTypes.Ascending;
            else if (MeasuresSort.Type == SortTypes.Ascending)
                MeasuresSort.Type = SortTypes.Descending;
            else if (MeasuresSort.Type == SortTypes.Descending)
                MeasuresSort.Type = SortTypes.None;

        }
        public void AddMemberAction(MemberAction Action)
        {
            if (Actions.Count > 0)
            {
                var lastAction = Actions[Actions.Count - 1];
                if (lastAction.TuplesAreEqual(Action)) // THE SAME member сell
                {
                    // Два DrillDown подряд делать смысла нет (просто оставляем старый)
                    if ((lastAction is MemberActionDrillDown) && (Action is MemberActionDrillDown))
                        return;

                    //  Expand после Collapse просто отменяет Collapse
                    //  Collapse после Expand просто отменяет Expand
                    if (((lastAction is MemberActionExpand) && (Action is MemberActionCollapse))
                            || ((lastAction is MemberActionCollapse) && (Action is MemberActionExpand))
                            )
                    {
                        Actions.RemoveAt(Actions.Count - 1);
                        return;
                    }
                }
            }
            Actions.Add(Action);
        }
        public AxisInfo Clone()
        {
            return new AxisInfo(this);
        }
        internal MdxAxis GetWrappedAxis(MdxAxis ax /*, Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject */)
        {
            MdxExpression axisExpression = ax.Expression;

            // 1. Если первым на оси стоит ключевое слово NON EMPTY то выражение формируем из его аргументов, а уже потом все это выражение обрамляем ключевым словом NON EMPTY 
            // 2. Если запрос на оси обрамлен функцией HIERARCHIZE, то выражение формируем из ее аргумента, а уже потом все это выражение обрамляем функцией HIERARCHIZE
            /* 
             * Флаг NonEmpty перенесен в MdxAxis. Проверок не требуется.
             * 
            MdxNonEmptyExpression nonEmpty = ax.Expression as MdxNonEmptyExpression;
            if (actions != null && actions.Count > 0)
            {
                    if (nonEmpty != null)
                    {
                            MdxExpression expression = nonEmpty.Expression;
                            MdxFunctionExpression hierarchize = expression as MdxFunctionExpression;
                            if (hierarchize != null && hierarchize.Name.ToLower() == "hierarchize" && hierarchize.Arguments.Count == 1)
                            {
                                    expression = hierarchize.Arguments[0];
                            }

                            axisExpression = new MdxNonEmptyExpression(
                                    new MdxFunctionExpression("HIERARCHIZE",
                                            new MdxExpression[]
                                    {
                                            GetWrappedExpression(expression, actions)
                                    }));
                    }
                    else
                    {
                            MdxExpression expression = ax.Expression;
                            MdxFunctionExpression hierarchize = expression as MdxFunctionExpression;
                            if (hierarchize != null && hierarchize.Name.ToLower() == "hierarchize" && hierarchize.Arguments.Count == 1)
                            {
                                    expression = hierarchize.Arguments[0];
                            }

                            axisExpression = new MdxFunctionExpression(
                                                     "HIERARCHIZE",
                                                     new MdxExpression[]
                                    {
                                            GetWrappedExpression(expression, actions)
                                    });
                    }
            }
            */

            if (Actions.Count > 0)
            {
                var expression = ax.Expression;
                axisExpression = this.GetWrappedExpression(expression/*, ConcretizeMdxObject*/);
            }
            axisExpression = SortExpression(axisExpression, MeasuresSort);

            return new MdxAxis(
                            ax.Name,
                            axisExpression,
                            ax.Having,
                            ax.DimensionProperties
                            )
            {
                // Возможность убрать пустые колонки
                NonEmpty = HideEmpty
            };
        }

        MdxExpression SortExpression(MdxExpression expr, SortDescriptor descr)
        {
            if (descr == null)
                return expr;

            if (!String.IsNullOrEmpty(descr.SortBy))
            {
                String orderType = String.Empty;
                if (descr.Type == SortTypes.Ascending)
                    orderType = "BASC";
                if (descr.Type == SortTypes.Descending)
                    orderType = "BDESC";

                if (!String.IsNullOrEmpty(orderType))
                {
                    expr = new MdxFunctionExpression("ORDER",
                    new MdxExpression[] {
                                expr,
                                new MdxObjectReferenceExpression(String.Format("({0})", descr.SortBy)),
                                new MdxConstantExpression(orderType) 
                            });
                }
            }
            return expr;
        }
        //static MemberActionDrillDown GetMaxDrillAction(MemberActionDrillDown maxDrillDownAction, MemberActionDrillDown Action)
        //{
        //  if (Action == null)
        //    return maxDrillDownAction;
        //  if (maxDrillDownAction == null)
        //    return Action;
        //  return maxDrillDownAction.ReturnMostRestrictive(Action);
        //}

        //private MdxExpression GetWrappedExpression2(MdxExpression expr/*, Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject*/)
        //{
        //  MemberActionDrillDown maxDrillDownAction = null;

        //  foreach (var Action in this.Actions)
        //    maxDrillDownAction = GetMaxDrillAction(maxDrillDownAction, Action as MemberActionDrillDown);

        //  if (maxDrillDownAction != null)
        //    expr = maxDrillDownAction.Process(expr);

        //  foreach (var Action in this.Actions)
        //  {
        //    if (Action is MemberActionDrillDown)
        //      continue;

        //    if (maxDrillDownAction != null)
        //      if (maxDrillDownAction.OutOfScope(Action))
        //        continue;

        //    expr = Action.Process(expr);
        //  }
        //  return expr;
        //}
        private MdxExpression GetWrappedExpression(MdxExpression expr)
        {
            foreach (var Action in this.Actions)
            {
                expr = Action.Process(expr);
            }
            return expr;
        }
    }
}
