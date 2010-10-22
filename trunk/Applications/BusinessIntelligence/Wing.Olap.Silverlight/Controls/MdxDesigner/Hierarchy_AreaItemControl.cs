/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Wing.Olap.Controls.MdxDesigner.Wrappers;
using Wing.Olap.Core.Metadata;

namespace Wing.Olap.Controls.MdxDesigner
{
    public class Hierarchy_AreaItemControl : FilteredItemControl
    {
        public Hierarchy_AreaItemControl(Hierarchy_AreaItemWrapper wrapper)
            : this(wrapper, null)
        {

        }

        public Hierarchy_AreaItemControl(Hierarchy_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {
            PropertyInfo prop = null;
            // Текст подсказки
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format(Localization.Tooltip_Hierarchy, wrapper.Caption));

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

        public Hierarchy_AreaItemWrapper Hierarchy
        {
            get { return Wrapper as Hierarchy_AreaItemWrapper; }
        }

        //public String FilterSet
        //{
        //    get
        //    {
        //        return Hierarchy.MembersFilter.FilterSet;
        //    }
        //    set
        //    {
        //        String str = value;
        //        str = str.Trim();
        //        if (str == "{}")
        //            str = String.Empty;

        //        Hierarchy.MembersFilter.FilterSet = str;
        //        IsFiltered = IsFiltered | !String.IsNullOrEmpty(str);
        //    }
        //}
    }
}
