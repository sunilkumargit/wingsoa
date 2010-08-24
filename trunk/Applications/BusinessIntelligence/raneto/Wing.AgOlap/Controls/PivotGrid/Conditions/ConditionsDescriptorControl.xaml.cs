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

namespace Ranet.AgOlap.Controls.PivotGrid.Conditions
{
    public partial class ConditionsDescriptorControl : UserControl
    {
        public ConditionsDescriptorControl()
        {
            InitializeComponent();

            lblMember.Text = Localization.MemberUniqueName_Label;
            lblConditions.Text = Localization.Conditions_Label;

            MemberControl.KeyDown += new KeyEventHandler(MemberControl_KeyDown);
            ConditionsList.SelectionChanged += new EventHandler<Ranet.AgOlap.Controls.General.SelectionChangedEventArgs<CellCondition>>(ConditionsList_SelectionChanged);
            ConditionDetails.PropertyChanged += new EventHandler(ConditionDetails_PropertyChanged);
        }

        void ConditionDetails_PropertyChanged(object sender, EventArgs e)
        {
            ConditionsList.Refresh();
        }

        void ConditionsList_SelectionChanged(object sender, Ranet.AgOlap.Controls.General.SelectionChangedEventArgs<CellCondition> e)
        {
            EndEdit();
            ConditionDetails.Initialize(e.NewValue);
        }

        void MemberControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EndEdit();
            }
        }

        CellConditionsDescriptor m_Descriptor = null;
        public CellConditionsDescriptor Descriptor
        {
            get { return m_Descriptor; }
        }

        public void Initialize(CellConditionsDescriptor descriptor)
        {
            m_Descriptor = descriptor;

            IsEnabled = Descriptor != null;
            MemberControl.Text = Descriptor != null ? Descriptor.MemberUniqueName : String.Empty;
            ConditionsList.Initialize(Descriptor != null ? Descriptor.Conditions.ToList<CellCondition>() : null);
            ConditionDetails.Initialize((Descriptor != null && Descriptor.Conditions.Count > 0) ? Descriptor.Conditions[0] : null);
        }

        public event EventHandler EditEnd;

        public void EndEdit()
        {
            if (Descriptor != null)
            {
                Descriptor.MemberUniqueName = MemberControl.Text;


                if (ConditionDetails.IsEnabled)
                {
                    ConditionDetails.EndEdit();
                }

                Descriptor.Conditions.Clear();
                foreach(var item in ConditionsList.List)
                {
                    Descriptor.Conditions.Add(item);
                }
                ConditionsList.Refresh();

                EventHandler handler = EditEnd;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
    }
}
