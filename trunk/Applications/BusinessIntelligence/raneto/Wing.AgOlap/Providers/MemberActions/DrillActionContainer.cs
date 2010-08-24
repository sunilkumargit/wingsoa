///*   
//    Copyright (C) 2009 Galaktika Corporation ZAO

//    This file is a part of Ranet.UILibrary.Olap

//    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    You should have received a copy of the GNU General Public License
//    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.

//    If GPL v.3 is not suitable for your products or company,
//    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
//    designed to meet your specific usage and distribution requirements.
//    If you have already obtained a commercial license from Galaktika Corp,
//    you can use this file under those license terms.
//*/

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Wing.Olap.Mdx;

//namespace Ranet.AgOlap.Providers.MemberActions
//{
//  public class DrillActionContainer // : IMdxFastClonable
//  {
//    //static int m_Id = 0;
//    public DrillActionContainer(String memberUniqueName, String hierarchyUniqueName)
//    {
//      //this.Created = System.Threading.Interlocked.Increment(ref m_Id);

//      MemberUniqueName = memberUniqueName;
//      HierarchyUniqueName = hierarchyUniqueName;
//    }

//    public readonly string MemberUniqueName;
//    public readonly string HierarchyUniqueName;

//    public MdxActionBase Action = null;
//    public IList<DrillActionContainer> Children = new List<DrillActionContainer>();
//    //internal readonly int Created;

//    DrillActionContainer(DrillActionContainer old)
//    :this(old.MemberUniqueName, old.HierarchyUniqueName)
//    {
//      if (old.Action != null)
//        Action = old.Action.Clone();

//      foreach (var child in old.Children)
//        Children.Add(child.Clone());
//    }
//    public DrillActionContainer Clone()
//    {
//      return new DrillActionContainer(this);
//    }
//  }


//  public interface IMdxAction //: IMdxFastClonable
//  {
//    MdxObject Process(MdxObject mdx, MdxActionContext context);
//  }

//  public class MdxActionContext
//  {
//    #region ParentObjectCollection
//    public class ParentObjectCollection : List<MdxObject>
//    {
//      public void Push(MdxObject obj)
//      {
//        base.Add(obj);
//      }

//      public MdxObject Pop()
//      {
//        if (base.Count == 0)
//        {
//          return null;
//        }

//        var res = this[base.Count - 1];
//        base.RemoveAt(base.Count - 1);

//        return res;
//      }

//      public MdxObject Peek()
//      {
//        if (base.Count == 0)
//        {
//          return null;
//        }

//        var res = this[base.Count - 1];

//        return res;
//      }
//    }
//    #endregion

//    public MdxActionContext(string hierarchyUniqueName)
//      : this(hierarchyUniqueName, string.Empty)
//    {
//    }

//    public MdxActionContext(string hierarchyUniqueName, string memberUniqueName)
//    {
//      this.HierarchyUniqueName = hierarchyUniqueName;
//      this.MemberUniqueName = memberUniqueName;
//    }

//    public string HierarchyUniqueName;
//    public string MemberUniqueName;
//    public readonly ParentObjectCollection Parents = new ParentObjectCollection();
//  }

//  public class DrillActionProcessor
//  {
//    private class ConcreteObjectAssigner
//    {
//      public ConcreteObjectAssigner()
//      {
//      }

//      public MdxObject Object;
//      public Action<MdxExpression> Assign;

//      public void Commit()
//      {
//        if (this.Object is MdxExpression)
//        {
//          this.Assign((MdxExpression)this.Object);
//        }
//      }
//    }

//    private class DrillContext
//    {
//      public DrillContext()
//      {
//      }

//      public DrillContext(
//          MdxActionContext mdxContext,
//          DrillActionContainer rootAction)
//      {
//        this.MdxContext = mdxContext;
//        this.RootAction = rootAction;
//      }

//      public MdxActionContext MdxContext;
//      public DrillActionContainer RootAction;
//      public readonly Dictionary<string, ConcreteObjectAssigner> ConcreteObjects = new Dictionary<string, ConcreteObjectAssigner>();
//      public bool Processed = false;

//      public MdxActionContext.ParentObjectCollection Parents
//      {
//        get
//        {
//          return this.MdxContext.Parents;
//        }
//      }
//    }

//    public DrillActionProcessor(
//        IEnumerable<DrillActionContainer> actions,
//        Func<MdxObject, MdxActionContext, MdxObject> concretizeMdxObject)
//    {
//      this.Actions = actions;
//      this.ConcretizeMdxObject = concretizeMdxObject;
//    }

//    public Func<MdxObject, MdxActionContext, MdxObject> ConcretizeMdxObject { get; private set; }
//    public IEnumerable<DrillActionContainer> Actions { get; private set; }

//    public MdxExpression Process(MdxExpression baseExpression)
//    {
//      //var hierarchies = new Dictionary<string, List<DrillActionContainer>>();
//      //foreach (var action in this.Actions)
//      //{
//      //    List<DrillActionContainer> hierarchyActions;
//      //    hierarchies.TryGetValue(action.HierarchyUniqueName, out hierarchyActions);
//      //    if (hierarchyActions == null)
//      //    {
//      //        hierarchyActions = new List<DrillActionContainer>();
//      //        hierarchies.Add(action.HierarchyUniqueName, hierarchyActions);
//      //    }

//      //    hierarchyActions.Add(action);
//      //}

//      var processedExpression = baseExpression;
//      //foreach (var kv in hierarchies)
//      //{
//      //var flatList = new List<DrillActionContainer>();
//      //this.GetFlatList(this.Actions, flatList);
//      //flatList.Sort((a, b) => Comparer<int>.Default.Compare(b.Created, a.Created));
//      //flatList)
//      //{
//      var drillContext = new DrillContext();
//      processedExpression = this.ProcessActions(
//          processedExpression,
//          this.Actions,
//          drillContext
//          ) as MdxExpression;
//      foreach (var co in drillContext.ConcreteObjects.Values)
//      {
//        if (co.Assign != null)
//        {
//          co.Commit();
//        }
//      }
//      //}
//      //}

//      return processedExpression;
//    }

//    private void GetFlatList(IEnumerable<DrillActionContainer> source, List<DrillActionContainer> dest)
//    {
//      foreach (var dac in source)
//      {
//        if (dac.Action != null)
//        {
//          dest.Add(dac);
//        }
//        this.GetFlatList(dac.Children, dest);
//      }
//    }
//    private MdxObject ProcessActions(
//        MdxObject baseObject,
//        IEnumerable<DrillActionContainer> rootActions,
//        DrillContext drillContext)
//    {
//      foreach (var rootAction in rootActions)
//      {
//        if (rootAction.Action != null)
//        {
//          drillContext.MdxContext = new MdxActionContext(rootAction.HierarchyUniqueName, rootAction.MemberUniqueName);
//          drillContext.RootAction = rootAction;
//          drillContext.Processed = false;
//          baseObject = this.ProcessConcreteObject(
//                  baseObject,
//                  drillContext,
//                  null);
//          if (!drillContext.Processed)
//          {
//            baseObject = rootAction.Action.Process(baseObject, drillContext.MdxContext);
//          }
//        }
//        baseObject = this.ProcessActions(baseObject, rootAction.Children, drillContext);
//      }

//      return baseObject;

//      //return this.ProcessConcreteObject(baseObject,
//      //        new DrillContext(
//      //            new MdxActionContext(hierarchyUniqueName),
//      //            actions));
//    }


//    //private MdxObject ProcessActions2(
//    //    MdxObject baseObject, 
//    //    IEnumerable<DrillActionContainer> actions,
//    //    MdxActionContext context)
//    //{
//    //    foreach (var action in actions)
//    //    {
//    //        if (action.Action != null)
//    //        {
//    //            context.MemberUniqueName = action.MemberUniqueName;
//    //            context.HierarchyUniqueName = action.HierarchyUniqueName;
//    //            baseObject = action.Action.Process(baseObject, context); //context.RootAction.Action.Process(baseObject, context.MdxContext);
//    //        }
//    //        baseObject = this.ProcessActions2(baseObject, action.Children, context);
//    //    }

//    //    return baseObject;
//    //}

//    private MdxObject ProcessConcreteObject(MdxObject baseObject, DrillContext context, Action<MdxExpression> assign)
//    {
//      if (baseObject == null)
//      {
//        return null;
//      }

//      var concreteObject = this.ConcretizeMdxObject != null ?
//          this.ConcretizeMdxObject(baseObject, context.MdxContext) :
//          baseObject;

//      if (concreteObject != null)
//      {
//        ConcreteObjectAssigner assigner;
//        context.ConcreteObjects.TryGetValue(context.MdxContext.HierarchyUniqueName, out assigner);
//        if (assigner == null)
//        {
//          assigner = new ConcreteObjectAssigner();
//          assigner.Object = concreteObject;
//          assigner.Assign = assign;
//          context.ConcreteObjects.Add(context.MdxContext.HierarchyUniqueName, assigner);
//        }

//        context.Processed = true;
//        concreteObject = context.RootAction.Action.Process(assigner.Object, context.MdxContext);

//        context.ConcreteObjects[context.MdxContext.HierarchyUniqueName].Object = concreteObject;

//        //concreteObject = this.ProcessActions2(baseObject, context.RootAction, context.MdxContext);
//        //concreteObject = this.ProcessActions(
//        //    concreteObject, 
//        //    context.RootAction,
//        //    context.MdxContext);

//        return concreteObject;
//      }

//      if (baseObject is MdxBinaryExpression)
//      {
//        var binExpr = baseObject as MdxBinaryExpression;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(binExpr.Left, context, left => binExpr.Left = left);
//        //var left = 
//        //if (left != null)
//        //{
//        //    binExpr.Left = left;
//        //}
//        this.ProcessConcreteObject(binExpr.Right, context, right => binExpr.Right = right);
//        context.Parents.Pop();
//        //var right = 
//        //if (right != null)
//        //{
//        //    binExpr.Right = right;
//        //}
//      }
//      if (baseObject is MdxCalcProperty)
//      {
//        var calcProp = baseObject as MdxCalcProperty;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(calcProp.Expression, context, expr => calcProp.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    calcProp.Expression = expr;
//        //}
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxCaseExpression)
//      {
//        var caseExpr = baseObject as MdxCaseExpression;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(caseExpr.Value, context, value => caseExpr.Value = value);
//        //var value = 
//        //if (value != null)
//        //{
//        //    caseExpr.Value = value;
//        //}
//        this.ProcessConcreteObject(caseExpr.Else, context, elseExpr => caseExpr.Else = elseExpr);
//        //var elseExpr = 
//        //if (elseExpr != null)
//        //{
//        //    caseExpr.Else = elseExpr;
//        //}
//        foreach (var whenClause in caseExpr.When)
//        {
//          this.ProcessConcreteObject(whenClause, context, null);
//        }
//        context.Parents.Pop();
//      }
//      //if (baseObject is MdxConstantExpression)
//      //{
//      //}
//      if (baseObject is MdxFunctionExpression)
//      {
//        var funcExpr = baseObject as MdxFunctionExpression;
//        context.Parents.Push(baseObject);
//        for (int i = 0; i < funcExpr.Arguments.Count; i++)
//        {
//          this.ProcessConcreteObject(funcExpr.Arguments[i], context, expr => funcExpr.Arguments[i] = expr);
//          //var expr = 
//          //if (expr != null)
//          //{
//          //    funcExpr.Arguments[i] = expr;
//          //}
//        }
//        context.Parents.Pop();
//      }
//      //if (baseObject is MdxObjectList<>)
//      //{
//      //}
//      //if (baseObject is MdxObjectReferenceExpression)
//      //{
//      //}
//      //if (baseObject is MdxParameterExpression)
//      //{
//      //}
//      if (baseObject is MdxPropertyExpression)
//      {
//        var propExpr = baseObject as MdxPropertyExpression;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(propExpr.Object, context, propObj => propExpr.Object = propObj);
//        //var propObj = 
//        //if (propObj != null)
//        //{
//        //    propExpr.Object = propObj;
//        //}
//        for (int i = 0; i < propExpr.Args.Count; i++)
//        {
//          this.ProcessConcreteObject(propExpr.Args[i], context, argExpr => propExpr.Args[i] = argExpr);
//          //var argExpr = 
//          //if (argExpr != null)
//          //{
//          //    propExpr.Args[i] = argExpr;
//          //}
//        }
//        context.Parents.Pop();
//      }
//      //if (baseObject is MdxSelectStatement)
//      //{
//      //}
//      if (baseObject is MdxSetExpression)
//      {
//        var setExpr = baseObject as MdxSetExpression;
//        context.Parents.Push(baseObject);
//        for (int i = 0; i < setExpr.Expressions.Count; i++)
//        {
//          this.ProcessConcreteObject(setExpr.Expressions[i], context, expr => setExpr.Expressions[i] = expr);
//          //var expr = 
//          //if (expr != null)
//          //{
//          //    setExpr.Expressions[i] = expr;
//          //}
//        }
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxTupleExpression)
//      {
//        var tupleExpr = baseObject as MdxTupleExpression;
//        context.Parents.Push(baseObject);
//        for (int i = 0; i < tupleExpr.Members.Count; i++)
//        {
//          this.ProcessConcreteObject(tupleExpr.Members[i], context, memberExpr => tupleExpr.Members[i] = memberExpr);
//          //var memberExpr = 
//          //if (memberExpr != null)
//          //{
//          //    tupleExpr.Members[i] = memberExpr;
//          //}
//        }
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxUnaryExpression)
//      {
//        var unaryExpr = baseObject as MdxUnaryExpression;

//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(unaryExpr.Expression, context, expr => unaryExpr.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    unaryExpr.Expression = expr;
//        //}
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxWhenClause)
//      {
//        var whenClause = baseObject as MdxWhenClause;

//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(whenClause.When, context, when => whenClause.When = when);
//        //var when = 
//        //if (when != null)
//        //{
//        //    whenClause.When = when;
//        //}
//        this.ProcessConcreteObject(whenClause.Then, context, then => whenClause.Then = then);
//        //var then = 
//        //if (then != null)
//        //{
//        //    whenClause.Then = then;
//        //}
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxWhereClause)
//      {
//        var whereClause = baseObject as MdxWhereClause;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(whereClause.Expression, context, expr => whereClause.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    whereClause.Expression = expr;
//        //}
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxWithCalculatedCellItem)
//      {
//        var withCell = baseObject as MdxWithCalculatedCellItem;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(withCell.AsExpression, context, asExpr => withCell.AsExpression = asExpr);
//        //var asExpr = 
//        //if (asExpr != null)
//        //{
//        //    withCell.AsExpression = asExpr;
//        //}
//        this.ProcessConcreteObject(withCell.Expression, context, expr => withCell.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    withCell.Expression = expr;
//        //}
//        this.ProcessConcreteObject(withCell.ForExpression, context, forExpr => withCell.ForExpression = forExpr);
//        //var forExpr = 
//        //if (forExpr != null)
//        //{
//        //    withCell.ForExpression = forExpr;
//        //}
//        this.ProcessConcreteObject(withCell.Name, context, nameExpr => withCell.Name = (MdxObjectReferenceExpression)nameExpr);
//        //var nameExpr = 
//        //if (nameExpr != null)
//        //{
//        //    withCell.Name = nameExpr;
//        //}
//        foreach (var calcProp in withCell.CalcProperties)
//        {
//          this.ProcessConcreteObject(calcProp, context, null);
//        }
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxWithMemberItem)
//      {
//        var withMember = baseObject as MdxWithMemberItem;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(withMember.Expression, context, expr => withMember.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    withMember.Expression = expr;
//        //}
//        this.ProcessConcreteObject(withMember.Name, context, nameExpr => withMember.Name = (MdxObjectReferenceExpression)nameExpr);
//        //var nameExpr = 
//        //if (nameExpr != null)
//        //{
//        //    withMember.Name = nameExpr;
//        //}
//        foreach (var calcProp in withMember.CalcProperties)
//        {
//          this.ProcessConcreteObject(calcProp, context, null);
//        }
//        context.Parents.Pop();
//      }
//      if (baseObject is MdxWithSetItem)
//      {
//        var withSet = baseObject as MdxWithSetItem;
//        context.Parents.Push(baseObject);
//        this.ProcessConcreteObject(withSet.Expression, context, expr => withSet.Expression = expr);
//        //var expr = 
//        //if (expr != null)
//        //{
//        //    withSet.Expression = expr;
//        //}
//        this.ProcessConcreteObject(withSet.Name, context, nameExpr => withSet.Name = (MdxObjectReferenceExpression)nameExpr);
//        //var nameExpr = 
//        //if (nameExpr != null)
//        //{
//        //    withSet.Name = nameExpr;
//        //}
//        foreach (var calcProp in withSet.CalcProperties)
//        {
//          this.ProcessConcreteObject(calcProp, context, null);
//        }

//        context.Parents.Pop();
//      }

//      return baseObject;
//    }
//  }

//  public abstract class MdxActionBase // : IMdxAction
//  {
//    protected static MdxTupleExpression GenTuple(PerformMemberActionArgs args)
//    {
//      var tuple = GenTupleBase(args);
//      tuple.Members.Add(new MdxObjectReferenceExpression(args.Member.UniqueName));
//      return tuple;
//    }
//    protected static MdxTupleExpression GenTupleBaseCurrent(PerformMemberActionArgs args)
//    {
//      var tuple = new MdxTupleExpression();
//      string lasthier = args.Member.HierarchyUniqueName;
//      for (int i = 0; i < args.Ascendants.Count; i++)
//      {
//        var member = args.Ascendants[i];

//        if (lasthier != member.HierarchyUniqueName)
//        {
//          lasthier = member.HierarchyUniqueName;
//          tuple.Members.Insert(0, new MdxObjectReferenceExpression(member.HierarchyUniqueName+".CURRENTMEMBER"));
//        }
//      }
//      return tuple;
//    }
//    protected static MdxTupleExpression GenTupleBase(PerformMemberActionArgs args)
//    {
//      var tuple = new MdxTupleExpression();
//      string lasthier = args.Member.HierarchyUniqueName;
//      for (int i = 0; i < args.Ascendants.Count; i++)
//      {
//        var member = args.Ascendants[i];

//        if (lasthier != member.HierarchyUniqueName)
//        {
//          lasthier = member.HierarchyUniqueName;
//          tuple.Members.Insert(0, new MdxObjectReferenceExpression(member.UniqueName));
//        }
//      }
//      return tuple;
//    }

//    #region IMdxAction Members

//    public MdxObject Process(MdxObject mdx, MdxActionContext context)
//    {
//      return this.ProcessCore(mdx, context);
//    }

//    protected abstract MdxObject ProcessCore(MdxObject mdx, MdxActionContext context);

//    #endregion


//    public abstract MdxActionBase Clone();

//  }
//  public class MdxExpandAction2 : MdxActionBase
//  {
//    PerformMemberActionArgs args;

//    public MdxExpandAction2(PerformMemberActionArgs args)
//    {
//      this.args = args;
//    }
//    #region IMdxAction Members

//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;
//      var tuple = GenTuple(args);
//      if(tuple.Members.Count==1)
//      {
//        return new MdxFunctionExpression
//        ( "DRILLDOWNMEMBER"
//        , expr
//        , tuple
//        ); 
//      }
//      expr = new MdxBinaryExpression
//       ( expr
//       , new MdxFunctionExpression(
//         "DRILLDOWNMEMBER"
//         , new MdxFunctionExpression
//            ("FILTER"
//            , expr
//            , new MdxBinaryExpression
//              ( GenTupleBaseCurrent(args)
//              , GenTupleBase(args)
//              , "IS"
//              )
//            )
//         , new MdxObjectReferenceExpression(args.Member.UniqueName)
//         )
//        ,"+"
//        );
//      return expr;


//    }

//    #endregion

//    #region ICloneable Members

//    public override MdxActionBase Clone()
//    {
//      return new MdxExpandAction2(args);
//    }

//    #endregion
//  }

//  public class MdxExpandAction : MdxActionBase
//  {
//    public MdxExpandAction(String uniqueName)
//    {
//      this.MemberUniqueName = uniqueName;
//    }
//    public readonly string MemberUniqueName;

//    #region IMdxAction Members

//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;

//      //context.Parents



//      return new MdxFunctionExpression(
//          "DRILLDOWNMEMBER",
//              expr,
//#warning Use context.MemberUniqueName instead this.MemberUniqueName?
// new MdxObjectReferenceExpression(context.MemberUniqueName)
//       );
//    }

//    #endregion

//    #region ICloneable Members

//    public override MdxActionBase Clone()
//    {
//      return new MdxExpandAction(MemberUniqueName);
//    }

//    #endregion
//  }
//  public class MdxCollapseAction2 : MdxActionBase
//  {
//    PerformMemberActionArgs args;

//    public MdxCollapseAction2(PerformMemberActionArgs args)
//    {
//      this.args = args;
//    }

//    #region IMdxAction Members
//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;

//      MdxExpression filter=new MdxUnaryExpression
//              ("NOT"
//              , new MdxFunctionExpression
//                ( "ISANCESTOR"
//                , new MdxObjectReferenceExpression(args.Member.UniqueName)
//                , new MdxObjectReferenceExpression(args.Member.HierarchyUniqueName+".CURRENTMEMBER")
//                )
//              );
//      var tupleBase=GenTupleBase(args);
//      if (tupleBase.Members.Count>0)
//      {
//        filter=new MdxBinaryExpression
//            ( filter
//            , new MdxUnaryExpression
//              ( "NOT"
//              , new MdxBinaryExpression
//                ( GenTupleBaseCurrent(args)
//                , GenTupleBase(args)
//                , "IS"  
//                )
//              )
//            , "OR"	
//            );
//      }

//      return new MdxFunctionExpression
//          ("FILTER"
//          , expr
//          , filter
//          );
//    }

//    #endregion

//    #region ICloneable Members

//    public override MdxActionBase Clone()
//    {
//      return new MdxCollapseAction2(args);
//    }

//    #endregion
//  }

//  public class MdxCollapseAction : MdxActionBase
//  {
//    public MdxCollapseAction(String uniqueName)
//    {
//      this.MemberUniqueName = uniqueName;
//    }

//    public readonly string MemberUniqueName;

//    #region IMdxAction Members

//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;

//      return new MdxFunctionExpression(
//          "DRILLUPMEMBER",
//          new MdxExpression[] 
//                {
//                    expr,
//#warning Use context.MemberUniqueName instead this.MemberUniqueName?
//                    new MdxObjectReferenceExpression(context.MemberUniqueName)
//                });
//    }

//    #endregion

//    #region ICloneable Members

//    public override MdxActionBase Clone()
//    {
//      return new MdxCollapseAction(MemberUniqueName);
//    }

//    #endregion
//  }

//  public class MdxDrillDownAction : MdxActionBase
//  {
//    public MdxDrillDownAction(
//        String uniqueName,
//        String hierarchyUniqueName,
//        int levelDepth)
//    {
//      this.MemberUniqueName = uniqueName;
//      this.HierarchyUniqueName = hierarchyUniqueName;
//      this.LevelDepth = levelDepth;
//    }

//    public readonly int LevelDepth = 0;
//#warning Use context.MemberUniqueName instead this.MemberUniqueName?
//    public readonly string MemberUniqueName;
//    public readonly string HierarchyUniqueName;
//    public readonly Func<MdxObject, MdxObject> TransformMdxObject;

//    #region IMdxAction Members

//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;

//      string uniqueName;
//      string hierarchyUniqueName;

//      if (context==null)
//      {
//        uniqueName=MemberUniqueName;
//        hierarchyUniqueName=HierarchyUniqueName;
//      }
//      else
//      {
//        uniqueName = context.MemberUniqueName;
//        hierarchyUniqueName = context.HierarchyUniqueName;
//      }



//      /*
//WITH
//MEMBER [Сценарий].[Сценарии].[Сценарий] AS 'iif (1,[Сценарий].[Сценарии].&[План],[Сценарий].[Сценарии].&[Факт])'
//SELECT
//HIERARCHIZE({Descendants([Период].[ГКМ].[Год].&[2008],[Период].[ГКМ].[Месяц])}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME ON 0,
//HIERARCHIZE(
//// ФИЛЬТР
//FILTER
//(
//DRILLDOWNMEMBER(FILTER(DRILLDOWNMEMBER(Crossjoin({StrToSet('[Персона].[Персонал].[Весь персонал]')},{[Номенклатура].[Вид-Группа-Номенклатура].[Вид].&[Технологические работы].Children}),[Номенклатура].[Вид-Группа-Номенклатура].[Группа].&[Абонентское и гарантийное обслуживание]),(((not ([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Группа].&[Абонентское и гарантийное обслуживание]))AND(not IsSibling([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER,[Номенклатура].[Вид-Группа-Номенклатура].[Группа].&[Абонентское и гарантийное обслуживание])))AND(not IsAncestor([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER,[Номенклатура].[Вид-Группа-Номенклатура].[Группа].&[Абонентское и гарантийное обслуживание])))),[Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//  , 
//  // УСЛОВИЕ ДЛЯ ФИЛЬТРА
//(
//          // ПЕРВОЕ УСЛОВИЕ - Убираем сам элемент если у него количество дочерних не равно 0
//    not (
//        ([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//      and ([Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE].Children.Count <> 0)
//      )
//  AND
//          // ВТОРОЕ УСЛОВИЕ - Убираем соседей, кроме самого элемента             
//    not (
//        IsSibling([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER,[Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//      and 
//        not([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//      )
//  AND
//          // ТРЕТЬЕ УСЛОВИЕ - Убираем предков элемента
//    not IsAncestor([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER,[Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//  // ЧЕТВЕРТОЕ УСЛОВИЕ - Оставляем только потомков
//  AND 
//  (
//    IsAncestor([Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE], [Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER) or ([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//  )
//)
//)

//) DIMENSION PROPERTIES PARENT_UNIQUE_NAME ON 1

//FROM [Бюджет]
//WHERE(
//[Measures].[Количество],
//[Статья].[Статьи].&[и_Ресурсы_Загрузка],
//[ЦФО].[Менеджмент].&[У-5],
//[Подразделение].[Подразделения].[Все подразделения].UNKNOWNMEMBER,
//[Сценарий].[Сценарии].&[План]
//)
//      */
//      MdxExpression drillDownExpr = new MdxFunctionExpression(
//          "DRILLDOWNMEMBER",
//          new MdxExpression[] 
//                {
//                    expr,
//                    new MdxObjectReferenceExpression(uniqueName)
//                });

//      return new MdxFunctionExpression(
//          "FILTER",
//          new MdxExpression[] 
//                {
//                    drillDownExpr,

//                    new MdxBinaryExpression(
//                        new MdxBinaryExpression(

//                        new MdxBinaryExpression
//                        (
//                            // ПЕРВОЕ УСЛОВИЕ - Убираем сам элемент если у него количество дочерних не равно 0
//                            new MdxUnaryExpression
//                            (
//                                "not ",
//                                new MdxBinaryExpression
//                                (
//                                        // Левый операнд
//                                        // Кусок([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//                                        new MdxBinaryExpression(
//                                            new MdxPropertyExpression(
//                                                new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                                "CURRENTMEMBER")
//                                            ,
//                                            new MdxObjectReferenceExpression(uniqueName),
//                                            " is "
//                                        ),
//                                        // Правый операнд
//                                        // Кусок ([Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE].Children.Count <> 0)
//                                        new MdxBinaryExpression(
//                                                // Левый операнд 
//                                                new MdxPropertyExpression(
//                                                    new MdxPropertyExpression(
//                                                        new MdxObjectReferenceExpression(uniqueName),
//                                                        "Children"),
//                                                    "Count"), 
//                                                // Правый операнд 
//                                                new MdxConstantExpression("0", MdxConstantKind.Integer),
//                                                // Операция
//                                                "<>"
//                                        ),
//                                        // Операция
//                                        "AND"
//                                )

//                            ),
//                            // ВТОРОЕ УСЛОВИЕ - Убираем соседей, кроме самого элемента             
//                            new MdxUnaryExpression
//                            (
//                                "not ",
//                                new MdxBinaryExpression
//                                (
//                                    // Левый операнд
//                                    // Кусок IsSibling([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER,[Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//                                    new MdxFunctionExpression
//                                    (
//                                        "IsSibling",
//                                        new MdxExpression[] 
//                                        {
//                                            new MdxPropertyExpression(
//                                                new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                               "CURRENTMEMBER"),
//                                            new MdxObjectReferenceExpression(uniqueName)
//                                        }
//                                    ),
//                                    // Правый операнд 
//                                    // Кусок not([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//                                    new MdxUnaryExpression
//                                    (
//                                        "not ",
//                                        new MdxBinaryExpression
//                                        (
//                                            new MdxPropertyExpression(
//                                                new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                                "CURRENTMEMBER")
//                                            ,
//                                            new MdxObjectReferenceExpression(uniqueName),
//                                            " is "
//                                        )
//                                    )
//                                    ,
//                                    // Операция
//                                    "AND"
//                                )
//                            )
//                            ,
//                            // Операция
//                            "AND"
//                        ),
//                        // ТРЕТЬЕ УСЛОВИЕ - Убираем предков элемента
//                        new MdxUnaryExpression
//                        (
//                            "not ",
//                            new MdxFunctionExpression(
//                                "IsAncestor",
//                                new MdxExpression[] 
//                                {
//                                    new MdxPropertyExpression(
//                                        new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                        "CURRENTMEMBER"),
//                                    new MdxObjectReferenceExpression(uniqueName)
//                                })
//                        )
//                        ,
//                        // Операция
//                        "AND"
//                        )
//                        ,

//                // ЧЕТВЕРТОЕ УСЛОВИЕ - Оставляем только потомков
//                        // Кусок IsAncestor([Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE], [Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER) or ([Номенклатура].[Вид-Группа-Номенклатура].CURRENTMEMBER is [Номенклатура].[Вид-Группа-Номенклатура].[Номенклатура].&[0x80000000000000DE])
//                        new MdxBinaryExpression
//                        (
//                            new MdxFunctionExpression
//                            (
//                                "IsAncestor",
//                                new MdxExpression[] 
//                                {
//                                    new MdxObjectReferenceExpression(uniqueName),
//                                    new MdxPropertyExpression(
//                                        new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                        "CURRENTMEMBER")
//                                }
//                            ),
//                            new MdxBinaryExpression
//                            (
//                                new MdxPropertyExpression(
//                                    new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                    "CURRENTMEMBER")
//                                    ,
//                                new MdxObjectReferenceExpression(uniqueName),
//                                " is "
//                            ),
//                            "OR"
//                        )
//                        ,
//                        // Операция
//                        "AND"
//                    )
//                }
//      );

//    }

//    #endregion


//    public override MdxActionBase Clone()
//    {
//      return new MdxDrillDownAction(MemberUniqueName, HierarchyUniqueName, LevelDepth);
//    }

//  }

//  public class MdxDrillUpAction : MdxActionBase
//  {
//    public MdxDrillUpAction(String uniqueName, String hierarchyUniqueName, int levelDepth)
//    {
//      //this.MemberUniqueName = uniqueName;
//      //this.HierarchyUniqueName = hierarchyUniqueName;
//      this.LevelDepth = levelDepth;
//    }

//    public readonly int LevelDepth = 0;
//#warning Use context.MemberUniqueName instead this.MemberUniqueName?
//    public readonly string MemberUniqueName;
//    public readonly string HierarchyUniqueName;

//    #region IMdxAction Members

//    protected override MdxObject ProcessCore(MdxObject mdx, MdxActionContext context)
//    {
//      MdxExpression expr = mdx as MdxExpression;
//      if (expr == null) return mdx;

//      string uniqueName;
//      string hierarchyUniqueName;

//      if (context == null)
//      {
//        uniqueName = MemberUniqueName;
//        hierarchyUniqueName = HierarchyUniqueName;
//      }
//      else
//      {
//        uniqueName = context.MemberUniqueName;
//        hierarchyUniqueName = context.HierarchyUniqueName;
//      }
//      /*
//       Формируем запрос вида:
//       *    SELECT
//HIERARCHIZE(
//FILTER(
//DRILLUPMEMBER(HIERARCHIZE(CROSSJOIN(DRILLDOWNMEMBER({[Customer].[Customer Geography].[Country].[Australia]},[Customer].[Customer Geography].[Country].&[Australia]),{[Sales Territory].[Sales Territory].[Sales Territory Group].Members})),[Customer].[Customer Geography].[State-Province].&[NSW]&[AU])
//,
//IsSibling([Customer].[Customer Geography].CURRENTMEMBER,[Customer].[Customer Geography].[State-Province].&[NSW]&[AU].PARENT)
//)
//) ON 0,
//HIERARCHIZE(HIERARCHIZE(head({[Product].[Product Categories].[Category].Members},10))) ON 1
//FROM [Adventure Works]
//       */
//      MdxExpression drillUpExpr = new MdxFunctionExpression(
//          "DRILLUPMEMBER",
//          new MdxExpression[] 
//                {
//                    expr,
//                    new MdxObjectReferenceExpression(uniqueName)
//                });

//      return new MdxFunctionExpression(
//          "FILTER",
//          new MdxExpression[] 
//                {
//                    drillUpExpr,
//                    new MdxFunctionExpression(
//                        "IsSibling",
//                        new MdxExpression[] 
//                        {
//                            new MdxPropertyExpression(
//                                new MdxObjectReferenceExpression(hierarchyUniqueName),
//                                "CURRENTMEMBER"),
//                            new MdxPropertyExpression(
//                                new MdxObjectReferenceExpression(uniqueName),
//                                "PARENT")
//                        }
//                    )             

//                });
//    }

//    #endregion

//    public override MdxActionBase Clone()
//    {
//      return new MdxDrillUpAction(MemberUniqueName, HierarchyUniqueName, LevelDepth);
//    }
//  }
//}
