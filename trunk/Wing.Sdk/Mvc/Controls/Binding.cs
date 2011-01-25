using System;


namespace Wing.Mvc.Controls
{
    public sealed class Binding
    {
        private string _propertyPath;
        private String[] _pathParts;
        private ResolveMethod[] _resolveMethods;
        private IValueProvider _valueProvider;
        private bool _resolvePending = true;

        private enum ResolveMethod
        {
            ViewDataEntry,
            PropertyValue,
            MethodCall,
            CurrentDataItem,
            VisualTreeObject
        };


        [System.Diagnostics.DebuggerStepThrough]
        public Binding()
        {

        }

        [System.Diagnostics.DebuggerStepThrough]
        public Binding(String propertyPath)
        {
            PropertyPath = propertyPath;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Binding(String propertyPath, IValueFormatter formatter, String formatString = null)
        {
            PropertyPath = propertyPath;
            Formatter = formatter;
            FormatString = formatString;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Binding(IValueProvider valueProvider, IValueFormatter formatter, String formatString = null)
        {
            ValueProvider = valueProvider;
            Formatter = formatter;
            FormatString = formatString;
        }

        public String PropertyPath
        {
            get { return _propertyPath; }
            set
            {
                if (_propertyPath == value)
                    return;
                _propertyPath = value;
                _resolvePending = true;
            }
        }

        public IValueProvider ValueProvider
        {
            get { return _valueProvider; }
            set
            {
                if (_valueProvider == value)
                    return;
                _valueProvider = value;
                _resolvePending = true;
            }
        }

        public String FormatString { get; set; }
        public IValueFormatter Formatter { get; set; }

        private void CompilePropertyPath()
        {
            _resolvePending = false;
            _pathParts = null;
            if (_propertyPath.IsEmpty() || _valueProvider != null)
                return;

            _pathParts = _propertyPath.Split('.');
            _resolveMethods = new ResolveMethod[_pathParts.Length];
            var symbol = _pathParts[0][0];
            var i = 1;

            switch (symbol)
            {
                case '@': _resolveMethods[0] = ResolveMethod.ViewDataEntry; break;
                case '=': _resolveMethods[0] = ResolveMethod.CurrentDataItem; break;
                case '#': _resolveMethods[0] = ResolveMethod.VisualTreeObject; break;
                default:
                    i = 0;
                    break;
            }

            if (i == 1)
                _pathParts[0] = _pathParts[0].Substring(1);

            for (; i < _pathParts.Length; i++)
            {
                if (_pathParts[i].EndsWith("()"))
                {
                    _resolveMethods[i] = ResolveMethod.MethodCall;
                    _pathParts[i] = _pathParts[i].Substring(0, _pathParts[i].Length - 2);
                }
                else
                    _resolveMethods[i] = ResolveMethod.PropertyValue;
            }
        }

        internal void Resolve(HtmlObject instance, ControlProperty targetProperty)
        {
            Object value = null;
            if (ValueProvider != null)
                value = ValueProvider.GetValue(instance.DataItem);
            else if (_pathParts != null || _resolvePending)
            {
                if (_resolvePending)
                    CompilePropertyPath();
                var data = instance.DataItem;
                for (var i = 0; i < _pathParts.Length; i++)
                {
                    var key = _pathParts[i];
                    try
                    {
                        switch (_resolveMethods[i])
                        {
                            case ResolveMethod.PropertyValue:
                                data = ReflectionHelper.ReadProperty(data, key); break;
                            case ResolveMethod.ViewDataEntry:
                                data = instance.CurrentContext[key]; break;
                            case ResolveMethod.MethodCall:
                                data = ReflectionHelper.InvokeInstanceMethod(data, key); break;
                            case ResolveMethod.CurrentDataItem:
                                data = instance.DataItem; break;
                            case ResolveMethod.VisualTreeObject:
                                throw new NotImplementedException();
                        }
                        if (data == null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new BindingException(String.Format("Error on resolve property path {0} to property {1}.{2}", PropertyPath, instance.GetType().Name, targetProperty.Name), ex);
                    }
                }
                value = data;
            }
            else
                value = instance.DataItem;

            if (Formatter != null)
            {
                value = Formatter.Format(value);
                if (Formatter.Align != TextAlignment.Default)
                    instance.SetValue(HtmlControl.TextAlignProperty, Formatter.Align);
            }
            if (!String.IsNullOrEmpty(FormatString))
                value = String.Format(FormatString, value);

            instance.SetValue(targetProperty, value);
        }
    }
}
