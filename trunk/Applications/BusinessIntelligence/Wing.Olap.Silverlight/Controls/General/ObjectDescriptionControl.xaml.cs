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
using Wing.Olap.Core;

namespace Wing.Olap.Controls.General
{
    public partial class ObjectDescriptionControl : UserControl
    {
        public event EventHandler EndEdit;

        bool m_IsReadOnly = false;
        public bool IsReadonly
        {
            get { return m_IsReadOnly; }
            set
            {
                m_IsReadOnly = value;
                txtCaption.IsReadOnly = value;
                txtName.IsReadOnly = value;
                txtDescription.IsReadOnly = value;
            }
        }

        public ObjectDescriptionControl()
        {
            InitializeComponent();

            lblName.Text = Localization.ObjectDescriptionControl_Name + ":";
            lblCaption.Text = Localization.ObjectDescriptionControl_Caption + ":";
            lblDescription.Text = Localization.ObjectDescriptionControl_Description + ":";

            txtName.KeyDown += new KeyEventHandler(OnKeyDown);
            txtCaption.KeyDown += new KeyEventHandler(OnKeyDown);
        }

        void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EventHandler handler = EndEdit;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        public ObjectDescription Object
        {
            get {
                ObjectDescription descr = new ObjectDescription();
                descr.Name = txtName.Text;
                descr.Caption = txtCaption.Text;
                descr.Description = txtDescription.Text;
                return descr; 
            }
            set {
                ObjectDescription descr = value;
                if (descr == null)
                    descr = new ObjectDescription();
                txtName.Text = descr.Name;
                txtCaption.Text = descr.Caption;
                txtDescription.Text = descr.Description;
            }
        }
    }
}
