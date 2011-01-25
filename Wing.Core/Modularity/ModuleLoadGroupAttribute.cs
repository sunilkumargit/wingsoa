using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modularity
{
    /// <summary>
    /// Set a load group to a module. This attribute should be used on classes that implement <see cref="IModule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModuleLoadGroupAttribute : Attribute
    {
        public ModuleLoadGroupAttribute(String groupName)
        {
            this.Group = groupName;
        }

        public string Group { get; private set; }
    }
}
