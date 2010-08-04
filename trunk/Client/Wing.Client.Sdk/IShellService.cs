using System;
using Wing.Composite.Regions;

namespace Wing.Client.Sdk
{
    public interface IShellService
    {
        void Navigate(IRootViewPresenter viewPresenter);
        void NavigateBack();
        IRegion MainBarLeft { get; }
        IRegion MainBarRight { get; }
        IRegion ContextBar { get; }
        void StatusMessage(String message, params string[] values);
        void DisplayProgressBar(int max);
        void DisplayWorkingBar();
        void UpdateProgressBarRelative(int relative);
        void UpdateProgressBarAbsolute(int value);
        void HideProgressOrWorkingBar();
    }
}



