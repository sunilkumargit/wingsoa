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
using System.Collections.ObjectModel;
using Wing.Utils;
using Wing.ServiceLocation;

namespace Wing.Client.Sdk.Services
{
    public class GlobalNavigateCommand : GlobalDelegateCommand
    {
        public GlobalNavigateCommand(String name, IViewPresenter target)
            : base(name, () =>
            {
                ServiceLocator.Current.GetInstance<IShellService>().Navigate(target);
            }) { }

        public GlobalNavigateCommand(String name, Type presenterType)
            : base(name, () =>
            {
                ServiceLocator.Current.GetInstance<IShellService>()
                    .Navigate((IViewPresenter)ServiceLocator.Current.GetInstance(presenterType));
            }) { }
    }

    public class GlobalNavigateCommand<TViewType> : GlobalNavigateCommand where TViewType : IViewPresenter
    {
        public GlobalNavigateCommand(String name) : base(name, typeof(TViewType)) { }
    }
}
