using System;
namespace Wing.Client.Modules.Shell.Views
{
    public interface IShellViewPresenter
    {
        IShellView View { get; }

        event EventHandler OnNavigateBack;
    }
}
