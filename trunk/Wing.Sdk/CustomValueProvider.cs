using System;

namespace Wing
{
    public class CustomValueProvider : IValueProvider
    {
        public CustomValueProvider(ValueProviderDelegate valueProvider)
        {
            ValueDelegate = valueProvider;
        }

        public ValueProviderDelegate ValueDelegate { get; set; }

        #region IValueProvider Members

        public object GetValue(object param)
        {
            return ValueDelegate.Invoke(param);
        }

        #endregion
    }

    public class CustomValueProvider<TResultType, TSourceType> : IValueProvider<TResultType, TSourceType>
    {
        public CustomValueProvider(ValueProviderDelegate<TResultType, TSourceType> valueDel)
        {
            ValueDelegate = valueDel;
        }

        public ValueProviderDelegate<TResultType, TSourceType> ValueDelegate { get; set; }

        #region IValueProvider<TResultType,TSourceType> Members

        public TResultType GetValue(TSourceType source)
        {
            return ValueDelegate.Invoke(source);
        }

        #endregion

        #region IValueProvider Members

        public object GetValue(object source)
        {
            return GetValue((TSourceType)source);
        }

        #endregion
    }

    public class CustomValueProvider<TSourceType> : CustomValueProvider<Object, TSourceType>
    {
        public CustomValueProvider(ValueProviderDelegate<Object, TSourceType> valueDel) : base(valueDel) { }
    }

    public delegate Object ValueProviderDelegate(Object item);
    public delegate TResultType ValueProviderDelegate<TResultType, TSourceType>(TSourceType item);
}
