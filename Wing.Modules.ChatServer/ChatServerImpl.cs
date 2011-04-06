using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Pipeline;

namespace Wing.Modules.ChatServer
{
    class ChatServerImpl : IChatServer
    {
        private Dictionary<String, IChatRoom> _rooms = new Dictionary<string, IChatRoom>();
        private Dictionary<String, IChatUser> _users = new Dictionary<string, IChatUser>();
        private Object __lockObject = new object();

        public const string CHAT_USER_SESSION_KEY = "_chat_user_";

        public IChatUser SignInUser(String userId, String name, String group)
        {
            var user = GetOrCreateUser(userId, name, group);
            user.Singin(ClientSession.Current);
            return user;
        }

        public IChatRoom GetRoom(string roomId)
        {
            IChatRoom result = null;
            _rooms.TryGetValue(roomId, out result);
            return result;
        }

        public IChatRoom CreateRoom()
        {
            var room = new ChatRoom(this);
            _rooms[room.RoomId] = room;
            return room;
        }

        public IChatRoom GetRoomFor(string user1, string user2, bool createIfNotExists)
        {
            var index = ChatRoom.CreateIndex(new String[] { user1, user2 });
            var room = _rooms.Values.FirstOrDefault(r => r.UserIndex == index);
            if (room == null && createIfNotExists)
            {
                var chatUser1 = GetUser(user1);
                var chatuser2 = GetUser(user2);
                if (chatUser1 != null && chatuser2 != null)
                {
                    room = CreateRoom();
                    room.RegisterUser(chatUser1);
                    room.RegisterUser(chatuser2);
                }
            }
            return room;
        }

        public IChatUser GetUser(String userId)
        {
            IChatUser result = null;
            _users.TryGetValue(userId.ToLower(), out result);
            return result;
        }

        public IEnumerable<IChatUser> GetContacts(IChatUser user)
        {
            return ServiceLocator.GetInstance<IChatContactListProvider>().GetContacts(user.UserId);
        }

        public ChatUserStatus GetUserStatus(String userId)
        {
            return ChatUserStatus.Online;
        }

        internal ChatUser GetOrCreateUser(String userId, String name, String group)
        {
            lock (__lockObject)
            {
                var user = (ChatUser)GetUser(userId);
                if (user == null)
                    _users[userId.ToLower()] = user = new ChatUser(userId, name, group, this);
                return user;
            }
        }
    }
}
