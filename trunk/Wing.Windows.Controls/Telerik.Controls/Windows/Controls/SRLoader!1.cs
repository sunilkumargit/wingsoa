namespace Telerik.Windows.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    public class SRLoader<T>
    {
        private static object internalSyncObject;
        private static SRLoader<T> loader;
        private ResourceManager resources;

        private SRLoader(string defaultNamespace)
        {
            defaultNamespace = "Wing.Windows.Controls";
            this.resources = new ResourceManager(string.Format(CultureInfo.CurrentCulture, "{0}.Strings", new object[] { defaultNamespace }), typeof(T).Assembly);
        }

        private static SRLoader<T> GetLoader(string defaultNamespace)
        {
            if (SRLoader<T>.loader == null)
            {
                lock (SRLoader<T>.InternalSyncObject)
                {
                    if (SRLoader<T>.loader == null)
                    {
                        SRLoader<T>.loader = new SRLoader<T>(defaultNamespace);
                    }
                }
            }
            return SRLoader<T>.loader;
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static object GetObject(string defaultNamespace, string name)
        {
            SRLoader<T> loader = SRLoader<T>.GetLoader(defaultNamespace);
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, SRLoader<T>.Culture);
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static ResourceManager GetResources(string defaultNamespace)
        {
            return SRLoader<T>.GetLoader(defaultNamespace).resources;
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static string GetString(string defaultNamespace, string name)
        {
            SRLoader<T> loader = SRLoader<T>.GetLoader(defaultNamespace);
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, SRLoader<T>.Culture);
        }

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static string GetString(string defaultNamespace, string name, params object[] args)
        {
            SRLoader<T> loader = SRLoader<T>.GetLoader(defaultNamespace);
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, SRLoader<T>.Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string stringArg = args[i] as string;
                if ((stringArg != null) && (stringArg.Length > 0x400))
                {
                    args[i] = stringArg.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture
        {
            get
            {
                return null;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (SRLoader<T>.internalSyncObject == null)
                {
                    object newObject = new object();
                    Interlocked.CompareExchange(ref SRLoader<T>.internalSyncObject, newObject, null);
                }
                return SRLoader<T>.internalSyncObject;
            }
        }
    }
}

