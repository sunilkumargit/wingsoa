namespace Telerik.Windows.Input
{
    using System;
    using System.Windows;

    internal class MouseClickCounter
    {
        private DateTime initialClickTime;
        private Point lastClickPoint;
        private DateTime lastClickTime;
        private int mouseDownCount;
        private int mouseUpCount;

        public int GetCount(MouseButtonState state)
        {
            if (state == MouseButtonState.Pressed)
            {
                return this.mouseDownCount;
            }
            return this.mouseUpCount;
        }

        private void IncreaseMouseCounter(MouseButtonState state)
        {
            if (state == MouseButtonState.Pressed)
            {
                this.mouseDownCount++;
            }
            else
            {
                this.mouseUpCount++;
            }
        }

        private void Reset()
        {
            this.mouseDownCount = 0;
            this.mouseUpCount = 0;
        }

        public void UpdateCounter(Point position, MouseButtonState state)
        {
            DateTime now = DateTime.Now;
            if (this.lastClickPoint != position)
            {
                this.initialClickTime = now;
                this.lastClickPoint = position;
                this.lastClickTime = now;
                this.Reset();
            }
            else if (now.Subtract(this.lastClickTime) > Mouse.DoubleClickDuration)
            {
                this.initialClickTime = now;
                this.lastClickTime = now;
                this.Reset();
            }
            else if (now.Subtract(this.initialClickTime) > TimeSpan.FromMilliseconds(25.0))
            {
                this.initialClickTime = now;
            }
            else
            {
                return;
            }
            this.IncreaseMouseCounter(state);
        }
    }
}

