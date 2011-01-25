using System;

namespace Wing
{
    public interface IValueProvider
    {
        Object GetValue(Object source);
    }

    public interface IValueProvider<TResultType, TSourceType> : IValueProvider
    {
        TResultType GetValue(TSourceType source);
    }

    public interface IValueSetterProvider
    {
        void SetValue(Object target, Object value);
    }
}
