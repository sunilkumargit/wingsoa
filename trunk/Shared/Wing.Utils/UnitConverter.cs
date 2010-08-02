using System;

namespace Wing.Utils
{
    public class UnitConverter : IUnitConverter
    {
        public UnitType SourceType { get; private set; }
        public UnitType ResultType { get; private set; }
        public UnitTranslator Translator { get; private set; }

        public UnitConverter(UnitType from, UnitType to)
        {
            SourceType = from;
            ResultType = to;
            Translator = new UnitTranslator(from);
        }

        public float this[float unit] { get { return Get(unit); } }

        public float Get(float unit)
        {
            switch (ResultType)
            {
                case UnitType.Centimeter: return Translator.ToCm(unit);
                case UnitType.Millimeter: return Translator.ToMm(unit);
                case UnitType.Point: return Translator.ToPt(unit);
                default: throw new Exception(String.Format("Cannot convert unit from {0} to {1}.", SourceType.ToString(), ResultType.ToString()));
            }
        }
    }
}
