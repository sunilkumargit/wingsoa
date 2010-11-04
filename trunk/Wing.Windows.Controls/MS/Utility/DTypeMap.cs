namespace MS.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using Telerik.Windows;

    internal class DTypeMap
    {
        private object[] entries;
        private int entriesCount;
        private Dictionary<DependencyObjectType, object> overflow;

        public DTypeMap(int entryCount)
        {
            this.entriesCount = entryCount;
            this.entries = new object[this.entriesCount];
            this.ActiveDTypes = new ItemStructList<DependencyObjectType>(0x80);
        }

        public void Clear()
        {
            for (int i = 0; i < this.entriesCount; i++)
            {
                this.entries[i] = null;
            }
            for (int j = 0; j < this.ActiveDTypes.Count; j++)
            {
                this.ActiveDTypes.List[j] = null;
            }
            if (this.overflow != null)
            {
                this.overflow.Clear();
            }
        }

        public ItemStructList<DependencyObjectType> ActiveDTypes { get; private set; }

        public object this[DependencyObjectType dependencyType]
        {
            get
            {
                if (dependencyType.Id < this.entriesCount)
                {
                    return this.entries[dependencyType.Id];
                }
                if ((this.overflow != null) && this.overflow.ContainsKey(dependencyType))
                {
                    return this.overflow[dependencyType];
                }
                return null;
            }
            set
            {
                if (dependencyType.Id < this.entriesCount)
                {
                    this.entries[dependencyType.Id] = value;
                }
                else
                {
                    if (this.overflow == null)
                    {
                        this.overflow = new Dictionary<DependencyObjectType, object>();
                    }
                    this.overflow[dependencyType] = value;
                }
                this.ActiveDTypes.Add(dependencyType);
            }
        }
    }
}

