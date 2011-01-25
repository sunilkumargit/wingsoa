using System;

namespace Wing
{
    public interface IValueFormatter
    {
        String Format(Object value);
        TextAlignment Align { get; }
    }
}
