using System;

namespace Wing.Modularity
{
    /// <summary>
    /// Specifies the category o a module. See <see cref="ModuleCategory"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModuleCategoryAttribute : Attribute
    {
        public ModuleCategory Category { get; private set; }

        public ModuleCategoryAttribute(ModuleCategory category)
        {
            Category = category;
        }
    }
}