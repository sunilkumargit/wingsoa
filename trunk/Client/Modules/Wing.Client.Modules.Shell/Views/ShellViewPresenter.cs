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
using Wing.Composite.Regions;

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
