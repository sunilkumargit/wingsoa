/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Olap.Controls.Buttons;
using Wing.Olap.Controls.General;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.MemberChoice.Filter
{
    public partial class OperationItemControl : UserControl
    {
        private bool m_IsRoot = false;
        public OperationItemControl(OperationTypes operation, bool isRoot)
        {
            InitializeComponent();

            RanetHotButton add = new RanetHotButton() { Margin = new Thickness(5, 0, 0, 0) };
            add.Click += new RoutedEventHandler(add_Click);
            add.Width = 18;
            add.Height = 18;
            add.Content = UiHelper.CreateIcon(UriResources.Images.AddHot16);
            LayoutRoot.Children.Add(add);
            Grid.SetColumn(add, 1);

            m_IsRoot = isRoot;
            operationControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;

            operationControl.SelectionChanged += new SelectionChangedEventHandler(operationControl_SelectionChanged);
            operationControl.DropDownClosed += new EventHandler(operationControl_DropDownClosed);

            InitItems();

            if (operationControl.Items.Count > 0)
            {
                operationControl.SelectedIndex = 0;
            }
        }

        void add_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<CustomItemEventArgs> handler = this.CustomCommandClick;
            if (handler != null)
            {
                handler(this, new CustomItemEventArgs(CustomControlTypes.AddOperand));
            }
        }

        private void InitItems()
        {
            operationControl.SelectionChanged -= new SelectionChangedEventHandler(operationControl_SelectionChanged);
            operationControl.Items.Clear();

            FilterOperationItemControl item = new FilterOperationItemControl(OperationTypes.And);
            operationControl.Items.Add(item);
            item = new FilterOperationItemControl(OperationTypes.Or);
            operationControl.Items.Add(item);

            ItemsSplitter splitter = new ItemsSplitter();
            splitter.IsTabStop = false;
            splitter.Width = 150;
            operationControl.Items.Add(splitter);

            CustomItemControl custom = new CustomItemControl(CustomControlTypes.AddOperation);
            operationControl.Items.Add(custom);

            custom = new CustomItemControl(CustomControlTypes.AddOperand);
            operationControl.Items.Add(custom);

            splitter = new ItemsSplitter();
            splitter.IsTabStop = false;
            splitter.Width = 150;
            operationControl.Items.Add(splitter);

            if (m_IsRoot)
            {
                custom = new CustomItemControl(CustomControlTypes.Clear);
                operationControl.Items.Add(custom);
            }
            else
            {
                custom = new CustomItemControl(CustomControlTypes.Delete);
                operationControl.Items.Add(custom);
            }
            operationControl.SelectionChanged += new SelectionChangedEventHandler(operationControl_SelectionChanged);
        }
        
        void operationControl_DropDownClosed(object sender, EventArgs e)
        {
            operationControl.SelectedIndex = m_SelectedOperationIndex;
            //int i = operationControl.SelectedIndex;
            //InitItems();
            //operationControl.SelectionChanged -= new SelectionChangedEventHandler(operationControl_SelectionChanged);
            //operationControl.SelectedIndex = i;
            //operationControl.SelectionChanged += new SelectionChangedEventHandler(operationControl_SelectionChanged);
        }

        public event EventHandler<CustomItemEventArgs> CustomCommandClick;

        int m_SelectedOperationIndex = -1;
        void operationControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Будем разрешать выбирать только операции
            FilterOperationItemControl operation = operationControl.SelectedItem as FilterOperationItemControl;
            if (operation != null)
                m_SelectedOperationIndex = operationControl.SelectedIndex;

            //ItemsSplitter splitter = operationControl.SelectedItem as ItemsSplitter;
            //if (splitter != null)
            //{
            //    if (e.RemovedItems != null && e.RemovedItems.Count > 0)
            //    {
            //        //splitter.UpdateLayout();
            //        //operationControl.SelectedIndex = operationControl.Items.IndexOf(e.RemovedItems[0]);
            //        //operationControl.SelectedItem = e.RemovedItems[0];
            //    }
            //    else
            //        operationControl.SelectedIndex = 0;
            //    return;
            //}

            CustomItemControl custom = operationControl.SelectedItem as CustomItemControl;
            if (custom != null)
            {
                //if (e.RemovedItems != null && e.RemovedItems.Count > 0)
                //{
                //    //custom.UpdateLayout();
                //    //operationControl.SelectedIndex = operationControl.Items.IndexOf(e.RemovedItems[0]);
                //    //operationControl.SelectedItem = e.RemovedItems[0];
                //}
                //else
                //    operationControl.SelectedIndex = 0;

                EventHandler<CustomItemEventArgs> handler = this.CustomCommandClick;
                if (handler != null)
                {
                    handler(this, new CustomItemEventArgs(custom.Type));
                }
                return;
            }
            //operationControl.UpdateLayout();
        }

        public OperationTypes Operation
        {
            get
            {
                FilterOperationItemControl ctrl = operationControl.SelectedItem as FilterOperationItemControl;
                if (ctrl != null)
                {
                    return ctrl.Operation;
                }
                return OperationTypes.And;
            }
        }
    }
}
