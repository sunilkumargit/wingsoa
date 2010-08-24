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
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AnalysisServices.AdomdClient;
using Ranet.Olap.Core.Metadata;
using System.Data;

namespace Ranet.Olap.Core.Providers
{
    public class OlapMetadataProvider 
    {
        public readonly ConnectionInfo Connection = null;

        public OlapMetadataProvider(ConnectionInfo connection)
        {
            if (connection == null)
            {
                System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Connection in Constructor equal Null \r\n",
                    DateTime.Now.ToString());

                throw new ArgumentNullException("connection");
            }

            Connection = connection;
        }

        public Dictionary<String, MeasureInfo> GetMeasures(String cubeName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measures List Started \r\n cubeName: '{1}'",
                    DateTime.Now.ToString(), cubeName);

                Dictionary<String, MeasureInfo> list = new Dictionary<String, MeasureInfo>();

                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Measure measure in cube.Measures)
                    {
                        MeasureInfo info = InfoHelper.CreateMeasureInfo(measure);
                        if (info != null)
                        {
                            if (!list.ContainsKey(info.UniqueName))
                                list.Add(info.UniqueName, info);
                        }
                    }
                }

                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measures List Completed ",
                    DateTime.Now.ToString(), cubeName);
            }
        }

        public MeasureInfo GetMeasure(String cubeName, String measureUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measure '{1}' Started \r\n cubeName: '{2}'",
                    DateTime.Now.ToString(), measureUniqueName, cubeName);

                if (String.IsNullOrEmpty(measureUniqueName))
                    return null;

                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Measure measure in cube.Measures)
                    {
                        MeasureInfo info = InfoHelper.CreateMeasureInfo(measure);
                        if (info != null)
                        {
                            if (info.UniqueName.ToLower() == measureUniqueName.ToLower())
                                return info;
                        }
                    }
                }

                return null;
            }
            finally
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measure '{1}' Completed ",
                    DateTime.Now.ToString(), measureUniqueName, cubeName);
            }
        }

        protected virtual AdomdConnection GetConnection()
        {
            AdomdConnection conn = new AdomdConnection(Connection.ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Возвращает список KPI
        /// </summary>
        /// <param name="cubeName"></param>
        /// <returns></returns>
        public Dictionary<String, KpiInfo> GetKPIs(String cubeName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Kpis List Started \r\n cubeName: '{1}'", 
                    DateTime.Now.ToString(), cubeName);

                Dictionary<String, KpiInfo> list = new Dictionary<String, KpiInfo>();

                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Kpi kpi in cube.Kpis)
                    {
                        KpiInfo info = InfoHelper.CreateKpiInfo(kpi);
                        if (info != null)
                        {
                            if (!list.ContainsKey(info.Name))
                                list.Add(info.Name, info);
                        }
                    }
                }
                else
                {
                    String str = String.Format(Localization.MetadataResponseException_CubeNotFound, cubeName, Connection.ConnectionID);
                    System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                        DateTime.Now.ToString(), cubeName, str); 
                    throw new OlapMetadataResponseException(str);
                }

                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Kpis List Completed \r\n", DateTime.Now.ToString());
            }
        }

        /// <summary>
        /// Возвращает KPI
        /// </summary>
        /// <param name="cubeName"></param>
        /// <returns></returns>
        public KpiInfo GetKPI(String cubeName, String kpiName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Kpi '{1}' Started \r\n cubeName: '{2}'", 
                    DateTime.Now.ToString(), kpiName, cubeName);

                if (String.IsNullOrEmpty(kpiName))
                    return null;

                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Kpi kpi in cube.Kpis)
                    {
                        KpiInfo info = InfoHelper.CreateKpiInfo(kpi);
                        if (info != null && info.Name.ToLower() == kpiName.ToLower())
                        {
                            return info;
                        }
                    }
                }
                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Kpi '{1}' Completed \r\n", DateTime.Now.ToString(), kpiName);
            }
        }

        public Dictionary<String, LevelInfo> GetLevels(string cubeName, string dimensionUniqueName, string hierarchyUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Levels List Started \r\n cubeName: '{1}' \r\n dimensionUniqueName: '{2}' \r\n hierarchyUniqueName: '{3}' ",
                    DateTime.Now.ToString(), cubeName, dimensionUniqueName, hierarchyUniqueName);

                Dictionary<String, LevelInfo> list = new Dictionary<String, LevelInfo>();

                // Ищем иерархию
                Hierarchy hierarchy = FindHierarchy(cubeName, dimensionUniqueName, hierarchyUniqueName);
                if (hierarchy != null)
                {
                    foreach (Level level in hierarchy.Levels)
                    {
                        LevelInfo info = InfoHelper.CreateLevelInfo(level);
                        if (info != null)
                        {
                            if (!list.ContainsKey(info.UniqueName))
                                list.Add(info.UniqueName, info);
                        }
                    }
                }
                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Levels List Completed ",
                    DateTime.Now.ToString());
            }
        }

        public LevelInfo GetLevel(string cubeName, string dimensionUniqueName, string hierarchyUniqueName, String levelUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Level '{1}' Started \r\n cubeName: '{2}' \r\n dimensionUniqueName: '{3}'", 
                    DateTime.Now.ToString(), levelUniqueName, cubeName, dimensionUniqueName);

                // Ищем уровень
                Level level = FindLevel(cubeName, dimensionUniqueName, hierarchyUniqueName, levelUniqueName);
                if (level != null)
                {
                    return InfoHelper.CreateLevelInfo(level);
                }
                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Level '{1}' Completed \r\n", DateTime.Now.ToString(), levelUniqueName);            
            }
        }

        public Dictionary<String, LevelPropertyInfo> GetLevelProperties(string cubeName, string dimensionUniqueName, string hierarchyUniqueName, String levelUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Level '{1}' Properties Started \r\n cubeName: '{2}' \r\n dimensionUniqueName: '{3}' \r\n hierarchyUniqueName: '{4}' ",
                    DateTime.Now.ToString(), levelUniqueName, cubeName, dimensionUniqueName, hierarchyUniqueName);

                Dictionary<String, LevelPropertyInfo> list = new Dictionary<String, LevelPropertyInfo>();
                // Ищем уровень
                Level level = FindLevel(cubeName, dimensionUniqueName, hierarchyUniqueName, levelUniqueName);
                if (level != null)
                {
                    // Свойства уровня - атрибуты
                    AdomdConnection conn = GetConnection();

                    AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
                    restrictions.Add("CATALOG_NAME", conn.Database);
                    restrictions.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                    restrictions.Add("DIMENSION_UNIQUE_NAME", dimensionUniqueName);
                    restrictions.Add("HIERARCHY_UNIQUE_NAME", hierarchyUniqueName);
                    restrictions.Add("LEVEL_UNIQUE_NAME", level.UniqueName);

                    DataSet ds = conn.GetSchemaDataSet("MDSCHEMA_PROPERTIES", restrictions);
                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable table = ds.Tables[0];

                        if (ds.Tables[0].Columns.Count > 0)
                        {
                            object obj = null;
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                Type type = null;
                                if (table.Columns.Contains("DATA_TYPE"))
                                {
                                    obj = row["DATA_TYPE"];
                                    System.Data.OleDb.OleDbType oleDbType = (System.Data.OleDb.OleDbType)(Convert.ToInt32(obj));
                                    type = OleDbTypeConverter.Convert(oleDbType);
                                }

                                String name = String.Empty;
                                if (table.Columns.Contains("PROPERTY_NAME"))
                                {
                                    obj = row["PROPERTY_NAME"];
                                    if (obj != null)
                                        name = obj.ToString();
                                }

                                String caption = String.Empty;
                                if (table.Columns.Contains("PROPERTY_CAPTION"))
                                {
                                    obj = row["PROPERTY_CAPTION"];
                                    if (obj != null)
                                        caption = obj.ToString();
                                }

                                String description = String.Empty;
                                if (table.Columns.Contains("DESCRIPTION"))
                                {
                                    obj = row["DESCRIPTION"];
                                    if (obj != null)
                                        description = obj.ToString();
                                }

                                int propertyType = 0;
                                if (table.Columns.Contains("PROPERTY_TYPE"))
                                {
                                    obj = row["PROPERTY_TYPE"];
                                    if (obj != null)
                                        propertyType = Convert.ToInt32(obj);
                                }

                                LevelPropertyInfo lpi = new LevelPropertyInfo();
                                lpi.Caption = caption;
                                lpi.Description = description;
                                lpi.Name = name;
                                lpi.ParentLevelId = level.UniqueName;
                                //lpi.DataType = type;
                                if ((propertyType & 0x04) == 0x04)
                                    lpi.IsSystem = true;

                                lpi.PropertyType = propertyType;

                                //info.LevelProperties.Add(lpi);
                                list.Add(lpi.Name, lpi);
                            }
                        }
                    }

                    //list.Add(info);
                }
                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Level '{1}' Properties Completed ", DateTime.Now.ToString(), levelUniqueName);
            }
        }

        public DimensionInfo GetDimensionToHierarchy(String cubeName, String hierarchyUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimension To Hierarchy '{1}' Started \r\n cubeName: '{2}' ",
                    DateTime.Now.ToString(), hierarchyUniqueName, cubeName);

                if (!String.IsNullOrEmpty(hierarchyUniqueName) && !String.IsNullOrEmpty(cubeName))
                {
                    AdomdConnection conn = GetConnection();
                    CubeDef cube = FindCube(cubeName);
                    if (cube != null)
                    {
                        foreach (Dimension dim in cube.Dimensions)
                        {
                            foreach (Hierarchy hierarchy in dim.Hierarchies)
                            {
                                if (hierarchy.UniqueName.ToLower() == hierarchyUniqueName.ToLower())
                                {
                                    return InfoHelper.CreateDimensionInfo(dim);
                                }
                            }
                        }
                    }
                }
                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimension To Hierarchy Completed \r\n", DateTime.Now.ToString());
            }
        }

        public HierarchyInfo GetHierarchy(String cubeName, string dimensionUniqueName, string hierarchyUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Hierarchy '{1}' Started \r\n cubeName: '{2}'", 
                    DateTime.Now.ToString(), hierarchyUniqueName, cubeName);

                AdomdConnection conn = GetConnection();

                Hierarchy hierarchy = FindHierarchy(cubeName, dimensionUniqueName, hierarchyUniqueName);
                if (hierarchy != null)
                {
                    return InfoHelper.CreateHierarchyInfo(hierarchy);
                }

                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Hierarchy '{1}' Completed \r\n", DateTime.Now.ToString(), hierarchyUniqueName);
            }
        }

        public Dictionary<String, HierarchyInfo> GetHierarchies(String cubeName, string dimensionUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Hierarchies Started \r\n cubeName: '{1}' \r\n dimensionUniqueName: '{2}'", 
                    DateTime.Now.ToString(), cubeName, dimensionUniqueName);

                Dictionary<String, HierarchyInfo> list = new Dictionary<String, HierarchyInfo>();
                AdomdConnection conn = GetConnection();

                Dimension dim = FindDimension(cubeName, dimensionUniqueName);
                if (dim != null)
                {
                    foreach (Hierarchy hierarchy in dim.Hierarchies)
                    {
                        HierarchyInfo info = InfoHelper.CreateHierarchyInfo(hierarchy);
                        if (info != null)
                        {
                            if (!list.ContainsKey(info.UniqueName))
                                list.Add(info.UniqueName, info);
                        }
                    }
                }

                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Hierarchies Completed \r\n", DateTime.Now.ToString());
            }
        }

        public Dictionary<String, DimensionInfo> GetDimensions(string cubeName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimensions List Started \r\n cubeName: '{1}' ",
                    DateTime.Now.ToString(), cubeName);

                Dictionary<String, DimensionInfo> list = new Dictionary<String, DimensionInfo>();
                AdomdConnection conn = GetConnection();
                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Dimension dim in cube.Dimensions)
                    {
                        DimensionInfo info = InfoHelper.CreateDimensionInfo(dim);
                        if (info != null)
                        {
                            if (!list.ContainsKey(info.UniqueName))
                            {
                                list.Add(info.UniqueName, info);
                            }
                        }
                    }
                }
                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimensions List Completed \r\n ",
                    DateTime.Now.ToString(), cubeName);
            }
        }

        public DimensionInfo GetDimension(string cubeName, String dimensionUniqueName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimension '{1}' Started \r\n cubeName: '{2}' ", 
                    DateTime.Now.ToString(), dimensionUniqueName, cubeName);

                AdomdConnection conn = GetConnection();

                Dimension dimension = FindDimension(cubeName, dimensionUniqueName);
                if (dimension != null)
                {
                    return InfoHelper.CreateDimensionInfo(dimension);
                }
                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Dimension '{1}' From Cube '{2}' Completed", DateTime.Now.ToString(), dimensionUniqueName, cubeName);
            }
        }

        /// <summary>
        /// Возвращает список кубов
        /// </summary>
        /// <returns></returns>
        public CubeDefInfo  GetCubeMetadata(String cubeName, MetadataQueryType type)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Getting Cube '{1}' Metadata Started",
                    DateTime.Now.ToString(), cubeName);

                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    CubeDefInfo cube_info = InfoHelper.CreateCubeInfo(cube);
                    foreach (Dimension dim in cube.Dimensions)
                    {
                        DimensionInfo dim_info = InfoHelper.CreateDimensionInfo(dim);
                        cube_info.Dimensions.Add(dim_info);

                        foreach (Hierarchy hierarchy in dim.Hierarchies)
                        {
                            HierarchyInfo hier_info = InfoHelper.CreateHierarchyInfo(hierarchy);
                            dim_info.Hierarchies.Add(hier_info);

                            foreach (Level level in hierarchy.Levels)
                            {
                                LevelInfo level_info = InfoHelper.CreateLevelInfo(level);
                                hier_info.Levels.Add(level_info);
                            }

                            //AdomdConnection conn = GetConnection(ConnectionString);

                            //// Для каждой иерархии пытаемся определить элемент, который имеет тип MemberTypeEnum.All
                            //try
                            //{
                            //    AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
                            //    restrictions.Add("CATALOG_NAME", conn.Database);
                            //    restrictions.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                            //    restrictions.Add("DIMENSION_UNIQUE_NAME", dim.UniqueName);
                            //    restrictions.Add("HIERARCHY_UNIQUE_NAME", hierarchy.UniqueName);
                            //    restrictions.Add("MEMBER_TYPE", 2/*MemberTypeEnum.All*/);

                            //    DataSet ds = conn.GetSchemaDataSet("MDSCHEMA_MEMBERS", restrictions);
                            //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            //    {
                            //        DataTable table = ds.Tables[0];
                            //        if (table.Columns.Contains("MEMBER_UNIQUE_NAME"))
                            //        {
                            //            object obj = ds.Tables[0].Rows[0]["MEMBER_UNIQUE_NAME"];
                            //            if (obj != null)
                            //                hier_info.Custom_AllMemberUniqueName = obj.ToString();
                            //        }
                            //    }
                            //}catch
                            //{
                            //}

                            //// Для каждой иерархии пытаемся определить элемент, который имеет тип MemberTypeEnum.Unknown
                            //try
                            //{
                            //    AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
                            //    restrictions.Add("CATALOG_NAME", conn.Database);
                            //    restrictions.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                            //    restrictions.Add("DIMENSION_UNIQUE_NAME", dim.UniqueName);
                            //    restrictions.Add("HIERARCHY_UNIQUE_NAME", hierarchy.UniqueName);
                            //    restrictions.Add("MEMBER_TYPE", 0 /*MemberTypeEnum.Unknown.All*/);

                            //    DataSet ds = conn.GetSchemaDataSet("MDSCHEMA_MEMBERS", restrictions);
                            //    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            //    {
                            //        DataTable table = ds.Tables[0];
                            //        if (table.Columns.Contains("MEMBER_UNIQUE_NAME"))
                            //        {
                            //            object obj = ds.Tables[0].Rows[0]["MEMBER_UNIQUE_NAME"];
                            //            if (obj != null)
                            //                hier_info.Custom_UnknownMemberUniqueName = obj.ToString();
                            //        }
                            //    }
                            //}
                            //catch
                            //{
                            //}
                        }
                    }

                    foreach (Kpi kpi in cube.Kpis)
                    {
                        KpiInfo kpi_info = InfoHelper.CreateKpiInfo(kpi);
                        cube_info.Kpis.Add(kpi_info);
                    }

                    foreach (Measure measure in cube.Measures)
                    {
                        MeasureInfo measure_info = InfoHelper.CreateMeasureInfo(measure);
                        cube_info.Measures.Add(measure_info);
                    }

                    foreach (NamedSet set in cube.NamedSets)
                    {
                        NamedSetInfo set_info = InfoHelper.CreateNamedSetInfo(set);
                        cube_info.NamedSets.Add(set_info);
                    }

                    if (type == MetadataQueryType.GetCubeMetadata_AllMembers)
                    {
                        AdomdConnection conn = GetConnection();
                        // Для каждой иерархии пытаемся определить элемент, который имеет тип MemberTypeEnum.All
                        try
                        {
                            AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
                            restrictions.Add("CATALOG_NAME", conn.Database);
                            restrictions.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                            restrictions.Add("MEMBER_TYPE", 2/*MemberTypeEnum.All*/);

                            DataSet ds = conn.GetSchemaDataSet("MDSCHEMA_MEMBERS", restrictions);
                            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                            {
                                DataTable table = ds.Tables[0];
                                if (table.Columns.Contains("MEMBER_UNIQUE_NAME") &&
                                    table.Columns.Contains("HIERARCHY_UNIQUE_NAME") &&
                                    table.Columns.Contains("DIMENSION_UNIQUE_NAME"))
                                {
                                    foreach (DataRow row in ds.Tables[0].Rows)
                                    {
                                        String dimension_UniqueName = row["DIMENSION_UNIQUE_NAME"] != null ? row["DIMENSION_UNIQUE_NAME"].ToString() : String.Empty;
                                        String hierarchy_UniqueName = row["HIERARCHY_UNIQUE_NAME"] != null ? row["HIERARCHY_UNIQUE_NAME"].ToString() : String.Empty;
                                        String member_UniqueName = row["MEMBER_UNIQUE_NAME"] != null ? row["MEMBER_UNIQUE_NAME"].ToString() : String.Empty;

                                        if (!String.IsNullOrEmpty(dimension_UniqueName) &&
                                            !String.IsNullOrEmpty(hierarchy_UniqueName) &&
                                            !String.IsNullOrEmpty(member_UniqueName))
                                        {
                                            DimensionInfo dimension = cube_info.GetDimension(dimension_UniqueName);
                                            if (dimension != null)
                                            {
                                                HierarchyInfo hierarchy = dimension.GetHierarchy(hierarchy_UniqueName);
                                                if (hierarchy != null)
                                                {
                                                    hierarchy.Custom_AllMemberUniqueName = member_UniqueName;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw ex;
                        }
                    }

                    cube_info.MeasureGroups = GetMeasureGroups(cubeName);

                    return cube_info;
                }

                return null;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Getting Cube '{1}' Metadata Completed",
    DateTime.Now.ToString(), cubeName);
            }
        }


        /// <summary>
        /// Возвращает список кубов
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, CubeDefInfo> GetCubes()
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Getting Cubes List Started", DateTime.Now.ToString());

                Dictionary<String, CubeDefInfo> list = new Dictionary<String, CubeDefInfo>();
                AdomdConnection conn = GetConnection();
                foreach (CubeDef cube in conn.Cubes)
                {
                    CubeDefInfo info = InfoHelper.CreateCubeInfo(cube);
                    if (info != null)
                    {
                        if (!list.ContainsKey(info.Name))
                            list.Add(info.Name, info);
                    }
                }
                return list;
            }
            finally {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Getting Cubes List Completed", DateTime.Now.ToString());
            }
        }

        private CubeDef FindCube(String cubeName)
        {
            if (!String.IsNullOrEmpty(cubeName))
            {
                String name = OlapHelper.ConvertToNormalStyle(cubeName);
                AdomdConnection conn = GetConnection();
                foreach (CubeDef cube in conn.Cubes)
                {
                    if (cube.Name.ToLower() == name.ToLower())
                    {
                        return cube;
                    }
                }
            }

            String str = String.Format(Localization.MetadataResponseException_CubeNotFound, cubeName, Connection.ConnectionID);
            System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                DateTime.Now.ToString(), cubeName, str);
            throw new OlapMetadataResponseException(str);
            //return null;
        }

        private Dimension FindDimension(String cubeName, String dimensionUniqueName)
        {
            AdomdConnection conn = GetConnection();
            CubeDef cube = FindCube(cubeName);
            if (cube != null)
            {
                foreach (Dimension dim in cube.Dimensions)
                {
                    if (dim.UniqueName.ToLower() == dimensionUniqueName.ToLower())
                    {
                        return dim;
                    }
                }
            }

            String str = String.Format(Localization.MetadataResponseException_DimensionByUniqueName_InCube_NotFound, dimensionUniqueName, cubeName, Connection.ConnectionID);
            System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                DateTime.Now.ToString(), cubeName, str);
            throw new OlapMetadataResponseException(str);

            //return null;
        }

        private Hierarchy FindHierarchy(String cubeName, String dimensionUniqueName, String hierarchyUniqueName)
        {
            AdomdConnection conn = GetConnection();
            if (!String.IsNullOrEmpty(dimensionUniqueName))
            {
                Dimension dim = FindDimension(cubeName, dimensionUniqueName);
                if (dim != null)
                {
                    foreach (Hierarchy hierarchy in dim.Hierarchies)
                    {
                        if (hierarchy.UniqueName.ToLower() == hierarchyUniqueName.ToLower())
                        {
                            return hierarchy;
                        }
                    }
                }
                String str = String.Format(Localization.MetadataResponseException_HierarchyByUniqueName_InDimension_NotFound, hierarchyUniqueName, dimensionUniqueName, Connection.ConnectionID);
                System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                    DateTime.Now.ToString(), cubeName, str);
                throw new OlapMetadataResponseException(str);
            }
            else
            {
                CubeDef cube = FindCube(cubeName);
                if (cube != null)
                {
                    foreach (Dimension dim in cube.Dimensions)
                    {
                        foreach (Hierarchy hierarchy in dim.Hierarchies)
                        {
                            if (hierarchy.UniqueName.ToLower() == hierarchyUniqueName.ToLower())
                            {
                                return hierarchy;
                            }
                        }
                    }
                }
                String str = String.Format(Localization.MetadataResponseException_HierarchyByUniqueName_InCube_NotFound, hierarchyUniqueName, cubeName, Connection.ConnectionID);
                System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                    DateTime.Now.ToString(), cubeName, str);
                throw new OlapMetadataResponseException(str);
            }
            //return null;
        }

        private Level FindLevel(String cubeName, String dimensionUniqueName, String hierarchyUniqueName, String levelUniqueName)
        {
            Hierarchy hierarchy = FindHierarchy(cubeName, dimensionUniqueName, hierarchyUniqueName);
            if(hierarchy != null)
            {
                foreach(Level level in hierarchy.Levels)
                {
                    if (level.UniqueName.ToLower() == levelUniqueName.ToLower())
                        return level;
                }
            }
            String str = String.Format(Localization.MetadataResponseException_LevelByUniqueName_InHierarchy_NotFound, levelUniqueName, hierarchyUniqueName, Connection.ConnectionID);
            System.Diagnostics.Trace.TraceError("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider: {1} \r\n",
                DateTime.Now.ToString(), cubeName, str);
            throw new OlapMetadataResponseException(str);
            //return null;
        }

        public List<MeasureGroupInfo> GetMeasureGroups(String cubeName)
        {
            try
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measures Groups List Started \r\n cubeName: '{1}'",
                    DateTime.Now.ToString(), cubeName);

                Dictionary<String, MeasureGroupInfo> list = new Dictionary<String, MeasureGroupInfo>();
                AdomdConnection conn = GetConnection();

                AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
                restrictions.Add("CATALOG_NAME", conn.Database);
                if (!String.IsNullOrEmpty(cubeName))
                {
                    restrictions.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                }

                #region Получение списка групп мер
                DataSet ds = conn.GetSchemaDataSet("MDSCHEMA_MEASUREGROUPS", restrictions);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    if (ds.Tables[0].Columns.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            MeasureGroupInfo info = new MeasureGroupInfo();
                            if (table.Columns.Contains("CATALOG_NAME"))
                            {
                                if (row["CATALOG_NAME"] != null)
                                {
                                    info.CatalogName = row["CATALOG_NAME"].ToString();
                                }
                            }

                            if (table.Columns.Contains("CUBE_NAME"))
                            {
                                if (row["CUBE_NAME"] != null)
                                {
                                    info.CubeName = row["CUBE_NAME"].ToString();
                                }
                            }

                            if (table.Columns.Contains("MEASUREGROUP_NAME"))
                            {
                                if (row["MEASUREGROUP_NAME"] != null)
                                {
                                    info.Name = row["MEASUREGROUP_NAME"].ToString();
                                }
                            }

                            if (table.Columns.Contains("DESCRIPTION"))
                            {
                                if (row["DESCRIPTION"] != null)
                                {
                                    info.Description = row["DESCRIPTION"].ToString();
                                }
                            }

                            if (table.Columns.Contains("MEASUREGROUP_CAPTION"))
                            {
                                if (row["MEASUREGROUP_CAPTION"] != null)
                                {
                                    info.Caption = row["MEASUREGROUP_CAPTION"].ToString();
                                }
                            }

                            if (table.Columns.Contains("IS_WRITE_ENABLED"))
                            {
                                if (row["IS_WRITE_ENABLED"] != null)
                                {
                                    info.IsWriteEnabled = Convert.ToBoolean(row["IS_WRITE_ENABLED"]);
                                }
                            }

                            if (!list.ContainsKey(info.Name))
                            {
                                list.Add(info.Name, info);
                            }
                        }
                    }
                }
                #endregion Получение списка групп мер

                #region Получение списка мер и распознавание для каких групп мер они относятся
                ds = conn.GetSchemaDataSet("MDSCHEMA_MEASURES", restrictions);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    if (ds.Tables[0].Columns.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            String measuresGroupName = string.Empty;
                            if (table.Columns.Contains("MEASUREGROUP_NAME"))
                            {
                                if (row["MEASUREGROUP_NAME"] != null)
                                {
                                    measuresGroupName = row["MEASUREGROUP_NAME"].ToString();
                                }
                            }

                            String measureUniqueName = String.Empty;
                            if (table.Columns.Contains("MEASURE_UNIQUE_NAME"))
                            {
                                if (row["MEASURE_UNIQUE_NAME"] != null)
                                {
                                    measureUniqueName = row["MEASURE_UNIQUE_NAME"].ToString();
                                }
                            }

                            if (!String.IsNullOrEmpty(measuresGroupName) &&
                                !String.IsNullOrEmpty(measureUniqueName))
                            {
                                if (list.ContainsKey(measuresGroupName))
                                {
                                    if (!list[measuresGroupName].Measures.Contains(measureUniqueName))
                                        list[measuresGroupName].Measures.Add(measureUniqueName);
                                }
                            }
                        }
                    }
                }
                #endregion Получение списка мер и распознавание для каких групп мер они относятся

                #region Получение списка KPI и распознавание для каких групп мер они относятся
                ds = conn.GetSchemaDataSet("MDSCHEMA_KPIS", restrictions);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable table = ds.Tables[0];

                    if (ds.Tables[0].Columns.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            String kpiName = string.Empty;
                            if (table.Columns.Contains("KPI_NAME"))
                            {
                                if (row["KPI_NAME"] != null)
                                {
                                    kpiName = row["KPI_NAME"].ToString();
                                }
                            }

                            String measuresGroupName = string.Empty;
                            if (table.Columns.Contains("MEASUREGROUP_NAME"))
                            {
                                if (row["MEASUREGROUP_NAME"] != null)
                                {
                                    measuresGroupName = row["MEASUREGROUP_NAME"].ToString();
                                }
                            }

                            if (!String.IsNullOrEmpty(measuresGroupName) &&
                                !String.IsNullOrEmpty(kpiName))
                            {
                                if (list.ContainsKey(measuresGroupName))
                                {
                                    if (!list[measuresGroupName].Kpis.Contains(kpiName))
                                        list[measuresGroupName].Kpis.Add(kpiName);
                                }
                            }
                        }
                    }
                }
                #endregion Получение списка KPI и распознавание для каких групп мер они относятся

                #region Получение списка измерений для каждой группы мер (MDSCHEMA_DIMENSIONS как оказалось не содержит информации о принадлежности измерения к группе мер - поэтому делаем несколько запросов MDSCHEMA_MEASUREGROUP_DIMENSIONS)

                foreach (MeasureGroupInfo info in list.Values)
                {
                    AdomdRestrictionCollection restrictions1 = new AdomdRestrictionCollection();
                    restrictions1.Add("CATALOG_NAME", conn.Database);
                    restrictions1.Add("CUBE_NAME", OlapHelper.ConvertToNormalStyle(cubeName));
                    restrictions1.Add("MEASUREGROUP_NAME", info.Name);
                    ds = conn.GetSchemaDataSet("MDSCHEMA_MEASUREGROUP_DIMENSIONS", restrictions1);

                    if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        DataTable table = ds.Tables[0];

                        if (ds.Tables[0].Columns.Count > 0)
                        {
                            foreach (DataRow row in ds.Tables[0].Rows)
                            {
                                if (table.Columns.Contains("DIMENSION_UNIQUE_NAME"))
                                {
                                    if (row["DIMENSION_UNIQUE_NAME"] != null)
                                    {
                                        String dimensionUniqueName = row["DIMENSION_UNIQUE_NAME"].ToString();
                                        if (!String.IsNullOrEmpty(dimensionUniqueName))
                                        {
                                            if (!info.Dimensions.Contains(dimensionUniqueName))
                                                info.Dimensions.Add(dimensionUniqueName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion Получение списка KPI и распознавание для каких групп мер они относятся

                List<MeasureGroupInfo> result = new List<MeasureGroupInfo>();
                foreach (MeasureGroupInfo info in list.Values)
                {
                    result.Add(info);
                }

                return result;
            }
            finally
            {
                System.Diagnostics.Trace.TraceInformation("{0} Ranet.Olap.Core.Providers.OlapMetadataProvider Get Measures Groups List Completed ",
                    DateTime.Now.ToString());
            }
        }

    }
}
