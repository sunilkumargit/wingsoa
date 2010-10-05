using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Wing.Client.Sdk.Services
{
    public class SyncContextService : ISyncContext
    {
        private DispatcherSynchronizationContext _context;
        Dispatcher _dispatcher;

        public SyncContextService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _context = new DispatcherSynchronizationContext(dispatcher);
        }

        private void InvokeSync(Action action)
        {
            if (_dispatcher.CheckAccess())
            {
                action();
            }
            else
                _context.Send((a) => { action(); }, null);
        }

        public TResult Sync<TResult>(Func<TResult> callback)
        {
            TResult result = default(TResult);
            InvokeSync(() =>
            {
                result = callback();
            });
            return result;
        }

        public TResult Sync<T, TResult>(Func<T, TResult> callback, T p1)
        {
            TResult result = default(TResult);
            InvokeSync(() =>
            {
                result = callback(p1);
            });
            return result;
        }

        public TResult Sync<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 p1, T2 p2)
        {
            TResult result = default(TResult);
            InvokeSync(() =>
            {
                result = callback(p1, p2);
            });
            return result;
        }

        public void Sync(Action callback)
        {
            InvokeSync(() =>
            {
                callback();
            });
        }

        public void Sync<T>(Action<T> callback, T p1)
        {
            InvokeSync(() =>
            {
                callback(p1);
            });
        }

        public void Sync<T1, T2>(Action<T1, T2> callback, T1 p1, T2 p2)
        {
            InvokeSync(() =>
            {
                callback(p1, p2);
            });
        }
    }
}
