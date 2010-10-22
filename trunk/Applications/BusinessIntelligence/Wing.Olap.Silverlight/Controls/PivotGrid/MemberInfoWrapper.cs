/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Core.Providers;

namespace Wing.Olap.Controls.PivotGrid
{
    public class MemberInfoWrapper<T>
    {
        public readonly MemberInfo Member = null;
        public readonly T UserData = default(T);

        public MemberInfoWrapper(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            Member = member;
        }

        public MemberInfoWrapper(MemberInfo member, T userData)
            : this(member)
        {
            UserData = userData;
        }
    }}
