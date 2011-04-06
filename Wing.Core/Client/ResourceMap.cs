using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Wing.Client
{
    class ResourceMap : IResourceMap
    {
        private static int _order = 0;

        public ResourceMap(string name, ResourceType resType, string url, ResourceLoadMode loadMode, string loadBefore = "", string loadAfter = "")
        {
            this.Name = name;
            this.Type = resType;
            this.Url = url;
            this.LoadMode = loadMode;
            this.LoadBefore = loadBefore;
            this.LoadAfter = LoadAfter;
            this.Order = Interlocked.Increment(ref _order);
        }

        public string Name { get; private set; }
        public ResourceType Type { get; private set; }
        public ResourceLoadMode LoadMode { get; private set; }
        public string Url { get; private set; }
        public string LoadBefore { get; private set; }
        public string LoadAfter { get; private set; }
        public int Order { get; private set; }
    }
}
