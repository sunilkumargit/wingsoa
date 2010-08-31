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
using Wing.Olap.Controls.MemberChoice.ClientServer;
using System.Collections.Generic;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.MemberChoice.Filter
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
