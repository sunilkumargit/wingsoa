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
using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.PivotGrid.Conditions
{
    public class CellConditionTypeItemControl : ItemControlBase
    {
        public CellConditionTypeItemControl(CellConditionType type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            {
                case CellConditionType.Equal:
                    Icon = UriResources.Images.Equal16;
                    Text = Localization.ValueFilter_Equal;
                    break;
                case CellConditionType.NotEqual:
                    Icon = UriResources.Images.NotEqual16;
                    Text = Localization.ValueFilter_NotEqual;
                    break;
                case CellConditionType.Less:
                    Icon = UriResources.Images.Less16;
                    Text = Localization.ValueFilter_Less;
                    break;
                case CellConditionType.LessOrEqual:
                    Icon = UriResources.Images.LessOrEqual16;
                    Text = Localization.ValueFilter_LessOrEqual;
                    break;
                case CellConditionType.Greater:
                    Icon = UriResources.Images.Greater16;
                    Text = Localization.ValueFilter_Greater;
                    break;
                case CellConditionType.GreaterOrEqual:
                    Icon = UriResources.Images.GreaterOrEqual16;
                    Text = Localization.ValueFilter_GreaterOrEqual;
                    break;
                case CellConditionType.Between:
                    Icon = UriResources.Images.Between16;
                    Text = Localization.ValueFilter_Between;
                    break;
                case CellConditionType.NotBetween:
                    Icon = UriResources.Images.NotBetween16;
                    Text = Localization.ValueFilter_NotBetween;
                    break;
                case CellConditionType.None:
                    Text = Localization.ComboBoxItem_None;
                    break;
            }
        }

        CellConditionType m_Type = CellConditionType.None;
        public CellConditionType Type
        {
            get
            {
                return m_Type;
            }
        }
    }
}