using System;
using System.IO;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class TemporaryFolderTests
    {
        [Test]
        public void TemporaryFolder_basedOnITemporaryFolder()
        {
            var temporaryFolder = new TemporaryFolder();
            Assert.That(temporaryFolder, Is.InstanceOf<ITemporaryFolder>());
        }
        [Test]
        public void Ctor_Folder_PlacedIntoWindowsTempoFolder()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                Assert.That(temporaryFolder.Folder.StartsWith(Path.GetTempPath()), Is.True);
            }
        }
        [Test]
        public void Dispose_DeletedFolder()
        {
            string createdTemporaryFolder;
            using (var temporaryFolder = new TemporaryFolder())
            {
                createdTemporaryFolder = temporaryFolder.Folder;
                Directory.Exists(temporaryFolder.Folder);
                File.WriteAllText(Path.Combine(temporaryFolder.Folder, "file.txt"), "test");
                string subFolder = Path.Combine(temporaryFolder.Folder, "subFolder");
                Directory.CreateDirectory(subFolder);
                File.WriteAllText(Path.Combine(subFolder, "file.txt"), "test");
            }
            Assert.That(Directory.Exists(createdTemporaryFolder), Is.False);
        }
        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void FileName_AfterDispose_GenerateObjectDisposedException()
        {
            var temporaryFolder = new TemporaryFolder();
            Assert.IsNotNull(temporaryFolder.Folder);
            temporaryFolder.Dispose();
            string folder = temporaryFolder.Folder;
            Assert.Fail("expected exception ObjectDisposedException");
        }
    }
}