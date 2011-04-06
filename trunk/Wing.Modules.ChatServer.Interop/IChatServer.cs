using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Wing.Pipeline;

namespace Wing.Modules.ChatServer
{
    public interface IChatServer
    {
        IChatUser GetUser(String userId);
        ChatUserStatus GetUserStatus(string UserId);
        IChatRoom GetRoom(String roomId);
        IChatRoom CreateRoom();
        IChatRoom GetRoomFor(String user1, String user2, bool createIfNotExists);
        IEnumerable<IChatUser> GetContacts(IChatUser user);
        IChatUser SignInUser(string login, string name, string group);
    }
}
