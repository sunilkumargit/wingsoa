/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxTupleExpression : MdxExpression
    {
        public readonly MdxObjectList<MdxExpression> Members = new MdxObjectList<MdxExpression>();
        public MdxTupleExpression() { this.Members.ListChanged += onListChanged; }

        public MdxTupleExpression(Tuple Tuple)
            : this(Tuple.GenerateMembers(new List<MdxExpression>()))
        { }

        public MdxTupleExpression(IEnumerable<MdxExpression> Members)
        {
            if (Members != null)
            {
                this.Members.AddRange(Members);
            }
            this.Members.ListChanged += onListChanged;
        }
        void onListChanged() { _ChildTokens = null; }
        public override string SelfToken
        {
            get { return "(..)"; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(LPAREN);
            if (Members.Count > 0)
            {
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(Members[0]);
                _ChildTokens.Add(DecShift);
                for (int i = 1; i < Members.Count; i++)
                {
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(COMMA);
                    _ChildTokens.Add(IncShift);
                    _ChildTokens.Add(Members[i]);
                    _ChildTokens.Add(DecShift);
                }
                _ChildTokens.Add(NewLine);
            }
            _ChildTokens.Add(RPAREN);
        }

        public override object Clone()
        {
            return new MdxTupleExpression(
                    (IEnumerable<MdxExpression>)this.Members.Clone());
        }
    }
}
