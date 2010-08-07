using System.Collections.Generic;
using System.Windows;

namespace Wing.Client.Sdk
{
    public interface IShellPresentationModel : IViewPresentationModel
    {
        string StatusMessage { get; set; }
        Visibility ProgressBarVisibility { get; }
        bool ProgressBarIsVisible { get; set; }
        bool ProgressBarIsIndeterminate { get; set; }
        int ProgressMaxValue { get; set; }
        int ProgressValue { get; set; }
        List<IViewPresenter> ActiveViews { get; set; }
    }
}
