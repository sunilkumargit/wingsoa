using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Security.Model;
using System.Collections.ObjectModel;

namespace Wing.Security.Impl
{
    class AuthorizationKeyImpl : IAuthorizationKey
    {
        private IAuthorizationService _service;
        private IAuthorizationKey _parent;
        private List<IAuthorizationKey> _items = new List<IAuthorizationKey>();

        public AuthorizationKeyImpl(IAuthorizationService service, IAuthorizationKey parent, String key, String caption, int order)
        {
            _service = service;
            _parent = parent;
            AuthorizationKey = key;
            Caption = caption;
            Order = order;
            SubKeys = new ReadOnlyCollection<IAuthorizationKey>(_items);
        }

        public IAuthorizationService Manager { get { return _service; } }
        public string AuthorizationKey { get; private set; }
        public string Caption { get; private set; }
        public int Order { get; private set; }
        public bool IsRoot
        {
            get { return _parent == null; }
        }

        public IAuthorizationKey Parent { get { return _parent; } }
        public ReadOnlyCollection<IAuthorizationKey> SubKeys { get; private set; }

        public bool IsUserGranted(string username)
        {
            return _service.IsUserAuthorized(username, this.AuthorizationKey);
        }

        internal void AddItem(IAuthorizationKey item)
        {
            _items.Add(item);
        }
    }
}
