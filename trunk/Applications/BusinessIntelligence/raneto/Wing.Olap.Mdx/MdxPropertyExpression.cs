/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Wing.UILibrary.Olap
 
    Wing.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Wing.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Wing.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
