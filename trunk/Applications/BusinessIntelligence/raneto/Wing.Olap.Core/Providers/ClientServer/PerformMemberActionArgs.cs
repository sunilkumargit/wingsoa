/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System;
using System.Net;
using System.Collections.Generic;

namespace Ranet.Olap.Core.Providers.ClientServer
{
    public class PerformMemberActionArgs : OlapActionBase
    {
        public PerformMemberActionArgs() {
            ActionType = OlapActionTypes.MemberAction;
        }

        public PerformMemberActionArgs(
            ShortMemberInfo member,
            int axisIndex,
            MemberActionType action,
            List<ShortMemberInfo> ascendants)
            :this()
        {
            this.Member = member;
            this.AxisIndex = axisIndex;
            this.Action = action;
            this.Ascendants = ascendants;
        }

        public String Connection = String.Empty;

        /// <summary>
        /// Предки
        /// </summary>
        public List<ShortMemberInfo> Ascendants = null;
        public ShortMemberInfo Member = null;
        public MemberActionType Action = MemberActionType.Expand;
        public int AxisIndex = 0;

        public String PivotID = String.Empty;
    }
}
