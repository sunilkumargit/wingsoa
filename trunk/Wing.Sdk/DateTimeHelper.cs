using System;

namespace Wing
{
    [System.Diagnostics.DebuggerStepThrough]
    public static class DateTimeHelper
    {
        public static DateTime DateOnly(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static int DaysBetween(DateTime date1, DateTime date2)
        {
            TimeSpan d = date1 - date2;
            return Convert.ToInt16(d.TotalDays);
        }

        public static String TimeString
        {
            get
            {
                DateTime d = DateTime.Now;
                return d.ToString("T") + "." + d.Millisecond.ToString().PadLeft(3, '0');
            }
        }

        public static String ToShortDate(DateTime date)
        {
            return date.ToString("d");
        }

        public static String PutSlashes(String strDate)
        {
            strDate = StringHelper.FilterNumbers(strDate);
            if (strDate.Length >= 8)
            {
                return strDate.Substring(0, 2) + "/"
                       + strDate.Substring(2, 2) + "/"
                       + strDate.Substring(4, 4);
            }
            return "";
        }

        public static DateTime Encode(Int32 year, Int32 month, Int32 day)
        {
            return new DateTime(year, month, day);
        }

        public static DateTime AddWeekdays(DateTime date, int days)
        {
            DateTime result = date;
            while (days > 0)
            {
                result = result.AddDays(1);
                if ((result.DayOfWeek != DayOfWeek.Saturday)
                    && (result.DayOfWeek != DayOfWeek.Sunday))
                    days--;
            }
            return result;
        }

        public static String IsNullDate(DateTime date, String ifnull)
        {
            if (date.Equals(DateTime.MinValue))
                return ifnull;
            else
                return date.ToString("d");
        }

        public static String IsNullDate(DateTime date)
        {
            return IsNullDate(date, "");
        }

        public static String ToDateString(DateTime date)
        {
            String result = IsNullDate(date, "");
            if (result != "")
            {
                result = PutSlashes(
                    date.Day.ToString().PadLeft(2, '0')
                    + date.Month.ToString().PadLeft(2, '0')
                    + date.Year.ToString());
            }
            return result;
        }

        public static String ToDateTimeString(DateTime date)
        {
            String result = IsNullDate(date, "");
            if (result != "")
            {
                result = ToDateString(date) + " "
                        + date.Hour.ToString().PadLeft(2, '0') + ":"
                        + date.Minute.ToString().PadLeft(2, '0');
            }
            return result;
        }

        public static DateTime Parse(String stringDate)
        {
            return ConversionHelper.ToDateTime(stringDate);
        }

        public static bool IsNullOrEmpty(DateTime dateTime)
        {
            return dateTime.Equals(DateTime.MinValue);
        }

        public static bool IsNullOrEmpty(Nullable<DateTime> dateTime)
        {
            return (dateTime == null) || (IsNullOrEmpty(dateTime.Value));
        }

        public static string GetNomeDoMes(DateTime datetime)
        {
            return GetNomeDoMes(datetime.Month);
        }

        public static readonly String[] NomeMeses = new String[] { "Janeiro", "Fevereiro", "Março", "Abril", "Maio", "Junho", "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro" };

        public static string GetNomeDoMes(int mes)
        {
            if (mes > 0 && mes <= 12)
                return NomeMeses[mes - 1];
            else
                return "*ERR*";
        }

        private static DateTime _1970 = new DateTime(1970, 1, 1, 0, 0, 0);
        public static double ToUnixTicks(DateTime dateTime)
        {
            return (dateTime.ToUniversalTime() - _1970).TotalMilliseconds;
        }

        public static int AgeInYears(DateTime birthdate)
        {
            int years = DateTime.Now.Year - birthdate.Year;
            if (DateTime.Now.Month < birthdate.Month || (DateTime.Now.Month == birthdate.Month && DateTime.Now.Day < birthdate.Day))
                years--;

            return years;
        }
    }

    public static class DateTimeExtensions
    {
        public static double ToUnixTicks(this DateTime dt)
        {
            return DateTimeHelper.ToUnixTicks(dt);
        }
    }
}
