using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NFile.Framework
{
    public class NCsvFileReader : IDisposable
    {
        private readonly TextReader m_textReader;

        public NCsvFileReader(TextReader textReader)
        {
            m_textReader = textReader;
        }

        public IList<T> ReadLines<T>()
        {
            Type itemType = typeof (T);
            IList<T> dataItems = new List<T>();
            List<string> header = m_textReader.ReadLine().Split(',').ToList();            

            string line;
            while ((line = m_textReader.ReadLine())!=null)
            {
                string[] rowItems = line.Split(',');

                T dataItem = (T) Activator.CreateInstance(itemType);

                PropertyInfo[] allPublicProperties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                List<PropertyInfo> properties = allPublicProperties.Where(pi => !pi.PropertyType.IsClass || pi.PropertyType == typeof(String)).ToList();

                for (int i = 0; i < properties.Count; i++)
                {
                    PropertyInfo property = properties[i];

                    switch (property.PropertyType.Name)
                    {
                        case "Int32":
                            property.SetValue(dataItem, int.Parse(rowItems[i]), null);
                            break;
                        case "Int64":
                            property.SetValue(dataItem, int.Parse(rowItems[i]), null);
                            break;
                        case "Float":
                            property.SetValue(dataItem, float.Parse(rowItems[i]), null);
                            break;
                        case "Double":
                            property.SetValue(dataItem, double.Parse(rowItems[i]), null);
                            break;
                        case "String":
                            property.SetValue(dataItem, rowItems[i], null);
                            break;
                    }                    
                }
                dataItems.Add(dataItem);
            }

            return dataItems;
        }
        public T Read<T>()
        {
            throw new NotImplementedException();
        }

        #region IDisposable
        public void Dispose()
        {
            m_textReader.Dispose();
        }
        #endregion
    }
}