/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Controls.General.ClientServer;
using Wing.Olap.Controls.MemberChoice.ClientServer;
using Wing.Olap.Controls.PivotGrid;
using System.Collections.Generic;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Providers;
using Wing.Olap.Core.Providers.ClientServer;
using Wing.Olap.Providers;

namespace Wing.Olap.Commands
{
    public static class CommandHelper
    {
        public static MetadataQuery CreateGetCubesQueryArgs(String connectionString)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.QueryType = MetadataQueryType.GetCubes;
            return args;
        }

        public static MetadataQuery CreateGetCubeMetadataArgs(String connectionString, String cubeName, MetadataQueryType type)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = type;
            return args;
        }

        public static MetadataQuery CreateGetMeasuresQueryArgs(String connectionString, String cubeName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = MetadataQueryType.GetMeasures;
            return args;
        }

        public static MetadataQuery CreateGetMeasureQueryArgs(String connectionString, String cubeName, String measureUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = MetadataQueryType.GetMeasure;
            args.MeasureUniqueName = measureUniqueName;
            return args;
        }

        public static MetadataQuery CreateGetKPIsQueryArgs(String connectionString, String cubeName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = MetadataQueryType.GetKPIs;
            return args;
        }

        public static MetadataQuery CreateGetKPIQueryArgs(String connectionString, String cubeName, String kpiName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = MetadataQueryType.GetKPI;
            args.KPIName = kpiName;
            return args;
        }

        public static MetadataQuery CreateGetDimensionsQueryArgs(String connectionString, String cubeName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.QueryType = MetadataQueryType.GetDimensions;
            return args;
        }

        public static MetadataQuery CreateGetHierarchyQueryArgs(String connectionString, String cubeName, String dimensionUniqueName, String hierarchyUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.QueryType = MetadataQueryType.GetHierarchy;
            return args;
        }

        public static MetadataQuery CreateGetDimensionQueryArgs(String connectionString, String cubeName, String dimensionUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.QueryType = MetadataQueryType.GetDimension;
            return args;
        }

        public static MetadataQuery CreateGetHierarchiesQueryArgs(String connectionString, String cubeName, String dimensionUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.QueryType = MetadataQueryType.GetHierarchies;
            return args;
        }

        public static MetadataQuery CreateGetLevelsQueryArgs(String connectionString, String cubeName, String dimensionUniqueName, String hierarchyUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.QueryType = MetadataQueryType.GetLevels;
            return args;
        }

        public static MetadataQuery CreateGetLevelQueryArgs(String connectionString, String cubeName, String dimensionUniqueName, String hierarchyUniqueName, String levelUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.LevelUniqueName = levelUniqueName;
            args.QueryType = MetadataQueryType.GetLevel;
            return args;
        }

        //public static MemberChoiceQuery CreateGetChildrenMembersQueryArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String memberUniqueName, String startLevelUniqueName, long begin, long count)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.StartLevelUniqueName = startLevelUniqueName;
        //    args.QueryType = MemberChoiceQueryType.GetChildrenMembers;
        //    args.BeginIndex = begin;
        //    args.Count = count;
        //    args.MemberUniqueName = memberUniqueName;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateGetRootMembersArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, long begin, long count)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.StartLevelUniqueName = startLevelUniqueName;
        //    args.QueryType = MemberChoiceQueryType.GetRootMembers;
        //    args.BeginIndex = begin;
        //    args.Count = count;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateLoadSetWithAscendantsArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String set)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.QueryType = MemberChoiceQueryType.LoadSetWithAscendants;
        //    args.Set = set;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateGetRootMembersCountArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.StartLevelUniqueName = startLevelUniqueName;
        //    args.QueryType = MemberChoiceQueryType.GetRootMembersCount;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateGetMemberArgs(String connectionString, String cubeName, String subCube, String memberUniqueName)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.MemberUniqueName = memberUniqueName;
        //    args.QueryType = MemberChoiceQueryType.GetMember;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateGetMembersArgs(String connectionString, String cubeName, String subCube, String set)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.Set = set;
        //    args.QueryType = MemberChoiceQueryType.GetMembers;
        //    args.SubCube = subCube;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateFindMembersArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, FilterOperationBase filter)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.StartLevelUniqueName = startLevelUniqueName;
        //    args.QueryType = MemberChoiceQueryType.FindMembers;
        //    args.SubCube = subCube;
        //    args.Filter = filter;
        //    return args;
        //}

        //public static MemberChoiceQuery CreateGetAscendantsArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, String memberUniqueName)
        //{
        //    MemberChoiceQuery args = new MemberChoiceQuery();
        //    args.Connection = connectionString;
        //    args.CubeName = cubeName;
        //    args.HierarchyUniqueName = hierarchyUniqueName;
        //    args.StartLevelUniqueName = startLevelUniqueName;
        //    args.QueryType = MemberChoiceQueryType.GetAscendants;
        //    args.MemberUniqueName = memberUniqueName;
        //    args.SubCube = subCube;
        //    return args;
        //}

        public static MetadataQuery CreateLoadLevelPropertiesArgs(String connectionString, String cubeName, String dimensionUniqueName, String hierarchyUniqueName, String levelUniqueName)
        {
            MetadataQuery args = new MetadataQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.DimensionUniqueName = dimensionUniqueName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.LevelUniqueName = levelUniqueName;
            args.QueryType = MetadataQueryType.GetLevelProperties;
            return args;
        }

        public static MdxQueryArgs CreateMdxQueryArgs(String connectionString, String query)
        {
            MdxQueryArgs args = new MdxQueryArgs();
            args.Connection = connectionString;
            args.Type = QueryTypes.Select;
            args.Queries.Add(query);
            return args;
        }

        public static PivotInitializeArgs CreatePivotInitializeArgs(String connectionString, String query, String updateScript)
        {
            PivotInitializeArgs args = new PivotInitializeArgs();
            args.Connection = connectionString;
            args.Query = query;
            args.UpdateScript = updateScript;
            return args;
        }

        public static PerformMemberActionArgs CreatePerformMemberActionArgs(
            MemberInfo member,
            int axisIndex,
            MemberActionType action,
            List<MemberInfo> ascendants)
        {
            PerformMemberActionArgs args = new PerformMemberActionArgs();
            args.Member = member;
            args.AxisIndex = axisIndex;
            args.Action = action;
            args.Ascendants = ascendants;
            return args;
        }

        //public static UpdateCubeArgs CreateUpdateCubeArgs(String connectionString, String cubeName, List<UpdateEntry> entries)
        //{
        //    UpdateCubeArgs args = new UpdateCubeArgs();
        //    args.ConnectionString = connectionString;
        //    args.CubeName = cubeName;
        //    args.Entries = entries;
        //    return args;
        //}

        //public static InvokeSchema CreateRunNavigationCommandSchema(string contextId, String dataSourceId, HistoryNavigationActionType actionType)
        //{
        //    InvokeSchema schema = new InvokeSchema(Commands.CommandId.HistoryNavigationCommandId);
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.ContextId, contextId));
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.DataSourceId, dataSourceId));
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.PivotGridHistoryNavigation, XmlUtility.Obj2XmlStr(actionType, Constants.XmlNamespace)));

        //    return schema;
        //}

        //public static InvokeSchema CreateToolBarInfoSchema(string contextId, String dataSourceId)
        //{
        //    InvokeSchema schema = new InvokeSchema(Commands.CommandId.ToolBarInfoCommandId);
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.ContextId, contextId));
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.DataSourceId, dataSourceId));

        //    return schema;
        //}
    }
}
