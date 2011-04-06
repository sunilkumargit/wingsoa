
namespace Wing.Modularity
{
    public abstract class ModuleBase : IModule
    {
        public void Initialize()
        {
            InitializeInternal();
        }

        protected virtual void InitializeInternal() { }

        public void Run()
        {
            RunInternal();
        }

        protected virtual void RunInternal() { }
    }
}
