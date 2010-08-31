/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxWithMemberItem : MdxWithClauseItem
    {
        public MdxWithMemberItem()
        {
        }

        public MdxWithMemberItem(MdxObjectReferenceExpression name, MdxExpression exp, IEnumerable<MdxCalcProperty> props)
            : base(name, exp, props)
        {
        }
        public override string SelfToken
        {
            get { return "MEMBER " + Name + " AS .."; }
        }
        static MdxToken MEMBER = new MdxToken("MEMBER");
        static MdxToken AS = new MdxToken("AS");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(MEMBER);
            _ChildTokens.Add(Name);
            _ChildTokens.Add(AS);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(this.Expression);
            _ChildTokens.Add(DecShift);
            GenerateCalcProp();
        }

        public override object Clone()
        {
            if (this.Expression == null)
            {
                return new MdxWithMemberItem();
            }

            return new MdxWithMemberItem(
                    (MdxObjectReferenceExpression)this.Name.Clone(),
                    (MdxExpression)this.Expression.Clone(),
                    (IEnumerable<MdxCalcProperty>)this.CalcProperties.Clone());
        }
    }
}
