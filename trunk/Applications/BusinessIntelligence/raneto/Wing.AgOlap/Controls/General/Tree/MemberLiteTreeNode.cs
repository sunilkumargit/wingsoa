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
using Ranet.Olap.Core.Data;

namespace Ranet.AgOlap.Controls.General.Tree
{
    public class MemberLiteTreeNode : CustomTreeNode
    {
        public readonly MemberData Info = null;

        public MemberLiteTreeNode(MemberData info)
            : base()
        {
            Info = info;
            Icon = UriResources.Images.MemberSmall16;
            MemberVisualizationType = MemberVisualizationTypes.Caption;
        }

        MemberVisualizationTypes m_MemberVisualizationType = MemberVisualizationTypes.Caption;
        public MemberVisualizationTypes MemberVisualizationType
        {
            get { return m_MemberVisualizationType; }
            set
            {
                m_MemberVisualizationType = value;
                // Определяем что именно нужно светить в контроле
                if (Info != null && Info != null)
                    Text = Info.GetText(m_MemberVisualizationType);
                else
                    Text = String.Empty;
            }
        }
    }
}
