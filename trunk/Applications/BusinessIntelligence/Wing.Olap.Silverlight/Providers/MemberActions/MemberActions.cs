/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using Wing.Olap.Controls;
using Wing.Olap.Mdx;

namespace Wing.Olap.Providers.MemberActions
{
    public abstract class MemberAction
    {
        public readonly PerformMemberActionArgs args;
        public MemberAction(PerformMemberActionArgs args)
        {
            this.args = args;
        }

        protected MdxTupleExpression GenTuple()
        {
            var tuple = GenTupleBase();
            tuple.Members.Add(new MdxObjectReferenceExpression(args.Member.UniqueName));
            return tuple;
        }
        protected MdxTupleExpression GenTupleBaseCurrent()
        {
            var tuple = new MdxTupleExpression();
            string lasthier = args.Member.HierarchyUniqueName;
            for (int i = 0; i < args.Ascendants.Count; i++)
            {
                var member = args.Ascendants[i];

                if (lasthier != member.HierarchyUniqueName)
                {
                    lasthier = member.HierarchyUniqueName;
                    tuple.Members.Insert(0, new MdxObjectReferenceExpression(member.HierarchyUniqueName + ".CURRENTMEMBER"));
                }
            }
            return tuple;
        }
        protected MdxTupleExpression GenTupleBase()
        {
            var tuple = new MdxTupleExpression();
            string lasthier = args.Member.HierarchyUniqueName;
            for (int i = 0; i < args.Ascendants.Count; i++)
            {
                var member = args.Ascendants[i];

                if (lasthier != member.HierarchyUniqueName)
                {
                    lasthier = member.HierarchyUniqueName;
                    tuple.Members.Insert(0, new MdxObjectReferenceExpression(member.UniqueName));
                }
            }
            return tuple;
        }
        public bool TuplesAreEqual(MemberAction Action)
        {
            if (Action == null)
                return false;

            if (args == null)
                return false;

            var args1 = Action.args;
            if (args1 == null)
                return false;

            if (args.Member.UniqueName != args1.Member.UniqueName)
                return false;

            var a = args.Ascendants;
            if (a == null)
                return false;

            var a1 = args1.Ascendants;
            if (a1 == null)
                return false;

            if (a.Count != a1.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                var mi = a[i];
                var mi1 = a1[i];
                if (mi.UniqueName != mi1.UniqueName)
                    return false;
            }
            return true;
        }
        public abstract MdxExpression Process(MdxExpression mdx);
        public abstract MemberAction Clone();
    }
    public class MemberActionExpand : MemberAction
    {
        public MemberActionExpand(PerformMemberActionArgs args) : base(args) { }

        public override MdxExpression Process(MdxExpression expr)
        {
            if (expr == null)
                return null;

            var tuple = GenTuple();
            if (tuple.Members.Count == 1)
            {
                return new MdxFunctionExpression
                ("DRILLDOWNMEMBER"
                , expr
                , tuple
                );
            }
            expr = new MdxBinaryExpression
             (expr
             , new MdxFunctionExpression(
                 "DRILLDOWNMEMBER"
                 , new MdxFunctionExpression
                        ("FILTER"
                        , expr
                        , new MdxBinaryExpression
                            (GenTupleBaseCurrent()
                            , GenTupleBase()
                            , "IS"
                            )
                        )
                 , new MdxObjectReferenceExpression(args.Member.UniqueName)
                 )
                , "+"
                );
            return expr;
        }
        public override MemberAction Clone()
        {
            return new MemberActionExpand(args);
        }
    }
    public class MemberActionCollapse : MemberAction
    {
        public MemberActionCollapse(PerformMemberActionArgs args) : base(args) { }

        public override MdxExpression Process(MdxExpression expr)
        {
            if (expr == null)
                return null;

            MdxExpression filter = new MdxUnaryExpression
                            ("NOT"
                            , new MdxFunctionExpression
                                ("ISANCESTOR"
                                , new MdxObjectReferenceExpression(args.Member.UniqueName)
                                , new MdxObjectReferenceExpression(args.Member.HierarchyUniqueName + ".CURRENTMEMBER")
                                )
                            );
            var tupleBase = GenTupleBase();
            if (tupleBase.Members.Count > 0)
            {
                filter = new MdxBinaryExpression
                        (filter
                        , new MdxUnaryExpression
                            ("NOT"
                            , new MdxBinaryExpression
                                (GenTupleBaseCurrent()
                                , GenTupleBase()
                                , "IS"
                                )
                            )
                        , "OR"
                        );
            }

            return new MdxFunctionExpression
                    ("FILTER"
                    , expr
                    , filter
                    );
        }
        public override MemberAction Clone()
        {
            return new MemberActionCollapse(args);
        }
    }
    public class MemberActionDrillDown : MemberAction
    {
        readonly DrillDownMode DrillDownMode;
        public bool SingleDimension
        { get { return 0 != ((int)this.DrillDownMode & 1); } }
        public bool HideSelf
        { get { return 0 != ((int)this.DrillDownMode & 2); } }

        public MemberActionDrillDown(PerformMemberActionArgs args, DrillDownMode DrillDownMode)
            : base(args)
        {
            this.DrillDownMode = DrillDownMode;
        }
        public override MdxExpression Process(MdxExpression expr)
        {
            if (expr == null)
                return null;

            string uniqueName = args.Member.UniqueName;
            string hierarchyUniqueName = args.Member.HierarchyUniqueName;

            var drillDownExpr =
                new MdxFunctionExpression
                ("Distinct"
                , new MdxFunctionExpression
                    ("DrillDownMember"
                    , expr
                    , new MdxObjectReferenceExpression(uniqueName)
                    )
                );
            MdxExpression filter = new MdxFunctionExpression
                        ("IsAncestor"
                        , new MdxObjectReferenceExpression(uniqueName)
                        , new MdxObjectReferenceExpression(hierarchyUniqueName + ".CURRENTMEMBER")
                        );

            MdxExpression isSelf = new MdxBinaryExpression
                            (new MdxObjectReferenceExpression(uniqueName)
                            , new MdxObjectReferenceExpression(hierarchyUniqueName + ".CURRENTMEMBER")
                            , "IS"
                            );

            if (HideSelf)
            {
                isSelf =
                        new MdxBinaryExpression
                        (isSelf
                        , new MdxFunctionExpression
                            ("IsLeaf"
                            , new MdxObjectReferenceExpression(uniqueName)
                            )
                        , "AND"
                        );
            };

            filter =
                    new MdxBinaryExpression
                    (filter
                    , isSelf
                    , "OR"
                    );
            if (!SingleDimension)
            {
                var tupleBase = GenTupleBase();
                if (tupleBase.Members.Count > 0)
                {
                    filter = new MdxBinaryExpression
                            (filter
                            , new MdxBinaryExpression
                                    (GenTupleBaseCurrent()
                                    , GenTupleBase()
                                    , "IS"
                                    )
                            , "AND"
                            );
                }
            }
            return new MdxFunctionExpression("FILTER", drillDownExpr, filter);

        }
        public override MemberAction Clone()
        {
            return new MemberActionDrillDown(args, DrillDownMode);
        }
        //public MemberActionDrillDown ReturnMostRestrictive(MemberActionDrillDown Action)
        //{
        //  if(Action.args.Ascendants.Count>this.args.Ascendants.Count)
        //    return Action;

        //  return this;
        //}
        //public bool OutOfScope(MemberAction TestedAction)
        //{
        //  if(TestedAction==null)
        //    return false;

        //  if(args==null)
        //    return false;

        //  var a=args.Ascendants;
        //  if(a==null)
        //    return false;

        //  var args1=TestedAction.args;
        //  if(args1==null)
        //    return false;
        //  var a1=args1.Ascendants;
        //  if(a1==null)
        //    return false;

        //  int i1=a1.Count;
        //  for(int i=a.Count; i>=0;i--,i1--)
        //  {
        //    if(i1<0)
        //      return false;

        //    var mi = a[i];
        //    var mi1=a1[i1];
        //    if (mi.UniqueName!=mi1.UniqueName)
        //      return true;
        //  }
        //  return false;
        //}
    }
}
