using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modularity
{
    /// <summary>
    /// Specifies a load priority for a module. Use with <see cref="ModuleCategoryAttribute"/> to determine the load order of a module.
    /// The load priority is used to determine the initial load order and don't determine the real load order.
    /// If a module has dependencies, that dependencies will be loaded before, even if they have a lower load priority.
    /// Each <see cref="ModuleCategory"/> has it's own load priority, in another words, load order is determinated first by
    /// the category of a module, and the modules in each category is ordered by 'dependency-load-order' and, after that, the <see cref=" ModulePriority"/>
    /// is applied. Take carefull on use of this attribute, it's designed for system prior for internal use only.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModulePriorityAttribute : Attribute
    {
        public ModulePriority Priority { get; private set; }

        public ModulePriorityAttribute(ModulePriority priority)
        {
            Priority = priority;
        }
    }
}
