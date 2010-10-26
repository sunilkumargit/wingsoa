using System;
using System.Windows.Threading;

namespace Wing.Client.Sdk.Services
{
    [System.Diagnostics.DebuggerStepThrough]
    public class SyncBrokerService : ISyncBroker
    {
        private DispatcherSynchronizationContext _context;
        Dispatcher _dispatcher;

        public SyncBrokerService(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _context = new DispatcherSynchronizationContext(dispatcher);
        }

        private void InvokeSync(Action action)
        {
            if (_dispatcher.CheckAccess())
                action();
            else
                _context.Send((a) => { action(); }, null);
        }

        private void InvokeAsync(Action action)
        {
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(action);
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

        public void Async(Action callback)
        {
            InvokeAsync(callback);
        }

        public void DelayAsync(TimeSpan delay, Action action)
        {
            InvokeAsync(() =>
            {
                var timer = new DispatcherTimer() { Interval = delay };
                timer.Tick += new EventHandler((sender, args) =>
                {
                    timer.Stop();
                    action();
                });
                timer.Start();
            });
        }

        public bool IsUIThread
        {
            get { return _dispatcher.CheckAccess(); }
        }

        public void EnsureNonUIThred()
        {
            if (IsUIThread)
                throw new InvalidOperationException("Invalid thread context.");
        }
    }
}
