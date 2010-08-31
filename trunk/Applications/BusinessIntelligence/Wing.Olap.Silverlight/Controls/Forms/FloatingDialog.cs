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
using Wing.AgOlap.Controls.General;

namespace Wing.AgOlap.Controls.Forms
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
