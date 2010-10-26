using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Wing.Client.Sdk;
using Wing.ServiceLocation;
using Wing.Logging;

namespace Wing.Client.Bootstrap
{
    [System.Diagnostics.DebuggerStepThrough]
    public class SynchorizedServiceLocator : Wing.UnityServiceLocator.UnityServiceLocator
    {
        private ISyncBroker _syncBroker;

        public SynchorizedServiceLocator(IUnityContainer container, ILogger logger, ISyncBroker syncBroker)
            : base(container, logger)
        {
            _syncBroker = syncBroker;
        }

        protected override TResult InvokeContainer<TResult>(Func<TResult> action)
        {
            // não invocar o metodo base
            //return base.InvokeContainer<TResult>(action);
            return _syncBroker.Sync<TResult>(action);
        }
    }
}
