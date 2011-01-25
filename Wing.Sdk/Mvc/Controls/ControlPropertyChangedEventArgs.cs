using System;

namespace Wing.Mvc.Controls
{
    public class ControlPropertyChangedEventArgs : EventArgs
    {
        internal ControlPropertyChangedEventArgs(HtmlObject target, ControlProperty property, Object currentvalue, Object newValue)
        {
            this.Target = target;
            this.Property = property;
            this.OldValue = currentvalue;
            this.NewValue = newValue;
        }

        public HtmlObject Target { get; private set; }
        public ControlProperty Property { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
    }
}
