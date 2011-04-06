using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Pipeline
{
    class OperationWrapper
    {
        public OperationWrapper(string name, Type paramType, Type resultType, Func<Object, IOperationContext, Object> func)
        {
            this.Name = name;
            this.ParamType = paramType;
            this.ResultType = resultType;
            this.Function = func;
        }

        public string Name { get; set; }
        public Type ParamType { get; set; }
        public Type ResultType { get; set; }
        public Func<Object, IOperationContext, Object> Function { get; set; }

    }
}
