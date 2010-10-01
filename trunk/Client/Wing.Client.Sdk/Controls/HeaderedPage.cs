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

namespace Wing.Client.Sdk.Controls
{
    public class HeaderedPage : HeaderedItemsControl
    {
        private TextBlock _pageTitle;
        private TextBlock _subTitle;

        public HeaderedPage()
        {
            var header = new StackPanel();
            _pageTitle = new TextBlock() { Name = "PageTitle", Foreground = ControlHelper.GetPredefinedNamedColor("DarkBlue"), FontSize = 18 };
            _subTitle = new TextBlock() { Name = "PageSubTitle", Foreground = ControlHelper.GetPredefinedNamedColor("DarkGreen"), FontSize = 10 };
            header.Margin = new Thickness(7);
            header.Orientation = Orientation.Vertical;
            header.Children.Add(_pageTitle);
            header.Children.Add(_subTitle);
            this.Header = header;
        }

        public String PageTitle { get { return _pageTitle.Text; } set { _pageTitle.Text = value; } }
        public String SubTitle { get { return _subTitle.Text; } set { _subTitle.Text = value; } }
    }
}
