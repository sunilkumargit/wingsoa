
namespace Wing.Modularity
{
    public abstract class ModuleBase : IModule
    {
        public virtual void Initialize() { }

        public virtual void Initialized() { }

        public virtual void Run() { }
    }
}
