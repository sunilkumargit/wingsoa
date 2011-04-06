using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Wing.Client
{
    class ResourceMapService : IResourceMapService
    {
        private Dictionary<ResourceLoadMode, List<IResourceMap>> _resources = new Dictionary<ResourceLoadMode, List<IResourceMap>>();
        private Hashtable _registered = new Hashtable();
        private List<IResourceMap> _mappings = new List<IResourceMap>();

        public ResourceMapService()
        {
            _resources[ResourceLoadMode.Plugin] = new List<IResourceMap>();
            _resources[ResourceLoadMode.ShellAddin] = new List<IResourceMap>();
            _resources[ResourceLoadMode.ContentAddin] = new List<IResourceMap>();
            _resources[ResourceLoadMode.GlobalAddin] = new List<IResourceMap>();
            _resources[ResourceLoadMode.OnDemand] = new List<IResourceMap>();
        }

        public void MapResource(string name, ResourceType resType, string url, ResourceLoadMode loadMode, string loadBefore = "", string loadAfter = "")
        {
            if (_registered.ContainsKey(name))
                throw new Exception("Recurso já existe: " + name);
            var map = new ResourceMap(name, resType, url, loadMode, loadBefore, loadAfter);
            _resources[loadMode].Add(map);
            _mappings.Add(map);
            _registered[name] = map;
        }

        public IEnumerable<IResourceMap> GetResourcesByMode(ResourceLoadMode mode)
        {
            return _resources[mode];
        }

        public IEnumerable<IResourceMap> Mappings
        {
            get { return _mappings; }
        }
    }
}
