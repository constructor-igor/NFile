using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NFile.Framework.Internals;

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
            Type dataObjectType = dataObject.GetType();

            if (NTypeHelper.IsEnumerableType(dataObjectType))
            {
                List<PropertyInfo> properties = WriteHeader(dataObjectType, dataObject);

                IEnumerable dataObjectList = dataObject as IEnumerable;
                foreach (object dataItem in dataObjectList)
                {
                    List<object> rowValues = properties.ConvertAll(pi => pi.GetValue(dataItem, null));
                    m_textWriter.WriteLine(String.Join(",", rowValues));
                }

                return;
            }
            
            List<PropertyInfo> objectProperties = dataObjectType
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToList();

            foreach (PropertyInfo propertyInfo in objectProperties)
            {
                object propertyObject = propertyInfo.GetValue(dataObject, null);
                Type propertyType = propertyInfo.PropertyType;
                if (NTypeHelper.IsEnumerableType(propertyType))
                {
                    if (propertyObject == null)
                        WriteHeader(propertyType);
                    else
                        Write(propertyObject);
                } else if (propertyInfo.PropertyType.Name == "String")
                {
                    m_textWriter.WriteLine(propertyObject);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        private List<PropertyInfo> WriteHeader(Type dataObjectType, object dataObject = null)
        {
            Type itemType = NTypeHelper.GetEnumerableType(dataObjectType);
            if (itemType == null)
                throw new ArgumentException(String.Format("Not found type of collection '{0}'.", dataObjectType));

            PropertyInfo[] allPublicProperties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> properties = allPublicProperties.Where(NTypeHelper.SupportedProperty).ToList();

            List<string> columnsNames = new List<string>();
            foreach (PropertyInfo propertyInfo in properties)
            {
                if (NTypeHelper.IsEnumerableType(propertyInfo.PropertyType))
                {
                    if (dataObject != null)
                    {
                        int sizeOfList = (dataObject as ICollection).Count;
                        for (int i = 0; i < sizeOfList; i++)
                        {
                            columnsNames.Add(String.Format("{0}_{1}", propertyInfo.Name, i));
                        }
                    }
                }
                else
                {
                    columnsNames.Add(propertyInfo.Name);
                }
            }

            m_textWriter.WriteLine(String.Join(",", columnsNames));
            return properties;
        }

        #region IDisposable
        public void Dispose()
        {
            m_textWriter.Dispose();
        }
        #endregion
    }
}
