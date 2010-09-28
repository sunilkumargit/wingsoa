using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Generic;
using Wing.Utils;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk.Controls
{
    public interface IExtensibleMenu
    {
        IExtensibleMenuItem CreateItem(String id, String caption);
        IExtensibleMenuItem CreateChildItem(String id, String caption, String parentId);
        IExtensibleMenuItem CreateItem(String id, IGlobalCommand command);
        IExtensibleMenuItem CreateChildItem(String id, String parentId, IGlobalCommand command);
        IExtensibleMenuItem GetItem(String id);
        void RemoveItem(String id);
        ReadOnlyCollection<IExtensibleMenuItem> Items { get; }
    }
}