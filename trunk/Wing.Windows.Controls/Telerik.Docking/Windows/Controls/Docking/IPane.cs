namespace Telerik.Windows.Controls.Docking
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
    internal interface IPane
    {
        void RemoveFromParent();

        bool CanDockInDocumentHost { get; set; }

        bool CanFloat { get; set; }

        bool CanUserClose { get; set; }

        bool CanUserPin { get; set; }

        bool IsDockable { get; }

        bool IsFloating { get; }

        bool IsHidden { get; set; }

        bool IsPinned { get; set; }

        string Title { get; set; }
    }
}

