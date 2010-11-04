namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using Telerik.Windows.Controls.Design;

    public class MouseBinding : InputBinding
    {
        public MouseBinding()
        {
        }

        internal MouseBinding(ICommand command, Telerik.Windows.Controls.MouseAction mouseAction) : this(command, new MouseGesture(mouseAction))
        {
        }

        public MouseBinding(ICommand command, MouseGesture gesture) : base(command, gesture)
        {
        }

        [TypeConverter(typeof(MouseGestureConverter))]
        public override InputGesture Gesture
        {
            get
            {
                return (base.Gesture as MouseGesture);
            }
            set
            {
                if (!(value is MouseGesture))
                {
                    throw new ArgumentException("InputBinding_ExpectedInputGesture");
                }
                base.Gesture = value;
            }
        }

        public Telerik.Windows.Controls.MouseAction MouseAction
        {
            get
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture != null)
                    {
                        return ((MouseGesture) this.Gesture).MouseAction;
                    }
                    return Telerik.Windows.Controls.MouseAction.None;
                }
            }
            set
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture == null)
                    {
                        this.Gesture = new MouseGesture(value);
                    }
                    else
                    {
                        ((MouseGesture) this.Gesture).MouseAction = value;
                    }
                }
            }
        }
    }
}

