using System;
using System.Windows;
using System.Windows.Controls;
using Wing.Composite.Presentation.Regions;

namespace Wing.Client.Sdk.Controls
{
    public class ViewBagDefaultContainer : CustomContentControl
    {
        public static DependencyProperty ContentRegionNameProperty = DependencyProperty.Register(
            "ContentRegionName", typeof(String), typeof(ViewBagDefaultContainer), new PropertyMetadata("Content", new PropertyChangedCallback(ContentRegionNameChanged)));

        private ContentControl _content;

        public ViewBagDefaultContainer()
        {
            ControlHelper.StretchContentControl(this);
            _content = new CustomContentControl();
            this.Content = _content;
            ControlHelper.StretchContentControl(_content);
            SetContentRegionName(ContentRegionName);
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
