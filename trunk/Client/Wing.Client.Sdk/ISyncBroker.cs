using System;

namespace Wing.Client.Sdk
{
    /// <summary>
    /// Perform synchronous and asyncronous operations on UI Thread (Main thread).
    /// Use this service to perform operations on UI Controls.
    /// </summary>
    public interface ISyncBroker
    {
        /// <summary>
        /// Returns true if is on UI thread context.
        /// </summary>
        bool IsUIThread { get; }

        /// <summary>
        /// Ensure that current context is not the one of UI Thread. Throws an exception if
        /// current thread is the UI Thread.
        /// </summary>
        void EnsureNonUIThred();

        /// <summary>
        /// Executes a synchronous operation on UI Thread context.
        /// </summary>
        /// <typeparam name="TResult">Result type of action</typeparam>
        /// <param name="callback">Callback action to execute</param>
        /// <returns></returns>
        TResult Sync<TResult>(Func<TResult> callback);

        /// <summary>
        /// Executes a synchonous operation on UI Thread context.
        /// </summary>
        /// <typeparam name="T">Type of parameter of callback action</typeparam>
        /// <typeparam name="TResult">Result type of action</typeparam>
        /// <param name="callback">Callback action to execute</param>
        /// <param name="p1">Parameter of callback action</param>
        /// <returns></returns>
        TResult Sync<T, TResult>(Func<T, TResult> callback, T p1);

        /// <summary>
        /// Executes a synchronous operation on UI Threa context
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="callback"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        TResult Sync<T1, T2, TResult>(Func<T1, T2, TResult> callback, T1 p1, T2 p2);

        /// <summary>
        /// Executes a synchronous operation on UI Threa context
        /// </summary>
        /// <param name="callback"></param>
        void Sync(Action callback);

        /// <summary>
        /// Executes a synchronous operation on UI Threa context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback"></param>
        /// <param name="p1"></param>
        void Sync<T>(Action<T> callback, T p1);

        /// <summary>
        /// Executes a synchronous operation on UI Threa context
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="callback"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        void Sync<T1, T2>(Action<T1, T2> callback, T1 p1, T2 p2);

        /// <summary>
        /// Executes an asynchronous operation on UI Threa context
        /// </summary>
        /// <param name="callback"></param>
        void Async(Action callback);

        /// <summary>
        /// Executes a delayed async operation on main thread.
        /// </summary>
        /// <param name="delay">Delay interval to execute</param>
        /// <param name="action">Callback action to execute</param>
        void DelayAsync(TimeSpan delay, Action action);
    }
}
