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
            Assert.That(temporaryFile, Is.InstanceOf<ITemporaryFile>());
        }
        [Test]
        public void Constructor_Empty_FileNameExtensionTmp()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.That(Path.GetExtension(temporaryFile.FileName), Is.EqualTo(".tmp"));
            }
        }
        [Test]
        public void Constructor_Empty_FileExists()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.That(File.Exists(temporaryFile.FileName), Is.True);
            }
        }
        [Test]
        public void Constructor_Empty_FilePlacedIntoTemporaryFolder()
        {
            using (var temporaryFile = new TemporaryFile())
            {
                Assert.That(Path.GetDirectoryName(temporaryFile.FileName) + @"\", Is.EqualTo(Path.GetTempPath()));
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
            Assert.That(File.Exists(fileName), Is.False);
        }
        [Test]
        public void Constructor_ExtensionTxt_FileNameExtensionTxt()
        {
            string fileName;
            using (var temporaryFile = new TemporaryFile(".txt"))
            {
                fileName = temporaryFile.FileName;

                Assert.That(Path.GetExtension(temporaryFile.FileName), Is.EqualTo(".txt"));
                Assert.That(Path.GetDirectoryName(temporaryFile.FileName) + @"\", Is.EqualTo(Path.GetTempPath()));
                
                Assert.That(File.Exists(temporaryFile.FileName), Is.False);
                File.WriteAllBytes(temporaryFile.FileName, new byte[]{0});
                Assert.That(File.Exists(temporaryFile.FileName), Is.True);
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
                    Assert.That(File.Exists(temporaryFile.FileName), Is.True);
                    AppDomain.CurrentDomain.SetData(APP_TRANSFER_FILE_NAME, temporaryFile.FileName);
                }
            );
            var createdTemporaryFileName = (string)testDomain.GetData(APP_TRANSFER_FILE_NAME);
            Assert.That(File.Exists(createdTemporaryFileName), Is.True);
            AppDomain.Unload(testDomain);

            Assert.That(File.Exists(createdTemporaryFileName), Is.False, "temporary file should be deleted by finalizer");
        }
        [Test, ExpectedException(typeof(ObjectDisposedException))]
        public void FileName_AfterDispose_GenerateObjectDisposedException()
        {
            var temporaryFile = new TemporaryFile();
            Assert.That(temporaryFile.FileName, Is.Not.Null);
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

