﻿/*   
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
    public sealed class MdxWithSetItem : MdxWithClauseItem
    {
        public MdxWithSetItem() { }

        public MdxWithSetItem(MdxObjectReferenceExpression name, MdxExpression exp, IEnumerable<MdxCalcProperty> props)
            : base(name, exp, props)
        {
        }
        public override string SelfToken
        {
            get { return "SET " + Name.UniqueName + " AS ..."; }
        }
        public static MdxObject SET = new MdxToken("SET");
        public static MdxObject AS = new MdxToken("AS");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(SET);
            _ChildTokens.Add(Name);
            _ChildTokens.Add(AS);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Expression);
            _ChildTokens.Add(DecShift);
        }

        public override object Clone()
        {
            if (this.Expression == null)
            {
                return new MdxWithSetItem();
            }

            return new MdxWithSetItem(
                    (MdxObjectReferenceExpression)this.Name.Clone(),
                    (MdxExpression)this.Expression.Clone(),
                    (IEnumerable<MdxCalcProperty>)this.CalcProperties.Clone());
        }
    }
}