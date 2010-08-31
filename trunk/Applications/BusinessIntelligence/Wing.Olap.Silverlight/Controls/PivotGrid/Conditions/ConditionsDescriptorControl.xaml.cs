/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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

namespace Wing.Olap.Controls.PivotGrid.Conditions
{
    public partial class ConditionsDescriptorControl : UserControl
    {
        public ConditionsDescriptorControl()
        {
            InitializeComponent();

            lblMember.Text = Localization.MemberUniqueName_Label;
            lblConditions.Text = Localization.Conditions_Label;

            MemberControl.KeyDown += new KeyEventHandler(MemberControl_KeyDown);
            ConditionsList.SelectionChanged += new EventHandler<Wing.Olap.Controls.General.SelectionChangedEventArgs<CellCondition>>(ConditionsList_SelectionChanged);
            ConditionDetails.PropertyChanged += new EventHandler(ConditionDetails_PropertyChanged);
        }

        void ConditionDetails_PropertyChanged(object sender, EventArgs e)
        {
            ConditionsList.Refresh();
        }

        void ConditionsList_SelectionChanged(object sender, Wing.Olap.Controls.General.SelectionChangedEventArgs<CellCondition> e)
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
                foreach (var item in ConditionsList.List)
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
