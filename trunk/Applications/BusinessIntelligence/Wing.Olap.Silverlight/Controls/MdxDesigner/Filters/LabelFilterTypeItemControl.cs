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

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class LabelFilterTypeItemControl : ItemControlBase
    {
        public LabelFilterTypeItemControl(LabelFilterTypes type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            {
                case LabelFilterTypes.Equal:
                    Icon = UriResources.Images.Equal16;
                    Text = Localization.ValueFilter_Equal;
                    break;
                case LabelFilterTypes.NotEqual:
                    Icon = UriResources.Images.NotEqual16;
                    Text = Localization.ValueFilter_NotEqual;
                    break;
                case LabelFilterTypes.Less:
                    Icon = UriResources.Images.Less16;
                    Text = Localization.ValueFilter_Less;
                    break;
                case LabelFilterTypes.LessOrEqual:
                    Icon = UriResources.Images.LessOrEqual16;
                    Text = Localization.ValueFilter_LessOrEqual;
                    break;
                case LabelFilterTypes.Greater:
                    Icon = UriResources.Images.Greater16;
                    Text = Localization.ValueFilter_Greater;
                    break;
                case LabelFilterTypes.GreaterOrEqual:
                    Icon = UriResources.Images.GreaterOrEqual16;
                    Text = Localization.ValueFilter_GreaterOrEqual;
                    break;
                case LabelFilterTypes.Between:
                    Icon = UriResources.Images.Between16;
                    Text = Localization.ValueFilter_Between;
                    break;
                case LabelFilterTypes.NotBetween:
                    Icon = UriResources.Images.NotBetween16;
                    Text = Localization.ValueFilter_NotBetween;
                    break;
                case LabelFilterTypes.BeginWith:
                    Icon = UriResources.Images.BeginWith16;
                    Text = Localization.LabelFilter_BeginWith;
                    break;
                case LabelFilterTypes.NotBeginWith:
                    Icon = UriResources.Images.NotBeginWith16;
                    Text = Localization.LabelFilter_NotBeginWith;
                    break;
                case LabelFilterTypes.EndWith:
                    Icon = UriResources.Images.EndWith16;
                    Text = Localization.LabelFilter_EndWith;
                    break;
                case LabelFilterTypes.NotEndWith:
                    Icon = UriResources.Images.NotEndWith16;
                    Text = Localization.LabelFilter_NotEndWith;
                    break;
                case LabelFilterTypes.Contain:
                    Icon = UriResources.Images.Contain16;
                    Text = Localization.LabelFilter_Contain;
                    break;
                case LabelFilterTypes.NotContain:
                    Icon = UriResources.Images.NotContain16;
                    Text = Localization.LabelFilter_NotContain;
                    break;
            }
        }

        LabelFilterTypes m_Type = LabelFilterTypes.Equal;
        public LabelFilterTypes Type
        {
            get {
                return m_Type;
            }
        }
    }
}