using System.Collections.Generic;
using System.Windows;
using Wing.Client.Sdk;

namespace Wing.Client.Modules.Shell.Views
{
    public interface IShellPresentationModel : IPresentationModel
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
