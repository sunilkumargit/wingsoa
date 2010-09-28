using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Wing.Client.Sdk.Controls;

namespace Flex.BusinessIntelligence.WingClient.Views.Home
{
    public partial class BIHomeView : UserControl, IBIHomeView
    {
        private TreeMenu _treeMenu;

        public BIHomeView()
        {
            InitializeComponent();
            SideBar.Caption = "Business Intelligence";
            _treeMenu = new TreeMenu();
            SideBar.Content = _treeMenu;
        }

        #region IBIHomeView Members

        public IExtensibleMenu MainMenu
        {
            get { return _treeMenu; }
        }

        #endregion
    }
}
