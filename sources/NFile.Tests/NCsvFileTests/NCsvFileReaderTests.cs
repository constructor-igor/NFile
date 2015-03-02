using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests.NCsvFileTests
{
    [TestFixture]
    public class NCsvFileReaderTests
    {
        [Test]
        public void TableHeader_EmptyList()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name,Id,Growth");

            using (NCsvFileReader csvFile = new NCsvFileReader(new StringReader(sb.ToString())))
            {
                IList<DataItem> actualContent = csvFile.ReadLines<DataItem>();
                Assert.That(actualContent.Any(), Is.False);
            }
        }
        [Test]
        public void TableHeaderData_Data()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name,Id,Growth");
            sb.AppendLine("Joe,1,170.5");
            sb.AppendLine("Tom,2,180.5");

            List<DataItem> expected = new List<DataItem>
            {
                new DataItem {Name = "Joe", Id = 1, Growth = 170.5},
                new DataItem {Name = "Tom", Id = 2, Growth = 180.5}
            };

            using (NCsvFileReader csvFile = new NCsvFileReader(new StringReader(sb.ToString())))
            {
                IList<DataItem> actualContent = csvFile.ReadLines<DataItem>();                
                Assert.That(actualContent, Is.EquivalentTo(expected));
            }
        }
    }
}