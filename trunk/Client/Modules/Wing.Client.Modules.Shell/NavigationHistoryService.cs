using System.Collections.Generic;
using System.Linq;
using Wing.Client.Sdk;
using System;

namespace Wing.Client.Modules.Shell
{
    public class NavigationHistoryService : INavigationHistoryService
    {
        private Stack<IViewPresenter> _stack = new Stack<IViewPresenter>();

        public void Push(IViewPresenter presenter)
        {
            if (_stack.Count == 0 || _stack.Peek() != presenter)
                _stack.Push(presenter);
            FireHistoryChangedEvent();
        }

        public IViewPresenter Pop()
        {
            if (_stack.Count == 0)
                return null;
            var result = _stack.Pop();
            FireHistoryChangedEvent();
            return result;
        }

        public void Remove(IViewPresenter presenter)
        {
            _stack = new Stack<IViewPresenter>(_stack.Where(p => p != presenter));
            FireHistoryChangedEvent();
        }

        public event EventHandler OnHistoryChanged;

        public int StackSize
        {
            get { return _stack.Count; }
        }

        [System.Diagnostics.DebuggerStepThrough]
        private void FireHistoryChangedEvent()
        {
            if (OnHistoryChanged != null)
                OnHistoryChanged.Invoke(this, new EventArgs());
        }
    }
}
