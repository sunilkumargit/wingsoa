using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Wing.Client.Sdk.Controls
{
    public class HeaderedPage : ContentControl
    {
        public static readonly DependencyProperty PageTitleProperty = DependencyProperty.Register("PageTitle",
            typeof(String), typeof(HeaderedPage), new PropertyMetadata(""));

        public static readonly DependencyProperty PageSubtitleProperty = DependencyProperty.Register("Subtitle",
            typeof(String), typeof(HeaderedPage), new PropertyMetadata(""));

        public HeaderedPage()
        {
            this.Style = (Style)Application.Current.Resources["HeaderedPageTemplate"];
            this.ApplyTemplate();
            ControlHelper.SetBorderBrushToDebug(this);
        } 

        public String PageTitle
        {
            get
            {
                return (String)GetValue(PageTitleProperty);
            }
            set
            {
                SetValue(PageTitleProperty, value);
            }
        }

        public String Subtitle
        {
            get
            {
                return (String)GetValue(PageSubtitleProperty);
            }
            set
            {
                SetValue(PageSubtitleProperty, value);
            }
        }
    }
}
