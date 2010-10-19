using System;
using Wing.Composite.Regions;

namespace Wing.Client.Sdk
{
    public interface IShellService
    {
        IRegionManager RegionManager { get; }
        void StatusMessage(String message, params string[] values);
        void DisplayProgressBar(int max);
        void DisplayWorkingBar();
        void UpdateProgressBarRelative(int relative);
        void UpdateProgressBarAbsolute(int value);
        void HideProgressOrWorkingBar();
        void Navigate(IViewPresenter viewPresenter);
        void NavigateBack();
        void Alert(String message);
        void ShowPopup(IPopupWindowPresenter presenter);
        IViewBagPresenter MainContentPresenter { get; }
        void DisplayWorkingStatus(string message, params string[] values);
        void HideWorkingStatus();
    }
}