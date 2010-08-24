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
using System.Text;
using Ranet.Olap.Core.Metadata;
using Microsoft.AnalysisServices.AdomdClient;

namespace Ranet.Olap.Core.Metadata
{
    public static class InfoHelper
    {
        public static CubeDefInfo CreateCubeInfo(CubeDef cube)
        {
            if (cube == null)
                return null;
            CubeDefInfo info = new CubeDefInfo();
            info.Caption = cube.Caption;
            info.Description = cube.Description;
            info.Name = cube.Name;
            info.LastProcessed = cube.LastProcessed;
            info.LastUpdated = cube.LastUpdated;
            info.Type = (CubeInfoType)(cube.Type);

            // Свойства
            foreach (Property prop in cube.Properties)
            {
                //PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }
            return info;
        }

        public static NamedSetInfo CreateNamedSetInfo(NamedSet set)
        {
            if (set == null)
                return null;
            NamedSetInfo info = new NamedSetInfo();

            info.Caption = set.Caption;
            info.Description = set.Description;
            info.Name = set.Name;
            info.DisplayFolder = set.DisplayFolder;
            info.Expression = set.Expression;

            // Информация о предках
            if (set.ParentCube != null)
            {
                info.ParentCubeId = set.ParentCube.Name;
                info.CustomProperties.Add(new PropertyInfo(InfoBase.CUBE_CAPTION, set.ParentCube.Caption));
            }

            // Свойства
            foreach (Property prop in set.Properties)
            {
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }
            return info;
        }

        public static DimensionInfo CreateDimensionInfo(Dimension dim)
        {
            if (dim == null)
                return null;
            DimensionInfo info = new DimensionInfo();
            info.Caption = dim.Caption;
            info.Description = dim.Description;
            info.DimensionType = (DimensionInfoTypeEnum)(dim.DimensionType);
            info.Name = dim.Name;
            info.WriteEnabled = dim.WriteEnabled;
            info.UniqueName = dim.UniqueName;

            // Информация о предках
            if (dim.ParentCube != null)
            {
                info.ParentCubeId = dim.ParentCube.Name;
                info.CustomProperties.Add(new PropertyInfo(InfoBase.CUBE_CAPTION, dim.ParentCube.Caption));
            }

            // Свойства
            foreach (Property prop in dim.Properties)
            {
                //                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }
            return info;
        }

        public static HierarchyInfo CreateHierarchyInfo(Hierarchy hierarchy)
        {
            if (hierarchy == null)
                return null;
            HierarchyInfo info = new HierarchyInfo();
            info.Caption = hierarchy.Caption;
            info.Description = hierarchy.Description;
            info.DefaultMember = hierarchy.DefaultMember;

            // Информация о предках
            if (hierarchy.ParentDimension != null)
            {
                info.ParentDimensionId = hierarchy.ParentDimension.UniqueName;
                info.CustomProperties.Add(new PropertyInfo(InfoBase.DIMENSION_CAPTION, hierarchy.ParentDimension.Caption));
                if (hierarchy.ParentDimension.ParentCube != null)
                {
                    info.ParentCubeId = hierarchy.ParentDimension.ParentCube.Name;
                    info.CustomProperties.Add(new PropertyInfo(InfoBase.CUBE_CAPTION, hierarchy.ParentDimension.ParentCube.Caption));
                }
            }

            try
            {
                info.DisplayFolder = hierarchy.DisplayFolder;
            }
            catch (System.NotSupportedException)
            {
                // Не поддерживается MSAS 2000
                info.DisplayFolder = String.Empty;
            }
            info.Name = hierarchy.Name;
            info.HierarchyOrigin = (HierarchyInfoOrigin)(hierarchy.HierarchyOrigin);
            info.UniqueName = hierarchy.UniqueName;

            // Свойства
            foreach (Property prop in hierarchy.Properties)
            {
                //PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }
            return info;
        }

        public static LevelInfo CreateLevelInfo(Level level)
        {
            if (level == null)
                return null;
            LevelInfo info = new LevelInfo();
            info.Caption = level.Caption;
            info.Description = level.Description;
            info.LevelNumber = level.LevelNumber;
            info.LevelType = (LevelInfoTypeEnum)(level.LevelType);
            info.MemberCount = level.MemberCount;
            info.Name = level.Name;
            info.UniqueName = level.UniqueName;

            // Информация о предках
            if (level.ParentHierarchy != null)
            {
                info.ParentHirerachyId = level.ParentHierarchy.UniqueName;
                info.CustomProperties.Add(new PropertyInfo(InfoBase.HIERARCHY_CAPTION, level.ParentHierarchy.Caption));
                if (level.ParentHierarchy.ParentDimension != null)
                {
                    info.ParentDimensionId = level.ParentHierarchy.ParentDimension.UniqueName;
                    info.CustomProperties.Add(new PropertyInfo(InfoBase.DIMENSION_CAPTION, level.ParentHierarchy.ParentDimension.Caption));
                    if (level.ParentHierarchy.ParentDimension.ParentCube != null)
                    {
                        info.ParentCubeId = level.ParentHierarchy.ParentDimension.ParentCube.Name;
                        info.CustomProperties.Add(new PropertyInfo(InfoBase.CUBE_CAPTION, level.ParentHierarchy.ParentDimension.ParentCube.Caption));
                    }
                }
            }

            // Свойства
            foreach (Property prop in level.Properties)
            {
                //PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }

            //// Свойства уровня
            //foreach(LevelProperty lp in level.LevelProperties)
            //{
            //    LevelPropertyInfo lpi = new LevelPropertyInfo();
            //    lpi.Caption = lp.Caption;
            //    lpi.Description = lp.Description;
            //    lpi.Name = lp.Name;
            //    if(lp.ParentLevel != null)
            //    {
            //        lpi.ParentLevelId = lp.ParentLevel.UniqueName;
            //    }
            //    lpi.UniqueName = lp...UniqueName;

            //    info.LevelProperties.Add(lpi);
            //}

            return info;
        }

        public static MeasureInfo CreateMeasureInfo(Measure measure)
        {
            if (measure == null)
                return null;
            MeasureInfo info = new MeasureInfo();
            info.Caption = measure.Caption;
            info.Description = measure.Description;
            info.Name = measure.Name;
            info.UniqueName = measure.UniqueName;
            info.DisplayFolder = measure.DisplayFolder;
            info.Expression = measure.Expression;
            info.NumericPrecision = measure.NumericPrecision;
            info.NumericScale = measure.NumericScale;
            info.Units = measure.Units;

            // Информация о предках
            if (measure.ParentCube != null)
            {
                info.ParentCubeId = measure.ParentCube.Name;
            }

            // Свойства
            foreach (Property prop in measure.Properties)
            {
                //PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }

            return info;
        }

        public static KpiInfo CreateKpiInfo(Kpi kpi)
        {
            if (kpi == null)
                return null;
            KpiInfo info = new KpiInfo();
            info.Caption = kpi.Caption;
            info.Description = kpi.Description;
            info.Name = kpi.Name;
            info.DisplayFolder = kpi.DisplayFolder;
            info.StatusGraphic = kpi.StatusGraphic;
            info.TrendGraphic = kpi.TrendGraphic;

            // Информация о предках
            if (kpi.ParentCube != null)
            {
                info.ParentCubeId = kpi.ParentCube.Name;
            }
            if (kpi.ParentKpi != null)
            {
                info.ParentKpiId = kpi.ParentKpi.Name;
            }

            // Свойства
            foreach (Property prop in kpi.Properties)
            {
                //PropertyInfo pi = new PropertyInfo(prop.Name, prop.Type, prop.Value);
                PropertyInfo pi = new PropertyInfo(prop.Name, prop.Value);
                info.Properties.Add(pi);
            }

            return info;
        }
    }
}
