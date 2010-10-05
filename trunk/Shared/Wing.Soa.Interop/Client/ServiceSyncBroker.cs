using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Soa.Interop.Client
{
    namespace DanielVaughan
    {
        /// <summary>
        /// Use to invoke an asynchronous WCF service method synchronously.
        /// </summary>
        public delegate IAsyncResult BeginAction(AsyncCallback asyncResult, object state);
        public delegate IAsyncResult BeginAction<TActionArgument1>(TActionArgument1 argument1, AsyncCallback asyncResult, object state);
        public delegate IAsyncResult BeginAction<TActionArgument1, TActionArgument2>(TActionArgument1 argument1, TActionArgument2 argument2, AsyncCallback asyncResult, object state);
        public delegate IAsyncResult BeginAction<TActionArgument1, TActionArgument2, TActionArgument3>(TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, AsyncCallback asyncResult, object state);
        public delegate IAsyncResult BeginAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4>(TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, TActionArgument4 argument4, AsyncCallback asyncResult, object state);
        public delegate void EndAction(IAsyncResult asyncResult);
        public delegate TReturn EndAction<TReturn>(IAsyncResult asyncResult);

        /// <summary>
        /// Enables one to call asynchronous WCF services, 
        /// via generated proxies, in a synchronous manner.
        /// </summary>
        public static class ServiceSyncBroker
        {
            const string synchronousCallInvalidMessage = "Synchronous WCF call can not be performed from UI Thread.";

            static void EnsureNonUIThread()
            {
                return;
            }

            #region Return type specified
            /// <summary>
            /// Performs the action synchronously.
            /// </summary>
            /// <exception cref="ConcurrencyException">Occurs if the call is executed from the UI thread.</exception>
            /// <exception cref="TargetInvocationException">Occurs if the specified <code>endAction</code> 
            /// throws a <code>TargetInvocationException</code></exception>
            /// <typeparam name="TReturn">The type of the object returned by the specified <code>endAction</code>.</typeparam>
            /// <param name="beginAction">The Begin[MethodName] method.</param>
            /// <param name="endAction">The End[MethodName] method.</param>
            /// <returns>The result of the <code>EndAction</code> method.</returns>
            /// <example>
            ///	try
            ///	{
            ///		/* Perform synchronous WCF call. */
            ///		result = SynchronousChannelBroker.PerformAction<string>(
            ///			someService.BeginGetString, someService.EndGetString);
            ///	}
            ///	catch (TargetInvocationException ex)
            ///	{
            ///		DisplayMessage(string.Format("Unable to communicate with server. {0} {1}", 
            ///			ex.Message, ex.StackTrace));
            ///	}
            /// </example>
            public static TReturn PerformAction<TReturn>(BeginAction beginAction, EndAction<TReturn> endAction)
            {
                EnsureNonUIThread();
                var beginResult = beginAction(null, null);
                var result = endAction(beginResult);
                return result;
            }

            /// <summary>
            /// Performs the action synchronously.
            /// </summary>
            /// <exception cref="ConcurrencyException">Occurs if the call is executed from the UI thread.</exception>
            /// <exception cref="TargetInvocationException">Occurs if the specified <code>endAction</code> 
            /// throws a <code>TargetInvocationException</code></exception>
            /// <typeparam name="TReturn">The type of the object returned 
            /// by the specified <code>endAction</code>.</typeparam>
            /// <typeparam name="TActionArgument1">The type of the argument required 
            /// by the specified <code>BeginAction</code>.</typeparam>
            /// <param name="beginAction">The Begin[MethodName] method.</param>
            /// <param name="endAction">The End[MethodName] method.</param>
            /// <param name="argument1">The argument required by the specified <code>BeginAction</code></param>
            /// <returns>The result of the <code>EndAction</code> method.</returns>
            /// <example>
            ///	try
            ///	{
            ///		/* Perform synchronous WCF call. */
            ///		result = SynchronousChannelBroker.PerformAction&lt;string, string&gt;(
            ///			someService.BeginGetString, someService.EndGetString, "test string");
            ///	}
            ///	catch (TargetInvocationException ex)
            ///	{
            ///		DisplayMessage(string.Format("Unable to communicate with server. {0} {1}", 
            ///			ex.Message, ex.StackTrace));
            ///	}
            /// </example>
            public static TReturn PerformAction<TReturn, TActionArgument1>(
                Func<TActionArgument1, AsyncCallback, object, IAsyncResult> beginAction,
                Func<IAsyncResult, TReturn> endAction, TActionArgument1 argument1)
            {
                EnsureNonUIThread();
                var beginResult = beginAction(argument1, null, null);
                var result = endAction(beginResult);
                return result;
            }

            /// <summary>
            /// Performs the action synchronously.
            /// </summary>
            /// <exception cref="ConcurrencyException">Occurs if the call is executed from the UI thread.</exception>
            /// <exception cref="TargetInvocationException">Occurs if the specified <code>endAction</code> 
            /// throws a <code>TargetInvocationException</code></exception>
            /// <typeparam name="TReturn">The type of the object returned 
            /// by the specified <code>endAction</code>.</typeparam>
            /// <typeparam name="TActionArgument1">The type of the first argument required 
            /// by the specified <code>BeginAction</code>.</typeparam>
            /// <typeparam name="TActionArgument2">The type of the second argument required 
            /// by the specified <code>BeginAction</code>.</typeparam>
            /// <param name="beginAction">The Begin[MethodName] method.</param>
            /// <param name="endAction">The End[MethodName] method.</param>
            /// <param name="argument1">The first argument required by the specified <code>BeginAction</code></param>
            /// <param name="argument2">The second argument required by the specified <code>BeginAction</code></param>
            /// <returns>The result of the <code>EndAction</code> method.</returns>
            /// <example>
            /// string result;
            ///	try
            ///	{
            ///		/* Perform synchronous WCF call. */
            ///		result = SynchronousChannelBroker.PerformAction&lt;string, string, int&gt;(
            ///			someService.BeginGetString, someService.EndGetString, "test string", 5);
            ///	}
            ///	catch (TargetInvocationException ex)
            ///	{
            ///		DisplayMessage(string.Format("Unable to communicate with server. {0} {1}", 
            ///			ex.Message, ex.StackTrace));
            ///	}
            /// </example>
            public static TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2>(
                BeginAction<TActionArgument1, TActionArgument2> beginAction,
                EndAction<TReturn> endAction,
                TActionArgument1 argument1, TActionArgument2 argument2)
            {
                EnsureNonUIThread();
                var beginResult = beginAction(argument1, argument2, null, null);
                var result = endAction(beginResult);
                return result;
            }

            public static TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2, TActionArgument3>(
                BeginAction<TActionArgument1, TActionArgument2, TActionArgument3> action,
                EndAction<TReturn> endAction,
                TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3)
            {
                EnsureNonUIThread();
                var beginResult = action(argument1, argument2, argument3, null, null);
                var result = endAction(beginResult);
                return result;
            }

            public static TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4>(
                BeginAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4> action,
                EndAction<TReturn> endAction,
                TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, TActionArgument4 argument4)
            {
                EnsureNonUIThread();
                var beginResult = action(argument1, argument2, argument3, argument4, null, null);
                var result = endAction(beginResult);
                return result;
            }
            #endregion

            #region void return type
            /// <summary>
            /// Performs the action synchronously.
            /// </summary>
            /// <exception cref="ConcurrencyException">Occurs if the call is executed from the UI thread.</exception>
            /// <exception cref="TargetInvocationException">Occurs if the specified <code>endAction</code> 
            /// throws a <code>TargetInvocationException</code></exception>
            /// <param name="beginAction">The Begin[MethodName] method.</param>
            /// <param name="endAction">The End[MethodName] method.</param>
            /// <returns>The result of the <code>EndAction</code> method.</returns>
            /// <example>
            ///	try
            ///	{
            ///		/* Perform synchronous WCF call. */
            ///		SynchronousChannelBroker.PerformAction&lt;string&gt;(
            ///			someService.BeginGetString, someService.EndGetString, "test string");
            ///	}
            ///	catch (TargetInvocationException ex)
            ///	{
            ///		DisplayMessage(string.Format("Unable to communicate with server. {0} {1}", 
            ///			ex.Message, ex.StackTrace));
            ///	}
            /// </example>
            public static void PerformAction(BeginAction beginAction, EndAction endAction)
            {
                EnsureNonUIThread();
                var beginResult = beginAction(null, null);
                endAction(beginResult);
            }

            /// <summary>
            /// Performs the action synchronously.
            /// </summary>
            /// <exception cref="ConcurrencyException">Occurs if the call is executed from the UI thread.</exception>
            /// <exception cref="TargetInvocationException">Occurs if the specified <code>endAction</code> 
            /// throws a <code>TargetInvocationException</code></exception>
            /// <param name="beginAction">The Begin[MethodName] method.</param>
            /// <param name="endAction">The End[MethodName] method.</param>
            /// <param name="argument1">The first argument required 
            /// by the specified <code>BeginAction</code></param>
            /// <returns>The result of the <code>EndAction</code> method.</returns>
            /// <example>
            ///	try
            ///	{
            ///		/* Perform synchronous WCF call. */
            ///		SynchronousChannelBroker.PerformAction(
            ///			someService.BeginGetString, someService.EndGetString);
            ///	}
            ///	catch (TargetInvocationException ex)
            ///	{
            ///		DisplayMessage(string.Format("Unable to communicate with server. {0} {1}", 
            ///			ex.Message, ex.StackTrace));
            ///	}
            /// </example>
            public static void PerformAction<TActionArgument1>(
                BeginAction<TActionArgument1> beginAction,
                EndAction endAction,
                TActionArgument1 argument1)
            {
                EnsureNonUIThread();
                var beginResult = beginAction(argument1, null, null);
                endAction(beginResult);
            }

            public static void PerformAction<TActionArgument1, TActionArgument2>(
                BeginAction<TActionArgument1, TActionArgument2> action,
                EndAction endAction,
                TActionArgument1 argument1, TActionArgument2 argument2)
            {
                EnsureNonUIThread();
                var beginResult = action(argument1, argument2, null, null);
                endAction(beginResult);
            }

            public static void PerformAction<TActionArgument1, TActionArgument2, TActionArgument3>(
                BeginAction<TActionArgument1, TActionArgument2, TActionArgument3> action,
                EndAction endAction,
                TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3)
            {
                EnsureNonUIThread();
                var beginResult = action(argument1, argument2, argument3, null, null);
                endAction(beginResult);
            }

            public static void PerformAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4>(
                BeginAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4> action,
                EndAction endAction,
                TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, TActionArgument4 argument4)
            {
                EnsureNonUIThread();
                var beginResult = action(argument1, argument2, argument3, argument4, null, null);
                endAction(beginResult);
            }
            #endregion

        }
    }
}

