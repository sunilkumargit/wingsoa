namespace Telerik.Windows.Controls.OutlookBar
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Telerik.Windows.Controls;

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification="The interface is used internally and there is no need to make it public.")]
    internal interface IOutlookBarItem
    {
        OutlookBarItemPosition Location { get; set; }
    }
}

