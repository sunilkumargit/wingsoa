namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using Telerik.Windows;
    using Telerik.Windows.Controls.Design;
    using Telerik.Windows.Input;

    [TypeConverter(typeof(MouseGestureConverter))]
    public class MouseGesture : InputGesture
    {
        public MouseGesture()
        {
        }

        public MouseGesture(Telerik.Windows.Controls.MouseAction mouseAction)
            : this(mouseAction, ModifierKeys.None)
        {
        }

        public MouseGesture(Telerik.Windows.Controls.MouseAction mouseAction, ModifierKeys modifiers)
        {
            this.Modifiers = modifiers;
            this.MouseAction = mouseAction;
        }

        internal static Telerik.Windows.Controls.MouseAction GetMouseAction(RoutedEventArgs inputArgs)
        {
            Telerik.Windows.Controls.MouseAction none = Telerik.Windows.Controls.MouseAction.None;
            Telerik.Windows.Input.MouseButtonEventArgs telerikMouseButtonArgs = inputArgs as Telerik.Windows.Input.MouseButtonEventArgs;
            if (telerikMouseButtonArgs == null)
            {
                if (inputArgs is Telerik.Windows.Input.MouseWheelEventArgs)
                {
                    return Telerik.Windows.Controls.MouseAction.WheelClick;
                }
                TestMouseEventArgs testEventArgs = inputArgs as TestMouseEventArgs;
                if (testEventArgs != null)
                {
                    switch (testEventArgs.Button)
                    {
                        case MouseButton.Left:
                            return Telerik.Windows.Controls.MouseAction.LeftClick;

                        case MouseButton.Middle:
                            return none;

                        case MouseButton.Right:
                            return Telerik.Windows.Controls.MouseAction.RightClick;
                    }
                }
                return none;
            }
            if ((telerikMouseButtonArgs.ClickCount <= 1) || (telerikMouseButtonArgs.ButtonState != MouseButtonState.Released))
            {
                switch (telerikMouseButtonArgs.ChangedButton)
                {
                    case MouseButton.Left:
                        return Telerik.Windows.Controls.MouseAction.LeftClick;

                    case MouseButton.Middle:
                        return Telerik.Windows.Controls.MouseAction.MiddleClick;

                    case MouseButton.Right:
                        return Telerik.Windows.Controls.MouseAction.RightClick;
                }
            }
            else
            {
                switch (telerikMouseButtonArgs.ChangedButton)
                {
                    case MouseButton.Left:
                        return Telerik.Windows.Controls.MouseAction.LeftDoubleClick;

                    case MouseButton.Right:
                        return Telerik.Windows.Controls.MouseAction.RightDoubleClick;
                }
                return Telerik.Windows.Controls.MouseAction.LeftDoubleClick;
            }
            return none;
        }

        internal static bool IsDefinedMouseAction(Telerik.Windows.Controls.MouseAction mouseAction)
        {
            return ((mouseAction >= Telerik.Windows.Controls.MouseAction.None) && (mouseAction <= Telerik.Windows.Controls.MouseAction.MiddleDoubleClick));
        }

        public override bool Matches(object targetElement, RoutedEventArgs inputEventArgs)
        {
            Telerik.Windows.Controls.MouseAction mouseAction = GetMouseAction(inputEventArgs);
            if (mouseAction == Telerik.Windows.Controls.MouseAction.None)
            {
                return false;
            }
            TestMouseEventArgs testEventArgs = inputEventArgs as TestMouseEventArgs;
            ModifierKeys modifiers = (testEventArgs == null) ? System.Windows.Input.Keyboard.Modifiers : testEventArgs.Modifiers;
            return ((this.MouseAction == mouseAction) && (this.Modifiers == modifiers));
        }

        public ModifierKeys Modifiers { get; set; }

        public Telerik.Windows.Controls.MouseAction MouseAction { get; set; }
    }
}

