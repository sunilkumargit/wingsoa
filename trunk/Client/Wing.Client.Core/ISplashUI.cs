
namespace Wing.Client.Core
{
    public interface ISplashUI
    {
        void DisplayMessage(string message);
        void DisplayLoadingBar();
        void DisplayProgressBar(int max);
        void UpdateProgressBar(int absoluteValue, int relativeValue);
        void HideProgressBar();
    }
}
