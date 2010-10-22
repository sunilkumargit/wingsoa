using Wing.Utils;

namespace Wing.Client.Sdk.Services
{
    public class DelegateCommandHandler : IGblCommandHandler
    {
        private CommandQueryStatusDelegate _queryStatusDelegate;
        private CommandExecuteDelegate _executeDelegate;

        public DelegateCommandHandler(CommandQueryStatusDelegate queryStatusDelegate, CommandExecuteDelegate executeDelegate)
        {
            Assert.NullArgument(queryStatusDelegate, "queryStatusDelegate");
            Assert.NullArgument(executeDelegate, "executeDelegate");
            _queryStatusDelegate = queryStatusDelegate;
            _executeDelegate = executeDelegate;
        }

        public void QueryStatus(IGblCommandQueryStatusContext ctx)
        {
            _queryStatusDelegate.Invoke(ctx);
        }

        public void Execute(IGblCommandExecuteContext ctx)
        {
            _executeDelegate.Invoke(ctx);
        }
    }

    public delegate void CommandQueryStatusDelegate(IGblCommandQueryStatusContext ctx);
    public delegate void CommandExecuteDelegate(IGblCommandExecuteContext ctx);
}
