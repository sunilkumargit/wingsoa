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
using Ranet.Olap.Core.Metadata;
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.MdxDesigner.Wrappers;
using System.Collections.Generic;
using Ranet.AgOlap.Controls.MemberChoice.Info;
using Ranet.AgOlap.Controls.PivotGrid.Controls;
using System.Text;

namespace Ranet.AgOlap.Controls.MdxDesigner
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
