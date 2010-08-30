/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

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
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls.PivotGrid
{
    public static class FastCommandHelper
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

        public static MemberChoiceQuery CreateGetChildrenMembersQueryArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String memberUniqueName, String startLevelUniqueName, long begin, long count)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.StartLevelUniqueName = startLevelUniqueName;
            args.QueryType = MemberChoiceQueryType.GetChildrenMembers;
            args.BeginIndex = begin;
            args.Count = count;
            args.MemberUniqueName = memberUniqueName;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateGetRootMembersArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, long begin, long count)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.StartLevelUniqueName = startLevelUniqueName;
            args.QueryType = MemberChoiceQueryType.GetRootMembers;
            args.BeginIndex = begin;
            args.Count = count;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateLoadSetWithAscendantsArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String set)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.QueryType = MemberChoiceQueryType.LoadSetWithAscendants;
            args.Set = set;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateGetRootMembersCountArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.StartLevelUniqueName = startLevelUniqueName;
            args.QueryType = MemberChoiceQueryType.GetRootMembersCount;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateGetMemberArgs(String connectionString, String cubeName, String subCube, String memberUniqueName)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.MemberUniqueName = memberUniqueName;
            args.QueryType = MemberChoiceQueryType.GetMember;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateGetMembersArgs(String connectionString, String cubeName, String subCube, String set)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.Set = set;
            args.QueryType = MemberChoiceQueryType.GetMembers;
            args.SubCube = subCube;
            return args;
        }

        public static MemberChoiceQuery CreateFindMembersArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, FilterOperationBase filter)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.StartLevelUniqueName = startLevelUniqueName;
            args.QueryType = MemberChoiceQueryType.FindMembers;
            args.SubCube = subCube;
            args.Filter = filter;
            return args;
        }

        public static MemberChoiceQuery CreateGetAscendantsArgs(String connectionString, String cubeName, String subCube, String hierarchyUniqueName, String startLevelUniqueName, String memberUniqueName)
        {
            MemberChoiceQuery args = new MemberChoiceQuery();
            args.Connection = connectionString;
            args.CubeName = cubeName;
            args.HierarchyUniqueName = hierarchyUniqueName;
            args.StartLevelUniqueName = startLevelUniqueName;
            args.QueryType = MemberChoiceQueryType.GetAscendants;
            args.MemberUniqueName = memberUniqueName;
            args.SubCube = subCube;
            return args;
        }

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
            args.Query = query;
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

        public static PerformMemberActionArgs CreatePerformMemberActionArgs(String connectionString,
            ShortMemberInfo member,
            int axisIndex,
            MemberActionType action,
            List<ShortMemberInfo> ascendants)
        {
            PerformMemberActionArgs args = new PerformMemberActionArgs();
            args.Connection = connectionString;
            args.Member = member;
            args.AxisIndex = axisIndex;
            args.Action = action;
            args.Ascendants = ascendants;
            return args;
        }

        public static UpdateCubeArgs CreateUpdateCubeArgs(String connectionString, String cubeName, List<UpdateEntry> entries)
        {
            UpdateCubeArgs args = new UpdateCubeArgs();
            args.ConnectionString = connectionString;
            args.CubeName = cubeName;
            args.Entries = entries;
            return args;
        }

        //public static InvokeSchema CreateRunNavigationCommandSchema(string contextId, String dataSourceId, HistoryNavigationActionType actionType)
        //{
        //    InvokeSchema schema = new InvokeSchema(Commands.CommandId.HistoryNavigationCommandId);
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.ContextId, contextId));
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.DataSourceId, dataSourceId));
        //    schema.Args.Add(new InvokeArg(KnownInvokeArgs.PivotGridHistoryNavigation, XmlUtility.Obj2XmlStr(actionType, Common.Namespace)));

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
