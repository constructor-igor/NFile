using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests.NCsvFileTests
{
    [TestFixture]
    public class NCsvFileWriterDataItemsTests
    {
        [Test]
        public void GenericList_TableHeader()
        {
            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(new List<DataItem>());
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder().AppendLine("Name,Id,Growth").ToString()));
        }
        [Test]
        public void TypedArray_TableHeader()
        {
            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(new DataItem[]{});
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder().AppendLine("Name,Id,Growth").ToString()));
        }
        [Test]
        [ExpectedException]
        public void List_Exception()
        {
            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(new ArrayList());
            }
        }

        [Test]
        public void GenericList_TableHeaderAndData()
        {
            List < DataItem > dataItemList = new List<DataItem>()
            {
                new DataItem {Name = "Joe", Id = 1, Growth = 170.5},
                new DataItem {Name = "Tom", Id = 2, Growth = 180.5}
            };

            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(dataItemList);
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder()
                .AppendLine("Name,Id,Growth")
                .AppendLine("Joe,1,170.5")
                .AppendLine("Tom,2,180.5")
                .ToString()));
        }
    }

    [TestFixture]
    public class NCsvFileWriterDataObjectTests
    {
        [Test]
        public void DataObjectWithEmptyList()
        {
            DataObject dataObject = new DataObject {VersionId = "Ver=1"};

            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(dataObject);
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder()
                .AppendLine("Ver=1")
                .AppendLine("Name,Id,Growth")
                .ToString()));
        }
        [Test]
        public void DataObjectWithDataItems()
        {
            DataObject dataObject = new DataObject
            {
                VersionId = "Ver=1",
                Items = new List<DataItem>
                {
                    new DataItem {Name = "Joe", Id = 1, Growth = 170.5},
                    new DataItem {Name = "Tom", Id = 2, Growth = 180.5}
                }
            };

            var sb = new StringBuilder();
            using (NCsvFileWriter csvFileWriter = new NCsvFileWriter(new StringWriter(sb)))
            {
                csvFileWriter.Write(dataObject);
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder()
                .AppendLine("Ver=1")
                .AppendLine("Name,Id,Growth")
                .AppendLine("Joe,1,170.5")
                .AppendLine("Tom,2,180.5")
                .ToString()));
        }
    }
}