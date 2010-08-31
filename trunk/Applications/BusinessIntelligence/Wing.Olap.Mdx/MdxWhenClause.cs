/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxWhenClause : MdxObject
    {
        MdxExpression _When;
        public MdxExpression When
        {
            get { return _When; }
            set { _When = value; }
        }
        MdxExpression _Then;
        public MdxExpression Then
        {
            get { return _Then; }
            set { _Then = value; _ChildTokens = null; }
        }
        public MdxWhenClause(MdxExpression when, MdxExpression then)
        {
            this._When = when;
            this._Then = then;
        }
        public override string SelfToken
        {
            get { return "WHEN ... THEN .."; }
        }
        public static readonly MdxToken WHEN = new MdxToken("WHEN");
        public static readonly MdxToken THEN = new MdxToken("THEN");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(WHEN);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(When);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(THEN);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Then);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            return new MdxWhenClause(
                    (MdxExpression)this.When.Clone(),
                    (MdxExpression)this.Then.Clone());
        }
    }
}
