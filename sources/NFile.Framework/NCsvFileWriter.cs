using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NFile.Framework
{
    public class NCsvFileWriter : IDisposable
    {
        private readonly TextWriter m_textWriter;

        public NCsvFileWriter(TextWriter textWriter)
        {
            m_textWriter = textWriter;
        }

        public void Write(object dataObject)
        {
            if (dataObject is IEnumerable)
            {
                Type itemType = GetEnumerableType(dataObject);
                if (itemType == null)
                    throw new ArgumentException(String.Format("Not found type of collection '{0}'.", dataObject.GetType()));

                PropertyInfo[] allPublicProperties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> properties = allPublicProperties.Where(pi => !pi.PropertyType.IsClass || pi.PropertyType == typeof(String)).ToList();
                List<string> columnsNames = properties.ConvertAll(pi => pi.Name);
                m_textWriter.WriteLine(String.Join(",", columnsNames));

                IEnumerable dataObjectList = dataObject as IEnumerable;
                foreach (object dataItem in dataObjectList)
                {
                    List<object> rowValues = properties.ConvertAll(pi => pi.GetValue(dataItem, null));
                    m_textWriter.WriteLine(String.Join(",", rowValues));
                }

                return;
            }
            throw new NotImplementedException();
        }

        #region IDisposable
        public void Dispose()
        {
            m_textWriter.Dispose();
        }
        #endregion

        /*
         * https://social.msdn.microsoft.com/Forums/vstudio/en-US/d8bfc1d6-fea5-4d6d-bb6d-1a16a181f0cf/getting-the-type-of-an-ienumerables-items-using-reflection
         * */
        private Type GetEnumerableType(object dataObject)
        {
            Type dataObjectType = dataObject.GetType();
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

    }
}
