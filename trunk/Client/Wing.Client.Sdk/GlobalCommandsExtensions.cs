using System;
using Wing.Client.Sdk.Services;

namespace Wing.Client.Sdk
{
    public static class GlobalCommandsExtensions
    {
        #region IGlobalCommand

        public static IGblCommand AddNavigateHandler(this IGblCommand command, IViewPresenter presenterInstance)
        {
            command.AddHandler(new NavigateCommandHandler(presenterInstance));
            return command;
        }

        public static IGblCommand AddNavigateHandler(this IGblCommand command, Type presenterType)
        {
            command.AddHandler(new NavigateCommandHandler(presenterType));
            return command;
        }

        public static IGblCommand AddNavigateHandler(this IGblCommand command, Type presenterType, Type parentPresenterType)
        {
            command.AddHandler(new NavigateCommandHandler(presenterType, parentPresenterType));
            return command;
        }


        public static IGblCommand AddNavigateHandler<TPresenterType>(this IGblCommand command) where TPresenterType : IViewPresenter
        {
            command.AddNavigateHandler(typeof(TPresenterType));
            return command;
        }

        public static IGblCommand AddNavigateHandler<TPresenterType, TParentPresenterType>(this IGblCommand command)
            where TPresenterType : IViewPresenter
            where TParentPresenterType : IViewBagPresenter
        {
            command.AddNavigateHandler(typeof(TPresenterType), typeof(TParentPresenterType));
            return command;
        }

        public static IGblCommand AddDelegateHandler(this IGblCommand command, CommandExecuteDelegate executeDelegate, CommandQueryStatusDelegate queryStatusDelegate)
        {
            command.AddHandler(new DelegateCommandHandler(queryStatusDelegate, executeDelegate));
            return command;
        }

        public static IGblCommand AddDelegateHandler(this IGblCommand command, CommandExecuteDelegate executeDelegate)
        {
            command.AddHandler(new DelegateCommandHandler(
                new CommandQueryStatusDelegate((ctx) =>
                {
                    ctx.Status = GblCommandStatus.Enabled;
                }), executeDelegate));
            return command;
        }

        #endregion
    }
}
