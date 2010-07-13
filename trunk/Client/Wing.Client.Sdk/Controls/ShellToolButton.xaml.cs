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
using System.Windows.Media.Imaging;

namespace Wing.Client.Sdk.Controls
{
    public partial class ShellToolButton : UserControl
    {
        public ShellToolButton()
        {
            InitializeComponent();
        }


        bool _isMouseInside = false;
        Point tempPoint = new Point();

        public event EventHandler<MouseButtonEventArgs> OnButtonClick;

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            _isMouseInside = true;
            Point p = e.GetPosition(this);
            tempPoint.X = p.X / ActualWidth;
            tempPoint.Y = 1;
            brushLight.Center = tempPoint;
            brushLight.GradientOrigin = tempPoint;

        }

        /// <summary>
        /// Title of the toolbar item
        /// </summary>
        public string Title
        {
            get { return itemTitle.Text; }
            set
            {
                itemTitle.Text = value;
                itemTitle.Visibility = String.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }


        /// <summary>
        /// The transition color when we hover over the button
        /// </summary>
        public Color TransitionColor
        {
            get
            {
                return transitionColor.Color;
            }
            set
            {
                transitionColor.Color = value;
                transitionSubColor.Color = Color.FromArgb(64, value.R, value.G, value.B);
            }
        }

        /// <summary>
        /// The source of the image to display for the control
        /// </summary>
        public ImageSource ImageSource
        {
            get { return itemImg.Source; }
            set
            {
                itemImg.Source = ControlHelper.ResolveImageSource(this, value);
            }
        }


        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseInside = true;
            animEnter.Begin();
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseInside = false;
            animLeave.Begin();

        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnButtonClick != null)
                OnButtonClick.Invoke(this, e);
        }
    }
}
