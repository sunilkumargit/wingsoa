/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

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
using Wing.AgOlap.Controls.General;
using Wing.AgOlap.Controls.Tab;
using Wing.AgOlap.Controls.Buttons;
using Wing.AgOlap.Features;
using Wing.AgOlap.Controls.ToolBar;
using Wing.Olap.Core.Providers.ClientServer;
using Wing.AgOlap.Providers;

namespace Wing.AgOlap.Controls.DataSourceInfo
{
    public class DataSourceInfoControl : UserControl
    {
        Wing.AgOlap.Controls.General.RichTextBox connectionString;
        Wing.AgOlap.Controls.General.RichTextBox mdxQuery;
        Wing.AgOlap.Controls.General.RichTextBox movedAxes_MdxQuery;
        Wing.AgOlap.Controls.General.RichTextBox updateScript;
        CheckBox parseMdx;
        CheckBox parseMovedAxesMdx;
        CheckBox parseUpdateScript;

        RanetTabControl queryInfoTabControl;

        TabItem scriptTab;

        public DataSourceInfoControl()
        {
            Grid LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            // Строка соединения
            TextBlock connectionString_Label = new TextBlock() { Text = Localization.Connection + ":" };
            LayoutRoot.Children.Add(connectionString_Label);

            connectionString = new Wing.AgOlap.Controls.General.RichTextBox() {IsReadOnly = true };
            connectionString.Height = 22;
            connectionString.Margin = new Thickness(0, 5, 0, 0);
            LayoutRoot.Children.Add(connectionString);
            Grid.SetRow(connectionString, 1);
            //connectionString.Background = new SolidColorBrush(Colors.Transparent);

            // Запрос
            queryInfoTabControl = new RanetTabControl();
            queryInfoTabControl.Margin = new Thickness(0, 5, 0, 0);
            LayoutRoot.Children.Add(queryInfoTabControl);
            Grid.SetRow(queryInfoTabControl, 2);

            // Закладка "MDX Query"
            TabItem queryTab = new TabItem();
            queryTab.Header = Localization.MDXQuery;
            queryTab.Style = queryInfoTabControl.Resources["TabControlOutputItem"] as Style;
            queryInfoTabControl.TabCtrl.Items.Add(queryTab);

            Grid queryTabLayoutRoot = new Grid() { Margin = new Thickness(0, 0, 0, 0) };
            queryTabLayoutRoot.RowDefinitions.Add(new RowDefinition());
            queryTabLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            mdxQuery = new Wing.AgOlap.Controls.General.RichTextBox() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, IsReadOnly = true };
            mdxQuery.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            mdxQuery.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            queryTabLayoutRoot.Children.Add(mdxQuery);
            Grid.SetRow(mdxQuery, 0);

            parseMdx = new CheckBox() { Margin = new Thickness(0, 5, 0, 0) };
            parseMdx.Content = Localization.DataSourceInfo_ShowParsedQuery;
            queryTabLayoutRoot.Children.Add(parseMdx);
            Grid.SetRow(parseMdx, 1);
            parseMdx.Checked += new RoutedEventHandler(parseMdx_CheckedChanged);
            parseMdx.Unchecked += new RoutedEventHandler(parseMdx_CheckedChanged);

            queryTab.Content = queryTabLayoutRoot;

            // Закладка "Moved Axes"
            TabItem movedAxesTab = new TabItem();
            movedAxesTab.Header = Localization.DataSourceInfo_MovedAxes;
            movedAxesTab.Style = queryInfoTabControl.Resources["TabControlOutputItem"] as Style;
            queryInfoTabControl.TabCtrl.Items.Add(movedAxesTab);

            Grid movedAxesTabLayoutRoot = new Grid() { Margin = new Thickness(0, 0, 0, 0) };
            movedAxesTabLayoutRoot.RowDefinitions.Add(new RowDefinition());
            movedAxesTabLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            movedAxes_MdxQuery = new Wing.AgOlap.Controls.General.RichTextBox() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, IsReadOnly = true };
            movedAxes_MdxQuery.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            movedAxes_MdxQuery.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            movedAxesTabLayoutRoot.Children.Add(movedAxes_MdxQuery);
            Grid.SetRow(movedAxes_MdxQuery, 0);

            parseMovedAxesMdx = new CheckBox() { Margin = new Thickness(0, 5, 0, 0) };
            parseMovedAxesMdx.Content = Localization.DataSourceInfo_ShowParsedQuery;
            movedAxesTabLayoutRoot.Children.Add(parseMovedAxesMdx);
            Grid.SetRow(parseMovedAxesMdx, 1);
            parseMovedAxesMdx.Checked += new RoutedEventHandler(parseMovedAxesMdx_CheckedChanged);
            parseMovedAxesMdx.Unchecked += new RoutedEventHandler(parseMovedAxesMdx_CheckedChanged);

            movedAxesTab.Content = movedAxesTabLayoutRoot;

            // Закладка "Update Script"
            scriptTab = new TabItem();
            scriptTab.Header = Localization.UpdateScript;
            scriptTab.Style = queryInfoTabControl.Resources["TabControlOutputItem"] as Style;
            queryInfoTabControl.TabCtrl.Items.Add(scriptTab);

            Grid scriptTabLayoutRoot = new Grid() { Margin = new Thickness(0, 0, 0, 0) };
            scriptTabLayoutRoot.RowDefinitions.Add(new RowDefinition());
            scriptTabLayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            updateScript = new Wing.AgOlap.Controls.General.RichTextBox() { AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, IsReadOnly = true };
            updateScript.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            updateScript.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scriptTabLayoutRoot.Children.Add(updateScript);
            Grid.SetRow(updateScript, 0);

            parseUpdateScript = new CheckBox() { Margin = new Thickness(0, 5, 0, 0) };
            parseUpdateScript.Content = Localization.DataSourceInfo_ShowParsedScript;
            scriptTabLayoutRoot.Children.Add(parseUpdateScript);
            Grid.SetRow(parseUpdateScript, 1);
            parseUpdateScript.Checked += new RoutedEventHandler(parseUpdateScript_CheckedChanged);
            parseUpdateScript.Unchecked += new RoutedEventHandler(parseUpdateScript_CheckedChanged);

            scriptTab.Content = scriptTabLayoutRoot;
          
            this.Content = LayoutRoot;

            TabToolBar toolBar = queryInfoTabControl.ToolBar;
            if (toolBar != null)
            {
                RanetToolBarButton copyBtn = new RanetToolBarButton();
                ToolTipService.SetToolTip(copyBtn, Localization.CopyToClipboard_ToolTip);
                copyBtn.Content = UiHelper.CreateIcon(UriResources.Images.Copy16);
                toolBar.Stack.Children.Add(copyBtn);
                copyBtn.Click += new RoutedEventHandler(CopyButton_Click);
            }
        }

        void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (queryInfoTabControl.TabCtrl.SelectedIndex == 0)
            {
                Wing.AgOlap.Features.Clipboard.SetClipboardText(mdxQuery.Text);
                return;
            }
            if (queryInfoTabControl.TabCtrl.SelectedIndex == 1)
            {
                Wing.AgOlap.Features.Clipboard.SetClipboardText(movedAxes_MdxQuery.Text);
                return;
            }
            if (queryInfoTabControl.TabCtrl.SelectedIndex == 2)
            {
                Wing.AgOlap.Features.Clipboard.SetClipboardText(updateScript.Text);
                return;
            }
        }

        void parseUpdateScript_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (m_Args != null)
            {
                if (parseUpdateScript.IsChecked.Value)
                {
                    updateScript.Text = m_Args.Parsed_UpdateScript;
                }
                else
                {
                    updateScript.Text = m_Args.UpdateScript;
                }
            }
        }

        void parseMdx_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (m_Args != null)
            {
                if (parseMdx.IsChecked.Value)
                {
                    mdxQuery.Text = m_Args.Parsed_MDXQuery;
                }
                else
                {
                    mdxQuery.Text = m_Args.MDXQuery;
                }
            }
        }

        void parseMovedAxesMdx_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (m_Args != null)
            {
                if (parseMovedAxesMdx.IsChecked.Value)
                {
                    movedAxes_MdxQuery.Text = m_Args.Parsed_MovedAxes_MDXQuery;
                }
                else
                {
                    movedAxes_MdxQuery.Text = m_Args.MovedAxes_MDXQuery;
                }
            }
        }

        public event RoutedEventHandler OKButtonClick;
        void SwitchButton_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventHandler handler = OKButtonClick;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        DataSourceInfoArgs m_Args;

        /// <summary>
        /// Видимость закладки "Скрипт обновления"
        /// </summary>
        public bool UpdateScriptVisibility
        {
            get {   return scriptTab.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    scriptTab.Visibility = Visibility.Visible;
                }
                else
                {
                    scriptTab.Visibility = Visibility.Collapsed;
                }
            }
        }

        public void Initialize(DataSourceInfoArgs args)
        {
            m_Args = args;
            if (args != null)
            {
                mdxQuery.Text = args.MDXQuery != null ? args.MDXQuery : String.Empty;
                updateScript.Text = args.UpdateScript != null ? args.UpdateScript : String.Empty;
                connectionString.Text = args.ConnectionString != null ? args.ConnectionString : String.Empty;
                movedAxes_MdxQuery.Text = args.MovedAxes_MDXQuery != null ? args.MovedAxes_MDXQuery : String.Empty;
                if (!String.IsNullOrEmpty(args.Parsed_MDXQuery))
                { 
                    parseMdx.Visibility = Visibility.Visible; 
                }
                else
                {
                    parseMdx.Visibility = Visibility.Collapsed;
                }

                if (!String.IsNullOrEmpty(args.Parsed_MovedAxes_MDXQuery))
                {
                    parseMovedAxesMdx.Visibility = Visibility.Visible;
                }
                else
                {
                    parseMovedAxesMdx.Visibility = Visibility.Collapsed;
                }

                if (!String.IsNullOrEmpty(args.Parsed_UpdateScript))
                {
                    parseUpdateScript.Visibility = Visibility.Visible;
                }
                else
                {
                    parseUpdateScript.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                mdxQuery.Text = String.Empty;
                updateScript.Text = String.Empty;
                connectionString.Text = String.Empty;
                movedAxes_MdxQuery.Text = String.Empty;
                parseMdx.Visibility = Visibility.Collapsed;
                parseMovedAxesMdx.Visibility = Visibility.Collapsed;
                parseUpdateScript.Visibility = Visibility.Collapsed;
            }
        }
    }
}
