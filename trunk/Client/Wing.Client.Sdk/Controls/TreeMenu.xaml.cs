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
using Wing.Client.Sdk.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wing.Utils;

namespace Wing.Client.Sdk.Controls
{
    public partial class TreeMenu : UserControl, IExtensibleMenu
    {
        private TreeMenuRootController _rootController;

        public TreeMenu()
        {
            InitializeComponent();
            _rootController = new TreeMenuRootController(MenuTree);
        }

        public IExtensibleMenuItem CreateItem(string id, string caption)
        {
            return _rootController.CreateItem(id, caption, null).VisualElement;
        }

        public IExtensibleMenuItem CreateItem(string id, IGlobalCommand command)
        {
            var item = CreateItem(id, "");
            item.BindCommand(command);
            return item;
        }

        public IExtensibleMenuItem CreateChildItem(string id, string caption, string parentId)
        {
            return _rootController.CreateItem(id, caption, parentId).VisualElement;
        }

        public IExtensibleMenuItem CreateChildItem(String id, String parentId, IGlobalCommand command)
        {
            var item = CreateChildItem(id, "", parentId);
            item.BindCommand(command);
            return item;
        }

        public IExtensibleMenuItem GetItem(string id)
        {
            var existing = _rootController.FindItem(id, true);
            if (existing != null)
                return existing.VisualElement;
            return null;
        }

        public void RemoveItem(string id)
        {
            _rootController.RemoveItem(id);
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<IExtensibleMenuItem> Items
        {
            get { return _rootController.VisualItems; }
        }

    }


    public class TreeMenuItem : TreeViewItem, IExtensibleMenuItem
    {
        public TreeMenuItem(String id)
            : base()
        {
            ItemId = id;
        }

        internal void SetController(ExtensibleMenuController controller)
        {
            _controller = controller;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            base.OnSelected(e);
            if (!RedirectSelectionToFirstChild)
            {
                if (OnSelect != null)
                    OnSelect.Invoke(this, new EventArgs());
                if (Command != null)
                    Command.Execute(null);
                IsSelected = false;
            }
            else
            {
                IsSelected = false;
                var firstChild = ((IExtensibleMenuItem)this).Items.FirstOrDefault();
                if (firstChild != null)
                    firstChild.Select();
            }
        }

        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ItemId { get; private set; }

        public string Caption
        {
            get { return this.Header.AsString(); }
            set { this.Header = value; NotifyPropertyChanged("Caption"); }
        }

        void IExtensibleMenuItem.Select()
        {
            this.IsSelected = true;
        }

        bool IExtensibleMenuItem.IsEnabled
        {
            get { return this.IsEnabled; }
            set { this.IsEnabled = value; NotifyPropertyChanged("IsEnabled"); }
        }

        bool IExtensibleMenuItem.IsVisible
        {
            get { return this.Visibility == System.Windows.Visibility.Visible; }
            set
            {
                this.Visibility = value ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                NotifyPropertyChanged("IsVisible");
            }
        }

        ReadOnlyCollection<IExtensibleMenuItem> IExtensibleMenuItem.Items
        {
            get { return _controller.VisualItems; }
        }

        public event EventHandler OnSelect;

        public event PropertyChangedEventHandler PropertyChanged;
        private ExtensibleMenuController _controller;
        private bool _redirectSelectionToFirstChild;

        public IGlobalCommand Command { get; private set; }

        public void BindCommand(IGlobalCommand command)
        {
            UnbindCommand();
            command.StateChanged += new SingleEventHandler<IGlobalCommand>(command_StateChanged);
            this.Command = command;
            command_StateChanged(command);
        }

        void command_StateChanged(IGlobalCommand sender)
        {
            var thisE = (IExtensibleMenuItem)this;
            thisE.IsVisible = sender.IsVisible;
            thisE.IsEnabled = sender.IsEnabled;
            thisE.Caption = sender.Caption;
        }

        public void UnbindCommand()
        {
            if (this.Command == null) return;
            Command.StateChanged -= command_StateChanged;
            this.Command = null;
        }

        public bool RedirectSelectionToFirstChild
        {
            get { return _redirectSelectionToFirstChild; }
            set { _redirectSelectionToFirstChild = value; NotifyPropertyChanged("RedirectSelectionToFirstChild"); }
        }
    }

    internal class TreeMenuRootController : ExtensibleMenuController
    {
        private TreeView _treeView;

        internal TreeMenuRootController(TreeView treeView)
            : base(null, null)
        {
            _treeView = treeView;
        }

        protected override ExtensibleMenuController CreateItemInstance(string id, string caption)
        {
            var treeItem = new TreeMenuItem(id);
            _treeView.Items.Add(treeItem);
            treeItem.Caption = caption;
            treeItem.FontWeight = FontWeights.Bold;
            treeItem.Foreground = ControlHelper.GetPredefinedNamedColor("DarkBlue");
            return new TreeMenuItemController(treeItem, id);
        }

        protected override void RemoveItemInstance(ExtensibleMenuController item)
        {
            _treeView.Items.Remove(item.VisualElement);
        }
    }

    internal class TreeMenuItemController : ExtensibleMenuController
    {
        public TreeMenuItemController(TreeMenuItem item, String id)
            : base(item, id) { }

        protected override ExtensibleMenuController CreateItemInstance(string id, string caption)
        {
            var item = ((TreeMenuItem)VisualElement);
            var treeItem = new TreeMenuItem(id);
            item.Items.Add(treeItem);
            item.IsExpanded = true;
            treeItem.Caption = caption;
            treeItem.FontWeight = FontWeights.Normal;
            treeItem.Foreground = ControlHelper.GetPredefinedNamedColor("DarkBlue");
            var result = new TreeMenuItemController(treeItem, id);
            treeItem.SetController(result);
            return result;
        }

        protected override void RemoveItemInstance(ExtensibleMenuController item)
        {
            ((TreeMenuItem)VisualElement).Items.Remove(item.VisualElement);
        }
    }

}
