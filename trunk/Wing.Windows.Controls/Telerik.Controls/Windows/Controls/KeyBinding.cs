namespace Telerik.Windows.Controls
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using Telerik.Windows.Controls.Design;

    public class KeyBinding : InputBinding
    {
        public KeyBinding()
        {
        }

        public KeyBinding(ICommand command, KeyGesture gesture) : base(command, gesture)
        {
        }

        public KeyBinding(ICommand command, System.Windows.Input.Key key, ModifierKeys modifiers) : base(command, new KeyGesture(key, modifiers))
        {
        }

        [TypeConverter(typeof(KeyGestureConverter))]
        public override InputGesture Gesture
        {
            get
            {
                return (base.Gesture as KeyGesture);
            }
            set
            {
                if (!(value is KeyGesture))
                {
                    throw new ArgumentException("InputBinding_ExpectedInputGesture");
                }
                base.Gesture = value;
            }
        }

        public System.Windows.Input.Key Key
        {
            get
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture != null)
                    {
                        return ((KeyGesture) this.Gesture).Key;
                    }
                    return System.Windows.Input.Key.None;
                }
            }
            set
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture == null)
                    {
                        this.Gesture = new KeyGesture(value, ModifierKeys.None, false);
                    }
                    else
                    {
                        this.Gesture = new KeyGesture(value, ((KeyGesture) this.Gesture).Modifiers, false);
                    }
                }
            }
        }

        public ModifierKeys Modifiers
        {
            get
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture != null)
                    {
                        return ((KeyGesture) this.Gesture).Modifiers;
                    }
                    return ModifierKeys.None;
                }
            }
            set
            {
                lock (InputBinding.DataLock)
                {
                    if (this.Gesture == null)
                    {
                        this.Gesture = new KeyGesture(System.Windows.Input.Key.None, value, false);
                    }
                    else
                    {
                        this.Gesture = new KeyGesture(((KeyGesture) this.Gesture).Key, value, false);
                    }
                }
            }
        }
    }
}

