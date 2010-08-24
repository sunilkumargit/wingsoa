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
using Ranet.Olap.Core.Metadata;
using Ranet.Olap.Core.Providers;
using Ranet.AgOlap.Controls.General.ClientServer;
using Ranet.Olap.Core.Providers.ClientServer;

namespace Ranet.AgOlap.Controls.MemberChoice.ClientServer
{
    public enum MemberChoiceQueryType
    {
        GetRootMembersCount,
        GetRootMembers,
        GetChildrenMembers,
        FindMembers,
        GetAscendants,
        GetMember,
        GetMembers,
        LoadSetWithAscendants
    }

    //public class MemberChoiceQuery : OlapActionBase
    //{
    //    public MemberChoiceQuery()
    //    {
    //        ActionType = OlapActionTypes.GetMembers;
    //    }

    //    public MemberChoiceQueryType QueryType = MemberChoiceQueryType.GetRootMembers;

    //    public String Connection = String.Empty;
    //    public String CubeName = String.Empty;
    //    /// <summary>
    //    /// Подкуб. Подставляется в FROM в виде: (/SubCube/). Если не задан то в выражение FROM подставляется: [/CubeName/]
    //    /// </summary>
    //    public String SubCube = String.Empty;
    //    public String HierarchyUniqueName = String.Empty;
    //    public String StartLevelUniqueName = String.Empty;
    //    public String MemberUniqueName = String.Empty;
    //    public String Set = String.Empty;
        
    //    public long BeginIndex = -1;
    //    public long Count = -1;

    //    public FilterOperationBase Filter = null;

    //    public List<LevelPropertyInfo> Properties = null;
    //}
}
