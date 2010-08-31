/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxCalcProperty : MdxObject
    {
        public string Name;
        string getName() { return Name; }
        MdxExpression _Expression;
        public MdxExpression Expression
        {
            get { return _Expression; }
            set { _Expression = value; _ChildTokens = null; }
        }
        public MdxCalcProperty(string Name, MdxExpression Expression)
        {
            this.Name = Name;
            this.Expression = Expression;
        }
        public override string SelfToken
        {
            get { return Name + "=..."; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(new MdxRef(getName));
            _ChildTokens.Add(EQ);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Expression);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            return new MdxCalcProperty(
                    this.Name,
                    (MdxExpression)this.Expression.Clone());
        }
    }
}
