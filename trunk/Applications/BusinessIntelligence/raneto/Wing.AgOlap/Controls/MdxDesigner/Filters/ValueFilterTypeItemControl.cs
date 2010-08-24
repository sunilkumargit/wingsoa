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