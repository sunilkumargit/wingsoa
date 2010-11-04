namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    internal interface IToolWindowAware
    {
        bool IsInToolWindow { get; set; }
    }
}

