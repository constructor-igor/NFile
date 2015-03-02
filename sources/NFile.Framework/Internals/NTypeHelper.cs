using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NFile.Framework.Internals
{
    internal class NTypeHelper
    {
        /*
         * https://social.msdn.microsoft.com/Forums/vstudio/en-US/d8bfc1d6-fea5-4d6d-bb6d-1a16a181f0cf/getting-the-type-of-an-ienumerables-items-using-reflection
        * */

        internal static Type GetEnumerableType(Type dataObjectType)
        {
            //Type dataObjectType = dataObject.GetType();
            if (dataObjectType.IsArray)
            {
                return dataObjectType.GetElementType();
            }

            Type elementType = null;

            Type[] interfaces = dataObjectType.GetInterfaces();
            foreach (Type i in interfaces)
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = i.GetGenericArguments()[0];
                }
            }
            return elementType;
        }
        internal static bool IsEnumerableType(Type dataObjectType)
        {
            if (dataObjectType == typeof(string))
                return false;
            if (dataObjectType.IsArray)
                return true;

            Type[] interfaces = dataObjectType.GetInterfaces();
            return interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        internal static bool SupportedProperty(PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;
            return !propertyType.IsClass || propertyType == typeof(String) || IsEnumerableType(propertyType);
        }
    }
}