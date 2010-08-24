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
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace Ranet.AgOlap.Controls
{
    internal static class UriResources
    {
        private static Dictionary<string, BitmapImage> m_Cashe = null;
        public static Dictionary<string, BitmapImage> Cashe
        {
            get{
                if (m_Cashe == null)
                    m_Cashe = new Dictionary<string, BitmapImage>();
                return m_Cashe;
            }
        }

        public static string GetResourceString(string assemblyName, string resourcePath)
        {
            if (assemblyName == "Galaktika.BI.ImageLibrary")
            {
                string[] parts = resourcePath.Split('.');
                string resultPath = string.Empty;
                foreach (var s in parts)
                {
                    if (s.ToLower().Equals("image") || s.ToLower().Equals("indicators") || s.ToLower().Equals("x16"))
                        resultPath += s + "/";
                    else
                        resultPath += s + ".";
                }
                return String.Format("/{0};component/{1}", assemblyName, resultPath.Remove(resultPath.Length - 1));
            }
            else
            {
                return String.Format("/{0};component/{1}", assemblyName, resourcePath);
            }
        }

        public static BitmapImage GetImage(string resName)
        {
            if(Cashe.ContainsKey(resName))
            {
                return Cashe[resName];
            }
            else
            {
                BitmapImage bmp = new BitmapImage(new Uri(resName, UriKind.Relative));
               if (bmp != null)
               {
                   Cashe.Add(resName, bmp);
               }
               return bmp;
            }
        }

        public class Images
        {
            #region PivotGrid
            const string TREE_PLUS_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/Plus.png";
            const string TREE_MINUS_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/Minus.png";
            const string REFRESH_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/Refresh.png";
            const string FORWARD_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/Forward.png";
            const string BACK_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/Back.png";
            const string TOBEGIN_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/ToBegin.png";
            const string TOEND_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/ToEnd.png";

            const string CANCEL_EDIT_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/CancelEdit.png";
            const string CONFIRM_EDIT_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/ConfirmEdit.png";
            const string USE_CHANGES_CASHE_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/UseChangesCache.png";
            const string EDIT_CELLS_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/EditCells.png";
            const string RESTORE_SIZE_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/RestoreDefaultSize.png";
            const string HIDE_EMPTY_COLUMNS_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/HideEmptyColumns.png";
            const string HIDE_EMPTY_ROWS_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/HideEmptyRows.png";
            const string TO_FOCUSED_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/ToFocused.png";
            const string ROTATE_AXES_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/RotateAxes.png";
            const string ACTION_NODE_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/ActionNode.png";
            const string HIDE_HINT_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/CloseHint.png";
            const string CELL_CONDITIONS_DESIGNER_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/CellConditionsDesigner.png";
            const string CELL_STYLE_16 = "/Ranet.AgOlap;component/Controls/PivotGrid/Images/CellStyle.png";

            public static BitmapImage CellStyle16
            {
                get
                {
                    return UriResources.GetImage(CELL_STYLE_16);
                }
            }

            public static BitmapImage CellConditionsDesigner16
            {
                get
                {
                    return UriResources.GetImage(CELL_CONDITIONS_DESIGNER_16);
                }
            }

            public static BitmapImage HideHint16
            {
                get
                {
                    return UriResources.GetImage(HIDE_HINT_16);
                }
            }

            public static BitmapImage RotateAxes16
            {
                get
                {
                    return UriResources.GetImage(ROTATE_AXES_16);
                }
            }

            public static BitmapImage ToFocused16
            {
                get
                {
                    return UriResources.GetImage(TO_FOCUSED_16);
                }
            }

            public static BitmapImage RestoreSize16
            {
                get
                {
                    return UriResources.GetImage(RESTORE_SIZE_16);
                }
            }

            public static BitmapImage ToBegin16
            {
                get
                {
                    return UriResources.GetImage(TOBEGIN_16);
                }
            }

            public static BitmapImage ToEnd16
            {
                get
                {
                    return UriResources.GetImage(TOEND_16);
                }
            }

            public static BitmapImage ActionNode16
            {
                get
                {
                    return UriResources.GetImage(ACTION_NODE_16);
                }
            }

            public static BitmapImage CancelEdit16
            {
                get
                {
                    return UriResources.GetImage(CANCEL_EDIT_16);
                }
            }

            public static BitmapImage ConfirmEdit16
            {
                get
                {
                    return UriResources.GetImage(CONFIRM_EDIT_16);
                }
            }

            public static BitmapImage UseChangesCache16
            {
                get
                {
                    return UriResources.GetImage(USE_CHANGES_CASHE_16);
                }
            }

            public static BitmapImage EditCells16
            {
                get
                {
                    return UriResources.GetImage(EDIT_CELLS_16);
                }
            }

            public static BitmapImage HideEmptyColumns16
            {
                get
                {
                    return UriResources.GetImage(HIDE_EMPTY_COLUMNS_16);
                }
            }

            public static BitmapImage HideEmptyRows16
            {
                get
                {
                    return UriResources.GetImage(HIDE_EMPTY_ROWS_16);
                }
            }

            public static BitmapImage TreePlus16
            {
                get
                {
                    return UriResources.GetImage(TREE_PLUS_16);
                }
            }

            public static BitmapImage TreeMinus16
            {
                get
                {
                    return UriResources.GetImage(TREE_MINUS_16);
                }
            }

            public static BitmapImage Refresh16
            {
                get
                {
                    return UriResources.GetImage(REFRESH_16);
                }
            }

            public static BitmapImage Forward16
            {
                get
                {
                    return UriResources.GetImage(FORWARD_16);
                }
            }

            public static BitmapImage Back16
            {
                get
                {
                    return UriResources.GetImage(BACK_16);
                }
            }
            #endregion PivotGrid

            #region MemberChoiceControl

            const string HAS_SELECTED_CHILDREN_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/HasSelectedChildren.png";
            const string LOAD_ALL_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/LoadAll.png";
            const string LOAD_NEXT_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/LoadNext.png";
            const string NOT_SELECTED_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/NotSelected.png";
            const string SELECTED_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/Selected.png";
            const string SELECTED_CHILDREN_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/SelectedChildren.png";
            const string HAS_EXCLUDED_CHILDREN_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/States/HasExcludedChildren.png";

            const string CLEAR_CHOICE_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/ToolBar/ClearChoice.png";

            const string MEMBER_PROPERTY_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MemberProperty.png";
            const string LEVEL_PROPERTY_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/LevelProperty.png";

            const string OR_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcgroupor.png";
            const string AND_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcgroupand.png";
            const string CLEAR_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcallremove.png";
            const string ADD_OPERAND_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcgroupaddcondition.png";
            const string ADD_OPERATION_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcgroupaddgroup.png";
            const string REMOVE_GROUP_16 = "/Ranet.AgOlap;component/Controls/MemberChoice/Images/Fcgroupremove.png";

            public static BitmapImage RemoveGroup16
            {
                get
                {
                    return UriResources.GetImage(REMOVE_GROUP_16);
                }
            }

            public static BitmapImage AddOperation16
            {
                get
                {
                    return UriResources.GetImage(ADD_OPERATION_16);
                }
            }

            public static BitmapImage AddOperand16
            {
                get
                {
                    return UriResources.GetImage(ADD_OPERAND_16);
                }
            }
            
            public static BitmapImage Clear16
            {
                get
                {
                    return UriResources.GetImage(CLEAR_16);
                }
            }

            public static BitmapImage And16
            {
                get
                {
                    return UriResources.GetImage(AND_16);
                }
            }

            public static BitmapImage Or16
            {
                get
                {
                    return UriResources.GetImage(OR_16);
                }
            }

            public static BitmapImage LevelProperty16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_PROPERTY_16);
                }
            }

            public static BitmapImage MemberProperty16
            {
                get
                {
                    return UriResources.GetImage(MEMBER_PROPERTY_16);
                }
            }

            public static BitmapImage ClearChoice16
            {
                get
                {
                    return UriResources.GetImage(CLEAR_CHOICE_16);
                }
            }

            public static BitmapImage HasExcludedChildren16
            {
                get
                {
                    return UriResources.GetImage(HAS_EXCLUDED_CHILDREN_16);
                }
            }

            public static BitmapImage SelectedChildren16
            {
                get
                {
                    return UriResources.GetImage(SELECTED_CHILDREN_16);
                }
            }

            public static BitmapImage Selected16
            {
                get
                {
                    return UriResources.GetImage(SELECTED_16);
                }
            }

            public static BitmapImage NotSelected16
            {
                get
                {
                    return UriResources.GetImage(NOT_SELECTED_16);
                }
            }

            public static BitmapImage HasSelectedChildren16
            {
                get
                {
                    return UriResources.GetImage(HAS_SELECTED_CHILDREN_16);
                }
            }

            public static BitmapImage LoadAll16
            {
                get
                {
                    return UriResources.GetImage(LOAD_ALL_16);
                }
            }

            public static BitmapImage LoadNext16
            {
                get
                {
                    return UriResources.GetImage(LOAD_NEXT_16);
                }
            }

            #endregion MemberChoiceControl

            #region MetaData

            const string CUBE_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Cube.png";
            const string MEASURE_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Measures.png";
            const string MEMBER_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Member.png";
            const string MEMBER_SMALL_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MemberSmall.png";
            const string DIMENSION_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Dimension.png";
            const string NAMED_SET_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/NamedSet.png";
            const string MEASURE_CALC_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MeasuresCalc.png";
            const string KPI_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/KPI.png";
            const string ATTRIBUTE_HIERARCHY_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/AttributeHierarchy.png";
            const string PARENT_CHILD_HIERARCHY_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/ParentChildHierarchy.png";
            const string USER_HIERARCHY_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/UserHierarchy.png";
            const string LEVEL_ALL_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.00.png";
            const string LEVEL_01_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.01.png";
            const string LEVEL_02_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.02.png";
            const string LEVEL_03_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.03.png";
            const string LEVEL_04_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.04.png";
            const string LEVEL_05_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.05.png";
            const string LEVEL_06_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.06.png";
            const string LEVEL_07_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.07.png";
            const string LEVEL_08_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.08.png";
            const string LEVEL_09_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/Levels/item.09.png";

            const string FOLDER_16 = "/Ranet.AgOlap;component/Controls/Images/Folder.png";
            const string FOLDER_OPEN_16 = "/Ranet.AgOlap;component/Controls/Images/FolderOpen.png";
            const string MEASURES_FOLDER_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MeasuresFolder.png";
            const string MEASURES_FOLDER_OPEN_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MeasuresFolderOpen.png";
            const string MEASURE_GROUP_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/MeasureGroup.png";
            const string DATA_MEMBER_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/DataMember.png";
            const string UNKNOWN_MEMBER_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/UnknownMember.png";
            const string CUSTOM_MEASURE_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/CustomMeasure.png";
            const string CUSTOM_NAMED_SET_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/CustomNamedSet.png";
            const string CUSTOM_CALCULATIONS_16 = "/Ranet.AgOlap;component/Controls/Images/OLAP/CustomCalculations.png";

            public static BitmapImage CustomCalculations16
            {
                get
                {
                    return UriResources.GetImage(CUSTOM_CALCULATIONS_16);
                }
            }

            public static BitmapImage CustomMeasure16
            {
                get
                {
                    return UriResources.GetImage(CUSTOM_MEASURE_16);
                }
            }

            public static BitmapImage CustomNamedSet16
            {
                get
                {
                    return UriResources.GetImage(CUSTOM_NAMED_SET_16);
                }
            }

            public static BitmapImage UnknownMember16
            {
                get
                {
                    return UriResources.GetImage(UNKNOWN_MEMBER_16);
                }
            }

            public static BitmapImage NamedSet16
            {
                get
                {
                    return UriResources.GetImage(NAMED_SET_16);
                }
            }

            public static BitmapImage DataMember16
            {
                get
                {
                    return UriResources.GetImage(DATA_MEMBER_16);
                }
            }

            public static BitmapImage MeasureGroup16
            {
                get
                {
                    return UriResources.GetImage(MEASURE_GROUP_16);
                }
            }

            public static BitmapImage MemberSmall16
            {
                get
                {
                    return UriResources.GetImage(MEMBER_SMALL_16);
                }
            }

            public static BitmapImage Member16
            {
                get
                {
                    return UriResources.GetImage(MEMBER_16);
                }
            }

            public static BitmapImage MeasuresFolder16
            {
                get
                {
                    return UriResources.GetImage(MEASURES_FOLDER_16);
                }
            }

            public static BitmapImage MeasuresFolderOpen16
            {
                get
                {
                    return UriResources.GetImage(MEASURES_FOLDER_OPEN_16);
                }
            }

            public static BitmapImage Folder16
            {
                get
                {
                    return UriResources.GetImage(FOLDER_16);
                }
            }

            public static BitmapImage FolderOpen16
            {
                get
                {
                    return UriResources.GetImage(FOLDER_OPEN_16);
                }
            }

            public static BitmapImage Measure16
            {
                get
                {
                    return UriResources.GetImage(MEASURE_16);
                }
            }

            public static BitmapImage MeasureCalc16
            {
                get
                {
                    return UriResources.GetImage(MEASURE_CALC_16);
                }
            }

            public static BitmapImage Kpi16
            {
                get
                {
                    return UriResources.GetImage(KPI_16);
                }
            }

            public static BitmapImage Cube16
            {
                get
                {
                    return UriResources.GetImage(CUBE_16);
                }
            }

            public static BitmapImage Dimension16
            {
                get
                {
                    return UriResources.GetImage(DIMENSION_16);
                }
            }

            public static BitmapImage AttributeHierarchy16
            {
                get
                {
                    return UriResources.GetImage(ATTRIBUTE_HIERARCHY_16);
                }
            }

            public static BitmapImage ParentChildHierarchy16
            {
                get
                {
                    return UriResources.GetImage(PARENT_CHILD_HIERARCHY_16);
                }
            }

            public static BitmapImage UserHierarchy16
            {
                get
                {
                    return UriResources.GetImage(USER_HIERARCHY_16);
                }
            }

            public static BitmapImage Level_All_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_ALL_16);
                }
            }

            public static BitmapImage Level_01_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_01_16);
                }
            }

            public static BitmapImage Level_02_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_02_16);
                }
            }

            public static BitmapImage Level_03_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_03_16);
                }
            }

            public static BitmapImage Level_04_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_04_16);
                }
            }

            public static BitmapImage Level_05_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_05_16);
                }
            }

            public static BitmapImage Level_06_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_06_16);
                }
            }

            public static BitmapImage Level_07_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_07_16);
                }
            }

            public static BitmapImage Level_08_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_08_16);
                }
            }

            public static BitmapImage Level_09_16
            {
                get
                {
                    return UriResources.GetImage(LEVEL_09_16);
                }
            }

            #endregion

            #region Logic

            const string ADD_16 = "/Ranet.AgOlap;component/Controls/Images/Fcadd.png";
            const string ADD_HOT_16 = "/Ranet.AgOlap;component/Controls/Images/Fcaddhot.png";
            const string REMOVE_16 = "/Ranet.AgOlap;component/Controls/Images/Fcremove.png";
            const string REMOVE_HOT_16 = "/Ranet.AgOlap;component/Controls/Images/Fcremovehot.png";

            public static BitmapImage Remove16
            {
                get
                {
                    return UriResources.GetImage(REMOVE_16);
                }
            }

            public static BitmapImage RemoveHot16
            {
                get
                {
                    return UriResources.GetImage(REMOVE_HOT_16);
                }
            }

            public static BitmapImage Add16
            {
                get
                {
                    return UriResources.GetImage(ADD_16);
                }
            }

            public static BitmapImage AddHot16
            {
                get
                {
                    return UriResources.GetImage(ADD_HOT_16);
                }
            }

            #endregion Logic

            #region OLAP

            const string MDX_16 = "/Ranet.AgOlap;component/Controls/Images/MDX.png";

            public static BitmapImage Mdx16
            {
                get
                {
                    return UriResources.GetImage(MDX_16);
                }
            }

            #endregion OLAP

            #region MDX-Designer
            const string FILTERS_AREA_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/FiltersArea.png";
            const string ROWS_AREA_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/RowsArea.png";
            const string COLUMNS_AREA_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/ColumnsArea.png";
            const string DATA_AREA_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/DataArea.png";
            const string MINI_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/MiniFilter.png";
            const string CHANGE_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/ChangeFilter.png";
            const string CHECKED_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Checked.png";
            const string CANCEL_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/CancelFilter.png";
            const string TOP_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Top.png";
            const string BOTTOM_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Bottom.png";
            const string ITEMS_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Items.png";
            const string PERCENT_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Percent.png";
            const string SUM_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/Sum.png";
            const string LABEL_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/LabelFilter.png";
            const string VALUE_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/ValueFilter.png";
            const string TOP_FILTER_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/TopFilter.png";
            const string ADD_MEASURE_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/AddMeasure.png";
            const string ADD_NAMED_SET_16 = "/Ranet.AgOlap;component/Controls/MdxDesigner/Images/AddNamedSet.png";

            public static BitmapImage AddMeasure16
            {
                get
                {
                    return UriResources.GetImage(ADD_MEASURE_16);
                }
            }

            public static BitmapImage AddNamedSet16
            {
                get
                {
                    return UriResources.GetImage(ADD_NAMED_SET_16);
                }
            }

            public static BitmapImage TopFilter16
            {
                get
                {
                    return UriResources.GetImage(TOP_FILTER_16);
                }
            }

            public static BitmapImage ValueFilter16
            {
                get
                {
                    return UriResources.GetImage(VALUE_FILTER_16);
                }
            }

            public static BitmapImage LabelFilter16
            {
                get
                {
                    return UriResources.GetImage(LABEL_FILTER_16);
                }
            }

            public static BitmapImage Sum16
            {
                get
                {
                    return UriResources.GetImage(SUM_16);
                }
            }

            public static BitmapImage Percent16
            {
                get
                {
                    return UriResources.GetImage(PERCENT_16);
                }
            }

            public static BitmapImage Items16
            {
                get
                {
                    return UriResources.GetImage(ITEMS_16);
                }
            }

            public static BitmapImage Bottom16
            {
                get
                {
                    return UriResources.GetImage(BOTTOM_16);
                }
            }

            public static BitmapImage Top16
            {
                get
                {
                    return UriResources.GetImage(TOP_16);
                }
            }

            public static BitmapImage CancelFilter16
            {
                get
                {
                    return UriResources.GetImage(CANCEL_FILTER_16);
                }
            }

            public static BitmapImage Checked16
            {
                get
                {
                    return UriResources.GetImage(CHECKED_16);
                }
            }

            public static BitmapImage ChangeFilter16
            {
                get
                {
                    return UriResources.GetImage(CHANGE_FILTER_16);
                }
            }

            public static BitmapImage MiniFilter16
            {
                get
                {
                    return UriResources.GetImage(MINI_FILTER_16);
                }
            }

            public static BitmapImage FiltersArea16
            {
                get
                {
                    return UriResources.GetImage(FILTERS_AREA_16);
                }
            }

            public static BitmapImage RowsArea16
            {
                get
                {
                    return UriResources.GetImage(ROWS_AREA_16);
                }
            }

            public static BitmapImage ColumnsArea16
            {
                get
                {
                    return UriResources.GetImage(COLUMNS_AREA_16);
                }
            }

            public static BitmapImage DataArea16
            {
                get
                {
                    return UriResources.GetImage(DATA_AREA_16);
                }
            }

            #endregion MDX-Designer

            #region Logic
            const string EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopequal.png";
            const string NOT_EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopnotequal.png";
            const string GREATER_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopgreater.png";
            const string GREATER_OR_EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopgreaterorequal.png";
            const string LESS_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopless.png";
            const string LESS_OR_EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcoplessorequal.png";
            const string BETWEEN_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopbetween.png";
            const string NOT_BETWEEN_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopnotbetween.png";
            const string BEGIN_WITH_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopbegin.png";
            const string END_WITH_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopend.png";
            const string CONTAIN_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopcontain.png";
            const string NOT_CONTAIN_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopnotcontain.png";
            const string NOT_BEGIN_WITH_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopbeginnot.png";
            const string NOT_END_WITH_16 = "/Ranet.AgOlap;component/Controls/Images/Logic/Fcopendnot.png";

            public static BitmapImage NotBeginWith16
            {
                get
                {
                    return UriResources.GetImage(NOT_BEGIN_WITH_16);
                }
            }

            public static BitmapImage NotEndWith16
            {
                get
                {
                    return UriResources.GetImage(NOT_END_WITH_16);
                }
            }

            public static BitmapImage NotContain16
            {
                get
                {
                    return UriResources.GetImage(NOT_CONTAIN_16);
                }
            }

            public static BitmapImage Contain16
            {
                get
                {
                    return UriResources.GetImage(CONTAIN_16);
                }
            }

            public static BitmapImage EndWith16
            {
                get
                {
                    return UriResources.GetImage(END_WITH_16);
                }
            }

            public static BitmapImage BeginWith16
            {
                get
                {
                    return UriResources.GetImage(BEGIN_WITH_16);
                }
            }

            public static BitmapImage NotBetween16
            {
                get
                {
                    return UriResources.GetImage(NOT_BETWEEN_16);
                }
            }

            public static BitmapImage Between16
            {
                get
                {
                    return UriResources.GetImage(BETWEEN_16);
                }
            }

            public static BitmapImage LessOrEqual16
            {
                get
                {
                    return UriResources.GetImage(LESS_OR_EQUAL_16);
                }
            }

            public static BitmapImage Less16
            {
                get
                {
                    return UriResources.GetImage(LESS_16);
                }
            }

            public static BitmapImage GreaterOrEqual16
            {
                get
                {
                    return UriResources.GetImage(GREATER_OR_EQUAL_16);
                }
            }

            public static BitmapImage Greater16
            {
                get
                {
                    return UriResources.GetImage(GREATER_16);
                }
            }

            public static BitmapImage NotEqual16
            {
                get
                {
                    return UriResources.GetImage(NOT_EQUAL_16);
                }
            }
            
            public static BitmapImage Equal16
            {
                get
                {
                    return UriResources.GetImage(EQUAL_16);
                }
            }
            #endregion Logic

            #region Calc
            const string CALC_DIVIDE_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Divide.png";
            const string CALC_EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Equal.png";
            const string CALC_FUNCTION_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Function.png";
            const string CALC_FUNCTION_DIVIDE_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionDivide.png";
            const string CALC_FUNCTION_EQUAL_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionEqual.png";
            const string CALC_FUNCTION_MINUS_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionMinus.png";
            const string CALC_FUNCTION_MULTIPLY_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionMultiply.png";
            const string CALC_FUNCTION_PERCENT_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionPercent.png";
            const string CALC_FUNCTION_PLUS_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionPlus.png";
            const string CALC_FUNCTION_TILDE_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/FunctionTilde.png";
            const string CALC_MINUS_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Minus.png";
            const string CALC_PLUS_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Plus.png";
            const string CALC_MULTIPLY_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Multiply.png";
            const string CALC_PERCENT_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Percent.png";
            const string CALC_TILDE_16 = "/Ranet.AgOlap;component/Controls/Images/Calc/Tilde.png";

            public static BitmapImage CalcTilde16
            {
                get
                {
                    return UriResources.GetImage(CALC_TILDE_16);
                }
            }

            public static BitmapImage CalcPercent16
            {
                get
                {
                    return UriResources.GetImage(CALC_PERCENT_16);
                }
            }

            public static BitmapImage CalcMultiply16
            {
                get
                {
                    return UriResources.GetImage(CALC_MULTIPLY_16);
                }
            }

            public static BitmapImage CalcPlus16
            {
                get
                {
                    return UriResources.GetImage(CALC_PLUS_16);
                }
            }

            public static BitmapImage CalcMinus16
            {
                get
                {
                    return UriResources.GetImage(CALC_MINUS_16);
                }
            }

            public static BitmapImage CalcFunctionTilde16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_TILDE_16);
                }
            }

            public static BitmapImage CalcFunctionPlus16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_PLUS_16);
                }
            }

            public static BitmapImage CalcFunctionPercent16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_PERCENT_16);
                }
            }

            public static BitmapImage CalcFunctionMultiply16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_MULTIPLY_16);
                }
            }

            public static BitmapImage CalcFunctionMinus16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_MINUS_16);
                }
            }

            public static BitmapImage CalcFunctionEqual16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_EQUAL_16);
                }
            }

            public static BitmapImage CalcFunctionDivide16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_DIVIDE_16);
                }
            }

            public static BitmapImage CalcFunction16
            {
                get
                {
                    return UriResources.GetImage(CALC_FUNCTION_16);
                }
            }

            public static BitmapImage CalcEqual16
            {
                get
                {
                    return UriResources.GetImage(CALC_EQUAL_16);
                }
            }

            public static BitmapImage CalcDivide16
            {
                get
                {
                    return UriResources.GetImage(CALC_DIVIDE_16);
                }
            }
            #endregion Calc

            const string COPY_16 = "/Ranet.AgOlap;component/Controls/Images/Copy.png";
            const string PASTE_16 = "/Ranet.AgOlap;component/Controls/Images/Paste.png";
            const string CUT_16 = "/Ranet.AgOlap;component/Controls/Images/Cut.png";
            const string DELETE_16 = "/Ranet.AgOlap;component/Controls/Images/Delete.png";
            const string EDIT_16 = "/Ranet.AgOlap;component/Controls/Images/Edit.png";
            const string ERROR_16 = "/Ranet.AgOlap;component/Controls/Images/Error.png";
            const string INFO_16 = "/Ranet.AgOlap;component/Controls/Images/Info.png";
            const string WARNING_16 = "/Ranet.AgOlap;component/Controls/Images/Warning.png";
            const string UP_16 = "/Ranet.AgOlap;component/Controls/Images/B_Up.png";
            const string DOWN_16 = "/Ranet.AgOlap;component/Controls/Images/B_Down.png";
            const string LEFT_16 = "/Ranet.AgOlap;component/Controls/Images/B_Left.png";
            const string RIGHT_16 = "/Ranet.AgOlap;component/Controls/Images/B_Right.png";
            const string PIVOT_GRID_16 = "/Ranet.AgOlap;component/Controls/Images/PivotTable.png";
            const string RUN_16 = "/Ranet.AgOlap;component/Controls/Images/Run.png";
            const string FORWARD_DOUBLE_16 = "/Ranet.AgOlap;component/Controls/Images/Forward_double.png";
            const string BACK_DOUBLE_16 = "/Ranet.AgOlap;component/Controls/Images/Back_double.png";

            const string MENU_16 = "/Ranet.AgOlap;component/Controls/Images/Menu.png";
            const string AUTO_RUN_16 = "/Ranet.AgOlap;component/Controls/Images/AutoRun.png";
            const string EXPORT_TO_EXCEL_16 = "/Ranet.AgOlap;component/Controls/Images/ExportToEXCEL.png";

            const string FILE_EXPORT_16 = "/Ranet.AgOlap;component/Controls/Images/FileExport.png";
            const string FILE_IMPORT_16 = "/Ranet.AgOlap;component/Controls/Images/FileImport.png";
            const string MEMBER_CHOICE_16 = "/Ranet.AgOlap;component/Controls/Images/MemberChoiceControl.png";

            const string FILE_EXTENSION_16 = "/Ranet.AgOlap;component/Controls/Images/FileExtension.png";
            const string FILE_ERROR_16 = "/Ranet.AgOlap;component/Controls/Images/FileError.png";

            const string REMOVE_CROSS_16 = "/Ranet.AgOlap;component/Controls/Images/Remove.png";
            const string REMOVE_ALL_CROSS_16 = "/Ranet.AgOlap;component/Controls/Images/RemoveAll.png";

            const string ERROR_SMALL_16 = "/Ranet.AgOlap;component/Controls/Images/ErrorSmall.png";
            const string NEW_16 = "/Ranet.AgOlap;component/Controls/Images/New.png";

            const string SORT_AZ_16 = "/Ranet.AgOlap;component/Controls/Images/SortAZ.png";
            const string SORT_ZA_16 = "/Ranet.AgOlap;component/Controls/Images/SortZA.png";
            const string SORT_NONE_16 = "/Ranet.AgOlap;component/Controls/Images/SortNone.png";

            const string CONTEXT_MENU_CHECKED_16 = "/Ranet.AgOlap;component/Controls/Images/ContextMenuItemChecked.png";
            const string CONTEXT_MENU_UNCHECKED_16 = "/Ranet.AgOlap;component/Controls/Images/ContextMenuItemUnChecked.png";
            
            const string SUB_MENU_16 = "/Ranet.AgOlap;component/Controls/Images/SubMenu.png";

            const string COLUMN_SORT_ASC_16 = "/Ranet.AgOlap;component/Controls/Images/Column_SortAsc.png";
            const string COLUMN_SORT_DESC_16 = "/Ranet.AgOlap;component/Controls/Images/Column_SortDesc.png";
            const string ROW_SORT_ASC_16 = "/Ranet.AgOlap;component/Controls/Images/Row_SortAsc.png";
            const string ROW_SORT_DESC_16 = "/Ranet.AgOlap;component/Controls/Images/Row_SortDesc.png";

            public static BitmapImage RowSortDesc16
            {
                get
                {
                    return UriResources.GetImage(ROW_SORT_DESC_16);
                }
            }

            public static BitmapImage RowSortAsc16
            {
                get
                {
                    return UriResources.GetImage(ROW_SORT_ASC_16);
                }
            }

            public static BitmapImage ColumnSortDesc16
            {
                get
                {
                    return UriResources.GetImage(COLUMN_SORT_DESC_16);
                }
            }

            public static BitmapImage ColumnSortAsc16
            {
                get
                {
                    return UriResources.GetImage(COLUMN_SORT_ASC_16);
                }
            }

            public static BitmapImage SubMenu16
            {
                get
                {
                    return UriResources.GetImage(SUB_MENU_16);
                }
            }

            public static BitmapImage ContexMenuUnChecked16
            {
                get
                {
                    return UriResources.GetImage(CONTEXT_MENU_UNCHECKED_16);
                }
            }

            public static BitmapImage ContexMenuChecked16
            {
                get
                {
                    return UriResources.GetImage(CONTEXT_MENU_CHECKED_16);
                }
            }

            public static BitmapImage SortZA16
            {
                get
                {
                    return UriResources.GetImage(SORT_ZA_16);
                }
            }

            public static BitmapImage SortNone16
            {
                get
                {
                    return UriResources.GetImage(SORT_NONE_16);
                }
            }

            public static BitmapImage SortAZ16
            {
                get
                {
                    return UriResources.GetImage(SORT_AZ_16);
                }
            }

            public static BitmapImage New16
            {
                get
                {
                    return UriResources.GetImage(NEW_16);
                }
            }

            public static BitmapImage ErrorSmall16
            {
                get
                {
                    return UriResources.GetImage(ERROR_SMALL_16);
                }
            }

            public static BitmapImage RemoveAllCross16
            {
                get
                {
                    return UriResources.GetImage(REMOVE_ALL_CROSS_16);
                }
            }

            public static BitmapImage RemoveCross16
            {
                get
                {
                    return UriResources.GetImage(REMOVE_CROSS_16);
                }
            }

            public static BitmapImage FileError16
            {
                get
                {
                    return UriResources.GetImage(FILE_ERROR_16);
                }
            }
            
            public static BitmapImage FileExtension16
            {
                get
                {
                    return UriResources.GetImage(FILE_EXTENSION_16);
                }
            }

            public static BitmapImage MemberChoice16
            {
                get
                {
                    return UriResources.GetImage(MEMBER_CHOICE_16);
                }
            }

            public static BitmapImage FileExport16
            {
                get
                {
                    return UriResources.GetImage(FILE_EXPORT_16);
                }
            }

            public static BitmapImage FileImport16
            {
                get
                {
                    return UriResources.GetImage(FILE_IMPORT_16);
                }
            }

            public static BitmapImage Left16
            {
                get
                {
                    return UriResources.GetImage(LEFT_16);
                }
            }

            public static BitmapImage Right16
            {
                get
                {
                    return UriResources.GetImage(RIGHT_16);
                }
            }

            public static BitmapImage ExportToExcel16
            {
                get
                {
                    return UriResources.GetImage(EXPORT_TO_EXCEL_16);
                }
            }

            public static BitmapImage AutoRun16
            {
                get
                {
                    return UriResources.GetImage(AUTO_RUN_16);
                }
            }

            public static BitmapImage Menu16
            {
                get
                {
                    return UriResources.GetImage(MENU_16);
                }
            }

            public static BitmapImage ForwardDouble16
            {
                get
                {
                    return UriResources.GetImage(FORWARD_DOUBLE_16);
                }
            }

            public static BitmapImage BackDouble16
            {
                get
                {
                    return UriResources.GetImage(BACK_DOUBLE_16);
                }
            }

            public static BitmapImage Run16
            {
                get
                {
                    return UriResources.GetImage(RUN_16);
                }
            }

            public static BitmapImage PivotGrid16
            {
                get
                {
                    return UriResources.GetImage(PIVOT_GRID_16);
                }
            }

            public static BitmapImage Up16
            {
                get
                {
                    return UriResources.GetImage(UP_16);
                }
            }

            public static BitmapImage Down16
            {
                get
                {
                    return UriResources.GetImage(DOWN_16);
                }
            }

            public static BitmapImage Error16
            {
                get
                {
                    return UriResources.GetImage(ERROR_16);
                }
            }

            public static BitmapImage Info16
            {
                get
                {
                    return UriResources.GetImage(INFO_16);
                }
            }

            public static BitmapImage Warning16
            {
                get
                {
                    return UriResources.GetImage(WARNING_16);
                }
            }

            public static BitmapImage Paste16
            {
                get
                {
                    return UriResources.GetImage(PASTE_16);
                }
            }

            public static BitmapImage Copy16
            {
                get
                {
                    return UriResources.GetImage(COPY_16);
                }
            }

            public static BitmapImage Cut16
            {
                get
                {
                    return UriResources.GetImage(CUT_16);
                }
            }

            public static BitmapImage Delete16
            {
                get
                {
                    return UriResources.GetImage(DELETE_16);
                }
            }

            public static BitmapImage Edit16
            {
                get
                {
                    return UriResources.GetImage(EDIT_16);
                }
            }
        }
    }
}
