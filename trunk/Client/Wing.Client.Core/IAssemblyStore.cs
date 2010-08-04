using System;

namespace Wing.Client.Core
{
    public interface IAssemblyStore
    {
        void SetBasePath(String path);
        void AddAssembly(String name, byte[] data);
        void RemoveAssembly(String name);
        void ConsolidateStore();
        byte[] GetAssemblyData(String name);
        void Unload();
    }

}