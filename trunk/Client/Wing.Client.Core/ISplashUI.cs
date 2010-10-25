
using System;
namespace Wing.Client.Core
{
    public interface ISplashUI
    {
        void ShowLoadingView();
        void ShowQuotaIncreaseView(Action callback);
        void DisplayMessage(string message);
        void DisplayLoadingBar();
        void DisplayProgressBar(int max);
        void UpdateProgressBar(int absoluteValue, int relativeValue);
        void HideProgressBar();
        void DisplayStatusMessage(string statusMessage);
        void DispatchToUI(Action action);
    }
}
