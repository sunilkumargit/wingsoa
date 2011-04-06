using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.EntityStore;
using Wing.Logging;
using Wing.Adapters.EntityStore;

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
                        TryLog("Creating store {0}...".Templ(name));
                        try
                        {
                            result = new NHibernateSqlServerEntityStore(name);
                            _stores[name] = result;
                        }
                        catch (Exception ex)
                        {
                            TryLog("Error on creating store {0}".Templ(name), ex);
                        }
                    }
                }
            }
            return result;
        }

        private void TryLog(String message, Exception ex = null)
        {
            var lm = ServiceLocator.TryGet<ILogManager>();
            if (lm == null)
                return;

            var logger = lm.GetSystemLogger();
            if (ex != null)
                logger.LogException(message, ex, Priority.High);
            else
                logger.Log(message, Category.Info, Priority.Low);
        }

        public IEntityStore GetStore(String name)
        {
            return GetStoreInternal(name, true);
        }

        public IEntityStore GetStoreInternal(string name, bool raiseException)
        {
            lock (__lockObject)
            {
                name = name.ToLower();
                IEntityStore result = null;
                if (!_stores.TryGetValue(name, out result) && raiseException)
                    throw new InvalidOperationException(String.Format("The store with name {0} does not exist. Check the spell and call CreateStore() if necessary."));
                return result;
            }
        }
    }
}
