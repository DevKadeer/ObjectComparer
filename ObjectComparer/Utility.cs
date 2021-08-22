using ObjectComparer.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ObjectComparer
{
    public static class Utility
    {
        public static readonly List<string> PrimitiveTypesExceptByte = new List<string>
        {
            $"System.{nameof(String)}",$"System.{nameof(Decimal)}",$"System.{nameof(Double)}",$"System.{nameof(Single)}",
            $"System.{nameof(Int64)}",$"System.{nameof(Int32)}",$"System.{nameof(Int16)}",
            $"System.{nameof(UInt64)}",$"System.{nameof(UInt32)}",$"System.{nameof(UInt16)}" ,
            $"System.{nameof(Boolean)}",$"System.{nameof(DateTime)}",$"System.{nameof(Enum)}"
            ,$"System.{nameof(Char)}"
        };
        public static bool AreEqual(object first, object second)
        {
            if (first == null || second == null)
            {
                return first == null && second == null;
            }

            var firstType = first.GetType();
            var secondType = second.GetType();

            if (firstType.InheritsOrImplements(typeof(IEquatable<>)))
            {
                return Compare(first, second);
            }

            switch (first)
            {
                case IComparable comparable:
                    return comparable.CompareTo(second) == 0;
                case IEnumerable enumerable:
                    {
                        return listEqualsWithoutOrder(enumerable, (IEnumerable)second);
                    }
            }

            // If it is class.
            var properties = firstType.GetProperties().Where(p => p.GetIndexParameters().Length == 0);
            foreach (var property in properties)
            {
                var property1 = firstType.GetProperty(property.Name);
                var property2 = secondType.GetProperty(property.Name);

                if (property2 == null || property1 == null)
                {
                    return false;
                }

                var expectValue = property1.GetValue(first);
                var actualValue = property2.GetValue(second);
                if (expectValue == null || actualValue == null)
                {
                    if (expectValue == null && actualValue == null)
                    {
                        continue;
                    }

                    return false;
                }

                var isEqual = AreEqual(expectValue, actualValue);

                if (isEqual == false)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Compare(object first, object second)
        {
            var firstType = first.GetType();
            var secondType = second.GetType();

            if (first.GetType() == second.GetType())
            {
                if (first is string firstString && second is string secondString)
                {
                    return firstString.Trim().IsEqualTo(secondString.Trim());
                }

                return first.Equals(second);
            }

            if (first.TryChangeType(secondType, out var casteFirst) && second.TryChangeType(firstType, out var casteSecond))
            {
                var stringType = typeof(string);
                var firstIsStringType = firstType == stringType && casteSecond?.GetType() == stringType;
                var secondIsStringType = secondType == stringType && casteFirst?.GetType() == stringType;
                return (firstIsStringType || Compare(first, casteSecond)) && (secondIsStringType || Compare(second, casteFirst));
            }

            return first.Equals(second);
        }

        public static bool listEqualsWithoutOrder(IEnumerable list1, IEnumerable list2)
        {
            var cnt = new Dictionary<object, int>();
            foreach (object s in list1)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]++;
                }
                else
                {
                    cnt.Add(s, 1);
                }
            }
            foreach (object s in list2)
            {
                if (cnt.ContainsKey(s))
                {
                    cnt[s]--;
                }
                else
                {
                    return false;
                }
            }
            return cnt.Values.All(c => c == 0);
        }
    }
}
