using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimilarObjectComparer.Extensions
{
    public static class PropertyExtension
    {
        private static readonly ConcurrentBag<Type> GetterTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentBag<Type> SetterTypes = new ConcurrentBag<Type>();
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> Getter = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();
        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> Setter = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
        private static readonly MethodInfo GetMethodInfo = typeof(PropertyExtension).GetMethod(nameof(GetDelegate), BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo SetMethodInfo = typeof(PropertyExtension).GetMethod(nameof(SetDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        /// More Faster than GetValue
        /// Returns value of property using delegate
        /// Most optimized code
        /// </summary>
        /// <param name="property"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetVal(this PropertyInfo property, object instance)
        {

            if (!Getter.ContainsKey(property) && !GetterTypes.Contains(property.DeclaringType))
            {
                var delCollection = property.DeclaringType.GetAllProperties().Where(x => x.CanRead).Select(x =>
                {
                    var getMethod = x.GetMethod;
                    var declaringClass = x.DeclaringType;
                    var typeOfResult = x.PropertyType;
                    var getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);
                    var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);
                    var callInnerGenericMethodWithTypes = GetMethodInfo
                        .MakeGenericMethod(declaringClass, typeOfResult);
                    return (x, (Func<object, object>)callInnerGenericMethodWithTypes.Invoke(null, new object[] { getMethodDelegate }));
                });
                foreach (var tuple in delCollection)
                {
                    Getter.TryAdd(tuple.x, tuple.Item2);
                }
                GetterTypes.Add(property.DeclaringType);
            }
            if (!property.CanRead)
            {
                throw new InvalidOperationException($"Property {property.Name} of {property.DeclaringType.FullName} is read protected.");
            }

            return (Getter.ContainsKey(property) ? Getter[property] : property.GetValue).Invoke(instance);
        }

        /// <summary>
        /// More Faster than SetValue
        /// Sets value to property using delegate
        /// </summary>
        /// <param name="property"></param>
        /// <param name="instance"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void SetVal(this PropertyInfo property, object instance, object value)
        {

            if (!Setter.ContainsKey(property) && !SetterTypes.Contains(property.DeclaringType))
            {
                var delCollection = property.DeclaringType.GetAllProperties().Where(x => x.CanWrite).Select(x =>
                {
                    var getSetMethod = x.SetMethod;
                    var declaringClass = x.DeclaringType;
                    var inputParamType = getSetMethod.GetParameters()[0].ParameterType;
                    var setMethodDelegateType = typeof(Action<,>).MakeGenericType(declaringClass, inputParamType);
                    var setMethodDelegate = getSetMethod.CreateDelegate(setMethodDelegateType);
                    var callInnerGenericMethodWithTypes = SetMethodInfo
                        .MakeGenericMethod(declaringClass, inputParamType);
                    return (x, (Action<object, object>)callInnerGenericMethodWithTypes.Invoke(null, new object[] { setMethodDelegate }));
                });
                foreach (var tuple in delCollection)
                {
                    Setter.TryAdd(tuple.x, tuple.Item2);
                }
                SetterTypes.Add(property.DeclaringType);
            }

            if (!property.CanWrite)
            {
                throw new InvalidOperationException($"Property {property.Name} of {property.DeclaringType.FullName} is write protected.");
            }

            (Setter.ContainsKey(property) ? Setter[property] : property.SetValue).Invoke(instance, value);
        }

        private static Func<object, object> GetDelegate<TClass, TResult>(Func<TClass, TResult> @delegate) => instance => @delegate((TClass)instance);
        private static Action<object, object> SetDelegate<TClass, TInput>(Action<TClass, TInput> @delegate) => (instance, value) => @delegate((TClass)instance, (TInput)value);

    }
}
