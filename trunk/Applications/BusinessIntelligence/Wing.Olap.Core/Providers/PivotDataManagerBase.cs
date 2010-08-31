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
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core;
using Ranet.Olap.Core.Providers;
using Ranet.Olap.Core.Providers.MemberActions;
using Ranet.Olap.Mdx.Compiler;
using Ranet.Olap.Mdx;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.Olap.Core.Providers
{
    public abstract class PivotDataManagerBase
    {
        public string Query { get; private set; }
        public readonly String UpdateScript = String.Empty;

        ConnectionInfo m_Connection = null;
        public ConnectionInfo Connection
        {
            get
            {
                if (m_Connection == null)
                    m_Connection = new ConnectionInfo();
                return m_Connection;
            }
        }

        public PivotDataManagerBase(ConnectionInfo connection, String query, String updateScript)
        {
            m_Connection = connection;
            Query = query;
            UpdateScript = updateScript;
            m_History = new HistoryManager();
            m_History.AddHistoryItem(new HistoryItem());
        }

        protected abstract IQueryExecuter CreateQueryExecutor(ConnectionInfo connection);

        private IQueryExecuter m_QueryExecutor;
        protected IQueryExecuter Executor
        {
            get
            {
                if (m_QueryExecutor == null)
                {
                    m_QueryExecutor = this.CreateQueryExecutor(this.Connection);
                    if (m_QueryExecutor == null)
                    {
                        throw new Exception("Unable to create query executor.");
                    }
                }

                return m_QueryExecutor;
            }
        }

        public void ChangeQuery(String query)
        {
            Query = query;
            m_History = new HistoryManager();
            m_History.AddHistoryItem(new HistoryItem());

            m_OriginalStatement = null;
        }

        HistoryManager m_History = null;
        /// <summary>
        /// Менеджер истории
        /// </summary>
        public HistoryManager History
        {
            get
            {
                return m_History;
            }
        }

        public Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject { get; set; }

        public virtual DataSourceInfoArgs GetDataSourceInfo(ServiceCommandArgs args)
        {
            DataSourceInfoArgs res = new DataSourceInfoArgs();

            bool queryIsOK = true;
            try
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    StringBuilder sb = new StringBuilder();
                    provider.GenerateMdxFromDom(this.CreateWrappedStatement(), sb, new MdxGeneratorOptions());

                    String new_Query = sb.ToString();

                    res.MDXQuery = new_Query;
                    res.MovedAxes_MDXQuery = MoveAxes(new_Query);
                }
            }
            catch (Exception ex)
            {
                res.MDXQuery = ex.ToString();
                queryIsOK = false;
            }

            res.ConnectionString = Executor.Connection.ConnectionID;
            res.UpdateScript = UpdateScript;

            return res;
        }

        private String MoveAxes(String originalQuery)
        {
            string result = String.Empty;
            if (!String.IsNullOrEmpty(originalQuery))
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    MdxSelectStatement statement = provider.ParseMdx(originalQuery) as MdxSelectStatement;
                    if (statement != null)
                    {
                        if (statement.Where == null)
                            statement.Where = new MdxWhereClause();

                        if (statement.Axes != null && statement.Axes.Count > 2)
                        {
                            IList<MdxExpression> expr = new List<MdxExpression>();
                            if (statement.Where.Expression != null)
                            {
                                expr.Add(statement.Where.Expression);
                            }

                            for (int i = 2; i < statement.Axes.Count; i++)
                            {
                                expr.Add(statement.Axes[i].Expression);
                            }

                            statement.Where.Expression = new MdxTupleExpression(expr);
                            while (statement.Axes.Count > 2)
                                statement.Axes.RemoveAt(2);
                        }

                        StringBuilder sb = new StringBuilder();
                        provider.GenerateMdxFromDom(statement, sb, new MdxGeneratorOptions());
                        result = sb.ToString();
                    }
                }
            }
            return result;
        }

        public CellSetData PerformMemberAction(PerformMemberActionArgs args)
        {
            if (args != null)
            {
                switch (args.Action)
                {
                    case MemberActionType.Expand:
                    case MemberActionType.Collapse:
                    case MemberActionType.DrillDown:
                    case MemberActionType.DrillUp:
                        // Удаляем все элементы истории, стоящие за текущим
                        History.CutRight();
                        HistoryItem clone = null;
                        if (History.CurrentHistoryItem != null)
                        {
                            // Клонируем текущий элемент истории чтобы действие добавлялось уже в клон
                            clone = (HistoryItem)(History.CurrentHistoryItem.Clone());
                        }
                        else
                        {
                            // Добавляем пустой элемент истории
                            HistoryItem first = new HistoryItem();
                            History.AddHistoryItem(first);

                            clone = new HistoryItem();
                        }

                        History.AddHistoryItem(clone);
                        break;
                }

                switch (args.Action)
                {
                    case MemberActionType.Expand:
                        ExpandMember(args);
                        break;
                    case MemberActionType.Collapse:
                        CollapseMember(args);
                        break;
                    case MemberActionType.DrillDown:
                        DrillDownMember(args);
                        break;
                    case MemberActionType.DrillUp:
                        DrillUpMember(args);
                        break;
                }
            }
            return RefreshQuery();
        }

        private void ExpandMember(PerformMemberActionArgs args)
        {
            //using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = null;
                if (!RotateAxes)
                {
                    actions = args.AxisIndex == 0 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                else
                {
                    actions = args.AxisIndex == 1 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                DrillActionContainer previous = null;
                DrillActionContainer container = null;

                // Родители
                if (args.Ascendants != null)
                {
                    for (int i = args.Ascendants.Count - 1; i >= 0; i--)
                    {
                        ShortMemberInfo mi = args.Ascendants[i];

                        container = History.CurrentHistoryItem.FindDrillActionContainer(actions, mi.UniqueName);
                        if (container == null)
                        {
                            container = new DrillActionContainer(mi.UniqueName, mi.HierarchyUniqueName);
                            if (previous == null)
                                actions.Add(container);
                            else
                                previous.Children.Add(container);
                        }
                        previous = container;
                    }
                }

                MdxDrillDownAction skip_container = null;
                // Сам элемент
                container = History.CurrentHistoryItem.FindDrillActionContainer(actions, args.Member.UniqueName);
                if (container == null)
                {
                    container = new DrillActionContainer(args.Member.UniqueName, args.Member.HierarchyUniqueName);
                    container.Action = new MdxExpandAction(args.Member.UniqueName);
                    if (previous == null)
                        actions.Add(container);
                    else
                        previous.Children.Add(container);
                }
                else
                {
                    // Если по данному элементу уже дедался дриллдаун - то дриллдаун остается в силе. А раскрытие - прокидываем 
                    skip_container = container.Action as MdxDrillDownAction;
                    if (skip_container == null)
                        container.Action = new MdxExpandAction(args.Member.UniqueName);
                }

                // Вложенные действия над элементами из этой же иерархии
                IList<DrillActionContainer> actionsToHierarchy = History.CurrentHistoryItem.FindDrillActionContainersByHierarchy(actions, args.Member.HierarchyUniqueName);
                if (actionsToHierarchy != null)
                {
                    // Все DrillUp и DrillDown операции зануляем если уровень у них глубже
                    foreach (DrillActionContainer action in actionsToHierarchy)
                    {
                        MdxDrillDownAction drillDownAction = action.Action as MdxDrillDownAction;
                        if (drillDownAction != null && drillDownAction.LevelDepth >= args.Member.LevelDepth)
                        {
                            if (skip_container != null && skip_container == drillDownAction)
                                continue;

                            action.Action = null;
                        }
                        MdxDrillUpAction drillUpAction = action.Action as MdxDrillUpAction;
                        if (drillUpAction != null && drillUpAction.LevelDepth >= args.Member.LevelDepth)
                        {
                            action.Action = null;
                        }
                    }
                }
            }
        }

        void CollapseMember(PerformMemberActionArgs args)
        {
            //using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = null;
                if (!RotateAxes)
                {
                    actions = args.AxisIndex == 0 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                else
                {
                    actions = args.AxisIndex == 1 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                DrillActionContainer previous = null;
                DrillActionContainer container = null;

                // Родители
                if (args.Ascendants != null)
                {
                    for (int i = args.Ascendants.Count - 1; i >= 0; i--)
                    {
                        ShortMemberInfo mi = args.Ascendants[i];

                        container = History.CurrentHistoryItem.FindDrillActionContainer(actions, mi.UniqueName);
                        if (container == null)
                        {
                            container = new DrillActionContainer(mi.UniqueName, mi.HierarchyUniqueName);
                            if (previous == null)
                                actions.Add(container);
                            else
                                previous.Children.Add(container);
                        }
                        previous = container;
                    }
                }

                // Сам элемент
                container = History.CurrentHistoryItem.FindDrillActionContainer(actions, args.Member.UniqueName);
                if (container == null)
                {
                    container = new DrillActionContainer(args.Member.UniqueName, args.Member.HierarchyUniqueName);
                    container.Action = new MdxCollapseAction(args.Member.UniqueName);
                    if (previous == null)
                        actions.Add(container);
                    else
                        previous.Children.Add(container);
                }
                else
                {
                    // Удалем все дочерние - тем самым как бы схлопываем элемент
                    // Удаляем все вложенные экшены для данной иерархии
                    IList<DrillActionContainer> actionsToHierarchy = History.CurrentHistoryItem.FindDrillActionContainersByHierarchy(container.Children, args.Member.HierarchyUniqueName);
                    if (actionsToHierarchy != null)
                    {
                        // Все действия зануляем
                        foreach (DrillActionContainer action in actionsToHierarchy)
                        {
                            action.Action = null;
                        }
                    }
                    container.Action = new MdxCollapseAction(args.Member.UniqueName);
                }
            }
        }

        void DrillDownMember(PerformMemberActionArgs args)
        {
            //using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = null;
                if (!RotateAxes)
                {
                    actions = args.AxisIndex == 0 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                else
                {
                    actions = args.AxisIndex == 1 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                DrillActionContainer previous = null;
                DrillActionContainer container = null;

                /*// Действия над элементами из этой же иерархии
                IList<DrillActionContainer> actionsToHierarchy = this.FindDrillActionContainersByHierarchy(actions, args.Member.HierarchyUniqueName);
                if (actionsToHierarchy != null)
                { 
                    // Все действия зануляем
                    foreach (DrillActionContainer action in actionsToHierarchy)
                    {
                        action.Action = null;
                    }
                }*/

                // Родители
                if (args.Ascendants != null)
                {
                    for (int i = args.Ascendants.Count - 1; i >= 0; i--)
                    {
                        ShortMemberInfo mi = args.Ascendants[i];
                        container = History.CurrentHistoryItem.FindDrillActionContainer(actions, mi.UniqueName);
                        if (container == null)
                        {
                            container = new DrillActionContainer(mi.UniqueName, mi.HierarchyUniqueName);
                            if (previous == null)
                                actions.Add(container);
                            else
                                previous.Children.Add(container);
                        }
                        previous = container;
                    }
                }

                // Сам элемент
                container = History.CurrentHistoryItem.FindDrillActionContainer(actions, args.Member.UniqueName);
                if (container == null)
                {
                    container = new DrillActionContainer(args.Member.UniqueName, args.Member.HierarchyUniqueName);
                    container.Action = new MdxDrillDownAction(args.Member.UniqueName, args.Member.HierarchyUniqueName, args.Member.LevelDepth);
                    if (previous == null)
                        actions.Add(container);
                    else
                        previous.Children.Add(container);
                }
                else
                {
                    container.Action = new MdxDrillDownAction(args.Member.UniqueName, args.Member.HierarchyUniqueName, args.Member.LevelDepth);
                }
            }
            /*using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = args.AxisIndex == 0 ? ColumnsActionChain : RowsActionChain;

                actions.Clear();
                
                DrillActionContainer container = new DrillActionContainer(args.Member.UniqueName);
                container.Action = new MdxDrillDownAction(args.Member.UniqueName);

                actions.Add(container);

                this.RefreshQuery();
            }*/
        }

        void DrillUpMember(PerformMemberActionArgs args)
        {
            //using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = null;
                if (!RotateAxes)
                {
                    actions = args.AxisIndex == 0 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                else
                {
                    actions = args.AxisIndex == 1 ? History.CurrentHistoryItem.ColumnsActionChain : History.CurrentHistoryItem.RowsActionChain;
                }
                DrillActionContainer previous = null;
                DrillActionContainer container = null;

                // Действия над элементами из этой же иерархии
                IList<DrillActionContainer> actionsToHierarchy = History.CurrentHistoryItem.FindDrillActionContainersByHierarchy(actions, args.Member.HierarchyUniqueName);
                if (actionsToHierarchy != null)
                {
                    // Все действия зануляем
                    foreach (DrillActionContainer action in actionsToHierarchy)
                    {
                        action.Action = null;
                    }
                }

                // Родители
                if (args.Ascendants != null)
                {
                    for (int i = args.Ascendants.Count - 1; i >= 0; i--)
                    {
                        ShortMemberInfo mi = args.Ascendants[i];

                        container = History.CurrentHistoryItem.FindDrillActionContainer(actions, mi.UniqueName);
                        if (container == null)
                        {
                            container = new DrillActionContainer(mi.UniqueName, mi.HierarchyUniqueName);
                            if (previous == null)
                                actions.Add(container);
                            else
                                previous.Children.Add(container);
                        }
                        previous = container;
                    }
                }

                // Сам элемент
                container = History.CurrentHistoryItem.FindDrillActionContainer(actions, args.Member.UniqueName);
                if (container == null)
                {
                    container = new DrillActionContainer(args.Member.UniqueName, args.Member.HierarchyUniqueName);
                    container.Action = new MdxDrillUpAction(args.Member.UniqueName, args.Member.HierarchyUniqueName, args.Member.LevelDepth);
                    if (previous == null)
                        actions.Add(container);
                    else
                        previous.Children.Add(container);
                }
                else
                {
                    // Удаляем все вложенные экшены для данной иерархии
                    IList<DrillActionContainer> child_actionsToHierarchy = History.CurrentHistoryItem.FindDrillActionContainersByHierarchy(container.Children, args.Member.HierarchyUniqueName);
                    if (child_actionsToHierarchy != null)
                    {
                        // Все действия зануляем
                        foreach (DrillActionContainer action in child_actionsToHierarchy)
                        {
                            action.Action = null;
                        }
                    }

                    container.Action = new MdxDrillUpAction(args.Member.UniqueName, args.Member.HierarchyUniqueName, args.Member.LevelDepth);
                }
            }
            /*using (WaitCursor wc = new WaitCursor())
            {
                IList<DrillActionContainer> actions = args.AxisIndex == 0 ? ColumnsActionChain : RowsActionChain;

                actions.Clear();

                DrillActionContainer container = new DrillActionContainer(args.Member.UniqueName);
                container.Action = new MdxDrillUpAction(args.Member.UniqueName);

                actions.Add(container);

                this.RefreshQuery();
            }*/
        }

        public bool HideEmptyRows { get; set; }
        public bool HideEmptyColumns { get; set; }
        public bool RotateAxes { get; set; }

        public virtual String ExportToExcel()
        {
            return String.Empty;
        }

        public CellSetData RefreshQuery()
        {
            return this.RefreshQuery(null);
        }

        public CellSetData RefreshQuery(Func<MdxObject, MdxObject> objectConsumerPattern)
        {
            CellSetData res = null;
            if (!string.IsNullOrEmpty(Query))
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    StringBuilder sb = new StringBuilder();
                    provider.GenerateMdxFromDom(this.CreateWrappedStatement(), sb, new MdxGeneratorOptions());

                    String new_Query = sb.ToString();

                    res = ExecuteQuery(new_Query);
                    //if (!String.IsNullOrEmpty(res))
                    //{
                    //    Application[QUERY] = new_Query;
                    //}
                }
            }
            else
            { 
                // Пустой запрос
                res = new CellSetData();
            }
            return res;
        }

        public String WrappedQuery
        {
            get
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    StringBuilder sb = new StringBuilder();
                    provider.GenerateMdxFromDom(this.CreateWrappedStatement(), sb, new MdxGeneratorOptions());

                    return sb.ToString();
                }
            }
        }

        public virtual int ExecuteNonQuery(string query)
        {
            if (Executor != null)
                return Executor.ExecuteNonQuery(query);
            return -1;
        }

        public virtual CellSetData ExecuteQuery(string query)
        {
            if (Executor != null)
                return Executor.ExecuteQuery(query);
            return null;
        }

        private MdxSelectStatement m_OriginalStatement = null;

        private MdxSelectStatement CreateWrappedStatement()
        {
            if (!string.IsNullOrEmpty(Query) && m_OriginalStatement == null)
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    m_OriginalStatement = provider.ParseMdx(this.Query) as MdxSelectStatement;
                }
            }

            MdxSelectStatement select = null;
            try
            {
                if (m_OriginalStatement != null)
                {
                    select = m_OriginalStatement.Clone() as MdxSelectStatement; // (MdxSelectStatement)Serializer.Deserialize(Serializer.Serialize(m_OriginalStatement));
                    if (select != null)
                    {
                        if (History.CurrentHistoryItem != null)
                        {
                            if (select.Axes.Count > 0)
                            {
                                select.Axes[0] = GetWrappedAxis(select.Axes[0], History.CurrentHistoryItem.ColumnsActionChain);
                                if (select.Axes.Count > 1)
                                {
                                    select.Axes[1] = GetWrappedAxis(select.Axes[1], History.CurrentHistoryItem.RowsActionChain);
                                }

                                // Переворот осей
                                if (RotateAxes)
                                {
                                    if (select.Axes.Count > 1)
                                    {
                                        MdxExpression axis0_expression = select.Axes[0].Expression;
                                        select.Axes[0].Expression = select.Axes[1].Expression;
                                        select.Axes[1].Expression = axis0_expression;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return select;
        }

        private MdxAxis GetWrappedAxis(MdxAxis ax, IList<DrillActionContainer> actions)
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

            if (actions.Count > 0)
            {
                var expression = ax.Expression;
                var hierarchize = expression as MdxFunctionExpression;
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


            // Возможность убрать пустые колонки
            /*
            if (nonEmpty == null && actions == History.CurrentHistoryItem.ColumnsActionChain && HideEmptyColumns)
            {
                axisExpression = new MdxNonEmptyExpression(axisExpression);
            }
            */
            // Возможность убрать пустые строки
            /*
            if (nonEmpty == null && actions == History.CurrentHistoryItem.RowsActionChain && HideEmptyRows)
            {
                axisExpression = new MdxNonEmptyExpression(axisExpression);
            }
            */
            if (History.CurrentHistoryItem.ColumnsActionChain.Equals(actions) && HideEmptyColumns)
            {
                ax.NonEmpty = true;
            }
            if (History.CurrentHistoryItem.RowsActionChain.Equals(actions) && HideEmptyRows)
            {
                ax.NonEmpty = true;
            }

            return new MdxAxis(
                    ax.Name,
                    axisExpression,
                    ax.Having,
                    ax.DimensionProperties
                    )
                    {
                        NonEmpty = ax.NonEmpty
                    };
        }

        private MdxExpression GetWrappedExpression(MdxExpression expr, IList<DrillActionContainer> actions)
        {
            if (actions == null)
                return expr;

            //foreach (DrillActionContainer container in actions)
            //{
            //    if (container.Action != null)
            //    {
            //        if (container.Action is MdxActionBase)
            //        {
            //            if (passedHierarchies.Contains(container.HierarchyUniqueName))
            //            {
            //                ((MdxActionBase)container.Action).ConcretizeMdxObject = null;
            //            }
            //            else
            //            {
            //                ((MdxActionBase)container.Action).ConcretizeMdxObject = this.ConcretizeMdxObject;
            //            }
            //        }

            //        MdxExpression newExpr = container.Action.Process(expr, new MdxActionContext(container.HierarchyUniqueName, container.MemberUniqueName)) as MdxExpression;
            //        if (!passedHierarchies.Contains(container.HierarchyUniqueName))
            //        {
            //            passedHierarchies.Add(container.HierarchyUniqueName);
            //        }
            //        if (newExpr != null)
            //            expr = newExpr;
            //    }

            //    if (container.Children != null && container.Children.Count > 0)
            //        expr = GetWrappedExpression(expr, container.Children);
            //}

            var processor = new DrillActionProcessor(actions, this.ConcretizeMdxObject);
            expr = processor.Process(expr);
            return expr;
        }

    }
}
