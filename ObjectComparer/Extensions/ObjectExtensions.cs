using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SimilarObjectComparer.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<Type, HashSet<object>> TypeAndEnumValues = new ConcurrentDictionary<Type, HashSet<object>>();
        private static readonly ConcurrentDictionary<Type, TypeConverter> TypeAndConverters = new ConcurrentDictionary<Type, TypeConverter>();

        public static bool CompareTo(this object obj, object another)
        {
            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            if (obj.GetType() != another.GetType()) return false;

            //properties: int, double, DateTime, etc, not class
            if (!obj.GetType().IsClass) return obj.Equals(another);

            var result = true;
            foreach (var property in obj.GetType().GetAllProperties())
            {
                var objValue = property.GetVal(obj);
                var anotherValue = property.GetVal(another);
                //Recursion
                if (!objValue.DeepCompare(anotherValue)) result = false;
            }
            return result;
        }

        public static bool DeepCompare(this object obj, object another)
        {
            if (ReferenceEquals(obj, another)) return true;
            if ((obj == null) || (another == null)) return false;
            //Compare two object's class, return false if they are difference
            if (obj.GetType() != another.GetType()) return false;

            var result = true;
            //Get all properties of obj
            //And compare each other
            foreach (var property in obj.GetType().GetAllProperties())
            {
                var objValue = property.GetVal(obj);
                var anotherValue = property.GetVal(another);
                if (!objValue.Equals(anotherValue)) result = false;
            }

            return result;
        }

        //public static bool JsonCompare(this object obj, object another, bool ignorecase = true)
        //{
        //    if (ReferenceEquals(obj, another)) return true;
        //    if ((obj == null) || (another == null)) return false;
        //    if (obj.GetType() != another.GetType()) return false;

        //    var objJson = JsonSerializer.Serialize(obj, GeneralConstants.DefaultJsonSerializerOptions);
        //    var anotherJson = JsonSerializer.Serialize(another, GeneralConstants.DefaultJsonSerializerOptions);

        //    return string.Equals(RegExConstants.RemoveSpace(objJson), RegExConstants.RemoveSpace(anotherJson), ignorecase ? System.StringComparison.InvariantCultureIgnoreCase : System.StringComparison.CurrentCulture);
        //}

        //public static object ChangeType(this object value, Type destinationType)
        //{
        //    var tc = TypeAndConverters.GetOrAdd(destinationType, _ => TypeDescriptor.GetConverter(destinationType));
        //    if (tc is EnumConverter enumConverter)
        //    {
        //        if (TryConvertFromEnum(value, destinationType, enumConverter, out var converted))
        //        {
        //            return converted;
        //        }
        //        throw new InvalidOperationException($"{value} cannot be converted to Enum {destinationType.FullName}");
        //    }
        //    if (tc is NullableConverter nullableConverter && nullableConverter.UnderlyingTypeConverter is EnumConverter nullableEnumConverter)
        //    {
        //        if (value == null)
        //        {
        //            return null;
        //        }
        //        if (TryConvertFromEnum(value, nullableConverter.UnderlyingType, nullableEnumConverter, out var converted))
        //        {
        //            return converted;
        //        }
        //        throw new InvalidOperationException($"{value} cannot be converted to Enum {destinationType.FullName}");
        //    }

        //    if (tc.IsValid(value) || tc.CanConvertFrom(value?.GetType()))
        //    {
        //        return tc.ConvertFrom(value);
        //    }

        //    if (destinationType.IsNullablePrimitive())
        //    {
        //        return Convert.ChangeType(value, Nullable.GetUnderlyingType(destinationType));
        //    }

        //    return Convert.ChangeType(value, destinationType);
        //}

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
            if (destinationType.IsNullablePrimitive())
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