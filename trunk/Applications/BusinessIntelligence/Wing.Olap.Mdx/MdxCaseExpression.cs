/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxCaseExpression : MdxExpression
    {
        public readonly MdxObjectList<MdxWhenClause> When = new MdxObjectList<MdxWhenClause>();
        private MdxExpression _Value = null;
        public MdxExpression Value
        {
            get { return _Value; }
            set { _Value = value; _ChildTokens = null; }
        }
        MdxExpression _Else = null;
        public MdxExpression Else
        {
            get { return _Else; }
            set { _Else = value; _ChildTokens = null; }
        }
        public MdxCaseExpression()
        {
            When.ListChanged += When_ListChanged;
        }
        void When_ListChanged()
        {
            _ChildTokens = null;
        }
        public MdxCaseExpression(IEnumerable<MdxWhenClause> whenItems)
            : this()
        {
            if (whenItems != null)
                When.AddRange(whenItems);
        }
        public MdxCaseExpression(IEnumerable<MdxWhenClause> whenItems, MdxExpression elseItem)
            : this(whenItems)
        {
            _Else = elseItem;
        }
        public MdxCaseExpression(MdxExpression ValueItem, IEnumerable<MdxWhenClause> whenItems, MdxExpression elseItem)
            : this(whenItems, elseItem)
        {
            this._Value = ValueItem;
        }
        public override string SelfToken
        {
            get { return "CASE ..."; }
        }
        public static MdxToken CASE = new MdxToken("CASE");
        public static MdxToken ELSE = new MdxToken("ELSE");
        public static MdxToken END = new MdxToken("END");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(CASE);
            if (Value != null)
            {
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(Value);
                _ChildTokens.Add(DecShift);
            }
            foreach (var w in When)
            {
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(w);
            }
            if (Else != null)
            {
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(ELSE);
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(Else);
                _ChildTokens.Add(DecShift);
            }
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(END);
        }

        public override object Clone()
        {
            if (this.When == null)
            {
                return new MdxCaseExpression();
            }

            if (this.Value == null && this.Else == null)
            {
                return new MdxCaseExpression(
                        (IEnumerable<MdxWhenClause>)this.When.Clone());
            }
            if (this.Value == null)
            {
                return new MdxCaseExpression(
                        (IEnumerable<MdxWhenClause>)this.When.Clone(),
                        (MdxExpression)this.Else.Clone());
            }

            return new MdxCaseExpression(
                    (MdxExpression)this.Value.Clone(),
                    (IEnumerable<MdxWhenClause>)this.When.Clone(),
                    (MdxExpression)this.Else.Clone());
        }
    }
}
