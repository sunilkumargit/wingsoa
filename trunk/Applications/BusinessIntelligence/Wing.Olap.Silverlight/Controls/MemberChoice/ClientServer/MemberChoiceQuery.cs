/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/


namespace Wing.Olap.Controls.MemberChoice.ClientServer
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
