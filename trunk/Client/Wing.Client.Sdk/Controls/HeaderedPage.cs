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
    public class HeaderedPage : HeaderedContentControl
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
            this.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
            this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            this.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            this.Padding = new Thickness(5);

            /*
MemoryStream sr = null;

ParserContext pc = null;

string xaml = string.Empty;

xaml = "<DataTemplate><TextBlock Text=\"Some Text\"/></DataTemplate>";

sr = new MemoryStream(Encoding.ASCII.GetBytes(xaml));

pc = new ParserContext();

pc.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");

pc.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");

DataTemplate datatemplate = (DataTemplate)XamlReader.Load(sr, pc);

this.Resources.Add("dt", datatemplate);             
             */
        }

        public String PageTitle { get { return _pageTitle.Text; } set { _pageTitle.Text = value; } }
        public String SubTitle { get { return _subTitle.Text; } set { _subTitle.Text = value; } }
    }
}
