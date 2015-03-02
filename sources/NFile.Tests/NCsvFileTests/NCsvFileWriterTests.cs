using System.Collections.Generic;
using System.IO;
using System.Text;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class NCsvFileWriterTests
    {
        [Test]
        public void GenericList_TableHeader()
        {
            var sb = new StringBuilder();
            using (NCsvFile csvFile = new NCsvFile(new StringWriter(sb)))
            {
                csvFile.Write(new List<DataItem>());
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder().AppendLine("Name,Id,Growth").ToString()));
        }
        [Test]
        public void TypedArray_TableHeader()
        {
            var sb = new StringBuilder();
            using (NCsvFile csvFile = new NCsvFile(new StringWriter(sb)))
            {
                csvFile.Write(new DataItem[]{});
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder().AppendLine("Name,Id,Growth").ToString()));
        }
        [Test]
        [ExpectedException]
        public void List_Exception()
        {
            var sb = new StringBuilder();
            using (NCsvFile csvFile = new NCsvFile(new StringWriter(sb)))
            {
                csvFile.Write(new List());
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
            using (NCsvFile csvFile = new NCsvFile(new StringWriter(sb)))
            {
                csvFile.Write(dataItemList);
            }
            Assert.That(sb.ToString(), Is.EqualTo(new StringBuilder()
                .AppendLine("Name,Id,Growth")
                .AppendLine("Joe,1,170.5")
                .AppendLine("Tom,2,180.5")
                .ToString()));
        }
    }

    public class DataItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double Growth { get; set; }
    }
    
}