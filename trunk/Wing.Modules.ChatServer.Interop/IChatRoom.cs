using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security;
using System.Collections.ObjectModel;

namespace Wing.Modules.ChatServer
{
    public interface IChatRoom
    {
        ReadOnlyObservableCollection<IChatUser> Users { get; }
        String RoomId { get; }
        String UserIndex { get; }
        void PostMessage(String senderKey, String from, String message);
        void RegisterUser(IChatUser user);
        void UnregisterUser(IChatUser user);
    }
}
