using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Wing.Modularity
{
    public class ReflectionOnlyModuleInfoBuilder : IModuleInfoBuilder
    {

        #region IModuleInfoBuilder Members

        public ModuleInfo BuildModuleInfo(Type type)
        {
            var moduleInfo = new ModuleInfo(type.Name, type.AssemblyQualifiedName);
            var attributes = CustomAttributeData.GetCustomAttributes(type);

            foreach (var attrData in attributes)
            {
                var attrDeclaringType = attrData.Constructor.DeclaringType.FullName;

                if (attrDeclaringType == typeof(ModuleAttribute).FullName)
                    moduleInfo.ModuleName = GetAttrParamValue<string>(attrData);
                else if (attrDeclaringType == typeof(ModuleDependencyAttribute).FullName)
                    moduleInfo.DependsOn.Add(GetAttrParamValue<string>(attrData));
                else if (attrDeclaringType == typeof(ModuleCategoryAttribute).FullName)
                    moduleInfo.ModuleCategory = GetAttrParamValue<ModuleCategory>(attrData);
                else if (attrDeclaringType == typeof(ModuleDescriptionAttribute).FullName)
                    moduleInfo.ModuleDescription = GetAttrParamValue<string>(attrData);
                else if (attrDeclaringType == typeof(ModulePriorityAttribute).FullName)
                    moduleInfo.ModulePriority = GetAttrParamValue<ModulePriority>(attrData);
            }
            moduleInfo.Ref = type.Assembly.CodeBase;

            return moduleInfo;
        }

        private T GetAttrParamValue<T>(CustomAttributeData attrData)
        {
            if (attrData.ConstructorArguments.Count > 0)
                return (T)attrData.ConstructorArguments[0].Value;
            return default(T);
        }

        #endregion
    }
}
