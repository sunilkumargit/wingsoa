﻿/*   
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

using System;
using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public sealed class MdxSetExpression : MdxExpression
    {
        public readonly MdxObjectList<MdxExpression> Expressions = new MdxObjectList<MdxExpression>();
        public MdxSetExpression() : this(null) { }
        public MdxSetExpression(IEnumerable<MdxExpression> Expressions)
        {
            if (Expressions != null)
            {
                this.Expressions.AddRange(Expressions);
            }
            this.Expressions.ListChanged += onListChanged;
        }
        void onListChanged() { _ChildTokens = null; }

        public override string SelfToken
        {
            get { return "{..}"; }
        }

        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(LBRACE);
            if (Expressions.Count > 0)
            {
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(Expressions[0]);
                _ChildTokens.Add(DecShift);
                for (int i = 1; i < Expressions.Count; i++)
                {
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(COMMA);
                    _ChildTokens.Add(IncShift);
                    _ChildTokens.Add(Expressions[i]);
                    _ChildTokens.Add(DecShift);
                }
                _ChildTokens.Add(NewLine);
            }
            _ChildTokens.Add(RBRACE);
        }

        public override object Clone()
        {
            if (this.Expressions == null)
            {
                return new MdxSetExpression();
            }

            return new MdxSetExpression(
                    (IEnumerable<MdxExpression>)this.Expressions.Clone());
        }
    }
}