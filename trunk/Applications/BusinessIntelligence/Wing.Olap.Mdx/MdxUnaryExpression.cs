/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxUnaryExpression : MdxExpression
    {
        public string Operand;
        string GetOperand() { return Operand; }
        private MdxExpression _Expression;
        public MdxExpression Expression
        {
            get { return _Expression; }
            set { _Expression = value; _ChildTokens = null; }
        }
        public MdxUnaryExpression(string Operand, MdxExpression Expression)
        {
            this.Operand = Operand;
            this.Expression = Expression;
        }
        public override string SelfToken
        {
            get { return Operand + "(...)"; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(new MdxRef(GetOperand));
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Expression);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            return new MdxUnaryExpression(
                    this.Operand,
                    (MdxExpression)this.Expression.Clone());
        }
    }
}
