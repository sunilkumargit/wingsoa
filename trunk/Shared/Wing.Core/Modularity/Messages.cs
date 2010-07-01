using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modularity
{
    public static class Messages
    {
        public static readonly string CyclicDependencyFound = "At least one cyclic dependency has been found in the module catalog. Cycles in the module dependencies must be avoided.";
        public static readonly string DefaultTextLoggerPattern = "{1}: {2}. Priority: {3}. Timestamp:{0:u}.";
        public static readonly string DependencyForUnknownModule = "Cannot add dependency for unknown module {0}";
        public static readonly string DependencyOnMissingModule = "A module declared a dependency on another module which is not declared to be loaded. Missing module(s): {0}";
        public static readonly string DirectoryNotFound = "Directory {0} was not found.";
        public static readonly string DuplicatedModule = "A duplicated module with name {0} has been found by the loader.";
        public static readonly string DuplicatedModuleGroup = "A duplicated module group with name {0} has been found by the loader.";
        public static readonly string FailedToGetType = "Unable to retrieve the module type {0} from the loaded assemblies.  You may need to specify a more fully-qualified type name.";
        public static readonly string FailedToLoadModule =
@"An exception occurred while initializing module '{0}'. 
    - The exception message was: {2}
    - The Assembly that the module was trying to be loaded from was:{1}
    Check the InnerException property of the exception for more information. If the exception occurred while creating an object in a 
    DI container, you can exception.GetRootException() to help locate the root cause of the problem.";
        public static readonly string FailedToLoadModuleNoAssemblyInfo =
@"An exception occurred while initializing module '{0}'. 
    - The exception message was: {1}
    Check the InnerException property of the exception for more information. If the exception occurred 
    while creating an object in a DI container, you can exception.GetRootException() to help locate the 
    root cause of the problem. ";
        public static readonly string FailedToRetrieveModule = "Failed to load type for module {0}. Error was: {1}.";
        public static readonly string IEnumeratorObsolete = "The IModuleEnumerator interface is no longer used and has been replaced by ModuleCatalog.";
        public static readonly string InvalidArgumentAssemblyUri = "The argument must be a valid absolute Uri to an assembly file.";
        public static readonly string InvalidDelegateRerefenceTypeException = "The Target of the IDelegateReference should be of type {0}.";
        public static readonly string ModuleDependenciesNotMetInGroup = "Module {0} depends on other modules that don't belong to the same group.";
        public static readonly string ModuleNotFound = "Module {0} was not found in the catalog.";
        public static readonly string ModulePathCannotBeNullOrEmpty = "The ModulePath cannot contain a null value or be empty.";
        public static readonly string ModuleTypeNotFound = "Failed to load type '{0}' from assembly '{1}'.";
        public static readonly string NoRetrieverCanRetrieveModule = "There is currently no moduleTypeLoader in the ModuleManager that can retrieve the specified module.";
        public static readonly string StringCannotBeNullOrEmpty = "The provided String argument {0} must not be null or empty.";
        public static readonly string ValueMustBeOfTypeModuleInfo = "The value must be of type ModuleInfo.";
    }
}
