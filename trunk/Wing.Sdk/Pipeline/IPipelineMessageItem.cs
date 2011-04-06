using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Pipeline
{
    public interface IPipelineMessageItem
    {
        int Id { get; }
        String MessageId { get; }
        Object Data { get; }
    }
}
