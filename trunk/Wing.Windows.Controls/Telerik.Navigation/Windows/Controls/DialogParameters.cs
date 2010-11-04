namespace Telerik.Windows.Controls
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    [StructLayout(LayoutKind.Sequential)]
    public struct DialogParameters
    {
        public object Content { get; set; }
        public object Header { get; set; }
        public EventHandler<WindowClosedEventArgs> Closed { get; set; }
        public EventHandler Opened { get; set; }
        public object OkButtonContent { get; set; }
        public object CancelButtonContent { get; set; }
        public object IconContent { get; set; }
        public Telerik.Windows.Controls.Theme Theme { get; set; }
        public ContentControl Owner { get; set; }
        public Style WindowStyle { get; set; }
        public Style ContentStyle { get; set; }
        public Brush ModalBackground { get; set; }
        public string DefaultPromptResultValue { get; set; }
        public static bool operator ==(DialogParameters one, DialogParameters two)
        {
            return one.IsEqual(two);
        }

        public static bool operator !=(DialogParameters one, DialogParameters two)
        {
            return !one.IsEqual(two);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(DialogParameters))
            {
                return false;
            }
            DialogParameters newObj = (DialogParameters) obj;
            return this.IsEqual(newObj);
        }

        public override int GetHashCode()
        {
            return (((((((((this.Content.GetHashCode() + this.IconContent.GetHashCode()) + this.Opened.GetHashCode()) + this.Closed.GetHashCode()) + this.OkButtonContent.GetHashCode()) + this.CancelButtonContent.GetHashCode()) + this.ModalBackground.GetHashCode()) + this.Theme.GetHashCode()) + this.Header.GetHashCode()) + this.DefaultPromptResultValue.GetHashCode());
        }

        internal bool IsEqual(DialogParameters newObj)
        {
            return (((((this.Header == newObj.Header) && (this.IconContent == newObj.IconContent)) && ((this.Content == newObj.Content) && (this.Opened == newObj.Opened))) && (((this.Closed == newObj.Closed) && (this.OkButtonContent == newObj.OkButtonContent)) && ((this.CancelButtonContent == newObj.CancelButtonContent) && (this.ModalBackground == newObj.ModalBackground)))) && ((this.Theme == newObj.Theme) && (this.DefaultPromptResultValue == newObj.DefaultPromptResultValue)));
        }
    }
}

