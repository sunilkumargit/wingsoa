namespace Telerik.Windows.Controls
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    internal class TripleClickBehavior : IDisposable
    {
        private int clickCount;
        internal TextBox textBox;
        internal DispatcherTimer tripleClickTimer;

        internal event EventHandler TripleClick;

        public TripleClickBehavior(TextBox textBox)
        {
            this.textBox = textBox;
            this.textBox.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.textBox_MouseLeftButtonDown), true);
        }

        public void Dispose()
        {
            this.textBox.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(this.textBox_MouseLeftButtonDown));
            this.textBox = null;
            if (this.tripleClickTimer != null)
            {
                this.tripleClickTimer.Tick -= new EventHandler(this.tripleClickTimer_Tick);
                this.tripleClickTimer = null;
            }
        }

        private void OnTripleClick()
        {
            if (this.TripleClick != null)
            {
                this.TripleClick(this.textBox, new EventArgs());
            }
        }

        internal void StartClickTimer()
        {
            if (this.tripleClickTimer != null)
            {
                this.tripleClickTimer.Stop();
            }
            this.tripleClickTimer = new DispatcherTimer();
            this.tripleClickTimer.Interval = TimeSpan.FromMilliseconds(300.0);
            this.tripleClickTimer.Tick += new EventHandler(this.tripleClickTimer_Tick);
            this.tripleClickTimer.Start();
        }

        private void textBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.StartClickTimer();
            if (this.clickCount > 1)
            {
                this.OnTripleClick();
            }
            this.clickCount++;
        }

        private void tripleClickTimer_Tick(object sender, EventArgs e)
        {
            this.tripleClickTimer.Stop();
            this.clickCount = 0;
        }
    }
}

