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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ranet.AgOlap.Controls.MemberChoice.ClientServer;
using Ranet.Olap.Core.Metadata;
using Ranet.AgOlap.Controls.General;
using Ranet.AgOlap.Controls.General.ItemControls;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.Buttons;

namespace Ranet.AgOlap.Controls.MemberChoice.Filter
{
    public partial class OperandItemControl : UserControl
    {
        public OperandItemControl()
        {
            InitializeComponent();

            RanetHotButton remove = new RanetHotButton() { Margin = new Thickness(5, 0, 0, 0) };
            remove.Click += new RoutedEventHandler(remove_Click);
            remove.Width = 18;
            remove.Height = 18;
            remove.Content = UiHelper.CreateIcon(UriResources.Images.RemoveHot16);
            LayoutRoot.Children.Add(remove);
            Grid.SetColumn(remove, 3);

            conditionControl.Items.Add(new ConditionItemControl(ConditionTypes.Contains));
            conditionControl.Items.Add(new ConditionItemControl(ConditionTypes.Equal));
            conditionControl.Items.Add(new ConditionItemControl(ConditionTypes.BeginWith));
            conditionControl.SelectedIndex = 0;

            valueControl.KeyDown += new KeyEventHandler(valueControl_KeyDown);
        }

        public event EventHandler ApplyFilter;
        void valueControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EventHandler handler = ApplyFilter;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler<CustomItemEventArgs> CustomCommandClick;

        void remove_Click(object sender, RoutedEventArgs e)
        {
            EventHandler<CustomItemEventArgs> handler = this.CustomCommandClick;
            if (handler != null)
            {
                handler(this, new CustomItemEventArgs(CustomControlTypes.Delete));
            }
        }

        public FilterOperand Operand
        {
            get
            {
                LevelPropertyItemControl ctrl = parameterControl.SelectedItem as LevelPropertyItemControl;
                if (ctrl != null)
                {
                    // Уровни, для которых присутствует данное свойство
                    List<String> levels = new List<string>();
                    if (m_Parameters != null)
                    {
                        foreach (LevelPropertyInfo info in m_Parameters)
                        {
                            if (info.Name == ctrl.Info.Name)
                            {
                                if (!levels.Contains(info.ParentLevelId))
                                    levels.Add(info.ParentLevelId);
                            }
                        }
                    }
                    return new FilterOperand(ctrl.Info.Name, levels, Condition, Value); 
                }

                MemberPropertyItemControl member_ctrl = parameterControl.SelectedItem as MemberPropertyItemControl;
                if (member_ctrl != null)
                {
                    return new FilterOperand(member_ctrl.Info.UniqueName, null, Condition, Value); 
                }
                return null;
            }
        }

        List<LevelPropertyInfo> m_Parameters = null;

        public void InitParameters(List<LevelPropertyInfo> parameters)
        {
            m_Parameters = parameters;
            parameterControl.Items.Clear();

            MemberPropertyInfo captionProp = new MemberPropertyInfo("Caption", "Caption");
            MemberPropertyInfo nameProp = new MemberPropertyInfo("Name", "Name");
            MemberPropertyInfo uniqueNameProp = new MemberPropertyInfo("UniqueName", "UNIQUE_NAME");

            MemberPropertyItemControl captionItem = new MemberPropertyItemControl(captionProp);
            parameterControl.Items.Add(captionItem);
            MemberPropertyItemControl nameItem = new MemberPropertyItemControl(nameProp);
            parameterControl.Items.Add(nameItem);
            MemberPropertyItemControl uniqueNameItem = new MemberPropertyItemControl(uniqueNameProp);
            parameterControl.Items.Add(uniqueNameItem);

            List<String> used = new List<string>();
            used.Add("Caption");
            used.Add("Name");
            used.Add("UniqueName");

            if (parameters != null)
            {
                foreach (LevelPropertyInfo info in parameters)
                {
                    if (!used.Contains(info.Name))
                    {
                        if (info.IsSystem == false ||
                            (info.IsSystem == true && info.Name.ToLower().StartsWith("key") == true) ||
                            (info.IsSystem == true && info.Name == "CUSTOM_ROLLUP") ||
                            (info.IsSystem == true && info.Name == "UNARY_OPERATOR"))
                        {
                            LevelPropertyItemControl item = new LevelPropertyItemControl(info);
                            parameterControl.Items.Add(item);
                        }
                        used.Add(info.Name);
                    }
                }
            }

            if (parameterControl.Items.Count > 0)
            {
                parameterControl.SelectedIndex = 0;
            }
        }

        ConditionTypes Condition
        {
            get
            {
                ConditionItemControl ctrl = conditionControl.SelectedItem as ConditionItemControl;
                if (ctrl != null)
                {
                    return ctrl.Condition;
                }
                return ConditionTypes.Equal;
            }
        }

        String Value
        {
            get
            {
                return valueControl.Text;
            }
        }
    }
}
