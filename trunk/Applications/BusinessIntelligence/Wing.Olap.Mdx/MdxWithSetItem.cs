/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxWithSetItem : MdxWithClauseItem
    {
        public MdxWithSetItem() { }

        public MdxWithSetItem(MdxObjectReferenceExpression name, MdxExpression exp, IEnumerable<MdxCalcProperty> props)
            : base(name, exp, props)
        {
        }
        public override string SelfToken
        {
            get { return "SET " + Name.UniqueName + " AS ..."; }
        }
        public static MdxObject SET = new MdxToken("SET");
        public static MdxObject AS = new MdxToken("AS");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(SET);
            _ChildTokens.Add(Name);
            _ChildTokens.Add(AS);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Expression);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            if (this.Expression == null)
            {
                return new MdxWithSetItem();
            }

            return new MdxWithSetItem(
                    (MdxObjectReferenceExpression)this.Name.Clone(),
                    (MdxExpression)this.Expression.Clone(),
                    (IEnumerable<MdxCalcProperty>)this.CalcProperties.Clone());
        }
    }
}
