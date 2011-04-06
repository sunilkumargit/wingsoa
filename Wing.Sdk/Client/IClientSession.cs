using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security;

namespace Wing.Client
{
    public interface IClientSession
    {
        String SessionId { get; }
        bool isAlive { get; }
        DateTime Created { get; }
        void SetUser(IUser user);
        IUser User { get; }
        bool IsAuthenticated { get; }
        void SendMessage(String messageId, Object data);
        Object GetData(string key);
        void SetData(string key, object data);
        bool Exists(string key);
        void RemoveData(string key);
        int DataCount { get; }
        void ClearData();
        void Close();
        event EventHandler<EventArgs> Closing;
        event EventHandler<EventArgs> Closed;
    }
}
