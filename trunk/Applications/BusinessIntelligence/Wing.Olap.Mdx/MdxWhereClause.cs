/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxWhereClause : MdxObject
    {
        private MdxExpression _Expression = null;
        public MdxExpression Expression
        {
            get { return _Expression; }
            set
            {
                _Expression = value;
                _ChildTokens = null;
            }
        }
        public MdxWhereClause() { }
        public MdxWhereClause(MdxExpression Expression)
        {
            this.Expression = Expression;
        }
        public override string SelfToken
        {
            get { return "WHERE ..."; }
        }
        public static MdxToken WHERE = new MdxToken("WHERE");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(WHERE);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(Expression);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            if (this.Expression == null)
            {
                return new MdxWhereClause();
            }

            return new MdxWhereClause(
                    (MdxExpression)this.Expression.Clone());
        }
    }
}
