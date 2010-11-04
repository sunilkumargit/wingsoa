namespace Telerik.Windows
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SourceItem
    {
        internal SourceItem(int startIndex, object source)
        {
            this = new SourceItem();
            this.StartIndex = startIndex;
            this.Source = source;
        }

        internal int StartIndex { get; private set; }
        internal object Source { get; private set; }
        public static bool operator ==(SourceItem sourceItem1, SourceItem sourceItem2)
        {
            return sourceItem1.Equals(sourceItem2);
        }

        public static bool operator !=(SourceItem sourceItem1, SourceItem sourceItem2)
        {
            return !sourceItem1.Equals(sourceItem2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object o)
        {
            return this.Equals((SourceItem) o);
        }

        public bool Equals(SourceItem sourceItem)
        {
            return ((sourceItem.StartIndex == this.StartIndex) && (sourceItem.Source == this.Source));
        }
    }
}

