using System;
using Wing.Composite.Regions;

namespace Wing.Client.Sdk
{
    public interface INavigationHistoryService
    {
        void Push(IViewPresenter presenter);
        IViewPresenter Pop();
        void Remove(IViewPresenter presenter);
    }
}
