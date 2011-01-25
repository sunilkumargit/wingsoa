using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;
using Wing.Server.Modules.ServerStorage;

namespace Wing.EntityStore
{
    class EntityStoreManager : IEntityStoreManager
    {
        private Dictionary<String, IEntityStore> _stores = new Dictionary<string, IEntityStore>();
        private Object __lockObject = new Object();

        public IEntityStore CreateStore(string name)
        {
            var result = GetStoreInternal(name, false);
            if (result == null)
            {
                lock (__lockObject)
                {
                    name = name.ToLower().Trim();
                    if (!_stores.TryGetValue(name, out result))
                    {
                        result = new NHibernateSqlCeEntityStore(name);
                        _stores[name] = result;
                    }
                }
            }
            return result;
        }

        public IEntityStore GetStore(String name)
        {
            return GetStoreInternal(name, true);
        }

        public IEntityStore GetStoreInternal(string name, bool raiseException)
        {
            name = name.ToLower();
            IEntityStore result = null;
            if (!_stores.TryGetValue(name, out result) && raiseException)
                throw new InvalidOperationException(String.Format("The store with name {0} does not exist. Check the spell and call CreateStore() if necessary."));
            return result;
        }
    }
}
