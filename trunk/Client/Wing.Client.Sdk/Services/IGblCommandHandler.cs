
namespace Wing.Client.Sdk.Services
{
    public interface IGblCommandHandler
    {
        void QueryStatus(IGblCommandQueryStatusContext ctx);
        void Execute(IGblCommandExecuteContext ctx);
    }
}