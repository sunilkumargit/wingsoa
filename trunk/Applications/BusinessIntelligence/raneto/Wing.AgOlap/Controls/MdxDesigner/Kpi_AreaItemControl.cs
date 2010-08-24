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

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    public enum KpiControlType
    { 
        Goal,
        Value,
        Trend,
        Status
    }

    public class Kpi_AreaItemControl : InfoItemControl
    {
        public readonly KpiControlType Type = KpiControlType.Value;

        public Kpi_AreaItemControl(Kpi_AreaItemWrapper wrapper)
            : this(wrapper, null)
        {
            
        }

        public Kpi_AreaItemControl(Kpi_AreaItemWrapper wrapper, BitmapImage icon)
            : base(wrapper, icon)
        {
            Type = wrapper.Type;
        }

        public Kpi_AreaItemWrapper Kpi
        {
            get { return Wrapper as Kpi_AreaItemWrapper; }
        }
    }
}
