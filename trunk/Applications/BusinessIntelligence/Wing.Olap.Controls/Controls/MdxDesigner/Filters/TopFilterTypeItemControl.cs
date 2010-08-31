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
using Wing.AgOlap.Controls.General.ItemControls;

namespace Wing.AgOlap.Controls.MdxDesigner.Filters
{
    public class TopFilterTypeItemControl : ItemControlBase
    {
        public TopFilterTypeItemControl(TopFilterTypes type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            { 
                case TopFilterTypes.Top:
                    Icon = UriResources.Images.Top16;
                    Text = Localization.Filter_Type_Top;
                    break;
                case TopFilterTypes.Bottom:
                    Icon = UriResources.Images.Bottom16;
                    Text = Localization.Filter_Type_Bottom;
                    break;
            }
        }

        TopFilterTypes m_Type = TopFilterTypes.Top;
        public TopFilterTypes Type
        {
            get {
                return m_Type;
            }
        }
    }
}
