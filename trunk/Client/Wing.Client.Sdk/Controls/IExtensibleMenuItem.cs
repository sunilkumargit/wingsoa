using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk.Controls
{
    public interface IExtensibleMenuItem : INotifyPropertyChanged
    {
        String ItemId { get; }
        String Caption { get; set; }
        bool IsEnabled { get; set; }
        bool IsVisible { get; set; }
        event EventHandler OnSelect;
        ReadOnlyCollection<IExtensibleMenuItem> Items { get; }
        void BindCommand(IGblCommand command);
        void UnbindCommand();
        IGblCommand Command { get; }
        bool RedirectSelectionToFirstChild { get; set; }
        void Select();
    }
}
