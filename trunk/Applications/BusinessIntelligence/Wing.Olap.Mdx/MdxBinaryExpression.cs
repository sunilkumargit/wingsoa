/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxBinaryExpression : MdxExpression
    {
        MdxExpression _Left;
        MdxObject _Right;
        public string Operator = "UNKNOWN_OPERATOR";
        string getOperator() { return Operator; }

        public MdxExpression Left
        {
            get { return _Left; }
            set { _Left = value; _ChildTokens = null; }
        }
        public MdxObject Right
        {
            get { return _Right; }
            set { _Right = value; _ChildTokens = null; }
        }
        public MdxBinaryExpression(
                MdxExpression leftExpression,
                MdxExpression rightExpression,
                string op)
        {
            this.Left = leftExpression;
            this.Right = rightExpression;
            this.Operator = op;
        }
        public override string SelfToken
        {
            get { return ".. " + Operator + " .."; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(LPAREN);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Left);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(new MdxRef(getOperator));
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(Right);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(RPAREN);
        }

        public override object Clone()
        {
            return new MdxBinaryExpression(
                    (MdxExpression)this.Left.Clone(),
                    (MdxExpression)this.Right.Clone(),
                    this.Operator);
        }
    }
}
