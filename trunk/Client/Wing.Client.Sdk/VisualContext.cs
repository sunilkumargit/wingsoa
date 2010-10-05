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

namespace Wing.Client.Sdk
{
    public static class VisualContext 
    {
        private static ISyncContext _syncContext;

        public static void SetSyncService(ISyncContext context)
        {
            _syncContext = context;
        }

        public static TResult Sync<TResult>(Func<TResult> callback)
        {
            return _syncContext.Sync<TResult>(callback);
        }

        public static TResult Sync<T, TResult>(Func<T, TResult> callback, T p1)
        {
            return _syncContext.Sync<T, TResult>(callback, p1);
        }

        public static TResult Sync<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 p1, T2 p2)
        {
            return _syncContext.Sync<T1, T2, TResult>(callback, p1, p2);
        }

        public static void Sync(Action callback)
        {
            _syncContext.Sync(callback);
        }

        public static void Sync<T>(Action<T> callback, T p1)
        {
            _syncContext.Sync<T>(callback, p1);
        }

        public static void Sync<T1, T2>(Action<T1, T2> callback, T1 p1, T2 p2)
        {
            _syncContext.Sync(callback, p1, p2);
        }
    }
}
