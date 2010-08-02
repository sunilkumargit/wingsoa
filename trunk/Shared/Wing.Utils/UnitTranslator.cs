using System;

namespace Wing.Utils
{
    public class UnitTranslator
    {
        public UnitType ResultType { get; private set; }

        public UnitTranslator(UnitType resultType)
        {
            ResultType = resultType;
        }

        public float FromCm(float cm)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return cm;
                case UnitType.Millimeter: return UnitsUtils.CmToMm(cm);
                case UnitType.Point: return UnitsUtils.CmToPt(cm);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float FromMm(float mm)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsUtils.MmToCm(mm);
                case UnitType.Millimeter: return mm;
                case UnitType.Point: return UnitsUtils.MmToPt(mm);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float FromPt(float pt)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsUtils.PtToCm(pt);
                case UnitType.Millimeter: return UnitsUtils.PtToMm(pt);
                case UnitType.Point: return pt;
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToCm(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return unit;
                case UnitType.Millimeter: return UnitsUtils.MmToCm(unit);
                case UnitType.Point: return UnitsUtils.PtToCm(unit);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToMm(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsUtils.CmToMm(unit);
                case UnitType.Millimeter: return unit;
                case UnitType.Point: return UnitsUtils.PtToMm(unit);
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }

        public float ToPt(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return UnitsUtils.CmToPt(unit);
                case UnitType.Millimeter: return UnitsUtils.MmToPt(unit);
                case UnitType.Point: return unit;
                default: throw new Exception("Unit not reconized " + ResultType.ToString());
            }
        }
    }
}
