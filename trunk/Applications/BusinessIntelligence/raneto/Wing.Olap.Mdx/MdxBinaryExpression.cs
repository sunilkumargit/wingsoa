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
    public sealed class MdxBinaryExpression : MdxExpression
    {
        MdxExpression _Left;
        MdxObject _Right;
        public string Operator = "UNKNOWN_OPERATOR";
        string getOperator() { return Operator; }

        public MdxExpression Left
        {
            get { return _Left; }
            set { _Left = value; _ChildTokens = null; }
        }
        public MdxObject Right
        {
            get { return _Right; }
            set { _Right = value; _ChildTokens = null; }
        }
        public MdxBinaryExpression(
                MdxExpression leftExpression,
                MdxExpression rightExpression,
                string op)
        {
            this.Left = leftExpression;
            this.Right = rightExpression;
            this.Operator = op;
        }
        public override string SelfToken
        {
            get { return ".. " + Operator + " .."; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(LPAREN);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(Left);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(new MdxRef(getOperator));
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(Right);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(RPAREN);
        }

        public override object Clone()
        {
            return new MdxBinaryExpression(
                    (MdxExpression)this.Left.Clone(),
                    (MdxExpression)this.Right.Clone(),
                    this.Operator);
        }
    }
}
