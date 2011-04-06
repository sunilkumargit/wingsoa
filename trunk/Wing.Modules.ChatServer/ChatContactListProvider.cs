using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security;

namespace Wing.Modules.ChatServer
{
    class ChatContactListProvider : IChatContactListProvider
    {
        Dictionary<String, List<IChatUser>> _contacts = new Dictionary<string, List<IChatUser>>();
        private ChatServerImpl _server;
        private Object __lockObject = new Object();

        public ChatContactListProvider(ChatServerImpl server)
        {
            _server = server;
        }

        public IEnumerable<IChatUser> GetContacts(string userId)
        {
            var account = ServiceLocator.GetInstance<IAccountService>();
            var user = account.GetUser(userId);
            if (user != null)
            {
                List<IChatUser> result = null;
                if (!_contacts.TryGetValue(user.Login.ToLower(), out result))
                {
                    if (!_contacts.TryGetValue(user.Login.ToLower(), out result))
                    {
                        result = new List<IChatUser>();
                        foreach (var usr in account.GetUsersInSchema(user.Schema.Id).Where(u => u.Login != user.Login))
                            result.Add(_server.GetOrCreateUser(usr.Login, usr.Name, usr.Schema.Name));
                        _contacts[user.Login.ToLower()] = result;
                    }
                }
                return result;
            }
            return null;
        }
    }
}
