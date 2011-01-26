using System;

namespace Wing.Server.Core
{
    public interface IAssemblyStore
    {
        void SetBasePath(String path);
        void AddAssembly(String name, byte[] data);
        void RemoveAssembly(String name);
        void ConsolidateStore();
        byte[] GetAssemblyData(String name);
        String[] GetAssemblyNames(PathMode mode);
        void Unload();
    }

    public enum PathMode
    {
        Store,
        Added,
        Removed
    }

}