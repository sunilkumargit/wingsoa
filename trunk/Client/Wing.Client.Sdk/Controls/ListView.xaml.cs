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
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Wing.Client.Sdk.Controls
{
    public partial class ListView : UserControl
    {
        public ListView()
        {
            InitializeComponent();
            var list = new ObservableCollection<ListViewItemWrapper>(){
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" },
                new ListViewItemWrapper() { IconSource = "Flex.BusinessIntelligence.WingClient;Component/../Assets/bi-icon.png", Text="BI" },
                new ListViewItemWrapper() { IconSource = "bi;/Assets/bi-icon.png", Text="BI 2 com texto comprido pra carai mano" }
            };
            InnerListBox.ItemsSource = list;

            var timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(4) };
            timer.Tick += (sender, args) => { list.Add(new ListViewItemWrapper() { Text = DateTime.Now.ToString() }); };
            timer.Start();
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

        public event SingleEventHandler<ListView, Object> ItemSelected;
        public event SingleEventHandler<ListView, Object> ItemTriggered;
    }

    public class ListViewItemWrapper
    {
        public String IconSource { get; set; }
        public String Text { get; set; }
        public Object Data { get; set; }
    }
}
