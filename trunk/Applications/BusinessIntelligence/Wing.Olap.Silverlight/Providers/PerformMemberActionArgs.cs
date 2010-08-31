/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Net;
using System.Collections.Generic;
using Wing.Olap.Core.Providers.ClientServer;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Providers
{
    public class PerformMemberActionArgs
    {
        public PerformMemberActionArgs() {
        }

        public PerformMemberActionArgs(
            MemberInfo member,
            int axisIndex,
            MemberActionType action,
            List<MemberInfo> ascendants)
            :this()
        {
            this.Member = member;
            this.AxisIndex = axisIndex;
            this.Action = action;
            this.Ascendants = ascendants;
        }

        /// <summary>
        /// Предки
        /// </summary>
        public List<MemberInfo> Ascendants = null;
        public MemberInfo Member = null;
        public MemberActionType Action = MemberActionType.Expand;
        public int AxisIndex = 0;
    }
}
