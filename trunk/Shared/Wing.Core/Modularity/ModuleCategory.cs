using System;

namespace Wing.Modularity
{
    public enum ModuleCategory
    {
        /// <summary>
        /// Core modules are the first modules initialized by ModuleManager. Usually modules in this category provides 
        /// core assets used by entire application.
        /// Core modules can depends only on another core modules. Depedency from Init or Common modules is not allowed.
        /// </summary>
        Core,

        /// <summary>
        /// Init modules are initialized after core modules and generally contains secondary services (not required for core funcionallity). 
        /// Init modules is used also to configure another system services. Init modules can depends on Core or another Init modules, 
        /// dependancy from Common modules are not allowed.
        /// </summary>
        Init,

        /// <summary>
        /// Common modules defines user-tasks. In another words, you should use Common modules to implement business logic, business services
        /// or application extensions.
        /// </summary>
        Common
    }
}
