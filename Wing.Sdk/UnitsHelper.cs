
namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class UnitsHelper
    {
        public static readonly float CentimetersPerInch = 2.54f;
        public static readonly float PointsPerInch = 72;
        public static readonly float PointsPerCentimeter = PointsPerInch / CentimetersPerInch;
        public static readonly float InchPerCentimeter = 1 / CentimetersPerInch;
        public static readonly float MillimetersPerCentimeter = 10;
        public static readonly float MillimetersPerInch = MillimetersPerCentimeter * CentimetersPerInch;

        public static float CentimeterToPoint(float cm)
        {
            return cm * PointsPerCentimeter;
        }

        public static float CentimeterToMillimeter(float cm)
        {
            return cm * MillimetersPerCentimeter;
        }

        public static float MillimeterToCentimeter(float mm)
        {
            return mm / MillimetersPerCentimeter;
        }

        public static float MmToPt(float mm)
        {
            return CentimeterToPoint(MillimeterToCentimeter(mm));
        }

        public static float PointToCentimeter(float pt)
        {
            return pt / PointsPerCentimeter;
        }

        public static float PointToMillimeter(float pt)
        {
            return PointToCentimeter(pt) * MillimetersPerCentimeter;
        }
    }
}