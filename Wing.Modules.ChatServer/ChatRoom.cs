using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wing.Modules.ChatServer
{
    class ChatRoom : IChatRoom
    {
        private ChatServerImpl _server;
        private ObservableCollection<IChatUser> _users;
        private String _userIndex;

        public ChatRoom(ChatServerImpl server)
        {
            _server = server;
            _users = new ObservableCollection<IChatUser>();
            Users = new ReadOnlyObservableCollection<IChatUser>(_users);
            RoomId = Guid.NewGuid().ToString("N");
        }

        public ReadOnlyObservableCollection<IChatUser> Users { get; private set; }

        public string RoomId { get; private set; }

        public void PostMessage(string senderKey, string from, string message)
        {
            var item = new RoomMessageItem()
            {
                Message = message,
                RoomId = this.RoomId,
                Sender = from,
                To = _users.JoinString(x => x.UserId, ";"),
                SenderKey = senderKey
            };
            foreach (var user in _users)
                user.PostMessage("chat_room_msg", item);
        }

        public string UserIndex { get { return _userIndex; } }

        public void RegisterUser(IChatUser user)
        {
            if (!_users.Any(u => u.UserId == user.UserId))
            {
                _users.Add(user);
                BuildIndex();
            }
        }

        public void UnregisterUser(IChatUser user)
        {
            user = _users.FirstOrDefault(u => u.UserId == user.UserId);
            if (user != null)
            {
                _users.Remove(user);
                BuildIndex();
            }
        }

        private void BuildIndex()
        {
            _userIndex = CreateIndex(_users.Select(u => u.UserId));
        }

        public static string CreateIndex(IEnumerable<String> users)
        {
            return users.OrderBy(u => u).JoinString(u => u, "/").ToLower();
        }
    }
}
