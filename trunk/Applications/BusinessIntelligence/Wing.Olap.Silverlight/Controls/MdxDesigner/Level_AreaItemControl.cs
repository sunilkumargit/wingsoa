/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.Olap.Core.Metadata;
using System.Windows.Media.Imaging;
using Wing.AgOlap.Controls.MdxDesigner.Wrappers;
using System.Collections.Generic;
using Wing.AgOlap.Controls.MemberChoice.Info;
using Wing.AgOlap.Controls.PivotGrid.Controls;
using System.Text;

namespace Wing.AgOlap.Controls.MdxDesigner
{
    public class Level_AreaItemControl : FilteredItemControl
    {
        public Level_AreaItemControl(Level_AreaItemWrapper wrapper)
            : this(wrapper, null)
        {

        }

        public Level_AreaItemControl(Level_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {
            PropertyInfo prop = null;
            // Текст подсказки
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format(Localization.Tooltip_Level, wrapper.Caption));

            prop = wrapper.GetCustomProperty(InfoBase.HIERARCHY_CAPTION);
            if (prop != null)
            {
                sb.AppendLine(String.Format(Localization.Tooltip_Hierarchy, prop.Value));
            }
            prop = wrapper.GetCustomProperty(InfoBase.DIMENSION_CAPTION);
            if (prop != null)
            {
                sb.AppendLine(String.Format(Localization.Tooltip_Dimension, prop.Value));
            }
            prop = wrapper.GetCustomProperty(InfoBase.CUBE_CAPTION);
            if (prop != null)
            {
                sb.AppendLine(String.Format(Localization.Tooltip_Cube, prop.Value));
            }

            String str = sb.ToString();
            str = str.TrimEnd('\n');
            str.TrimEnd('\r');

            // Подсказка
            ToolTip.Caption = wrapper.Caption;
            ToolTip.Text = str;
        }

        public Level_AreaItemWrapper Level
        {
            get { return Wrapper as Level_AreaItemWrapper; }
        }

        //public String FilterSet
        //{
        //    get
        //    {
        //        return Level.MembersFilter.FilterSet;
        //    }

        //    set
        //    {
        //        String str = value;
        //        str = str.Trim();
        //        if (str == "{}")
        //            str = String.Empty;

        //        Level.MembersFilter.FilterSet = str;
        //        IsFiltered = !String.IsNullOrEmpty(str);
        //        IsFiltered = IsFiltered | !String.IsNullOrEmpty(str);
        //    }
        //}
    }
}
