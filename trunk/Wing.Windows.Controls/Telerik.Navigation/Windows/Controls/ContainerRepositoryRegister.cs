namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows;

    internal static class ContainerRepositoryRegister
    {
        internal static readonly DependencyProperty IsStoredProperty = DependencyProperty.RegisterAttached("IsStored", typeof(bool), typeof(ContainerRepositoryRegister), null);
    }
}

