namespace Telerik.Windows.Controls
{
    using System;

    internal class DeferredAction<T1, T2> : DeferredActionBase
    {
        private Action<T1, T2> deferredAction;
        private T1 param1;
        private T2 param2;

        public DeferredAction(Action<T1, T2> action, T1 parameter1, T2 parameter2)
        {
            this.deferredAction = action;
            this.param1 = parameter1;
            this.param2 = parameter2;
        }

        public override void Execute()
        {
            this.deferredAction(this.param1, this.param2);
            this.deferredAction = null;
            this.param1 = default(T1);
            this.param2 = default(T2);
        }
    }
}

