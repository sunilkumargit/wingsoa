namespace Telerik.Windows.Controls
{
    using System;
    using Telerik.Windows;
    using Wing.Windows.Controls.Telerik.Navigation.Windows.Controls;

    [ScriptableType]
    public class RadTreeViewItemEditedEventArgs : RadRoutedEventArgs
    {
        private object newValue;
        private object oldValue;

        public RadTreeViewItemEditedEventArgs(object newValue, object oldValue, RoutedEvent routedEvent, object source) : base(routedEvent, source)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
        }

        [Obsolete("Please use the NewValue property.", true), ScriptableMember]
        public string NewText
        {
            get
            {
                return (this.newValue as string);
            }
        }

        [ScriptableMember]
        public object NewValue
        {
            get
            {
                return this.newValue;
            }
        }

        [ScriptableMember, Obsolete("Please use the OldValue property.", true)]
        public string OldText
        {
            get
            {
                return (this.oldValue as string);
            }
        }

        [ScriptableMember]
        public object OldValue
        {
            get
            {
                return this.oldValue;
            }
        }
    }
}

