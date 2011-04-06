using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modules.ChatServer
{
    public interface IChatContactListProvider
    {
        IEnumerable<IChatUser> GetContacts(String userId);
    }
}
