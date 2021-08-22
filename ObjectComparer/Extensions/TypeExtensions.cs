using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ObjectComparer.Extensions
{
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypeAndPropInfos = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public static PropertyInfo[] GetAllProperties(this Type type)
        {
            return TypeAndPropInfos.GetOrAdd(type, _ => type.GetProperties());
        }
        public static bool InheritsOrImplements(this Type child, Type parent)
        {
            parent = ResolveGenericTypeDefinition(parent);

            var currentChild = child.IsGenericType
                                   ? child.GetGenericTypeDefinition()
                                   : child;

            while (currentChild != typeof(object))
            {
                if (parent == currentChild || HasAnyInterfaces(parent, currentChild))
                    return true;

                if (currentChild.BaseType != null && currentChild.BaseType == parent)
                {
                    return true;
                }

                currentChild = currentChild.BaseType != null && currentChild.BaseType.IsGenericType
                                   ? currentChild.BaseType.GetGenericTypeDefinition()
                                   : currentChild.BaseType;

                if (currentChild == null)
                    return false;
            }
            return false;
        }


        private static bool HasAnyInterfaces(Type parent, Type child)
        {
            return child.GetInterfaces()
                .Any(childInterface =>
                {
                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == parent;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type parent)
        {
            var shouldUseGenericType = true;
            if (parent.IsGenericType && parent.GetGenericTypeDefinition() != parent)
                shouldUseGenericType = false;

            if (parent.IsGenericType && shouldUseGenericType)
                parent = parent.GetGenericTypeDefinition();
            return parent;
        }


        public static bool IsNullablePrimitive(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        public static bool IsNullablePrimitive(this PropertyInfo type)
        {
            return Nullable.GetUnderlyingType(type.PropertyType) != null;
        }
    }
}
