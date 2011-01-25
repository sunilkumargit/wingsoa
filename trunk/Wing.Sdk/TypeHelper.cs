namespace Wing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    [System.Diagnostics.DebuggerStepThrough]
    public static class TypeHelper
    {

        // method is used primarily for lighting up new .NET Framework features even if MVC targets the previous version
        // thisParameter is the 'this' parameter if target method is instance method, should be null for static method
        public static TDelegate CreateDelegate<TDelegate>(Assembly assembly, string typeName, string methodName, object thisParameter) where TDelegate : class
        {
            // ensure target type exists
            Type targetType = assembly.GetType(typeName, false /* throwOnError */);
            if (targetType == null)
            {
                return null;
            }

            return CreateDelegate<TDelegate>(targetType, methodName, thisParameter);
        }

        public static TDelegate CreateDelegate<TDelegate>(Type targetType, string methodName, object thisParameter) where TDelegate : class
        {
            // ensure target method exists
            ParameterInfo[] delegateParameters = typeof(TDelegate).GetMethod("Invoke").GetParameters();
            Type[] argumentTypes = Array.ConvertAll(delegateParameters, pInfo => pInfo.ParameterType);
            MethodInfo targetMethod = targetType.GetMethod(methodName, argumentTypes);
            if (targetMethod == null)
            {
                return null;
            }

            TDelegate d = Delegate.CreateDelegate(typeof(TDelegate), thisParameter, targetMethod, false /* throwOnBindFailure */) as TDelegate;
            return d;
        }


        public static Type ExtractGenericInterface(Type queryType, Type interfaceType)
        {
            Func<Type, bool> matchesInterface = t => t.IsGenericType && t.GetGenericTypeDefinition() == interfaceType;
            return (matchesInterface(queryType)) ? queryType : queryType.GetInterfaces().FirstOrDefault(matchesInterface);
        }

        public static object GetDefaultValue(Type type)
        {
            return (TypeAllowsNullValue(type)) ? null : Activator.CreateInstance(type);
        }

        public static bool IsCompatibleObject<T>(object value)
        {
            return (value is T || (value == null && TypeAllowsNullValue(typeof(T))));
        }

        public static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static bool StrongTryGetValueImpl<TKey, TValue>(object dictionary, string key, out object value)
        {
            IDictionary<TKey, TValue> strongDict = (IDictionary<TKey, TValue>)dictionary;

            TValue strongValue;
            bool retVal = strongDict.TryGetValue((TKey)(object)key, out strongValue);
            value = strongValue;
            return retVal;
        }

        private static bool TryGetValueFromNonGenericDictionary(object dictionary, string key, out object value)
        {
            IDictionary weakDict = (IDictionary)dictionary;

            bool containsKey = weakDict.Contains(key);
            value = (containsKey) ? weakDict[key] : null;
            return containsKey;
        }

        public static bool TypeAllowsNullValue(Type type)
        {
            return (!type.IsValueType || IsNullableValueType(type));
        }
    }
}
