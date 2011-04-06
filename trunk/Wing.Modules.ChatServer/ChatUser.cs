using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Client;

namespace Wing.Modules.ChatServer
{
    class ChatUser : IChatUser
    {
        private ChatServerImpl _server;
        private List<IClientSession> _sessions = new List<IClientSession>();

        public ChatUser(string userId, String userName, String group, ChatServerImpl server)
        {
            UserId = userId;
            Username = userName;
            Group = group;
            Status = ChatUserStatus.Offline;
            _server = server;
        }

        public string UserId { get; private set; }
        public string Username { get; private set; }
        public ChatUserStatus Status { get; private set; }
        public string Group { get; private set; }
        public IEnumerable<IChatUser> Contacts { get { return _server.GetContacts(this); } }
        public Object GetContact()
        {
            return new { UserId = UserId, Name = Username, Group = Group, Status = (int)Status };
        }

        public void PostMessage(String messageId, Object data)
        {
            foreach (var session in this._sessions)
                session.SendMessage(messageId, data);
        }

        internal void Singin(IClientSession client)
        {
            this.Status = ChatUserStatus.Online;
            if (_sessions.AddIfNotExists(client))
            {
                client.Closing += new EventHandler<EventArgs>(client_Closing);
                BroadcastStatus();
            }
        }

        void client_Closing(object sender, EventArgs e)
        {
            var session = (IClientSession)sender;
            session.Closing -= client_Closing;
            SignOut(session);
        }

        internal void SignOut(IClientSession session)
        {
            if (_sessions.Remove(session))
            {
                if (_sessions.Count == 0)
                    this.Status = ChatUserStatus.Offline;
                BroadcastStatus();
            }
        }

        internal void BroadcastStatus()
        {
            foreach (var user in this.Contacts.Where(u => u.Status == ChatUserStatus.Online))
                user.PostMessage("chat_usr_status", this.GetContact());
        }
    }
}
