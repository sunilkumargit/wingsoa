
namespace Wing.Client.Core
{
    public class ClientAssemblyStore : IsolatedStorageAssemblyStore
    {
        public ClientAssemblyStore()
        {
            SetBasePath("AssemblyStore");
        }
    }
}