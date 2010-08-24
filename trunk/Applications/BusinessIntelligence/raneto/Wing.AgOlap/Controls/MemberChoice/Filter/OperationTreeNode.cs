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
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using System.Collections.Generic;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.MemberChoice.Filter
{
    public class OperationTreeNode : FilterTreeNode
    {
        List<LevelPropertyInfo> m_Properties;

        OperationItemControl item_ctrl = null;
        public OperationTreeNode(OperationTypes operation, bool isRoot, List<LevelPropertyInfo> properties)
        {
            m_Properties = properties;
            item_ctrl = new OperationItemControl(operation, isRoot);
            Header = item_ctrl;
            item_ctrl.CustomCommandClick += new EventHandler<CustomItemEventArgs>(item_ctrl_CustomCommandClick);

            IsExpanded = true;
        }

        void item_ctrl_CustomCommandClick(object sender, CustomItemEventArgs e)
        {
            switch (e.Type)
            {
                case CustomControlTypes.Clear:
                    this.Items.Clear();
                    break;
                case CustomControlTypes.Delete:
                    FilterTreeNode node = this.Parent as FilterTreeNode;
                    if (node != null)
                    {
                        node.Items.Remove(this);
                    }
                    break;
                case CustomControlTypes.AddOperation:
                    OperationTreeNode operationNode = new OperationTreeNode(OperationTypes.And, false, m_Properties);
                    operationNode.ApplyFilter += new EventHandler(OnApplyFilter);
                    this.Items.Add(operationNode);
                    OperandTreeNode operand1_Node = new OperandTreeNode(m_Properties);
                    operand1_Node.ApplyFilter += new EventHandler(OnApplyFilter);
                    operationNode.Items.Add(operand1_Node);
                    break;
                case CustomControlTypes.AddOperand:
                    OperandTreeNode operandNode = new OperandTreeNode(m_Properties);
                    operandNode.ApplyFilter += new EventHandler(OnApplyFilter);
                    this.Items.Add(operandNode);
                    break;

            }
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

        public OperationTypes Operation
        {
            get {
                return item_ctrl.Operation;
            }
        }
        

    }
}
