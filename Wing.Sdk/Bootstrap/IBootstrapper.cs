namespace Wing.Bootstrap
{
    public interface IBootstrapper
    {
        void Run(IBootLogger settings, IPathMapper pathMapper);
    }
}