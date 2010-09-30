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
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk
{
    public static class GlobalCommandsExtensions
    {
        #region IGlobalCommand

        public static IGlobalCommand AddNavigateHandler(this IGlobalCommand command, IViewPresenter presenterInstance)
        {
            command.AddHandler(new NavigateCommandHandler(presenterInstance));
            return command;
        }

        public static IGlobalCommand AddNavigateHandler(this IGlobalCommand command, Type presenterType)
        {
            command.AddHandler(new NavigateCommandHandler(presenterType));
            return command;
        }

        public static IGlobalCommand AddNavigateHandler<TPresenterType>(this IGlobalCommand command) where TPresenterType : IViewPresenter
        {
            command.AddNavigateHandler(typeof(TPresenterType));
            return command;
        }
        #endregion
    }
}
