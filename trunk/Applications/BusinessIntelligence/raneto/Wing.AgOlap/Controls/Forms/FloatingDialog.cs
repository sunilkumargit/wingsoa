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
using Ranet.AgOlap.Controls.General;

namespace Ranet.AgOlap.Controls.Forms
{
    public class FloatingDialog : FloatingDialogBase
    {
        public double MinWidth
        {
            set {
                ctrl.MinWidth = value;
            }
            get {
                return ctrl.MinWidth;
            }
        }

        public double MinHeight
        {
            set
            {
                ctrl.MinHeight = value;
            }
            get
            {
                return ctrl.MinHeight;
            }
        }

        public double Width
        {
            set {
                ctrl.Width = value;
            }
            get {
                return ctrl.Width;
            }
        }

        public double Height
        {
            set
            {
                ctrl.Height = value;
            }
            get
            {
                return ctrl.Height;
            }
        }

        public String Caption
        {
            get {
                return ctrl.CaptionTextBlock.Text;
            }
            set {
                ctrl.CaptionTextBlock.Text = value;
            }
        }

        public FloatingDialog()
        {
            ctrl = new FloatingWindowControl();
            ctrl.Close += new EventHandler(ctrl_Close);
        }

        void ctrl_Close(object sender, EventArgs e)
        {
            Close();
        }
        
        FloatingWindowControl ctrl;

        protected override FrameworkElement GetContent()
        {
            return ctrl;
        }

        public void SetContent(UIElement content)
        {
            ctrl.ContentRoot.Children.Clear();
            ctrl.ContentRoot.Children.Add(content);
            ctrl.UpdateLayout();
        }

        protected override void OnClickOutside()
        {
            if (base.PopUpStyle == FloatingDialogStyle.ModalDimmed)
                Close();
        }
    }
}
