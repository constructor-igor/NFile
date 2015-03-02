using System.IO;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class TemporaryCollectionTests
    {
        [Test]
        public void Dispose_Add3TemporaryFiles_FilesDeleted()
        {
            string fileName1;
            string fileName2;
            string fileName3;
            using (var temporaryCollection = new TemporaryCollection())
            {
                ITemporaryFile file1 = temporaryCollection.AddFile();
                ITemporaryFile file2 = temporaryCollection.AddFile(".log");
                ITemporaryFile file3 = temporaryCollection.AddFile(new TemporaryFile());

                File.WriteAllText(file2.FileName, "");

                fileName1 = file1.FileName;
                fileName2 = file2.FileName;
                fileName3 = file3.FileName;

                Assert.That(File.Exists(fileName1), Is.True);
                Assert.That(File.Exists(fileName2), Is.True);
                Assert.That(File.Exists(fileName3), Is.True);
            }
            Assert.That(File.Exists(fileName1), Is.False);
            Assert.That(File.Exists(fileName2), Is.False);
            Assert.That(File.Exists(fileName3), Is.False);
        }
        [Test]
        public void Dispose_AddTemporaryFileAndTemporaryFolder_AllDeleted()
        {
            string fileName;
            string folderName;
            using (var temporaryCollection = new TemporaryCollection())
            {
                ITemporaryFile file = temporaryCollection.AddFile();
                ITemporaryFolder folder = temporaryCollection.AddFolder();

                fileName = file.FileName;
                folderName = folder.Folder;

                Assert.That(File.Exists(fileName), Is.True);
                Assert.That(Directory.Exists(folderName), Is.True);
            }
            Assert.That(File.Exists(fileName), Is.False);
            Assert.That(Directory.Exists(folderName), Is.False);
        }
        [Test]
        public void Dispose_DoubleRun_NoExceptions()
        {
            var temporaryCollection = new TemporaryCollection();
            try
            {
                temporaryCollection.AddFile();
                temporaryCollection.AddFolder();
            }
            finally
            {
                temporaryCollection.Dispose();
                temporaryCollection.Dispose();
            }
            Assert.Pass();
        }
    }
}