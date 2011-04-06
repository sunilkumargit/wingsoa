using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.ObjectModel;
using Wing.Security;
using System.Diagnostics;

namespace Wing.Client
{
    class ClientSessionManager : IClientSessionManager
    {
        private Dictionary<String, IClientSession> _sessions = new Dictionary<string, IClientSession>();
        private Dictionary<String, List<IClientSession>> _sessionsByUser = new Dictionary<string, List<IClientSession>>();
        private Object __lockObject = new Object();

        private const string CLIENT_SESSION_KEY = "_client_session_id_";

        public IClientSession CurrentSession
        {
            [DebuggerStepThrough]
            get { return GetSessionInternal(null, true); }
        }

        public IClientSession[] GetSessionsForUser(string username)
        {
            List<IClientSession> result = null;
            if (_sessionsByUser.TryGetValue(username, out result))
                return result.ToArray();
            return new IClientSession[0];
        }

        public void CloseSession(IClientSession session)
        {
            this.DisposeSession(session as ClientSession);
        }

        public IClientSession GetSession(string sessionId)
        {
            return GetSessionInternal(sessionId, false);
        }

        private IClientSession GetSessionInternal(String sessionId, bool createIfNotExists)
        {
            sessionId = sessionId.HasValue() ? sessionId
                : (HttpContext.Current == null ? (string)ContextData.Get(CLIENT_SESSION_KEY)
                    : HttpContext.Current.Session[CLIENT_SESSION_KEY].AsString());

            if (sessionId.IsEmpty())
                sessionId = Guid.NewGuid().ToString("N").ToLower();

            IClientSession result = null;
            if (!_sessions.TryGetValue(sessionId, out result) && createIfNotExists)
            {
                lock (__lockObject)
                {
                    result = new ClientSession(sessionId, this);
                    if (HttpContext.Current == null)
                        ContextData.Set(CLIENT_SESSION_KEY, sessionId);
                    else
                        HttpContext.Current.Session[CLIENT_SESSION_KEY] = sessionId;
                    _sessions[sessionId] = result;
                }
            }
            else if (!result.isAlive)
                return GetSessionInternal(Guid.NewGuid().ToString("N").ToLower(), true);
            return result;
        }

        public IEnumerable<IClientSession> Sessions
        {
            get { return _sessions.Values.AsParallel().AsEnumerable(); }
        }

        public void CleanUp()
        {
            //
        }

        internal void DisposeSession(ClientSession session)
        {
            session.SetUser(null); // atualizar a lista de usuários.
            _sessionsByUser["[no user]"].Remove(session);
        }

        internal void NotifySessionUserChanged(ClientSession clientSession, IUser old, IUser user)
        {
            var user1 = old == null ? "[no user]" : old.Login;
            var user2 = user == null ? "[no user]" : user.Login;

            SetSessionByUser(user1, clientSession, false);
            SetSessionByUser(user2, clientSession, true);
        }

        private void SetSessionByUser(string login, IClientSession session, bool setting)
        {
            List<IClientSession> buffer = null;
            if (!_sessionsByUser.TryGetValue(login, out buffer))
                buffer = _sessionsByUser[login] = new List<IClientSession>();
            if (setting)
                buffer.Add(session);
            else
                buffer.Remove(session);
        }
    }
}
