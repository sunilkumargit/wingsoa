/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxPropertyExpression : MdxExpression
    {
        MdxExpression _Object = null;
        string _Name;
        public readonly MdxObjectList<MdxExpression> Args = new MdxObjectList<MdxExpression>();
        bool _IsFunction = false;
        public bool IsFunction
        {
            get
            {
                return _IsFunction;
            }
            set
            {
                _IsFunction = value;
                if (!_IsFunction)
                    Args.Clear();

                _ClearChildTokens();
            }
        }
        public MdxExpression Object
        {
            get { return _Object; }
            set { _Object = value; _ClearChildTokens(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; _ClearChildTokens(); }
        }
        string getName() { return _Name; }

        public MdxPropertyExpression(MdxExpression expr, string PropertyName)
        {
            this.Object = expr;
            this.Name = PropertyName;
            this.Args.ListChanged += Args_ListChanged;
        }

        void Args_ListChanged()
        {
            _ClearChildTokens();
            if (Args.Count > 0)
                _IsFunction = true;
        }
        public MdxPropertyExpression(MdxExpression expr, MdxFunctionExpression func)
            : this(expr, func.Name)
        {
            this.Args.AddRange(func.Arguments);
            this.IsFunction = true;
        }
        public override string SelfToken
        {
            get { return "(..)." + Name + (IsFunction ? "(..)" : ""); }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(Object);
            _ChildTokens.Add(DOT);
            _ChildTokens.Add(new MdxRef(getName));
            if (IsFunction)
            {
                if (Args.Count > 0)
                {
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(LPAREN);
                    _ChildTokens.Add(IncShift);
                    _ChildTokens.Add(Args[0]);
                    _ChildTokens.Add(DecShift);
                    for (int i = 1; i < Args.Count; i++)
                    {
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(COMMA);
                        _ChildTokens.Add(IncShift);
                        _ChildTokens.Add(Args[i]);
                        _ChildTokens.Add(DecShift);
                    }
                    _ChildTokens.Add(NewLine);
                }
                else
                    _ChildTokens.Add(LPAREN);

                _ChildTokens.Add(RPAREN);
            }
        }

        public override object Clone()
        {
            MdxPropertyExpression clone = null;
            if (this.Object == null)
            {
                clone = new MdxPropertyExpression(null, this.Name);
            }
            else
            {
                clone = new MdxPropertyExpression(
                    (MdxExpression)this.Object.Clone(),
                    this.Name);
            }

            if (IsFunction)
            {
                clone.Args.AddRange((IEnumerable<MdxExpression>)Args.Clone());
                clone.IsFunction = true;
            }
            return clone;
        }
    }
}
