using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Modularity
{
    public class InvalidCategoryDependencyException:ModularityException
    {
        public InvalidCategoryDependencyException() : base() { }

        public InvalidCategoryDependencyException(String message) : base(message) { }

        public InvalidCategoryDependencyException(string moduleName, string message) : base(moduleName, message) { }
    }
}
