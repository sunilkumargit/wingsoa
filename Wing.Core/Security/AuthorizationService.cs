using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;
using Wing.Security.Model;
using Wing.Security.Impl;

namespace Wing.Security
{
    class AuthorizationService : IAuthorizationService
    {
        private IEntityStore _store;
        private Dictionary<String, AuthorizationKeyImpl> _keys = new Dictionary<string, AuthorizationKeyImpl>();
        private Dictionary<String, AuthorizationStatus> _cache = new Dictionary<string, AuthorizationStatus>();

        public AuthorizationService()
        {
            _store = ServiceLocator.GetInstance<IEntityStoreManager>().CreateStore("authorization");
            _store.RegisterEntity<AuthorizationKeyStatusModel>();
            _keys["/"] = new AuthorizationKeyImpl(this, null, "/", "Raiz", 0);
            SetStatus("user", "admin", "/", AuthorizationStatus.Grant);
        }

        private AuthorizationStatus GetStatus(String type, string name, IAuthorizationKey key)
        {
            var cacheKey = String.Format("{0}:{1}:{2}", type, name, key.AuthorizationKey);
            AuthorizationStatus result = AuthorizationStatus.Deny;
            if (_cache.TryGetValue(cacheKey, out result))
            {
                var model = GetModel(type, name, key.AuthorizationKey, false);
                if (model != null)
                    result = model.Status;
                _cache[cacheKey] = result;
            }
            return result;
        }

        public bool IsUserAuthorized(string username, string authorizationKey)
        {
            return GetStatus("user", username, GetKeyInternal(authorizationKey, true)) == AuthorizationStatus.Grant;
        }

        public IAuthorizationKey CreateKey(string keyPath, string caption, int order = 0)
        {
            Assert.EmptyString(keyPath, "keyPath");
            var path = keyPath.Split(new Char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var pathStr = "/";
            AuthorizationKeyImpl parent = GetKeyInternal("/", true);
            for (var i = 0; i < path.Length; i++)
            {
                var part = path[i];
                pathStr += part;
                var key = GetKeyInternal(pathStr);
                if (key == null)
                {
                    key = i < path.Length - 1 ?
                        new AuthorizationKeyImpl(this, parent, pathStr, part, 0) :
                        new AuthorizationKeyImpl(this, parent, pathStr, caption, order);
                    this._keys[key.AuthorizationKey] = key;
                }
                parent.AddItem(key);
                parent = key;
            }
            return parent;
        }

        public IAuthorizationKey CreateKey(string parentKey, string key, string caption, int order = 0)
        {
            return CreateKey(String.Format("{0}/{1}", parentKey, key), caption, order);
        }

        private String NormalizePath(String path)
        {
            path = path.Replace("//", "/");
            if (path == "/")
                return path;
            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            if (!path.StartsWith("/"))
                path = "/" + path;
            return path.ToLower();
        }

        private AuthorizationKeyImpl GetKeyInternal(string basePath, bool throwException = false)
        {
            if (basePath == "")
                basePath = "/";
            AuthorizationKeyImpl result = null;
            _keys.TryGetValue(NormalizePath(basePath), out result);
            if (result == null && throwException)
                throw new InvalidOperationException("Authorization key not found: " + basePath);
            return result;
        }

        public IAuthorizationKey GetKey(string basePath = "")
        {
            return GetKeyInternal(basePath, false);
        }

        private AuthorizationKeyStatusModel GetModel(String type, String name, String authorizationKey, bool createIfNotExists)
        {
            Assert.EmptyString(type, "type");
            Assert.EmptyString(name, "name");
            Assert.EmptyString(authorizationKey, "authorizationKey");
            var qry = _store.CreateQuery<AuthorizationKeyStatusModel>();
            qry.AddFilterEqual("AccountType", type);
            qry.AddFilterEqual("AccountName", name);
            qry.AddFilterEqual("AuthorizationKey", authorizationKey);
            var result = qry.FindFirst();
            if (result == null && createIfNotExists)
            {
                result = new AuthorizationKeyStatusModel()
                {
                    AccountType = type,
                    AccountName = name,
                    AuthorizationKey = authorizationKey,
                    Status = AuthorizationStatus.Default
                };
            }
            return result;
        }

        private void SetStatus(string type, String name, String authorizationKey, AuthorizationStatus status)
        {
            var key = GetKeyInternal(authorizationKey, true);
            var cacheKey = String.Format("{0}:{1}:{2}", type, name, authorizationKey);
            var model = GetModel(type, name, authorizationKey, true);
            model.Status = status;
            if (_store.Save(model))
                this._cache[cacheKey] = status;

        }

        public void SetRoleStatus(string rolename, string authorizationKey, AuthorizationStatus status)
        {
            SetStatus("role", rolename, authorizationKey, status);
        }

        public void SetUserStatus(string username, string authorizationKey, AuthorizationStatus status)
        {
            SetStatus("user", username, authorizationKey, status);
        }

        public AuthorizationStatus GetRoleStatus(string rolename, string authorizationKey)
        {
            return GetStatus("role", rolename, GetKeyInternal(authorizationKey, true));
        }

        public AuthorizationStatus GetUserStatus(string username, string authorizationKey)
        {
            return GetStatus("role", username, GetKeyInternal(authorizationKey, true));
        }
    }
}
