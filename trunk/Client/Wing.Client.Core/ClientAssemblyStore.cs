using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.IO.IsolatedStorage;

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
