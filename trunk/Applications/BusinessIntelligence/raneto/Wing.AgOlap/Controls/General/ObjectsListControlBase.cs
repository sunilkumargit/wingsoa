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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.Storage;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Ranet.AgOlap.Controls.ToolBar;

namespace Ranet.AgOlap.Controls.General
{
    public class CustomEventArgs<T> : EventArgs
    {
        public readonly T Args = default(T);
        public bool Handled = false;
        public bool Cancel = false;

        public CustomEventArgs(T args)
        {
            Args = args;
        }
    }

    public class SelectionChangedEventArgs<T> : EventArgs
    {
        public readonly T OldValue;
        public readonly T NewValue;

        public SelectionChangedEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    public class ObjectsListControlBase<T> : UserControl where T : new() 
    {
        protected CustomTree Tree;
        Grid grdIsWaiting;
        protected RanetToolBar ToolBar;
        protected RanetToolBarButton AddButton;
        protected RanetToolBarButton DeleteButton;
        protected RanetToolBarButton DeleteAllButton;

        public ObjectsListControlBase()
        {
            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            ToolBar = new RanetToolBar();
            LayoutRoot.Children.Add(ToolBar);

            AddButton = new RanetToolBarButton();
            AddButton.Content = UiHelper.CreateIcon(UriResources.Images.New16);
            AddButton.Click += new RoutedEventHandler(m_AddButton_Click);
            ToolTipService.SetToolTip(AddButton, Localization.Toolbar_NewButton_ToolTip);
            ToolBar.AddItem(AddButton);

            DeleteButton = new RanetToolBarButton();
            DeleteButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveCross16);
            DeleteButton.Click += new RoutedEventHandler(m_DeleteButton_Click);
            DeleteButton.IsEnabled = false;
            ToolTipService.SetToolTip(DeleteButton, Localization.Toolbar_DeleteButton_ToolTip);
            ToolBar.AddItem(DeleteButton);
            DeleteButton.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnDeleteButton_IsEnabledChanged);

            DeleteAllButton = new RanetToolBarButton();
            DeleteAllButton.Content = UiHelper.CreateIcon(UriResources.Images.RemoveAllCross16);
            DeleteAllButton.Click += new RoutedEventHandler(m_DeleteAllButton_Click);
            DeleteAllButton.IsEnabled = false;
            ToolTipService.SetToolTip(DeleteAllButton, Localization.Toolbar_DeleteAllButton_ToolTip);
            ToolBar.AddItem(DeleteAllButton);
            DeleteAllButton.IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnDeleteAllButton_IsEnabledChanged);
            
            Tree = new CustomTree() { BorderBrush = new SolidColorBrush(Colors.DarkGray) };
            Tree.SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(m_Tree_SelectedItemChanged);
            LayoutRoot.Children.Add(Tree);
            Grid.SetRow(Tree, 1);

            grdIsWaiting = new Grid() { Background = new SolidColorBrush(Color.FromArgb(125, 0xFF, 0xFF, 0xFF)) };
            grdIsWaiting.Visibility = Visibility.Collapsed;
            BusyControl m_Waiting = new BusyControl();
            m_Waiting.Text = Localization.Loading;
            grdIsWaiting.Children.Add(m_Waiting);
            LayoutRoot.Children.Add(grdIsWaiting);
            Grid.SetColumnSpan(grdIsWaiting, LayoutRoot.ColumnDefinitions.Count > 0 ? LayoutRoot.ColumnDefinitions.Count : 1);
            Grid.SetRowSpan(grdIsWaiting, LayoutRoot.RowDefinitions.Count > 0 ? LayoutRoot.RowDefinitions.Count : 1);

            this.Content = LayoutRoot;

            this.Loaded += new RoutedEventHandler(ObjectsListControlBase_Loaded);
            Tree.KeyDown += new KeyEventHandler(m_Tree_KeyDown);
        }

        public event DependencyPropertyChangedEventHandler DeleteAllButton_IsEnabledChanged;
        public event DependencyPropertyChangedEventHandler DeleteButton_IsEnabledChanged;

        void OnDeleteAllButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyPropertyChangedEventHandler handler = DeleteAllButton_IsEnabledChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        void OnDeleteButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DependencyPropertyChangedEventHandler handler = DeleteButton_IsEnabledChanged;
            if (handler != null)
            {
                handler(this, e);
            }            
        }

        void m_DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteAll_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                OnDeleteAllButtonClick();
            }
        }

        void m_DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(Localization.DeleteCurrent_Question, Localization.Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                OnDeleteButtonClick();
            }
        }

        void m_AddButton_Click(object sender, RoutedEventArgs e)
        {
            OnAddButtonClick();
        }

        protected virtual void OnAddButtonClick()
        {
            AddItem(new T());
        }

        protected virtual void OnDeleteButtonClick()
        {
            DeleteItem(CurrentObject);

            EventHandler<SelectionChangedEventArgs<T>> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs<T>(CurrentObject, List.Count > 0 ? List[0] : default(T)));
            }
        }

        protected virtual void OnDeleteAllButtonClick()
        {
            Initialize(null);

            EventHandler<SelectionChangedEventArgs<T>> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs<T>(default(T), default(T)));
            }
        }

        void m_Tree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                T current = CurrentObject;
                if (current != null)
                {
                    EventHandler<CustomEventArgs<T>> handler = ObjectSelected;
                    if (handler != null)
                    {
                        handler(this, new CustomEventArgs<T>(current));
                    }
                }
            }
        }

        void ObjectsListControlBase_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tree.Items.Count > 0)
            {
                TreeNode<T> node = Tree.Items[0] as TreeNode<T>;
                if (node != null)
                {
                    node.IsSelected = true;
                    node.Focus();
                }
            }
        }

        public event EventHandler<SelectionChangedEventArgs<T>> SelectionChanged;
        public event EventHandler<CustomEventArgs<T>> ObjectSelected;

        void m_Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeNode<T> node_old = e.OldValue as TreeNode<T>;
            T descr_old = default(T);
            if (node_old != null)
            {
                descr_old = node_old.Info;
            }

            TreeNode<T> node_new = e.NewValue as TreeNode<T>;
            T descr_new = default(T);
            if (node_new != null)
            {
                descr_new = node_new.Info;
            }
            else
            {
                // Иногда по неясным причинам в e.NewValue приходит {object} хотя при установке IsSelected = true узел был корректный. Поэтому в случае если узел определить не удалось, считаем что выбран нулевой узел
                if (e.NewValue != null && Tree.Items.Count > 0)
                {
                    TreeNode<T> node0 = Tree.Items[0] as TreeNode<T>;
                    if (node0 != null)
                    {
                        descr_new = node0.Info; 
                    }
                }
            }

            EventHandler<SelectionChangedEventArgs<T>> handler = SelectionChanged;
            if (handler != null)
            {
                handler(this, new SelectionChangedEventArgs<T>(descr_old, descr_new));
            }

            DeleteButton.IsEnabled = descr_new != null;

            Refresh();
        }

        public T CurrentObject
        {
            get
            {
                TreeNode<T> node = null;
                // m_Tree.SelectedItem иногда равно null. Несмотря на то что среди узлов есть такой, у которого IsSlected == true

                if (Tree.SelectedItem == null)
                {
                    // Сами попытаемся найти выбранный элемент.
                    foreach (object obj in Tree.Items)
                    {
                        TreeNode<T> x = obj as TreeNode<T>;
                        if (x != null && x.IsSelected)
                        {
                            node = x;
                            break;
                        }
                    }
                }
                else
                {
                    node = Tree.SelectedItem as TreeNode<T>;
                }

                if(node != null)
                {
                    return node.Info;
                }
                return default(T);
            }
            set
            {
                if (value != null)
                {
                    TreeViewItem selected = null;
                    foreach (object obj in Tree.Items)
                    {
                        TreeViewItem item = obj as TreeViewItem;
                        if (item != null)
                        {
                            item.IsSelected = false;
                            TreeNode<T> node = item as TreeNode<T>;
                            if (node != null && node.Info.Equals(value))
                            {
                                selected = item;
                            }
                        }
                    }
                    if (selected != null)
                        selected.IsSelected = true;
                }
            }
        }

        bool m_IsWaiting = false;
        public bool IsWaiting
        {
            get { return m_IsWaiting; }
            set
            {
                if (m_IsWaiting != value)
                {
                    if (value)
                    {
                        this.Cursor = Cursors.Wait;
                        grdIsWaiting.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.Cursor = Cursors.Arrow;
                        grdIsWaiting.Visibility = Visibility.Collapsed;
                    }
                    this.IsEnabled = !value;
                    if (this.IsEnabled)
                    {
                        Tree.Focus();
                    }
                    m_IsWaiting = value;
                }
            }
        }

        List<T> m_List = null;
        public List<T> List
        {
            get {
                if (m_List == null)
                {
                    m_List = new List<T>();
                }
                return m_List;
            }
        }

        public void Initialize(List<T> list)
        {
            if (list != null && list.Count > 0)
                Initialize(list, list[0]);
            else
                Initialize(list, default(T));
        }
        
        public void Initialize(List<T> list, T toSelect)
        {
            m_List = list;
            Tree.Items.Clear();

            TreeNode<T> select = null;
            if (list != null)
            {
                foreach (T descr in list)
                {
                    TreeNode<T> node = AddItemNode(descr);
                    if (descr.Equals(toSelect))
                    {
                        select = node;
                    }
                }
            }

            if (select != null)
            {
                // Через событие делаем узел выбранным (иначе на нем фокус не ставится)
                select.Loaded += new RoutedEventHandler(node_Loaded);
            }

            DeleteButton.IsEnabled = CurrentObject != null;
            DeleteAllButton.IsEnabled = Tree.Items.Count > 0;
        }

        void node_Loaded(object sender, RoutedEventArgs e)
        {
            TreeNode<T> node = sender as TreeNode<T>;
            if (node != null)
            {
                node.Loaded -= new RoutedEventHandler(node_Loaded);
                node.IsSelected = true;
                node.Focus();
            }
        }

        public virtual void Refresh()
        {
        }

        public void AddItem(T item)
        {
            if (item != null)
            {
                List.Add(item);
                AddItemNode(item);
                CurrentObject = item;
            }
        }

        public void DeleteItem(T item)
        {
            if (item != null)
            {
                if(List.Contains(item))
                    List.Remove(item);
                foreach (object obj in Tree.Items)
                {
                    TreeNode<T> node = obj as TreeNode<T>;
                    if (node != null && node.Info.Equals(item))
                    {
                        Tree.Items.Remove(node);
                        break;
                    }
                }
            }

            DeleteButton.IsEnabled = CurrentObject != null;
            DeleteAllButton.IsEnabled = Tree.Items.Count > 0;
        }

        TreeNode<T> AddItemNode(T item)
        {
            TreeNode<T> node = null;
            if (item != null)
            {
                node = BuildTreeNode(item);
                if (node != null)
                {
                    node.MouseDoubleClick += new MouseDoubleClickEventHandler(node_MouseDoubleClick);
                    Tree.Items.Add(node);
                }
            }

            DeleteButton.IsEnabled = CurrentObject != null;
            DeleteAllButton.IsEnabled = Tree.Items.Count > 0;
            return node;
        }

        public virtual TreeNode<T> BuildTreeNode(T item)
        {
            return new TreeNode<T>("Node", null, item);
        }

        void node_MouseDoubleClick(object sender, EventArgs e)
        {
            TreeNode<T> node = sender as TreeNode<T>;
            if (node != null && node.Info != null)
            {
                EventHandler<CustomEventArgs<T>> handler = ObjectSelected;
                if (handler != null)
                {
                    handler(this, new CustomEventArgs<T>(node.Info));
                }
            }
        }

        public virtual bool Contains(String name)
        {
            return false;
        }
    }
}
