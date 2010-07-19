using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using Wing.Composite.Regions;

namespace Wing.Client.Sdk
{
    public interface IShellService
    {
        void BeginNavigate(Object view);
        void Navigate(Object view);
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



