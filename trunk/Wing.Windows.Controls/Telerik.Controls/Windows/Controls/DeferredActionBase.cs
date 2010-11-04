namespace Telerik.Windows.Controls
{
    using System;

    internal abstract class DeferredActionBase
    {
        protected DeferredActionBase()
        {
        }

        public abstract void Execute();
    }
}

