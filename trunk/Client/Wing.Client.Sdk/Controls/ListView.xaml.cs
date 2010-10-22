using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Wing.Utils;

namespace Wing.Client.Sdk.Controls
{
    public partial class ListView : UserControl
    {
        private ObservableCollection<ListViewItemWrapper> _items = new ObservableCollection<ListViewItemWrapper>();
        private bool _bindingPending = false;
        

        public ListView()
        {
            InitializeComponent();
            InnerListBox.ItemsSource = _items;
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            var img = (Image)sender;
            ControlHelper.TryLoadImage(this, img.Source);
        }

        private void ListItemDoubleClickEvent(object sender, RoutedEventArgs args)
        {
            var item = (ListViewItemWrapper)InnerListBox.SelectedItem;
            if (item != null)
            {
                if (ItemTriggered != null)
                    ItemTriggered.Invoke(this, item.Data);
            }
        }

        private void InnerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = (ListViewItemWrapper)e.AddedItems[0];
                if (ItemSelected != null)
                    ItemSelected.Invoke(this, item.Data);
            }
        }

        public String IconSourcePropertyName { get; set; }
        public String TextPropertyName { get; set; }
        public String DefaultIconSource { get; set; }

        public IEnumerable DataSource
        {
            get { return _dataSource; }
            set
            {
                if (_dataSource == value)
                    return;
                if (_dataSource != null && _dataSource is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)_dataSource).CollectionChanged -= ListView_CollectionChanged;
                _dataSource = value;
                BindDataSource();
                if (_dataSource != null && _dataSource is INotifyCollectionChanged)
                    ((INotifyCollectionChanged)_dataSource).CollectionChanged += new NotifyCollectionChangedEventHandler(ListView_CollectionChanged);
            }
        }

        void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_bindingPending)
                return;
            _bindingPending = true;
            VisualContext.DelayAsync(TimeSpan.FromMilliseconds(2000), () =>
            {
                _bindingPending = false;
                BindDataSource();
            });
        }

        public void BindDataSource()
        {
            VisualContext.Async(() =>
            {
                _items.Clear();
                if (_dataSource != null)
                {
                    foreach (var o in _dataSource)
                    {
                        _items.Add(new ListViewItemWrapper()
                        {
                            ListView = this,
                            Data = o
                        });
                    }
                }
            });
        }

        public event SingleEventHandler<ListView, Object> ItemSelected;
        public event SingleEventHandler<ListView, Object> ItemTriggered;
        private IEnumerable _dataSource;
    }

    public class ListViewItemWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _iconSource;
        private string _text;
        private object _data;
        private bool _refreshNeeded = true;

        private void CheckRefreshData()
        {
            if (!_refreshNeeded)
                return;

            var iconSource = "";
            var text = "";

            if (Data != null)
            {
                if (ListView.IconSourcePropertyName.HasValue())
                    iconSource = ReflectionUtils.ReadProperty<String>(Data, ListView.IconSourcePropertyName);
                if (!iconSource.HasValue())
                    iconSource = ListView.DefaultIconSource;
                if (ListView.TextPropertyName.HasValue())
                    text = ReflectionUtils.ReadProperty<String>(Data, ListView.TextPropertyName);
            }

            _refreshNeeded = false;
            IconSource = iconSource;
            Text = text;

        }

        internal ListView ListView { get; set; }
        public String IconSource
        {
            get
            {
                CheckRefreshData();
                return _iconSource;
            }
            set { _iconSource = value; NotifyPropertyChanged("IconSource"); }
        }

        public String Text
        {
            get
            {
                CheckRefreshData();
                return _text;
            }
            set { _text = value; NotifyPropertyChanged("Text"); }
        }

        public Object Data
        {
            get { return _data; }
            set
            {
                _data = value;
                if (_data != null && _data is INotifyPropertyChanged)
                {
                    ((INotifyPropertyChanged)_data).PropertyChanged += new PropertyChangedEventHandler(ListViewItemWrapper_DataPropertyChanged);
                }
                NotifyPropertyChanged("Data");
            }
        }

        void ListViewItemWrapper_DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ListView.TextPropertyName || e.PropertyName == ListView.IconSourcePropertyName)
            {
                _refreshNeeded = true;
                CheckRefreshData();
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
