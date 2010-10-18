using System;
using System.Collections.Generic;
using System.Linq;
using Wing.Utils;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;
using System.Collections;

namespace Wing.Client.Sdk.Controls
{
    public partial class ListView : UserControl
    {
        private ObservableCollection<ListViewItemWrapper> _items = new ObservableCollection<ListViewItemWrapper>();

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
                _dataSource = value;
                BindDataSource();
            }
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
                        var iconSource = "";
                        var text = "";
                        if (IconSourcePropertyName.HasValue())
                            iconSource = ReflectionUtils.ReadProperty<String>(o, IconSourcePropertyName);
                        if (!iconSource.HasValue())
                            iconSource = DefaultIconSource;
                        if (TextPropertyName.HasValue())
                            text = ReflectionUtils.ReadProperty<String>(o, TextPropertyName);
                        _items.Add(new ListViewItemWrapper()
                        {
                            IconSource = iconSource,
                            Text = text,
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

        public String IconSource { get { return _iconSource; } set { _iconSource = value; NotifyPropertyChanged("IconSource"); } }
        public String Text { get { return _text; } set { _text = value; NotifyPropertyChanged("Text"); } }
        public Object Data { get { return _data; } set { _data = value; NotifyPropertyChanged("Data"); } }

        [System.Diagnostics.DebuggerStepThrough]
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
