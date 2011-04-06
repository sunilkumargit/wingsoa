using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modules.ChatServer
{
    public interface IChatUser
    {
        String UserId { get; }
        String Username { get; }
        String Group { get; }
        ChatUserStatus Status { get; }
        Object GetContact();
        void PostMessage(String messageId, Object data);
    }
}
