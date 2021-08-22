using ObjectComparer.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectComparer
{
    public static class Utility
    {
        public static bool ComparGenerice<T>(T Object1, T object2)
        {
            //Get the type of the object
            Type type = typeof(T);

            //return false if any of the object is false
            if (Object1 == null || object2 == null)
                return false;

            //Loop through each properties inside class and get values for the property from both the objects and compare
            foreach (System.Reflection.PropertyInfo property in type.GetProperties())
            {
                if (property.Name != "ExtensionData")
                {
                    string Object1Value = string.Empty;
                    string Object2Value = string.Empty;
                    if (type.GetProperty(property.Name).GetValue(Object1, null) != null)
                        Object1Value = type.GetProperty(property.Name).GetValue(Object1, null).ToString();
                    if (type.GetProperty(property.Name).GetValue(object2, null) != null)
                        Object2Value = type.GetProperty(property.Name).GetValue(object2, null).ToString();
                    if (Object1Value.Trim() != Object2Value.Trim())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
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
                        var expectEnumerator = enumerable.GetEnumerator();
                        var actualEnumerator = ((IEnumerable)second).GetEnumerator();

                        var canGetExpectMember = expectEnumerator.MoveNext();
                        var canGetActualMember = actualEnumerator.MoveNext();

                        while (canGetExpectMember && canGetActualMember)
                        {
                            var isEqual = AreEqual(expectEnumerator.Current, actualEnumerator.Current);
                            if (!isEqual)
                            {
                                return false;
                            }

                            canGetExpectMember = expectEnumerator.MoveNext();
                            canGetActualMember = actualEnumerator.MoveNext();
                        }

                        return canGetExpectMember == canGetActualMember;
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
        public static bool ScrambledEquals<T>(IEnumerable<T> list1, IEnumerable<T> list2, IEqualityComparer<T> comparer)
        {
            var cnt = new Dictionary<T, int>(comparer);
            foreach (T s in list1)
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
            foreach (T s in list2)
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
