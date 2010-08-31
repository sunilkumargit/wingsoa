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
    public class ValueFilterTypeItemControl : ItemControlBase
    {
        public ValueFilterTypeItemControl(ValueFilterTypes type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            {
                case ValueFilterTypes.Equal:
                    Icon = UriResources.Images.Equal16;
                    Text = Localization.ValueFilter_Equal;
                    break;
                case ValueFilterTypes.NotEqual:
                    Icon = UriResources.Images.NotEqual16;
                    Text = Localization.ValueFilter_NotEqual;
                    break;
                case ValueFilterTypes.Less:
                    Icon = UriResources.Images.Less16;
                    Text = Localization.ValueFilter_Less;
                    break;
                case ValueFilterTypes.LessOrEqual:
                    Icon = UriResources.Images.LessOrEqual16;
                    Text = Localization.ValueFilter_LessOrEqual;
                    break;
                case ValueFilterTypes.Greater:
                    Icon = UriResources.Images.Greater16;
                    Text = Localization.ValueFilter_Greater;
                    break;
                case ValueFilterTypes.GreaterOrEqual:
                    Icon = UriResources.Images.GreaterOrEqual16;
                    Text = Localization.ValueFilter_GreaterOrEqual;
                    break;
                case ValueFilterTypes.Between:
                    Icon = UriResources.Images.Between16;
                    Text = Localization.ValueFilter_Between;
                    break;
                case ValueFilterTypes.NotBetween:
                    Icon = UriResources.Images.NotBetween16;
                    Text = Localization.ValueFilter_NotBetween;
                    break;
            }
        }

        ValueFilterTypes m_Type = ValueFilterTypes.Equal;
        public ValueFilterTypes Type
        {
            get {
                return m_Type;
            }
        }
    }
}