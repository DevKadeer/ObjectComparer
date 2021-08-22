using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ObjectComparer.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<Type, HashSet<object>> TypeAndEnumValues = new ConcurrentDictionary<Type, HashSet<object>>();
        private static readonly ConcurrentDictionary<Type, TypeConverter> TypeAndConverters = new ConcurrentDictionary<Type, TypeConverter>();

        public static bool TryChangeType(this object value, Type destinationType, out object changedValue)
        {
            var tc = TypeAndConverters.GetOrAdd(destinationType, _ => TypeDescriptor.GetConverter(destinationType));
            if (tc is EnumConverter enumConverter)
            {
                return TryConvertFromEnum(value, destinationType, enumConverter, out changedValue);
            }
            if (tc is NullableConverter nullableConverter && nullableConverter.UnderlyingTypeConverter is EnumConverter nullableEnumConverter)
            {
                if (value == null)
                {
                    changedValue = null;
                    return true;
                }
                return TryConvertFromEnum(value, nullableConverter.UnderlyingType, nullableEnumConverter, out changedValue);
            }
            if (tc.IsValid(value) || tc.CanConvertFrom(value?.GetType()))
            {
                changedValue = tc.ConvertFrom(value);
                return true;

            }

            if (Nullable.GetUnderlyingType(destinationType) != null)
            {
                return TryConvertType(value, Nullable.GetUnderlyingType(destinationType), out changedValue);
            }
            return TryConvertType(value, destinationType, out changedValue);
        }

        private static bool TryConvertType(object value, Type conversionType, out object val)
        {
            try
            {
                val = Convert.ChangeType(value, conversionType);
                return true;
            }
            catch (Exception)
            {
                val = null;
                return false;
            }
        }

        private static bool TryConvertFromEnum(object value, Type t, EnumConverter enumConverter, out object convertedVal)
        {
            var values = TypeAndEnumValues.GetOrAdd(t, _ =>
            {
                var prop = Enum.GetValues(t).OfType<object>().Select(o => new { Key = (int)o, Value = o });
                var val = new HashSet<object>();
                foreach (var element in prop)
                {
                    val.Add(element.Key);
                    val.Add(element.Key.ToString());
                    val.Add(element.Value);
                    val.Add(element.Value.ToString());
                }
                return val;
            });
            if (values.Contains(value))
            {
                convertedVal = enumConverter.ConvertFrom(value.ToString());
                return true;
            }
            convertedVal = null;
            return false;
        }
    }
}