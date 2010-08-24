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
using Ranet.AgOlap.Controls.PivotGrid.Controls;

namespace Ranet.AgOlap.Controls.MdxDesigner
{
    // Не используется. Добавлен для проведения работ по формированию команд DrillDown, Expand, Collapse самим MDX дизайнером.
    public class DesignerPivotGridControl : UpdateablePivotGridControl
    {
        public DesignerPivotGridControl()
            : base()
        {
            this.Loaded += new RoutedEventHandler(DesignerPivotGridControl_Loaded);
        }

        #region События
        public event MemberActionEventHandler OnPerformMemberAction;
        protected void Raise_PerformMemberAction(MemberActionEventArgs args)
        {
            MemberActionEventHandler handler = this.OnPerformMemberAction;
            if (handler != null)
            {
                handler(this, args);
            }
        }
        #endregion События

        protected override void PerformMemberAction(MemberActionEventArgs e)
        {
            base.PerformMemberAction(e);
        }

        void DesignerPivotGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            ToBeginButton.Visibility = Visibility.Collapsed;
            ToEndButton.Visibility = Visibility.Collapsed;
            BackButton.Visibility = Visibility.Collapsed;
            ForwardButton.Visibility = Visibility.Collapsed;
            NavigationToolBarSplitter.Visibility = Visibility.Collapsed;
        }
    }
}
