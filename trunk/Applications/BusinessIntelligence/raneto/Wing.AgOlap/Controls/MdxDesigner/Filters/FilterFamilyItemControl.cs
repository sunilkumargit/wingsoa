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
using Ranet.AgOlap.Controls.General.ItemControls;

namespace Ranet.AgOlap.Controls.MdxDesigner.Filters
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
