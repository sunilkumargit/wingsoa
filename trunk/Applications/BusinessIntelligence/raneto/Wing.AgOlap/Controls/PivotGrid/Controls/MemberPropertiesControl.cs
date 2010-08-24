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
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.Tab;
using Ranet.AgOlap.Controls.ToolBar;
using System.Text;
using Ranet.AgOlap.Features;
using System.Collections.Generic;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.PivotGrid.Controls
{
    public class MemberPropertiesControl : UserControl
    {
        protected RanetTabControl TabCtrl;
        protected PropertiesListControl PropertiesCtrl;

        public MemberPropertiesControl()
        {
            TabCtrl = new RanetTabControl();

            PropertiesCtrl = new PropertiesListControl();

            TabItem PropertiesTab = new TabItem();
            PropertiesTab.Header = Localization.Properties;
            PropertiesTab.Style = TabCtrl.Resources["TabControlOutputItem"] as Style;
            PropertiesTab.Content = PropertiesCtrl;
            TabCtrl.TabCtrl.Items.Add(PropertiesTab);

            PropertiesTab.Header = Localization.Properties;

            TabToolBar toolBar = TabCtrl.ToolBar;
            if (toolBar != null)
            {
                RanetToolBarButton copyBtn = new RanetToolBarButton();
                ToolTipService.SetToolTip(copyBtn, Localization.CopyToClipboard_ToolTip);
                copyBtn.Content = UiHelper.CreateIcon(UriResources.Images.Copy16);
                toolBar.Stack.Children.Add(copyBtn);
                copyBtn.Click += new RoutedEventHandler(CopyButton_Click);
            }

            this.Content = TabCtrl;
        }

        void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (TabCtrl.TabCtrl.SelectedIndex == 0)
            {
                foreach(var prop in PropertiesCtrl.List)
                {
                    sb.AppendFormat("{0}\t", prop.Property);
                    sb.Append(prop.Value);
                    sb.Append(Environment.NewLine);                                        
                }
            }
            Ranet.AgOlap.Features.Clipboard.SetClipboardText(sb.ToString());
        }

        public void Initialize(MemberData member)
        {
            List<PropertyItem> list = new List<PropertyItem>();
            if (member != null)
            {
                foreach (PropertyData pair in member.MemberProperties)
                {
                    PropertyItem item = new PropertyItem();

                    String caption = pair.Name;
                    if (caption.StartsWith("-") && caption.EndsWith("-"))
                        caption = caption.Trim('-');
                    item.Property = caption;

                    if (pair.Value != null)
                    {
                        item.Value = pair.Value.ToString();
                    }
                    else
                    {
                        item.Value = String.Empty;
                    }
                    list.Add(item);
                }
            }
            PropertiesCtrl.Initialize(list);
        }

        public void Initialize(MemberInfo member)
        {
            List<PropertyItem> list = new List<PropertyItem>();
            if (member != null && member.PropertiesDictionary != null)
            {
                foreach (KeyValuePair<String, object> pair in member.PropertiesDictionary)
                {
                    PropertyItem item = new PropertyItem();
                    item.Property = pair.Key;
                    if (item.Property == "DrilledDown")
                    {
                        // Элементы на оси объединяются если идут подряд одинаковые. При этом значение данного свойства формируется по ИЛИ
                        item.Value = member.DrilledDown.ToString();
                    }
                    else
                    {
                        if (pair.Value != null)
                        {
                            item.Value = pair.Value.ToString();
                        }
                        else
                        {
                            item.Value = String.Empty;
                        }
                    }
                    list.Add(item);
                }
            }

            PropertiesCtrl.Initialize(list);
        }
    }
}
