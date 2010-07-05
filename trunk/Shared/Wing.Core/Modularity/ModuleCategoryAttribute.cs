using System;

namespace Wing.Modularity
{
    /// <summary>
    /// Specifies that the current module is system module and should be load before non-system modules. 
    /// Use this attribute on a module that add basic services to avoid use of DepedencyAttribute on every 'common' module.
    /// System modules should not be dependedant of a non-system module. 
    /// This attribute should be used on classes that implement <see cref="IModule"/>.
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