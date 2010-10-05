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
    public interface ISyncContext
    {
        TResult Sync<TResult>(Func<TResult> callback);
        TResult Sync<T, TResult>(Func<T, TResult> callback, T p1);
        TResult Sync<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 p1, T2 p2);
        void Sync(Action callback);
        void Sync<T>(Action<T> callback, T p1);
        void Sync<T1, T2>(Action<T1, T2> callback, T1 p1, T2 p2);
    }
}
