using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

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