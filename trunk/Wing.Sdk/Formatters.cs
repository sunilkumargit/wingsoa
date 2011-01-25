using System;

namespace Wing
{
    public static class Formatters
    {
        public static readonly IValueFormatter ShortDate = new ShortDateFormatter();
        public static readonly IValueFormatter DateTime = new DateTimeFormatter();
        public static readonly IValueFormatter DataShortTime = new DateShortTimeFormatter();
        public static readonly IValueFormatter Percent = new PercentFormatter();
        public static readonly IValueFormatter PercentRounded = new PercentRoundedFormatter();
        public static readonly IValueFormatter PercentInteger = new PercentIntegerFormatter();
        public static readonly IValueFormatter Currency = new CurrencyFormatter();
        public static readonly IValueFormatter String = new StringFormatter();
        public static readonly IValueFormatter StringUpperCase = new StringUpperCaseFormatter();
        public static readonly IValueFormatter Integer = new IntegerFormatter();
        public static readonly IValueFormatter Number = new NumberFormatter();
        public static readonly IValueFormatter NumberRounded = new NumberRoundedFormatter();
        public static readonly IValueFormatter Boolean = new BooleanFormatter();
        public static readonly IValueFormatter Telefone = new PhoneFormatter();
        public static IValueFormatter Custom(Func<Object, String> valueFormatterDel) { return new CustomFormatter(valueFormatterDel); }
    }

    public class BooleanFormatter : CustomFormatter
    {
        public BooleanFormatter() : base((v) => ConversionHelper.ToBoolean(v) ? "Sim" : "Não") { }
    }

    public class CurrencyFormatter : CustomFormatter
    {
        public CurrencyFormatter() : base((v) => { return ConversionHelper.ToDecimal(v).ToString("C"); }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class CustomDateFormatter : CustomFormatter
    {
        public CustomDateFormatter(String format)
            : base(v =>
            {
                var dateTime = ConversionHelper.ToDateTime(v);
                if (dateTime.Year < 1900)
                    return "";
                return dateTime.ToString(format);
            }) { }
    }

    public class DateShortTimeFormatter : CustomDateFormatter
    {
        public DateShortTimeFormatter() : base("dd/MM/yyyy HH:mm") { }
    }

    public class DateTimeFormatter : CustomDateFormatter
    {
        public DateTimeFormatter() : base("dd/MM/yyyy HH:mm:ss") { }
    }

    public class IntegerFormatter : CustomFormatter
    {
        public IntegerFormatter() : base((v) => { return ConversionHelper.ToInt32(v).ToString(); }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class NumberFormatter : CustomFormatter
    {
        public NumberFormatter() : base((v) => { return ConversionHelper.ToDecimal(v).ToString("N"); }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class NumberRoundedFormatter : CustomFormatter
    {
        public NumberRoundedFormatter() : base((v) => { return Math.Round(ConversionHelper.ToDecimal(v), 2).ToString(); }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class PhoneFormatter : CustomFormatter
    {
        public PhoneFormatter() : base((v) => FormatHelper.StuffChars(v.AsString().FilterNumbers(), "(xx)xxxx-xxxx", true)) { }
    }

    public class StringUpperCaseFormatter : StringFormatter
    {
        public override string Format(object value)
        {
            return base.Format(value).ToUpper();
        }
    }


    public class StringFormatter : IValueFormatter
    {
        #region IValueFormatter Members

        public virtual string Format(object value)
        {
            if (value != null)
                return value.ToString();
            return "";
        }

        public TextAlignment Align { get { return TextAlignment.Left; } }

        #endregion
    }

    public class ShortDateFormatter : CustomDateFormatter
    {
        public ShortDateFormatter() : base("dd/MM/yyyy") { }
    }

    public class PercentRoundedFormatter : CustomFormatter
    {
        public PercentRoundedFormatter() : base((v) => { return Math.Round(ConversionHelper.ToDecimal(v), 2).ToString() + "%"; }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class PercentIntegerFormatter : CustomFormatter
    {
        public PercentIntegerFormatter() : base((v) => { return ConversionHelper.ToInt32(v).ToString() + "%"; }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
    }

    public class PercentFormatter : CustomFormatter
    {
        #region IValueFormatter Members
        public PercentFormatter() : base((v) => { return ConversionHelper.ToDecimal(v).ToString("N") + "%"; }) { }
        public override TextAlignment Align { get { return TextAlignment.Right; } }
        #endregion
    }
}
