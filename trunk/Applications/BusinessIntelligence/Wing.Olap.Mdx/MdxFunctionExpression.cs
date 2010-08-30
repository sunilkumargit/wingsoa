/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of UILibrary.OLAP
 
    UILibrary.OLAP is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with UILibrary.OLAP.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides UILibrary.OLAP under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
*/

using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxFunctionExpression : MdxExpression
    {
        public string Name = "NONAME";
        string getName() { return Name; }
        public readonly MdxObjectList<MdxExpression> Arguments = new MdxObjectList<MdxExpression>();

        public MdxFunctionExpression(string name)
        {
            this.Name = name;
            Arguments.ListChanged += _ClearChildTokens;
        }
        public MdxFunctionExpression(string name, params MdxExpression[] args)
            : this(name, (IEnumerable<MdxExpression>)args)
        {
        }

        public MdxFunctionExpression(string name, IEnumerable<MdxExpression> args)
            : this(name)
        {
            if (args != null)
                Arguments.AddRange(args);
        }
        public override string SelfToken
        {
            get { return Name + "(..)"; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();

            _ChildTokens.Add(new MdxRef(getName));
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(LPAREN);
            _ChildTokens.Add(IncShift);
            if (Arguments.Count > 0)
            {
                _ChildTokens.Add(Arguments[0]);
                _ChildTokens.Add(DecShift);
                for (int i = 1; i < Arguments.Count; i++)
                {
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(COMMA);
                    _ChildTokens.Add(IncShift);
                    _ChildTokens.Add(Arguments[i]);
                    _ChildTokens.Add(DecShift);
                }
                _ChildTokens.Add(NewLine);
            }
            _ChildTokens.Add(RPAREN);
        }

        public override object Clone()
        {
            return new MdxFunctionExpression(
                    this.Name,
                    (IEnumerable<MdxExpression>)this.Arguments.Clone());
        }
    }
}
