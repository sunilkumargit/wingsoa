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
    public sealed class MdxWithMemberItem : MdxWithClauseItem
    {
        public MdxWithMemberItem()
        {
        }

        public MdxWithMemberItem(MdxObjectReferenceExpression name, MdxExpression exp, IEnumerable<MdxCalcProperty> props)
            : base(name, exp, props)
        {
        }
        public override string SelfToken
        {
            get { return "MEMBER " + Name + " AS .."; }
        }
        static MdxToken MEMBER = new MdxToken("MEMBER");
        static MdxToken AS = new MdxToken("AS");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(MEMBER);
            _ChildTokens.Add(Name);
            _ChildTokens.Add(AS);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(this.Expression);
            _ChildTokens.Add(DecShift);
            GenerateCalcProp();
        }

        public override object Clone()
        {
            if (this.Expression == null)
            {
                return new MdxWithMemberItem();
            }

            return new MdxWithMemberItem(
                    (MdxObjectReferenceExpression)this.Name.Clone(),
                    (MdxExpression)this.Expression.Clone(),
                    (IEnumerable<MdxCalcProperty>)this.CalcProperties.Clone());
        }
    }
}
