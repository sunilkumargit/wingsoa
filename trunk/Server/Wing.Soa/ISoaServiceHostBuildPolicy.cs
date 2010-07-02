using System;
using System.ServiceModel;
using System.Collections;
using System.Collections.Generic;
using Wing.Utils;

namespace Wing.Soa
{
    public interface ISoaServiceHostBuildPolicy
    {
        void Apply(SoaServiceHostBuildContext context);
        void PostApply(SoaServiceHostBuildContext context);
    }
}
