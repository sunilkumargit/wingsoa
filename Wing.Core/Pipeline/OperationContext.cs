using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Client;

namespace Wing.Pipeline
{
    class OperationContext : IOperationContext
    {
        private OperationWrapper _wrapper;

        public OperationContext(IClientSession session, OperationWrapper wrapper, Object rawParamValue)
        {
            _wrapper = wrapper;
            Session = session;
            RawValue = rawParamValue;
        }

        public IClientSession Session { get; private set; }
        public string Name { get { return _wrapper.Name; } }
        public Type ParamType { get { return _wrapper.ParamType; } }
        public Type ResultType { get { return _wrapper.ResultType; } }
        public Object RawValue { get; private set; }
    }
}
