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
    public abstract class MdxWithClauseItem : MdxObject
    {
        MdxObjectReferenceExpression _Name;
        public MdxObjectReferenceExpression Name
        {
            get { return _Name; }
            set { _Name = value; _ChildTokens = null; }
        }
        public MdxExpression Expression;

        public readonly MdxObjectList<MdxCalcProperty> CalcProperties = new MdxObjectList<MdxCalcProperty>();

        protected MdxWithClauseItem() : this(new MdxObjectReferenceExpression("NONAME"), null) { }
        protected MdxWithClauseItem(MdxObjectReferenceExpression Name, MdxExpression Expression)
        {
            this.Name = Name;
            this.Expression = Expression;
            this.CalcProperties.ListChanged += _ClearChildTokens;
        }
        protected MdxWithClauseItem(MdxObjectReferenceExpression name, MdxExpression exp, IEnumerable<MdxCalcProperty> props)
            : this(name, exp)
        {
            if (props != null)
                CalcProperties.AddRange(props);
        }
        protected void GenerateCalcProp()
        {
            foreach (var cp in CalcProperties)
            {
                _ChildTokens.Add(IncShift);
                _ChildTokens.Add(NewLine);
                _ChildTokens.Add(COMMA);
                _ChildTokens.Add(cp);
                _ChildTokens.Add(DecShift);
            }
        }
    }
}
