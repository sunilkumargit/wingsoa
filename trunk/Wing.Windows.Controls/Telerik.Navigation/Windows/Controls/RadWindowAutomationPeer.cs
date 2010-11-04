namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    public class RadWindowAutomationPeer : FrameworkElementAutomationPeer, ITransformProvider, IWindowProvider
    {
        public RadWindowAutomationPeer(RadWindow owner) : base((FrameworkElement) owner)
        {
        }

        public void Close()
        {
            this.OwnerAsRadWindow().Close();
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Window;
        }

        protected override string GetClassNameCore()
        {
            return "RadWindow";
        }

        protected override string GetNameCore()
        {
            string nameCore = base.GetNameCore();
            if (string.IsNullOrEmpty(nameCore))
            {
                nameCore = "Rad Window";
            }
            return nameCore;
        }

        public override object GetPattern(PatternInterface patternInterface)
        {
            if ((patternInterface != PatternInterface.Window) && (patternInterface != PatternInterface.Transform))
            {
                return base.GetPattern(patternInterface);
            }
            return this;
        }

        private void GuarantyEnabled()
        {
            if (!base.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }
        }

        public void Move(double x, double y)
        {
            this.GuarantyEnabled();
            if (!this.CanMove)
            {
                throw new InvalidOperationException("Can not move non movable RadWindow.");
            }
            if (double.IsInfinity(x) || double.IsNaN(x))
            {
                throw new ArgumentOutOfRangeException("x");
            }
            if (double.IsInfinity(y) || double.IsNaN(y))
            {
                throw new ArgumentOutOfRangeException("y");
            }
            this.OwnerAsRadWindow().Left = x;
            this.OwnerAsRadWindow().Top = y;
        }

        private RadWindow OwnerAsRadWindow()
        {
            RadWindow radWindow = base.Owner as RadWindow;
            if (radWindow == null)
            {
                throw new InvalidOperationException("The Owner have to be RadWindow");
            }
            return radWindow;
        }

        internal void RaiseAutomationBoundingRectangleChanged(object oldParam, object newParam)
        {
            base.RaisePropertyChangedEvent(AutomationElementIdentifiers.BoundingRectangleProperty, oldParam, newParam);
        }

        internal void RaiseAutomationHasKeyboardFocusChanged(bool newState)
        {
            base.RaisePropertyChangedEvent(AutomationElementIdentifiers.HasKeyboardFocusProperty, !newState, newState);
        }

        internal void RaiseAutomationWindowStateChanged(object oldState, object newState)
        {
            base.RaisePropertyChangedEvent(WindowPatternIdentifiers.WindowVisualStateProperty, oldState, newState);
        }

        public void Resize(double width, double height)
        {
            this.GuarantyEnabled();
            if (!this.CanResize)
            {
                throw new InvalidOperationException("Can not resize non resizable RadWindow.");
            }
            if ((double.IsInfinity(width) || double.IsNaN(width)) || (0.0 > width))
            {
                throw new ArgumentOutOfRangeException("width");
            }
            if ((double.IsInfinity(height) || double.IsNaN(height)) || (0.0 > height))
            {
                throw new ArgumentOutOfRangeException("height");
            }
            this.OwnerAsRadWindow().Width = width;
            this.OwnerAsRadWindow().Height = height;
        }

        public void Rotate(double degrees)
        {
            throw new InvalidOperationException("RadWindow still do not support rotation.");
        }

        public void SetVisualState(WindowVisualState state)
        {
            switch (state)
            {
                case WindowVisualState.Maximized:
                    this.OwnerAsRadWindow().WindowState = WindowState.Maximized;
                    return;

                case WindowVisualState.Minimized:
                    this.OwnerAsRadWindow().WindowState = WindowState.Minimized;
                    return;
            }
            this.OwnerAsRadWindow().WindowState = WindowState.Normal;
        }

        public bool WaitForInputIdle(int milliseconds)
        {
            return false;
        }

        public bool CanMove
        {
            get
            {
                return (this.OwnerAsRadWindow().CanMove && (this.OwnerAsRadWindow().WindowState != WindowState.Maximized));
            }
        }

        public bool CanResize
        {
            get
            {
                return (this.OwnerAsRadWindow().ResizeMode == ResizeMode.CanResize);
            }
        }

        public bool CanRotate
        {
            get
            {
                return false;
            }
        }

        public WindowInteractionState InteractionState
        {
            get
            {
                return WindowInteractionState.Running;
            }
        }

        public bool IsModal
        {
            get
            {
                return this.OwnerAsRadWindow().IsModal;
            }
        }

        public bool IsTopmost
        {
            get
            {
                return this.OwnerAsRadWindow().IsActiveWindow;
            }
        }

        public bool Maximizable
        {
            get
            {
                return (this.OwnerAsRadWindow().ResizeMode == ResizeMode.CanResize);
            }
        }

        public bool Minimizable
        {
            get
            {
                if (this.OwnerAsRadWindow().ResizeMode != ResizeMode.CanResize)
                {
                    return (this.OwnerAsRadWindow().ResizeMode == ResizeMode.CanMinimize);
                }
                return true;
            }
        }

        public WindowVisualState VisualState
        {
            get
            {
                switch (this.OwnerAsRadWindow().WindowState)
                {
                    case WindowState.Minimized:
                        return WindowVisualState.Minimized;

                    case WindowState.Maximized:
                        return WindowVisualState.Maximized;
                }
                return WindowVisualState.Normal;
            }
        }
    }
}

