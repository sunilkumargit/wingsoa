
using System;
namespace Wing.Client.Sdk
{
    public interface INavigationHistoryService
    {
        void Push(IViewPresenter presenter);
        IViewPresenter Pop();
        void Remove(IViewPresenter presenter);
        int StackSize { get; }
        event EventHandler OnHistoryChanged;
    }
}
