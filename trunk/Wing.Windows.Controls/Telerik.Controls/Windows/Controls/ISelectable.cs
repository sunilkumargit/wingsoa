namespace Telerik.Windows.Controls
{
    using System;
    using Telerik.Windows;

    internal interface ISelectable
    {
        void OnSelected(RadRoutedEventArgs e);
        void OnUnselected(RadRoutedEventArgs e);

        bool IsSelected { get; set; }
    }
}

