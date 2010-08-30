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
