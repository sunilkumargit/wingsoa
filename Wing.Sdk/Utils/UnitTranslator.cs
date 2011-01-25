using System;

namespace Wing.Utils
{
    [System.Diagnostics.DebuggerStepThrough]
    public class UnitTranslator
    {
        public UnitType ResultType { get; private set; }

        public UnitTranslator(UnitType resultType)
        {
            ResultType = resultType;
        }

        public float FromCentimeter(float cm)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return cm;
                case UnitType.Millimeter: return UnitsHelper.CentimeterToMillimeter(cm);
                case UnitType.Point: return UnitsHelper.CentimeterToPoint(cm);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float FromMillimeter(float mm)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsHelper.MillimeterToCentimeter(mm);
                case UnitType.Millimeter: return mm;
                case UnitType.Point: return UnitsHelper.MmToPt(mm);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float FromPoint(float pt)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsHelper.PointToCentimeter(pt);
                case UnitType.Millimeter: return UnitsHelper.PointToMillimeter(pt);
                case UnitType.Point: return pt;
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToCentimeter(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return unit;
                case UnitType.Millimeter: return UnitsHelper.MillimeterToCentimeter(unit);
                case UnitType.Point: return UnitsHelper.PointToCentimeter(unit);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToMilimeter(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsHelper.CentimeterToMillimeter(unit);
                case UnitType.Millimeter: return unit;
                case UnitType.Point: return UnitsHelper.PointToMillimeter(unit);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToPoint(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsHelper.CentimeterToPoint(unit);
                case UnitType.Millimeter: return UnitsHelper.MmToPt(unit);
                case UnitType.Point: return unit;
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }
    }
}
