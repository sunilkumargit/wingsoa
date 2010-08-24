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
using Ranet.Olap.Core;

namespace Ranet.AgOlap.Controls.General
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
