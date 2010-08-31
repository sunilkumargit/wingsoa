/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Wing.Olap.Controls.MemberChoice.ClientServer;
using System.Windows.Controls;
using Wing.Olap.Core.Metadata;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.MemberChoice.Filter
{
    public partial class FilterBuilderControl : UserControl
    {
        public FilterBuilderControl()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(FilterBuilderControl_Loaded);

            Tree.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        }

        void FilterBuilderControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public FilterOperationBase GetFilter()
        {
            if (Tree.Items.Count > 0)
            {
                FilterTreeNode node = Tree.Items[0] as FilterTreeNode;
                if (node != null)
                {
                    FilterOperationBase res = BuildFilter(node);
                    return res;
                }
            }
            return null;
        }

        private FilterOperationBase BuildFilter(FilterTreeNode node)
        {
            // Если узел - операция
            OperationTreeNode operationNode = node as OperationTreeNode;
            {
                if (operationNode != null)
                {
                    FilterOperation operation = new FilterOperation(operationNode.Operation);
                    foreach (object obj in operationNode.Items)
                    {
                        FilterTreeNode childNode = obj as FilterTreeNode;
                        if (childNode != null)
                        {
                            FilterOperationBase filter = BuildFilter(childNode);
                            if (filter != null)
                            {
                                operation.Children.Add(filter);
                            }
                        }
                    }
                    return operation;
                }
            }

            // Если узел - операнд
            OperandTreeNode operand = node as OperandTreeNode;
            if (operand != null)
            {
                return operand.Operand;
            }

            return null;
        }

        public void Initialize(List<LevelPropertyInfo> properties)
        {
            Tree.Items.Clear();
            OperationTreeNode operationNode = new OperationTreeNode(OperationTypes.And, true, properties);
            operationNode.ApplyFilter += new EventHandler(OnApplyFilter);
            Tree.Items.Add(operationNode);
            OperandTreeNode node = new OperandTreeNode(properties);
            node.ApplyFilter += new EventHandler(ApplyFilter);
            operationNode.Items.Add(node);
            operationNode.IsExpanded = true;
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
    }
}
