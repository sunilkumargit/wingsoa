using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security;
using Wing.Pipeline;
using System.Collections;

namespace Wing.Client
{
    class ClientSession : IClientSession
    {
        private IUser _user;
        private Hashtable _data = new Hashtable();
        private ClientSessionManager _manager;

        public ClientSession(string sessionId, ClientSessionManager manager)
        {
            SessionId = sessionId.IsEmpty() ?
                Guid.NewGuid().ToString("N").ToLower() :
                sessionId;
            _manager = manager;
            isAlive = true;
        }

        public string SessionId { get; private set; }
        public bool isAlive { get; internal set; }
        public DateTime Created { get; private set; }
        public event EventHandler<EventArgs> Closing;
        public event EventHandler<EventArgs> Closed;
        public void SetUser(IUser user)
        {
            if (user == _user)
                return;
            if (user != null && _user != null && user.Login == _user.Login)
                return;
            var old = _user;
            _user = user;
            _manager.NotifySessionUserChanged(this, old, user);
        }

        public void Close()
        {
            if (this.Closing != null) this.Closing.Invoke(this, new EventArgs());
            isAlive = false;
            _manager.DisposeSession(this);
            if (this.Closed != null) this.Closed.Invoke(this, new EventArgs());
        }

        public IUser User
        {
            get { return _user; }
        }

        public bool IsAuthenticated
        {
            get { return _user != null; }
        }

        public void SendMessage(string messageId, object data)
        {
            ServiceLocator.GetInstance<IPipelineManager>()
                .SendMessage(this, messageId, data);
        }

        public object GetData(string key)
        {
            return _data[key];
        }

        public void SetData(string key, object data)
        {
            _data[key] = data;
        }

        public bool Exists(string key)
        {
            return _data.ContainsKey(key);
        }

        public void RemoveData(string key)
        {
            _data.Remove(key);
        }

        public int DataCount
        {
            get { return _data.Count; }
        }

        public void ClearData()
        {
            _data.Clear();
        }
    }
}
