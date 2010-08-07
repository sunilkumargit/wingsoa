using System;
using System.Collections.Generic;
using System.Windows;
using Wing.Client.Core;
using Wing.Client.Modules.Shell.Views;
using Wing.Client.Sdk;
using Wing.Client.Sdk.Events;
using Wing.Composite.Events;
using Wing.Composite.Regions;
using Wing.Events;
using System.Linq;

namespace Wing.Client.Modules.Shell
{
    public class NavigationHistoryService : INavigationHistoryService
    {
        private Stack<IViewPresenter> _stack = new Stack<IViewPresenter>();

        public void Push(IViewPresenter presenter)
        {
            if (_stack.Count == 0 || _stack.Peek() != presenter)
                _stack.Push(presenter);
        }

        public IViewPresenter Pop()
        {
            if (_stack.Count == 0)
                return null;
            return _stack.Pop();
        }

        public void Remove(IViewPresenter presenter)
        {
            _stack = new Stack<IViewPresenter>(_stack.Where(p => p != presenter));
        }
    }
}
