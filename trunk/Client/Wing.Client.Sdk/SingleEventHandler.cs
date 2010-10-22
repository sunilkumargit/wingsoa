
namespace Wing.Client.Sdk
{
    public delegate void SingleEventHandler<TSenderType>(TSenderType sender);
    public delegate void SingleEventHandler<TSenderType, TEventArgsType>(TSenderType sender, TEventArgsType args);
}
