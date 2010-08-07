using System.Collections.Generic;
using System.Linq;
using Wing.Client.Sdk;

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
