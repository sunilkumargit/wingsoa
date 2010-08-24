/*   
    Copyright (C) 2009 Galaktika Corporation ZAO

    This file is a part of Ranet.UILibrary.Olap
 
    Ranet.UILibrary.Olap is a free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
      
    You should have received a copy of the GNU General Public License
    along with Ranet.UILibrary.Olap.  If not, see <http://www.gnu.org/licenses/>.
  
    If GPL v.3 is not suitable for your products or company,
    Galaktika Corp provides Ranet.UILibrary.Olap under a flexible commercial license
    designed to meet your specific usage and distribution requirements.
    If you have already obtained a commercial license from Galaktika Corp,
    you can use this file under those license terms.
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
