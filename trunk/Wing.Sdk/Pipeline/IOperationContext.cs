using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wing.Client;

namespace Wing.Pipeline
{
    public interface IOperationContext
    {
        IClientSession Session { get; }
        string Name { get; }
        Type ParamType { get; }
        Type ResultType { get; }
    }
}
