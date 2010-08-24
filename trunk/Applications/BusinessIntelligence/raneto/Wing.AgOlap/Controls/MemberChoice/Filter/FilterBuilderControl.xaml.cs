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
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using System.Windows.Controls;
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.MemberChoice.Filter
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
