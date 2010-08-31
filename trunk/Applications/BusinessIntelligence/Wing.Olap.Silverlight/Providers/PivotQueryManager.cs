using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Olap.Core;
using Wing.Olap.Providers.MemberActions;
using Wing.Olap.Mdx.Compiler;
using System.Text;
using Wing.Olap.Mdx;
using System.Collections.Generic;
using Wing.Olap.Core.Providers.ClientServer;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;
using Wing.Olap.Controls;

namespace Wing.Olap.Providers
{
    public enum ServiceCommandType
    {
        None,
        Refresh,
        ToBegin,
        Forward,
        Back,
        ToEnd,
        HideEmptyRows,
        HideEmptyColumns,
        ShowEmptyRows,
        ShowEmptyColumns,
        GetDataSourceInfo,
        ExportToExcel,
        NormalAxes,
        RotateAxes
    }

    public class PivotQueryManager : HistoryManager<HistoryItem4MdxQuery>
    {
        internal DrillDownMode DrillDownMode = DrillDownMode.BySingleDimensionHideSelf;
        string m_Query = string.Empty;

        public string Query
        {
            get { return m_Query; }
            private set
            {
                ClearHistory();
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    m_OriginalStatement = provider.ParseMdx(value) as MdxSelectStatement;
                }
                if (m_OriginalStatement == null)
                    return;

                m_Query = value;
                if (m_OriginalStatement.Axes.Count < 1)
                    return;
                CurrentHistoryItem.ColumnsActionChain.HideEmpty = m_OriginalStatement.Axes[0].NonEmpty;
                if (m_OriginalStatement.Axes.Count < 2)
                    return;
                CurrentHistoryItem.RowsActionChain.HideEmpty = m_OriginalStatement.Axes[1].NonEmpty;
            }
        }
        MdxSelectStatement m_OriginalStatement = null;
        //		public Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject { get; set; }
        public readonly String UpdateScript = String.Empty;

        public SortDescriptor Axis0_MeasuresSort
        {
            get { return CurrentHistoryItem.ColumnsActionChain.MeasuresSort; }
            set
            {
                AddCurrentStateToHistory();
                CurrentHistoryItem.ColumnsActionChain.MeasuresSort = value;
            }
        }
        public SortDescriptor Axis1_MeasuresSort
        {
            get { return CurrentHistoryItem.RowsActionChain.MeasuresSort; }
            set
            {
                AddCurrentStateToHistory();
                CurrentHistoryItem.RowsActionChain.MeasuresSort = value;
            }
        }

        public PivotQueryManager(String query, String updateScript)
        {
            this.UpdateScript = updateScript;
            this.Query = query;
        }

        public void ChangeQuery(String query)
        {
            this.Query = query;
        }
        public virtual DataSourceInfoArgs GetDataSourceInfo(UpdateEntry entry)
        {
            DataSourceInfoArgs res = new DataSourceInfoArgs();

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
            }

            //res.ConnectionString = Connection.ConnectionID;
            res.UpdateScript = UpdateScript;

            return res;
        }

        public PivotGridToolBarInfo GetToolBarInfo()
        {
            PivotGridToolBarInfo toolBarInfo = new PivotGridToolBarInfo();
            toolBarInfo.HistorySize = this.HistorySize;
            toolBarInfo.CurrentHistoryIndex = this.CurrentHistoryItemIndex;
            toolBarInfo.HideEmptyRows = this.CurrentHistoryItem.RowsActionChain.HideEmpty;
            toolBarInfo.HideEmptyColumns = this.CurrentHistoryItem.ColumnsActionChain.HideEmpty;
            toolBarInfo.RotateAxes = this.CurrentHistoryItem.RotateAxes;

            return toolBarInfo;
        }

        static MdxSelectStatement GetMoveAxesStatement(String originalQuery)
        {
            MdxSelectStatement statement = null;
            if (!String.IsNullOrEmpty(originalQuery))
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    statement = provider.ParseMdx(originalQuery) as MdxSelectStatement;
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
                    }
                }
            }
            return statement;
        }

        private String MoveAxes(String originalQuery)
        {
            string result = String.Empty;
            MdxSelectStatement statement = GetMoveAxesStatement(originalQuery);
            if (statement != null)
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    StringBuilder sb = new StringBuilder();
                    provider.GenerateMdxFromDom(statement, sb, new MdxGeneratorOptions());
                    result = sb.ToString();
                }
            }
            return result;
        }

        MemberAction CreateMemberAction(PerformMemberActionArgs args)
        {
            switch (args.Action)
            {
                case MemberActionType.Expand:
                    return new MemberActionExpand(args);
                case MemberActionType.Collapse:
                    return new MemberActionCollapse(args);
                case MemberActionType.DrillDown:
                    return new MemberActionDrillDown(args, DrillDownMode);
                default:
                    return null;
            }

        }
        public String PerformMemberAction(PerformMemberActionArgs args)
        {
            if (args != null)
            {
                var Action = CreateMemberAction(args);
                if (Action != null)
                {
                    AddCurrentStateToHistory();
                    this.CurrentHistoryItem.AddMemberAction(args.AxisIndex, Action);
                }
                else if (args.Action == MemberActionType.SortByValue)
                {
                    AddCurrentStateToHistory();
                    this.CurrentHistoryItem.SortByValue(args.AxisIndex, args);
                }
            }
            return RefreshQuery();
        }

        public String PerformServiceCommand(ServiceCommandType actionType)
        {
            try
            {
                switch (actionType)
                {
                    case ServiceCommandType.HideEmptyColumns:
                    case ServiceCommandType.ShowEmptyColumns:
                    case ServiceCommandType.RotateAxes:
                    case ServiceCommandType.NormalAxes:
                    case ServiceCommandType.HideEmptyRows:
                    case ServiceCommandType.ShowEmptyRows:
                        AddCurrentStateToHistory();
                        break;
                    default:
                        break;
                }

                switch (actionType)
                {
                    case ServiceCommandType.Forward:
                        this.MoveNext();
                        break;
                    case ServiceCommandType.Back:
                        this.MoveBack();
                        break;
                    case ServiceCommandType.ToBegin:
                        this.ToBegin();
                        break;
                    case ServiceCommandType.ToEnd:
                        this.ToEnd();
                        break;
                    case ServiceCommandType.HideEmptyColumns:
                        this.CurrentHistoryItem.ColumnsActionChain.HideEmpty = true;
                        break;
                    case ServiceCommandType.ShowEmptyColumns:
                        this.CurrentHistoryItem.ColumnsActionChain.HideEmpty = false;
                        break;
                    case ServiceCommandType.RotateAxes:
                        this.CurrentHistoryItem.RotateAxes = true;
                        break;
                    case ServiceCommandType.NormalAxes:
                        this.CurrentHistoryItem.RotateAxes = false;
                        break;
                    case ServiceCommandType.HideEmptyRows:
                        this.CurrentHistoryItem.RowsActionChain.HideEmpty = true;
                        break;
                    case ServiceCommandType.ShowEmptyRows:
                        this.CurrentHistoryItem.RowsActionChain.HideEmpty = false;
                        break;
                    case ServiceCommandType.ExportToExcel:
                        break;
                    case ServiceCommandType.GetDataSourceInfo:
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

            return RefreshQuery();
        }

        public virtual String ExportToExcel()
        {
            return String.Empty;
        }

        public String RefreshQuery()
        {
            return this.RefreshQuery(null);
        }

        public String RefreshQuery(Func<MdxObject, MdxObject> objectConsumerPattern)
        {
            String res = String.Empty;
            if (!string.IsNullOrEmpty(Query))
            {
                using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                {
                    StringBuilder sb = new StringBuilder();
                    provider.GenerateMdxFromDom(this.CreateWrappedStatement(), sb, new MdxGeneratorOptions());

                    res = sb.ToString();
                }
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
        private MdxSelectStatement CreateWrappedStatement()
        {
            if (m_OriginalStatement == null)
                return null;

            try
            {
                MdxSelectStatement select = (MdxSelectStatement)m_OriginalStatement.Clone();
                if (this.CurrentHistoryItem == null)
                    return select;

                return this.CurrentHistoryItem.CreateWrappedStatement(select);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public virtual IEnumerable<String> BuildUpdateScripts(String cubeName, IEnumerable<UpdateEntry> entries)
        {
            var commands = UpdateScriptParser.GetUpdateScripts(cubeName, UpdateScript, entries);
            return commands;
        }


        public String BuildDrillThrough(CellInfo cell)
        {
            String result = String.Empty;
            if (cell != null)
            {
                //var tuple = new Dictionary<String, MemberInfo>();
                //if (cell.ColumnMember != null && cell.ColumnMember != MemberInfo.Empty)
                //{
                //    cell.ColumnMember.CollectAncestors(tuple);
                //}
                //if (cell.RowMember != null && cell.RowMember != MemberInfo.Empty)
                //{
                //    cell.RowMember.CollectAncestors(tuple);
                //}

                //var statement = GetMoveAxesStatement(RefreshQuery());
                var statement = this.CreateWrappedStatement();
                var tuple = cell.GetTuple();
                if (statement != null && tuple != null && tuple.Count > 0)
                {
                    statement.Axes.Clear();
                    List<MdxExpression> members = new List<MdxExpression>();
                    foreach (var member in tuple.Values)
                    {
                        var expr = new MdxObjectReferenceExpression(member.UniqueName, member.Caption);
                        members.Add(expr);
                    }

                    statement.Axes.Add(new MdxAxis("0", new MdxTupleExpression(members)));

                    using (MdxDomProvider provider = MdxDomProvider.CreateProvider())
                    {
                        StringBuilder sb = new StringBuilder();
                        provider.GenerateMdxFromDom(statement, sb, new MdxGeneratorOptions());
                        result = sb.ToString();
                    }

                    if (!String.IsNullOrEmpty(result))
                        result = String.Format("DRILLTHROUGH {0}", result);
                }
            }
            return result;
        }
    }
}
