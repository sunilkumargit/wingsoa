namespace Telerik.Windows.Controls
{
    using System;
    using System.Windows.Controls;

    public interface IFrame
    {
        string NavigationIdentifier { get; set; }

        Panel ParentContainer { get; set; }

        string Title { get; set; }
    }
}

