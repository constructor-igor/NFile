using System.IO;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class AcceptanceTests
    {
        [Test]
        public void TemporaryFileSimpleDemo()
        {
            using (TemporaryFile temporaryFile = new TemporaryFile())
            {
                File.WriteAllText(temporaryFile.FileName, "[0]: test");
                File.AppendAllText(temporaryFile.FileName, "[1]: additional text");

                string allText = File.ReadAllText(temporaryFile.FileName);
                Assert.AreEqual("[0]: test" + "[1]: additional text", allText);
            }
        }
        [Test]
        public void TemporaryFolderSimpleDemo()
        {
            using (TemporaryFolder temporaryFolder = new TemporaryFolder())
            {
                string filePath = Path.Combine(temporaryFolder.Folder, "file1.txt");
                File.WriteAllText(filePath, "[0]: test");
                File.AppendAllText(filePath, "[1]: additional text");

                string allText = File.ReadAllText(filePath);
                Assert.AreEqual("[0]: test" + "[1]: additional text", allText);
            }
        }
    }
}