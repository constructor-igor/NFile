using System;
using System.Text;
using NFile.Framework;
using NUnit.Framework;

namespace NFile.Tests
{
    [TestFixture]
    public class FolderComparisonTests
    {
        [Test]
        public void FolderComparison_2NullFolders_True()
        {
            IFolder folder1 = null;
            IFolder folder2 = null;
            IFolderComparisonReport report = folder1.Compare(folder2);
            Assert.IsTrue(report.Equal);
        }
        [Test]
        public void FolderComparison_NullAndNotExistsFolders_True()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder folder1 = new DataFolder(temporaryFolder.Folder + "\\" + "test");
                IFolder folder2 = null;
                IFolderComparisonReport report = folder1.Compare(folder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_NotExistsAndNullFolders_True()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder folder1 = null;
                IFolder folder2 = new DataFolder(temporaryFolder.Folder + "\\" + "test");
                IFolderComparisonReport report = folder1.Compare(folder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_ExistsAndNullFolders_False()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder nullFolder = null;
                IFolderComparisonReport report = temporaryFolder.Compare(nullFolder);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_NotExistsAndNotExistsFolders_True()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder folder1 = new DataFolder(temporaryFolder.Folder + "\\" + "test1");
                IFolder folder2 = new DataFolder(temporaryFolder.Folder + "\\" + "test2");

                IFolderComparisonReport report = folder1.Compare(folder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_ExistsAndNotExistsFolders_False()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder folder2 = new DataFolder(temporaryFolder.Folder + "\\" + "test2");

                IFolderComparisonReport report = temporaryFolder.Compare(folder2);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_NotExistsAndExistsFolders_False()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                IFolder folder2 = new DataFolder(temporaryFolder.Folder + "\\" + "test2");

                IFolderComparisonReport report = folder2.Compare(temporaryFolder);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_EmptyAndEmptyFolders_True()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_Folder1With1FileAndFolder2With2Files_False()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFile("file1_1");
                temporaryFolder2.CreateFile("file2_1");
                temporaryFolder2.CreateFile("file2_2");
                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_FoldersWith1DifferentFile_False()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFile("file_1");
                temporaryFolder2.CreateFile("file_2");
                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_FoldersWith1SameFile_True()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFile("file_1");
                temporaryFolder2.CreateFile("file_1");
                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_FoldersWithSameSubFolders_True()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFolder("test1");
                temporaryFolder2.CreateFolder("test1");

                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsTrue(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_FoldersWithDifferentSubFolders_False()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFolder("test1");
                temporaryFolder2.CreateFolder("test2");

                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsFalse(report.Equal);
            }
        }
        [Test]
        public void FolderComparison_FoldersWithSameSubFoldersButDifferentFiles_False()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                IFolder folder1 = temporaryFolder1.CreateFolder("test");
                IFolder folder2 = temporaryFolder2.CreateFolder("test");

                folder1.CreateFile("file_1");
                folder2.CreateFile("file_2");
                IFolderComparisonReport report = temporaryFolder1.Compare(temporaryFolder2);
                Assert.IsFalse(report.Equal);
            }
        }

        [Test]
        public void FolderComparison_NullFilesListAndNullFolder_True()
        {
            IFolder folder = null;
            string[] filesList = null;
            IFolderComparisonReport folderComparisonReport = folder.Compare(filesList);
            Assert.IsTrue(folderComparisonReport.Equal);
        }
        [Test]
        public void FolderComparison_FilesListFoundInFolder_True()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                temporaryFolder.CreateFile("file1.txt");
                temporaryFolder.CreateFile("file2.txt");

                string[] filesList = new StringBuilder()
                    .AppendLine("file1.txt")
                    .AppendLine("file2.txt")
                    .ToString()
                    .Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

                IFolderComparisonReport folderComparisonReport = temporaryFolder.Compare(filesList);
                Assert.IsTrue(folderComparisonReport.Equal);
            }
        }
        [Test]
        public void FolderComparison_FilesListFoundInFolderAndSubFolder_True()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                temporaryFolder.CreateFile("file1.txt");
                temporaryFolder.CreateFile("file2.txt");
                IFolder subFolder = temporaryFolder.CreateFolder("subFolder");
                subFolder.CreateFile("subFolderFile1.dat");
                subFolder.CreateFile("subFolderFile2.dat");

                string[] filesList = new StringBuilder()
                    .AppendLine("file1.txt")
                    .AppendLine("file2.txt")
                    .AppendLine(@"subFolder\subFolderFile1.dat")
                    .AppendLine(@"subFolder\subFolderFile2.dat")
                    .ToString()
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                IFolderComparisonReport folderComparisonReport = temporaryFolder.Compare(filesList);
                Assert.IsTrue(folderComparisonReport.Equal);
            }
        }
        [Test]
        public void FolderComparison_FilesListNotFoundInFolderAndSubFolder_False()
        {
            using (var temporaryFolder = new TemporaryFolder())
            {
                temporaryFolder.CreateFile("file1.txt");
                temporaryFolder.CreateFile("file2.txt");
                IFolder subFolder = temporaryFolder.CreateFolder("subFolder");
                subFolder.CreateFile("subFolderFile1.dat");

                string[] filesList = new StringBuilder()
                    .AppendLine("file1.txt")
                    .AppendLine("file2.txt")
                    .AppendLine(@"subFolder\subFolderFile1.dat")
                    .AppendLine(@"subFolder\subFolderFile2.dat")
                    .ToString()
                    .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                IFolderComparisonReport folderComparisonReport = temporaryFolder.Compare(filesList);
                Assert.IsFalse(folderComparisonReport.Equal);
            }
        }

        [Test]
        public void FolderComparison_DifferentFilesInFolders_FalseAndListOfDifferentFiles()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFile("test1.txt");
                temporaryFolder1.CreateFile("test2.txt");
                temporaryFolder2.CreateFile("test1.txt");
                temporaryFolder2.CreateFile("test3.txt");

                IFolderComparisonReport folderComparisonReport = temporaryFolder1.Compare(temporaryFolder2, ReportOption.CollectDifferentFiles);
                CollectionAssert.AreEqual(new[] {"test2.txt"}, folderComparisonReport.Folder1Files);
                CollectionAssert.AreEqual(new[] {"test3.txt"}, folderComparisonReport.Folder2Files);
                Assert.IsFalse(folderComparisonReport.Equal);
            }
        }
        [Test]
        public void FolderComparison_DifferentFilesInSubFolders_FalseAndListOfDifferentFiles()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                temporaryFolder1.CreateFile("test1.txt");
                temporaryFolder1.CreateFile("test2.txt");
                temporaryFolder2.CreateFile("test1.txt");
                temporaryFolder2.CreateFile("test3.txt");

                IFolder subFolder1 = temporaryFolder1.CreateFolder("subFolder");
                IFolder subFolder2 = temporaryFolder2.CreateFolder("subFolder");
                subFolder1.CreateFile("test.txt");
                subFolder1.CreateFile("test1.txt");
                subFolder2.CreateFile("test.txt");
                subFolder2.CreateFile("test2.txt");

                IFolderComparisonReport folderComparisonReport = temporaryFolder1.Compare(temporaryFolder2, ReportOption.CollectDifferentFiles);
                CollectionAssert.AreEqual(new[] { "test2.txt", "subfolder\\test1.txt" }, folderComparisonReport.Folder1Files);
                CollectionAssert.AreEqual(new[] { "test3.txt", "subfolder\\test2.txt" }, folderComparisonReport.Folder2Files);
                Assert.IsFalse(folderComparisonReport.Equal);
            }
        }
        [Test]
        public void FolderComparison_DifferentSubFolders_FalseAndListOfDifferentFiles()
        {
            using (var temporaryFolder1 = new TemporaryFolder())
            using (var temporaryFolder2 = new TemporaryFolder())
            {
                IFolder subFolder1 = temporaryFolder1.CreateFolder("subFolder1");
                IFolder subFolder2 = temporaryFolder2.CreateFolder("subFolder2");
                subFolder1.CreateFile("test11.txt");
                subFolder1.CreateFile("test12.txt");
                subFolder2.CreateFile("test21.txt");
                subFolder2.CreateFile("test22.txt");

                IFolderComparisonReport folderComparisonReport = temporaryFolder1.Compare(temporaryFolder2, ReportOption.CollectDifferentFiles);
                CollectionAssert.AreEqual(new[] { "subfolder1" }, folderComparisonReport.Folder1Files);
                CollectionAssert.AreEqual(new[] { "subfolder2" }, folderComparisonReport.Folder2Files);
                Assert.IsFalse(folderComparisonReport.Equal);
            }
        }
    }
}