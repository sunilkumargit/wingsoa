/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class TopFilterTargetItemControl : ItemControlBase
    {
        public TopFilterTargetItemControl(TopFilterTargetTypes type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            { 
                case TopFilterTargetTypes.Items:
                    Icon = UriResources.Images.Items16;
                    Text = Localization.Filter_Target_Items;
                    break;
                case TopFilterTargetTypes.Percent:
                    Icon = UriResources.Images.Percent16;
                    Text = Localization.Filter_Target_Percent;
                    break;
                case TopFilterTargetTypes.Sum:
                    Icon = UriResources.Images.Sum16;
                    Text = Localization.Filter_Target_Sum;
                    break;
            }
        }

        TopFilterTargetTypes m_Type = TopFilterTargetTypes.Items;
        public TopFilterTargetTypes Type
        {
            get {
                return m_Type;
            }
        }
    }
}
