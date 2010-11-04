namespace Telerik.Windows.Controls.Docking
{
    using System.Collections.Generic;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;

    public interface ISplitItem
    {
        IEnumerable<RadPane> EnumeratePanes();

        System.Windows.Controls.Control Control { get; }

        RadSplitContainer ParentContainer { get; }
    }
}

