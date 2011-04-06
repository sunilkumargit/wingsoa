using System;

namespace Wing.Modularity
{
    public class ModuleInfoBuilder : IModuleInfoBuilder
    {
        #region IModuleInfoBuilder Members

        public ModuleInfo BuildModuleInfo(Type type)
        {
            var moduleInfo = new ModuleInfo(type.Name, type.AssemblyQualifiedName);
            var attributes = type.GetCustomAttributes(true);

            foreach (var attr in attributes)
            {
                if (attr.GetType() == typeof(ModuleAttribute))
                    moduleInfo.ModuleName = ((ModuleAttribute)attr).ModuleName;
                else if (attr.GetType() == typeof(ModuleDependencyAttribute))
                    moduleInfo.DependsOn.Add(((ModuleDependencyAttribute)attr).ModuleName);
                else if (attr.GetType() == typeof(ModuleCategoryAttribute))
                    moduleInfo.ModuleCategory = ((ModuleCategoryAttribute)attr).Category;
                else if (attr.GetType() == typeof(ModuleDescriptionAttribute))
                    moduleInfo.ModuleDescription = ((ModuleDescriptionAttribute)attr).Description;
                else if (attr.GetType() == typeof(ModulePriorityAttribute))
                    moduleInfo.ModulePriority = ((ModulePriorityAttribute)attr).Priority;
            }
            return moduleInfo;
        }

        #endregion
    }
}
