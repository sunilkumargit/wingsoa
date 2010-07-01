using System;

namespace Wing.Modularity
{
    /// <summary>
    /// Add a description text to a module. This attribute should be used on classes that implement <see cref="IModule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModuleDescriptionAttribute : Attribute
    {
        private readonly string _description;

        /// <summary>
        /// Initializes a new instance of <see cref="ModuleDescriptionAttribute"/>
        /// </summary>
        /// <param name="description"></param>
        public ModuleDescriptionAttribute(string description)
        {
            _description = description;
        }

        /// <summary>
        /// Gets the description text of the module.
        /// </summary>
        public string Description
        {
            get { return _description; }
        }
    }
}
