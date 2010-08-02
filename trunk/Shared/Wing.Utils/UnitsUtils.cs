
namespace Wing.Utils
{
    public static class UnitsUtils
    {
        public static readonly float CentimetersPerInch = 2.54f;
        public static readonly float PointsPerInch = 72;
        public static readonly float PointsPerCentimeter = PointsPerInch / CentimetersPerInch;
        public static readonly float InchPerCentimeter = 1 / CentimetersPerInch;
        public static readonly float MillimetersPerCentimeter = 10;
        public static readonly float MillimetersPerInch = MillimetersPerCentimeter * CentimetersPerInch;

        public static float CmToPt(float cm)
        {
            return cm * PointsPerCentimeter;
        }

        public static float CmToMm(float cm)
        {
            return cm * MillimetersPerCentimeter;
        }

        public static float MmToCm(float mm)
        {
            return mm / MillimetersPerCentimeter;
        }

        public static float MmToPt(float mm)
        {
            return CmToPt(MmToCm(mm));
        }

        public static float PtToCm(float pt)
        {
            return pt / PointsPerCentimeter;
        }

        public static float PtToMm(float pt)
        {
            return PtToCm(pt) * MillimetersPerCentimeter;
        }
    }
}