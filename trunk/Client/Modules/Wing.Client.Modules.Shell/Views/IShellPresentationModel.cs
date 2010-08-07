using Wing.Client.Sdk;
using System.Windows;
using System.Collections.Generic;

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
