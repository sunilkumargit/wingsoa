using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace UILibrary.Olap.UITestApplication
{
    public partial class ComboBoxColors : ComboBox
    {
        public ComboBoxColors()
        {
            int i = 0;
            foreach (var pic in typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                Color c = (Color)pic.GetValue(null, null);
                this.Items.Add(new ComboBoxItem() { Tag = new SolidColorBrush(c), Content =/*i++.ToString()+"."+*/pic.Name, Background = new SolidColorBrush(c) });
            }
            this.SelectionChanged += new SelectionChangedEventHandler(onSelectionChanged);
        }

        void onSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.Background = new SolidColorBrush((((this.SelectedItem as ComboBoxItem).Tag) as SolidColorBrush).Color);
        }
    }
}
