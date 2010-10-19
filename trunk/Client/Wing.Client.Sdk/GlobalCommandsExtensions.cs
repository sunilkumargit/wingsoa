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

        public static IGlobalCommand AddNavigateHandler(this IGlobalCommand command, Type presenterType, Type parentPresenterType)
        {
            command.AddHandler(new NavigateCommandHandler(presenterType, parentPresenterType));
            return command;
        }


        public static IGlobalCommand AddNavigateHandler<TPresenterType>(this IGlobalCommand command) where TPresenterType : IViewPresenter
        {
            command.AddNavigateHandler(typeof(TPresenterType));
            return command;
        }

        public static IGlobalCommand AddNavigateHandler<TPresenterType, TParentPresenterType>(this IGlobalCommand command)
            where TPresenterType : IViewPresenter
            where TParentPresenterType : IViewBagPresenter
        {
            command.AddNavigateHandler(typeof(TPresenterType), typeof(TParentPresenterType));
            return command;
        }

        public static IGlobalCommand AddDelegateHandler(this IGlobalCommand command, CommandExecuteDelegate executeDelegate, CommandQueryStatusDelegate queryStatusDelegate)
        {
            command.AddHandler(new DelegateCommandHandler(queryStatusDelegate, executeDelegate));
            return command;
        }

        public static IGlobalCommand AddDelegateHandler(this IGlobalCommand command, CommandExecuteDelegate executeDelegate)
        {
            command.AddHandler(new DelegateCommandHandler(
                new CommandQueryStatusDelegate((IGlobalCommand cmd, ref object parameter, ref GblCommandStatus status, ref bool handled) =>
                {
                    status = GblCommandStatus.Enabled;
                }), executeDelegate));
            return command;
        }

        #endregion
    }
}
