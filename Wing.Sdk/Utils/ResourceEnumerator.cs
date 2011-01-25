using System;
using System.Collections.Generic;
using System.IO;

namespace Wing.Utils
{
    public class ResourceEnumerator
    {
        public ResourceEnumerator(String path, bool recurse, params String[] patterns)
        {
            foreach (String pattern in patterns)
                EnumerateInternal(path, pattern, recurse);
        }

        private void EnumerateInternal(String path, String pattern, bool recurse)
        {
            String[] files = Directory.GetFiles(path, pattern);
            foreach (String file in files) _resources.Add(file);
            if (recurse)
            {
                String[] dirs = Directory.GetDirectories(path);
                foreach (String dir in dirs)
                    EnumerateInternal(dir, pattern, true);
            }
        }

        private List<String> _resources = new List<string>();

        public List<String> Resources
        {
            get { return _resources; }
        }

        public static ResourceEnumerator Enumerate(String path, bool recurse, params String[] patterns)
        {
            return new ResourceEnumerator(path, recurse, patterns);
        }
    }
}
