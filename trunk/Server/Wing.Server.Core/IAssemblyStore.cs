using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.IO;

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