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

            using (NCsvFileReader csvFile = new NCsvFileReader(new StringReader(sb.ToString())))
            {
                IList<DataItem> actualContent = csvFile.ReadLines<DataItem>();
                Assert.That(actualContent.Any(), Is.True);
            }
        }
    }
}