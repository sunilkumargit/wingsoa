namespace Telerik.Windows.Controls
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, CompilerGenerated, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    internal class Resource
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource()
        {
        }

        internal static string CalendarDayMonthNamesFormatInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarDayMonthNamesFormatInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarGetDayNamesInvalidDaynamesFormat
        {
            get
            {
                return ResourceManager.GetString("CalendarGetDayNamesInvalidDaynamesFormat", resourceCulture);
            }
        }

        internal static string CalendarMonthViewHeaderText
        {
            get
            {
                return ResourceManager.GetString("CalendarMonthViewHeaderText", resourceCulture);
            }
        }

        internal static string CalendarOnAreDatesInPastSelectableChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnAreDatesInPastSelectableChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnDisplayDateChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnDisplayDateChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnDisplayDateEndChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnDisplayDateEndChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnDisplayDateStartChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnDisplayDateStartChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnFirstDayOfWeekChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnFirstDayOfWeekChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnMonthViewHeaderTextChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnMonthViewHeaderTextChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnSelectableDateEndChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnSelectableDateEndChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnSelectableDateStartChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnSelectableDateStartChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnSelectedDateChangedInvalidValue
        {
            get
            {
                return ResourceManager.GetString("CalendarOnSelectedDateChangedInvalidValue", resourceCulture);
            }
        }

        internal static string CalendarOnSelectedDatesChangedReadOnly
        {
            get
            {
                return ResourceManager.GetString("CalendarOnSelectedDatesChangedReadOnly", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("Telerik.Windows.Controls.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
    }
}

