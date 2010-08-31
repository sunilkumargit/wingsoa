/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

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
