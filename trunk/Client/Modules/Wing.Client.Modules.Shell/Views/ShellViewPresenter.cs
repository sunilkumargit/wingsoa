using System;

namespace Wing.Client.Modules.Shell.Views
{
    public class ShellViewPresenter : IShellViewPresenter
    {
        public IShellView View { get; private set; }

        public ShellViewPresenter(IShellView view)
        {
            View = view;
        }


        public event EventHandler OnNavigateBack;
    }
}
