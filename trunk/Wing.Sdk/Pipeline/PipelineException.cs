using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wing.Pipeline
{
    public class PipelineException : Exception
    {
        public PipelineException(String message, Exception innerException = null) : base(message, innerException) { }
    }
}
