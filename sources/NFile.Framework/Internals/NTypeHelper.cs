using System;
using System.Collections.Generic;
using System.Linq;

namespace NFile.Framework.Internals
{
    internal class NTypeHelper
    {
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
    }
}