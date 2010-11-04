namespace Telerik.Windows.Controls
{
    using System;

    internal class DeferredAction<T1, T2, T3> : DeferredActionBase
    {
        private Action<T1, T2, T3> deferredDelegate;
        private T1 param1;
        private T2 param2;
        private T3 param3;

        public DeferredAction(Action<T1, T2, T3> action, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            this.deferredDelegate = action;
            this.param1 = parameter1;
            this.param2 = parameter2;
            this.param3 = parameter3;
        }

        public override void Execute()
        {
            this.deferredDelegate(this.param1, this.param2, this.param3);
            this.deferredDelegate = null;
            this.param1 = default(T1);
            this.param2 = default(T2);
            this.param3 = default(T3);
        }
    }
}

