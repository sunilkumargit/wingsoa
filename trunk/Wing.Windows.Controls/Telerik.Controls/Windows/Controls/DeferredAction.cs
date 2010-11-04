namespace Telerik.Windows.Controls
{
    using System;

    internal class DeferredAction : DeferredActionBase
    {
        private Action deferredAction;

        public DeferredAction(Action action)
        {
            this.deferredAction = action;
        }

        public override void Execute()
        {
            this.deferredAction();
            this.deferredAction = null;
        }
    }
}

