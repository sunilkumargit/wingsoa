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
    public sealed class MdxWithCalculatedCellItem : MdxWithClauseItem
    {
        MdxExpression _ForExpression;
        MdxExpression _AsExpression;

        public MdxExpression ForExpression
        {
            get { return _ForExpression; }
            set { _ForExpression = value; _ChildTokens = null; }
        }
        public MdxExpression AsExpression
        {
            get { return _AsExpression; }
            set { _AsExpression = value; _ChildTokens = null; }
        }
        public MdxWithCalculatedCellItem
            (MdxExpression ForExpression
            , MdxExpression AsExpression
            )
            : base(new MdxObjectReferenceExpression("CELL CALCULATION"), AsExpression)
        {
            this._ForExpression = ForExpression;
            this._AsExpression = AsExpression;
        }
        public override string SelfToken
        {
            get { return "CELL CALCULATION..."; }
        }
        static MdxToken CELL_CALCULATION_FOR = new MdxToken("CELL CALCULATION FOR");
        static MdxToken AS = new MdxToken("AS");
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(CELL_CALCULATION_FOR);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(ForExpression);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(AS);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(ForExpression);
            _ChildTokens.Add(DecShift);
            GenerateCalcProp();
        }

        public override object Clone()
        {
            return new MdxWithCalculatedCellItem(
                    (MdxExpression)this.ForExpression.Clone(),
                    (MdxExpression)this.AsExpression.Clone());
        }
    }
}
