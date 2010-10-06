using System;
namespace Wing.Soa.Interop.Client
{
    public interface IServiceSyncBroker
    {
        void PerformAction(BeginAction beginAction, EndAction endAction);
        void PerformAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4>(BeginAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4> action, EndAction endAction, TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, TActionArgument4 argument4);
        void PerformAction<TActionArgument1, TActionArgument2, TActionArgument3>(BeginAction<TActionArgument1, TActionArgument2, TActionArgument3> action, EndAction endAction, TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3);
        void PerformAction<TActionArgument1, TActionArgument2>(BeginAction<TActionArgument1, TActionArgument2> action, EndAction endAction, TActionArgument1 argument1, TActionArgument2 argument2);
        void PerformAction<TActionArgument1>(BeginAction<TActionArgument1> beginAction, EndAction endAction, TActionArgument1 argument1);
        TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4>(BeginAction<TActionArgument1, TActionArgument2, TActionArgument3, TActionArgument4> action, EndAction<TReturn> endAction, TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3, TActionArgument4 argument4);
        TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2, TActionArgument3>(BeginAction<TActionArgument1, TActionArgument2, TActionArgument3> action, EndAction<TReturn> endAction, TActionArgument1 argument1, TActionArgument2 argument2, TActionArgument3 argument3);
        TReturn PerformAction<TReturn, TActionArgument1, TActionArgument2>(BeginAction<TActionArgument1, TActionArgument2> beginAction, EndAction<TReturn> endAction, TActionArgument1 argument1, TActionArgument2 argument2);
        TReturn PerformAction<TReturn, TActionArgument1>(Func<TActionArgument1, AsyncCallback, object, IAsyncResult> beginAction, Func<IAsyncResult, TReturn> endAction, TActionArgument1 argument1);
        TReturn PerformAction<TReturn>(BeginAction beginAction, EndAction<TReturn> endAction);
    }
}
