using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modules.ChatServer
{
    public class ChatLogonResult
    {
        private IEnumerable<Object> _users;
        private object _user;

        public ChatLogonResult(IChatUser user, IEnumerable<IChatUser> users)
        {
            _users = users.Select(u => u.GetContact()).ToList();
            _user = user.GetContact();
            Signed = true;
        }

        public IEnumerable<Object> Users
        {
            get
            {
                return _users;
            }
        }

        public Object User
        {
            get { return _user; }
        }

        public bool Signed { get; private set; }
    }
}
