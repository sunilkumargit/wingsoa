/*
  Wing.Olap 
  Olap library for MSAS, Siverlight and WingServer.
  (C)2010 Marcelo R Santos (mdezem@hotmail.com)
*/

using System.Collections.Generic;

namespace Wing.Olap.Mdx
{
    public enum UpdateBehavior
    {
        None = 0,
        USE_EQUAL_ALLOCATION = 1,
        USE_EQUAL_INCREMENT = 2,
        USE_WEIGHTED_ALLOCATION = 3,
        USE_WEIGHTED_INCREMENT = 4
    }

    public sealed class MdxUpdateStatement : MdxStatement
    {
        MdxObject _Cube = null;
        MdxExpression _Destination = null;
        MdxExpression _Source = null;
        MdxExpression _ByWeightedAllocation = null;
        MdxExpression _ByWeightedIncrement = null;
        UpdateBehavior _UpdateBehavior = UpdateBehavior.None;

        public MdxObject Cube
        {
            get { return _Cube; }
            set { _Cube = value; _ChildTokens = null; }
        }
        public MdxExpression Destination
        {
            get { return _Destination; }
            set { _Destination = value; _ChildTokens = null; }
        }
        public MdxExpression Source
        {
            get { return _Source; }
            set { _Source = value; _ChildTokens = null; }
        }
        public MdxExpression ByWeightedAllocation
        {
            get { return _ByWeightedAllocation; }
            set { _ByWeightedAllocation = value; _ChildTokens = null; }
        }
        public MdxExpression ByWeightedIncrement
        {
            get { return _ByWeightedIncrement; }
            set { _ByWeightedIncrement = value; _ChildTokens = null; }
        }
        public UpdateBehavior UpdateBehavior
        {
            get { return _UpdateBehavior; }
            set { _UpdateBehavior = value; _ChildTokens = null; }
        }
        public MdxUpdateStatement()
        {
        }
        public MdxUpdateStatement(MdxObject Cube, MdxExpression Destination, MdxExpression Source)
            : this()
        {
            this.Cube = Cube;
            this.Destination = Destination;
            this.Source = Source;
        }

        public static MdxObject UPDATE_CUBE = new MdxToken("UPDATE CUBE");
        public static MdxObject SET = new MdxToken("SET");
        public static MdxObject BY = new MdxToken("BY");
        public static MdxObject USE_EQUAL_ALLOCATION = new MdxToken("USE_EQUAL_ALLOCATION");
        public static MdxObject USE_EQUAL_INCREMENT = new MdxToken("USE_EQUAL_INCREMENT");
        public static MdxObject USE_WEIGHTED_ALLOCATION = new MdxToken("USE_WEIGHTED_ALLOCATION");
        public static MdxObject USE_WEIGHTED_INCREMENT = new MdxToken("USE_WEIGHTED_INCREMENT");

        public override string SelfToken
        {
            get { return "UPDATE CUBE .."; }
        }
        protected override void FillChilds()
        {
            _ChildTokens = new List<MdxObject>();
            _ChildTokens.Add(UPDATE_CUBE);
            _ChildTokens.Add(Cube);
            _ChildTokens.Add(SET);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(Destination);
            _ChildTokens.Add(DecShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(EQ);
            _ChildTokens.Add(IncShift);
            _ChildTokens.Add(NewLine);
            _ChildTokens.Add(Source);
            _ChildTokens.Add(DecShift);
            switch (UpdateBehavior)
            {
                case UpdateBehavior.USE_EQUAL_ALLOCATION:
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(USE_EQUAL_ALLOCATION);
                    break;
                case UpdateBehavior.USE_EQUAL_INCREMENT:
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(USE_EQUAL_INCREMENT);
                    break;
                case UpdateBehavior.USE_WEIGHTED_ALLOCATION:
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(USE_WEIGHTED_ALLOCATION);
                    if (ByWeightedAllocation != null)
                    {
                        _ChildTokens.Add(BY);
                        _ChildTokens.Add(IncShift);
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(ByWeightedAllocation);
                        _ChildTokens.Add(DecShift);
                    }
                    break;
                case UpdateBehavior.USE_WEIGHTED_INCREMENT:
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(USE_WEIGHTED_ALLOCATION);
                    if (ByWeightedAllocation != null)
                    {
                        _ChildTokens.Add(BY);
                        _ChildTokens.Add(IncShift);
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(ByWeightedAllocation);
                        _ChildTokens.Add(DecShift);
                    }
                    _ChildTokens.Add(NewLine);
                    _ChildTokens.Add(USE_WEIGHTED_INCREMENT);
                    if (ByWeightedIncrement != null)
                    {
                        _ChildTokens.Add(BY);
                        _ChildTokens.Add(IncShift);
                        _ChildTokens.Add(NewLine);
                        _ChildTokens.Add(ByWeightedIncrement);
                        _ChildTokens.Add(DecShift);
                    }
                    break;
                case UpdateBehavior.None:
                default:
                    break;
            }
        }

        public override object Clone()
        {
            return new MdxUpdateStatement(
                    (MdxObject)this.Cube.Clone(),
                    this.Destination != null ? (MdxExpression)this.Destination.Clone() : null,
                    this.Source != null ? (MdxExpression)this.Source.Clone() : null)
                    {
                        ByWeightedAllocation = this.ByWeightedAllocation,
                        ByWeightedIncrement = this.ByWeightedIncrement,
                        UpdateBehavior = this.UpdateBehavior
                    };
        }
    }
}

