/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
using Wing.AgOlap.Controls.General.Tree;
using Wing.AgOlap.Controls.General.ItemControls;
using System.Collections.Generic;
using Wing.Olap.Core.Data;
using Wing.Olap.Core.Providers;

namespace Wing.AgOlap.Controls.General
{
    public class PropertiesControl : UserControl
    {
        Grid LayoutRoot;
        ContentControl m_PropertiesOwnerControl;
        PropertiesListControl m_PropertiesList;
        public PropertiesListControl PropertiesList
        {
            get { return m_PropertiesList; }
        }

        public new Brush Background
        {
            get { return LayoutRoot.Background; }
            set { LayoutRoot.Background = value; }
        }

        public PropertiesControl()
        { 
            LayoutRoot = new Grid();

            LayoutRoot = new Grid();
            LayoutRoot.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            LayoutRoot.RowDefinitions.Add(new RowDefinition());

            m_PropertiesOwnerControl = new ContentControl();
            m_PropertiesOwnerControl.Margin = new Thickness(3);
            m_PropertiesOwnerControl.VerticalAlignment = VerticalAlignment.Center;
            m_PropertiesOwnerControl.VerticalContentAlignment = VerticalAlignment.Center;
            m_PropertiesOwnerControl.Visibility = Visibility.Collapsed;
            LayoutRoot.Children.Add(m_PropertiesOwnerControl);

            m_PropertiesList = new PropertiesListControl();
            m_PropertiesList.Margin = new Thickness(3);
            LayoutRoot.Children.Add(m_PropertiesList);
            Grid.SetRow(m_PropertiesList, 1);
            m_PropertiesList.Clear();

            this.Content = LayoutRoot;
        }

        public bool IsLoading
        {
            set {
                if (value)
                {
                    m_PropertiesOwnerControl.Visibility = Visibility.Visible;
                    WaitTreeControl ctrl = new WaitTreeControl() { Text = Localization.Loading };
                    m_PropertiesOwnerControl.Content = ctrl;
                    m_PropertiesList.Clear();
                }
                else
                {
                    m_PropertiesOwnerControl.Visibility = Visibility.Visible;
                    m_PropertiesOwnerControl.Content = null;
                    m_PropertiesList.Clear();
                }
            }
        }

        public void Clear()
        {
            m_PropertiesOwnerControl.Content = null;
            m_PropertiesList.Clear();
        }

        public void Initialize(MemberData member)
        {
            m_PropertiesOwnerControl.Visibility = Visibility.Visible;
            m_PropertiesOwnerControl.Content = new MemberItemControl(member);
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
            m_PropertiesList.Initialize(list);
        }

        public void Initialize(MemberInfo member)
        {
            m_PropertiesOwnerControl.Visibility = Visibility.Visible;
            m_PropertiesOwnerControl.Content = new MemberInfoItemControl(member);

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

            m_PropertiesList.Initialize(list);
        }
    }
}
