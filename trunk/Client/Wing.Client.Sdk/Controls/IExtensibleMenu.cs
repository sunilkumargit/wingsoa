using System;
using System.Collections.ObjectModel;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk.Controls
{
    public interface IExtensibleMenu
    {
        IExtensibleMenuItem CreateItem(String id, String caption);
        IExtensibleMenuItem CreateChildItem(String id, String caption, String parentId);
        IExtensibleMenuItem CreateItem(String id, IGblCommand command);
        IExtensibleMenuItem CreateChildItem(String id, String parentId, IGblCommand command);
        IExtensibleMenuItem GetItem(String id);
        void RemoveItem(String id);
        ReadOnlyCollection<IExtensibleMenuItem> Items { get; }
    }
}