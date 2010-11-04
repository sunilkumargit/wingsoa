namespace Telerik.Windows.Controls
{
    using System;

    internal class DeferredAction<T> : DeferredActionBase
    {
        private Action<T> deferredAction;
        private T param1;

        public DeferredAction(Action<T> action, T parameter1)
        {
            this.deferredAction = action;
            this.param1 = parameter1;
        }

        public override void Execute()
        {
            this.deferredAction(this.param1);
            this.deferredAction = null;
            this.param1 = default(T);
        }
    }
}

