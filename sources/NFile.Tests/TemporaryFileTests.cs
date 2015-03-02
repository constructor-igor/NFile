using System;
using System.IO;
using Moq;
using NFile.Framework;
using NFile.Framework.Internals;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class TemporaryFileTests
    {
        [Test]
        public void TemporaryFile_basedOnITemporaryFile()
        {
            var temporaryFile = new TemporaryFile();
            Assert.IsInstanceOf<ITemporaryFile>(temporaryFile);
        }
        [Test]
        public void Constructor_Empty_FileNameExtensionTmp()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.AreEqual(".tmp", Path.GetExtension(temporaryFile.FileName));
            }
        }
        [Test]
        public void Constructor_Empty_FileExists()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.IsTrue(File.Exists(temporaryFile.FileName));
            }
        }
        [Test]
        public void Constructor_Empty_FilePlacedIntoTemporaryFolder()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.AreEqual(Path.GetTempPath(), Path.GetDirectoryName(temporaryFile.FileName)+@"\");
            }
        }
        [Test]
        public void Dispose_Empty_FileDeleted()
        {
            string fileName;
            using (var temporaryFile = new TemporaryFile())
            {
                fileName = temporaryFile.FileName;
            }
            Assert.IsFalse(File.Exists(fileName));
        }
        [Test]
        public void Constructor_ExtensionTxt_FileNameExtensionTxt()
        {
            string fileName;
            using (var temporaryFile = new TemporaryFile(".txt"))
            {
                fileName = temporaryFile.FileName;

                Assert.AreEqual(".txt", Path.GetExtension(temporaryFile.FileName));
                Assert.AreEqual(Path.GetTempPath(), Path.GetDirectoryName(temporaryFile.FileName) + @"\");
                
                Assert.IsFalse(File.Exists(temporaryFile.FileName));
                File.WriteAllBytes(temporaryFile.FileName, new byte[]{0});
                Assert.IsTrue(File.Exists(temporaryFile.FileName));
            }
            Assert.IsFalse(File.Exists(fileName));
        }
        [Test]
        public void TemporaryFile_NoDispose_FileDeleted()
        {
            const string APP_TRANSFER_FILE_NAME = "TemporaryFileName";
            AppDomain testDomain = CreateDomain();

            testDomain.DoCallBack(delegate
                {
                    var temporaryFile = new TemporaryFile();
                    Assert.IsTrue(File.Exists(temporaryFile.FileName));
                    AppDomain.CurrentDomain.SetData(APP_TRANSFER_FILE_NAME, temporaryFile.FileName);
                }
            );
            var createdTemporaryFileName = (string)testDomain.GetData(APP_TRANSFER_FILE_NAME);
            Assert.IsTrue(File.Exists(createdTemporaryFileName));
            AppDomain.Unload(testDomain);

            Assert.IsFalse(File.Exists(createdTemporaryFileName), "temporary file should be deleted by finalizer");
        }
        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void FileName_AfterDispose_GenerateObjectDisposedException()
        {
            var temporaryFile = new TemporaryFile();
            Assert.IsNotNull(temporaryFile.FileName);
            temporaryFile.Dispose();
            string fileName = temporaryFile.FileName;
            Assert.Fail("expected exception ObjectDisposedException");
        }
        [Test]
        public void Dispose_DoubleCalling_NoIssues()
        {
            var mock = new Mock<IFileOperations>();
            mock.Setup(foo => foo.GetTempFileName()).Returns(() => "fileName.tmp");

            var temporaryFile = TemporaryFile.Create(mock.Object);
            temporaryFile.Dispose();
            temporaryFile.Dispose();
            mock.Verify(foo => foo.Delete("fileName.tmp"), Times.Once());
            mock.Verify(foo => foo.Delete(It.IsAny<string>()), Times.Once());
        }
        [Test, ExpectedException(typeof(IOException))]
        public void Dispose_FileCaught_ExpectedException()
        {
            var temporaryFile = new TemporaryFile();
            using (new FileStream(temporaryFile.FileName, FileMode.Open))
            {
                temporaryFile.Dispose();
            }
        }
        [Test]
        [Ignore("should be implemented")]
        public void Finalizer_DisposeNotCalledFileCaught_NoException()
        {
//            var mock = new Mock<IFileOperations>();
//
//            const string APP_TRANSFER_FILE_NAME = "TemporaryFileName";
//            AppDomain testDomain = CreateDomain();
//            testDomain.SetData("moq", mock.Object);
//
//            testDomain.DoCallBack(delegate
//                {
//                    var temporaryFile = new TemporaryFile();
//                    Assert.IsTrue(File.Exists(temporaryFile.FileName));
//                    AppDomain.CurrentDomain.SetData(APP_TRANSFER_FILE_NAME, temporaryFile.FileName);
//                }
//            );
//
            throw new NotImplementedException();
        }

        #region helper
        AppDomain CreateDomain()
        {
            var info = new AppDomainSetup { ApplicationBase = Directory.GetCurrentDirectory() };
            AppDomain testDomain = AppDomain.CreateDomain("TestTemporaryFileDomain", null, info);
            return testDomain;
        }
        #endregion
    }
}

