using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Wing.Client
{
    public interface IClientSessionManager
    {
        IClientSession CurrentSession { get; }
        IClientSession[] GetSessionsForUser(String username);
        IClientSession GetSession(String sessionId);
        IEnumerable<IClientSession> Sessions { get; }
        void CloseSession(IClientSession session);
        void CleanUp();
    }
}
