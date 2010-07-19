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

namespace Wing.Client.Sdk.Controls
{
    public partial class ToolButton : UserControl
    {
        public ToolButton()
        {
            InitializeComponent();
            this.TitleForeground = new SolidColorBrush(Colors.Black);
        }


        Point tempPoint = new Point();

        public event EventHandler<MouseButtonEventArgs> OnButtonClick;

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
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
            get { return ButtonTitle.Text; }
            set
            {
                ButtonTitle.Text = value;
                ButtonTitle.Visibility = String.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Brush TitleForeground { get { return ButtonTitle.Foreground; } set { ButtonTitle.Foreground = value; } }


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
            get { return ButtonImage.Source; }
            set { ButtonImage.Source = ControlHelper.ResolveImageSource(this, value); }
        }

        public Stretch ImageStretch
        {
            get { return ButtonImage.Stretch; }
            set { ButtonImage.Stretch = value; }
        }

        public double ImageWidth
        {
            get { return ButtonImage.Width; }
            set { ButtonImage.Width = value; }
        }

        public double ImageHeight
        {
            get { return ButtonImage.Height; }
            set { ButtonImage.Height = value; }
        }

        public AlignmentX ImageAlign
        {
            get { return Grid.GetColumn(ButtonImage) == 0 ? AlignmentX.Left : AlignmentX.Right; }
            set
            {
                if (value == AlignmentX.Left)
                {
                    Grid.SetColumn(ButtonImage, 0);
                    Grid.SetColumn(ButtonTitle, 1);
                }
                else
                {
                    Grid.SetColumn(ButtonImage, 1);
                    Grid.SetColumn(ButtonTitle, 0);
                }
            }
        }

        public double TitleFontSize
        {
            get { return ButtonTitle.FontSize; }
            set { ButtonTitle.FontSize = value; }
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            animEnter.Begin();
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            animLeave.Begin();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnButtonClick != null)
                OnButtonClick.Invoke(this, e);
        }
    }
}
