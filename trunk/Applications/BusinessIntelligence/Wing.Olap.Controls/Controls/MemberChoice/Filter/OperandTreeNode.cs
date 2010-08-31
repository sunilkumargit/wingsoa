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
using System.Windows.Controls;
using Wing.AgOlap.Controls.MemberChoice.ClientServer;
using Wing.Olap.Core.Metadata;
using System.Collections.Generic;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls.MemberChoice.Filter
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
