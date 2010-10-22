/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using Wing.Olap.Controls.General.ItemControls;

namespace Wing.Olap.Controls.MdxDesigner.Filters
{
    public class FilterFamilyItemControl : ItemControlBase
    {
        public FilterFamilyItemControl(FilterFamilyTypes type)
        {
            m_Type = type;
            Text = type.ToString();

            switch (type)
            {
                case FilterFamilyTypes.LabelFilter:
                    Icon = UriResources.Images.LabelFilter16;
                    Text = Localization.LabelFilter_Name;
                    break;
                case FilterFamilyTypes.ValueFilter:
                    Icon = UriResources.Images.ValueFilter16;
                    Text = Localization.ValueFilter_Name;
                    break;
                case FilterFamilyTypes.TopFilter:
                    Icon = UriResources.Images.TopFilter16;
                    Text = Localization.TopFilter_Name;
                    break;
            }
        }

        FilterFamilyTypes m_Type = FilterFamilyTypes.TopFilter;
        public FilterFamilyTypes Type
        {
            get {
                return m_Type;
            }
        }
    }
}
