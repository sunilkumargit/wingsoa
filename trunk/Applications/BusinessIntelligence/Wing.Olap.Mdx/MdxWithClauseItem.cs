/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
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
