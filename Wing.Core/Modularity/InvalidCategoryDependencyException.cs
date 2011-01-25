using System;

namespace Wing.Modularity
{
    public class InvalidCategoryDependencyException : ModularityException
    {
        public InvalidCategoryDependencyException() : base() { }

        public InvalidCategoryDependencyException(String message) : base(message) { }

        public InvalidCategoryDependencyException(string moduleName, string message) : base(moduleName, message) { }
    }
}
