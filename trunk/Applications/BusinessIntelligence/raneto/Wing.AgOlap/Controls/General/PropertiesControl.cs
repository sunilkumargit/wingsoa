/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see
  	<http://www.gnu.org/licenses/> 
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
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
using Ranet.AgOlap.Controls.General.Tree;
using Ranet.AgOlap.Controls.General.ItemControls;
using System.Collections.Generic;
using Ranet.Olap.Core.Data;
using Ranet.Olap.Core.Providers;

namespace Ranet.AgOlap.Controls.General
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
