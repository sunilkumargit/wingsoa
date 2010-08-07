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
using Wing.Composite.Presentation.Regions;

namespace Wing.Client.Sdk.Controls
{
    public class ViewBagDefaultContainer : ContentControl
    {
        public static DependencyProperty ContentRegionNameProperty = DependencyProperty.Register(
            "ContentRegionName", typeof(String), typeof(ViewBagDefaultContainer), new PropertyMetadata("Content", new PropertyChangedCallback(ContentRegionNameChanged)));

        private ContentControl _content;

        public ViewBagDefaultContainer()
        {
            Configure(this);
            _content = new ContentControl();
            this.Content = _content;
            Configure(_content);
            SetContentRegionName(ContentRegionName);
        }


        private void Configure(ContentControl control)
        {
            control.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            control.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            control.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
        }


        public static void ContentRegionNameChanged(DependencyObject instance, DependencyPropertyChangedEventArgs target)
        {
            ((ViewBagDefaultContainer)instance).SetContentRegionName((String)target.NewValue);
        }

        private void SetContentRegionName(String regionName)
        {
            RegionManager.SetRegionName(_content, ContentRegionName);
        }

        public String ContentRegionName
        {
            get { return (String)GetValue(ContentRegionNameProperty); }
            set { SetValue(ContentRegionNameProperty, value); }
        }
    }
}
