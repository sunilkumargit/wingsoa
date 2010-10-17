using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;

namespace Wing.Client.Sdk.Controls.Behaviors
{
    public class DoubleClick : TriggerBase<UIElement>
    {
        private readonly DispatcherTimer _timer;
        private Point _clickPosition;

        public DoubleClick()
        {
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 300)
            };

            _timer.Tick += OnTimerTick;
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseLeftButtonUp += new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
        }

        void AssociatedObject_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;

            if (_timer.IsEnabled)
            {
                _timer.Stop();
                Point position = e.GetPosition(element);

                if (Math.Abs(_clickPosition.X - position.X) < 1 && Math.Abs(_clickPosition.Y - position.Y) < 1)
                {
                    InvokeActions(null);
                }
            }
            else
            {
                _timer.Start();
                _clickPosition = e.GetPosition(element);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseLeftButtonUp -= new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp);
            if (_timer.IsEnabled)
                _timer.Stop();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
        }
    }
}
