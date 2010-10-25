using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Wing.Client.Sdk.Controls
{
    public partial class WaitIndicator : UserControl
    {
        #region Member Variables
        private Ellipse[] m_ellipseArray = null;
        private bool _running;
        #endregion

        #region Constructor
        public WaitIndicator()
        {
            InitializeComponent();

            // Create a control array that allows use to easy enumerate the ellipses
            // without resorting to reflection
            m_ellipseArray = new Ellipse[8];
            m_ellipseArray[0] = Ellipse1;
            m_ellipseArray[1] = Ellipse2;
            m_ellipseArray[2] = Ellipse3;
            m_ellipseArray[3] = Ellipse4;
            m_ellipseArray[4] = Ellipse5;
            m_ellipseArray[5] = Ellipse6;
            m_ellipseArray[6] = Ellipse7;
            m_ellipseArray[7] = Ellipse8;

        }
        #endregion

        public bool Started
        {
            get { return _running; }
            set
            {
                _running = value;
                if (!_running)
                    Stop();
                else
                    Start();
            }
        }

        #region Public Functions
        public void Start()
        {
            LayoutRoot.Visibility = Visibility.Visible;
            IndicatorStoryboard.Begin();
        }

        public void Stop()
        {
            LayoutRoot.Visibility = Visibility.Collapsed;
            IndicatorStoryboard.Stop();
        }
        #endregion

        #region Event Handlers
        private void IndicatorStoryboard_Completed(object sender, EventArgs e)
        {
            // Loop through each ellipse (backwards) and set its fill colour to that
            // of the previous ellipse to get the effect of the colours changing
            // around the circle of ellipses
            Brush ellipse8LastBrush = Ellipse8.Fill;

            for (int index = 7; index > 0; index--)
            {
                m_ellipseArray[index].Fill = m_ellipseArray[index - 1].Fill;
            }

            Ellipse1.Fill = ellipse8LastBrush;

            // We need to restart the storyboard timer, at the completion of
            // which this function / event handler will be called again
            IndicatorStoryboard.Begin();
        }
        #endregion
    }
}
