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
using System.Windows.Controls;
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using Ranet.Olap.Core.Metadata;
using System.Collections.Generic;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.MemberChoice.Filter
{
    public class OperandTreeNode : FilterTreeNode
    {
        OperandItemControl item_ctrl = null;
        public OperandTreeNode(List<LevelPropertyInfo> properties)
        {
            item_ctrl = new OperandItemControl();
            item_ctrl.ApplyFilter += new EventHandler(OnApplyFilter);
            item_ctrl.InitParameters(properties);
            Header = item_ctrl;

            item_ctrl.CustomCommandClick += new EventHandler<CustomItemEventArgs>(item_ctrl_CustomCommandClick);
        }

        public event EventHandler ApplyFilter;

        void OnApplyFilter(object sender, EventArgs e)
        {
            EventHandler handler = ApplyFilter;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }   
        }

        void item_ctrl_CustomCommandClick(object sender, CustomItemEventArgs e)
        {
            switch (e.Type)
            {
                case CustomControlTypes.Delete:
                    FilterTreeNode node = this.Parent as FilterTreeNode;
                    if (node != null)
                    {
                        node.Items.Remove(this);
                    }
                    break;
            }
        }

        public FilterOperand Operand
        {
            get {
                return item_ctrl.Operand;
            }
        }
    }
}
