using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Client
{
    public interface IResourceMap
    {
        String Name { get; }
        ResourceType Type { get; }
        ResourceLoadMode LoadMode { get; }
        String Url { get; }
        String LoadBefore { get; }
        String LoadAfter { get; }
        int Order { get; }
    }
}
