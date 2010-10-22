using System;

namespace Wing.Client.Sdk
{
    public static class VisualContext
    {
        private static ISyncBroker _syncBroker;

        public static void SetSyncBroker(ISyncBroker context)
        {
            _syncBroker = context;
        }

        public static TResult Sync<TResult>(Func<TResult> callback)
        {
            return _syncBroker.Sync<TResult>(callback);
        }

        public static TResult Sync<T, TResult>(Func<T, TResult> callback, T p1)
        {
            return _syncBroker.Sync<T, TResult>(callback, p1);
        }

        public static TResult Sync<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 p1, T2 p2)
        {
            return _syncBroker.Sync<T1, T2, TResult>(callback, p1, p2);
        }

        public static void Sync(Action callback)
        {
            _syncBroker.Sync(callback);
        }

        public static void Sync<T>(Action<T> callback, T p1)
        {
            _syncBroker.Sync<T>(callback, p1);
        }

        public static void Sync<T1, T2>(Action<T1, T2> callback, T1 p1, T2 p2)
        {
            _syncBroker.Sync(callback, p1, p2);
        }

        public static void Async(Action callback)
        {
            _syncBroker.Async(callback);
        }

        public static void DelayAsync(TimeSpan delay, Action action)
        {
            _syncBroker.DelayAsync(delay, action);
        }
    }
}
