namespace Telerik.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Telerik.Windows.Controls;

    internal class DependencyObjectType
    {
        private DependencyObjectType baseDType;
        private static int dependencyTypeCount;
        private static Dictionary<Type, DependencyObjectType> dependencyTypeFromCLRType = new Dictionary<Type, DependencyObjectType>();
        private static object lockObject = new object();

        private DependencyObjectType()
        {
        }

        public static DependencyObjectType FromSystemType(Type systemType)
        {
            if (systemType == null)
            {
                throw new ArgumentNullException("systemType");
            }
            if (!typeof(DependencyObject).IsAssignableFrom(systemType))
            {
                throw new ArgumentException(Telerik.Windows.Controls.SR.Get("DTypeNotSupportForSystemType", new object[] { systemType.Name }));
            }
            return FromSystemTypeInternal(systemType);
        }

        internal static DependencyObjectType FromSystemTypeInternal(Type systemType)
        {
            lock (lockObject)
            {
                return FromSystemTypeRecursive(systemType);
            }
        }

        private static DependencyObjectType FromSystemTypeRecursive(Type systemType)
        {
            DependencyObjectType type = null;
            if (dependencyTypeFromCLRType.ContainsKey(systemType))
            {
                type = dependencyTypeFromCLRType[systemType];
            }
            if (type == null)
            {
                type = new DependencyObjectType {
                    SystemType = systemType
                };
                dependencyTypeFromCLRType[systemType] = type;
                if (systemType != typeof(DependencyObject))
                {
                    type.baseDType = FromSystemTypeRecursive(systemType.BaseType);
                }
                type.Id = dependencyTypeCount++;
            }
            return type;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public bool IsInstanceOfType(DependencyObject dependencyObject)
        {
            if (dependencyObject != null)
            {
                DependencyObjectType dependencyObjectType = dependencyObject.GetDependencyObjectType();
                do
                {
                    if (dependencyObjectType.Id == this.Id)
                    {
                        return true;
                    }
                    dependencyObjectType = dependencyObjectType.baseDType;
                }
                while (dependencyObjectType != null);
            }
            return false;
        }

        public bool IsSubclassOf(DependencyObjectType dependencyObjectType)
        {
            if (dependencyObjectType != null)
            {
                for (DependencyObjectType type = this.baseDType; type != null; type = type.baseDType)
                {
                    if (type.Id == dependencyObjectType.Id)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public DependencyObjectType BaseType
        {
            get
            {
                return this.baseDType;
            }
        }

        public int Id { get; private set; }

        public string Name
        {
            get
            {
                return this.SystemType.Name;
            }
        }

        public Type SystemType { get; private set; }
    }
}

