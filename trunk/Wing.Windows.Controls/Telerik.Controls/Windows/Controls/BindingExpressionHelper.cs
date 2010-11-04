namespace Telerik.Windows.Controls
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;

    public class BindingExpressionHelper : FrameworkElement
    {
        private static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(BindingExpressionHelper), null);

        public static Func<object, object> CreateGetValueFunc(Type itemType, string propertyPath)
        {
            System.Linq.Expressions.Expression get;
            if ((propertyPath != null) && (propertyPath.IndexOfAny(new char[] { '.', '[', ']', '(', ')' }) > 0))
            {
                return item => GetValueThroughBinding(item, propertyPath);
            }
            ParameterExpression parameter = System.Linq.Expressions.Expression.Parameter(itemType, "item");
            if (string.IsNullOrEmpty(propertyPath))
            {
                get = parameter;
            }
            else
            {
                try
                {
                    get = System.Linq.Expressions.Expression.PropertyOrField(parameter, propertyPath);
                }
                catch (ArgumentException)
                {
                    get = System.Linq.Expressions.Expression.Constant(null);
                }
            }
            LambdaExpression lambda = System.Linq.Expressions.Expression.Lambda(get, new ParameterExpression[] { parameter });
            Delegate compiled = lambda.Compile();
            return (Func<object, object>)typeof(BindingExpressionHelper).GetMethod("ToUntypedFunc", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(new Type[] { itemType, lambda.Body.Type }).Invoke(null, new object[] { compiled });
        }

        public static object GetValue(object item, string propertyPath)
        {
            if (item == null)
            {
                return null;
            }
            return CreateGetValueFunc(item.GetType(), propertyPath)(item);
        }

        internal static object GetValue(object item, Binding binding)
        {
            return GetValueThroughBinding(item, binding);
        }

        private static object GetValueThroughBinding(object item, string propertyPath)
        {
            return GetValueThroughBinding(item, new Binding(propertyPath ?? "."));
        }

        private static object GetValueThroughBinding(object item, Binding binding)
        {
            object result;
            BindingExpressionHelper helper = new BindingExpressionHelper();
            try
            {
                helper.DataContext = item;
                BindingOperations.SetBinding(helper, ValueProperty, binding);
                result = helper.GetValue(ValueProperty);
            }
            finally
            {
                helper.ClearValue(ValueProperty);
            }
            return result;
        }

        private static Func<object, object> ToUntypedFunc<T, TResult>(Func<T, TResult> func)
        {
            return item => func((T)item);
        }
    }
}

