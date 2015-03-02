using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NFile.Framework
{
    public enum ReportOption { EqualOnly, CollectDifferentFiles }

    public interface IFolderComparisonReport
    {
        bool Equal { get; }
        List<string> Folder1Files { get; }
        List<string> Folder2Files { get; }
    }
    public class FolderComparisonReport: IFolderComparisonReport
    {
        public bool Equal { get; private set; }
        public List<string> Folder1Files { get; private set; }
        public List<string> Folder2Files { get; private set; }
        public FolderComparisonReport(bool equal)
        {
            Equal = equal;
            Folder1Files = new List<string>();
            Folder2Files = new List<string>();
        }
        public FolderComparisonReport(List<string> folder1Files, List<string> folder2Files)
        {
            Folder1Files = folder1Files;
            Folder2Files = folder2Files;
            Equal = !Folder1Files.Any() && !Folder2Files.Any();
        }
    }

    public static class FolderComparison
    {
        public static IFolderComparisonReport Compare(this IFolder folder1, IFolder folder2, ReportOption reportOption = ReportOption.EqualOnly)
        {
            #region Null & Not Exists folders cases
            if (folder1 == null)
            {
                if (folder2 == null || !folder2.Exists())
                    return new FolderComparisonReport(equal: true);
                return new FolderComparisonReport(equal: false);
            }
            if (folder2 == null)
            {
                if (!folder1.Exists())
                    return new FolderComparisonReport(equal: true);
                return new FolderComparisonReport(equal: false);
            }

            if (!folder1.Exists() && !folder2.Exists())
                return new FolderComparisonReport(equal: true);

            if (!folder1.Exists() && folder2.Exists())
                return new FolderComparisonReport(equal: false);
            if (folder1.Exists() && !folder2.Exists())
                return new FolderComparisonReport(equal: false);
            #endregion

            #region Files comparison
            string[] entries1 = Directory.GetFileSystemEntries(folder1.Folder);
            string[] entries2 = Directory.GetFileSystemEntries(folder2.Folder);

            if (entries1.Length == 0 && entries2.Length == 0)
                return new FolderComparisonReport(equal: true);

            switch (reportOption)
            {
                case ReportOption.EqualOnly:
                    if (entries1.Length != entries2.Length)
                        return new FolderComparisonReport(equal: false);
                    break;
            }

            string[] files1 = Array.ConvertAll(Directory.GetFiles(folder1.Folder), fileName => Path.GetFileName(fileName).ToLower());
            string[] files2 = Array.ConvertAll(Directory.GetFiles(folder2.Folder), fileName => Path.GetFileName(fileName).ToLower());

            var uniqInFolder1 = new List<string>();
            var uniqInFolder2 = new List<string>();

            switch (reportOption)
            {
                case ReportOption.EqualOnly:
                    bool foundAllFilesFromList1InList2 = Found(files1, files2);
                    if (!foundAllFilesFromList1InList2)
                        return new FolderComparisonReport(equal: false);

                    bool foundAllFilesFromList2InList1 = Found(files2, files1);
                    if (!foundAllFilesFromList2InList1)
                        return new FolderComparisonReport(equal: false);
                    break;
                case ReportOption.CollectDifferentFiles:
                    FoundUniq(files1, files2, uniqInFolder1, uniqInFolder2);
                    break;
                default:
                    throw new NotImplementedException(String.Format("reportOption {0} not implemented", reportOption));
            }

            #endregion

            #region Directory comparison
            string[] directories1 = Array.ConvertAll(Directory.GetDirectories(folder1.Folder), folderName => Path.GetFileName(folderName).ToLower());
            string[] directories2 = Array.ConvertAll(Directory.GetDirectories(folder2.Folder), folderName => Path.GetFileName(folderName).ToLower());

            switch (reportOption)
            {
                case ReportOption.EqualOnly:
                    bool foundAllDirectoriesFromList1InList2 = Found(directories1, directories2);
                    if (!foundAllDirectoriesFromList1InList2)
                        return new FolderComparisonReport(equal: false);
                    bool foundAllDirectoriesFromList2InList1 = Found(directories2, directories1);
                    if (!foundAllDirectoriesFromList2InList1)
                        return new FolderComparisonReport(equal: false);
                    break;
                case ReportOption.CollectDifferentFiles:
                    FoundUniq(directories1, directories2, uniqInFolder1, uniqInFolder2);
                    break;
                default:
                    throw new NotImplementedException(String.Format("reportOption {0} not implemented", reportOption));
            }

            foreach (string subFolder in directories1)
            {
                IFolder subFolder1 = new DataFolder(Path.Combine(folder1.Folder, subFolder));
                IFolder subFolder2 = new DataFolder(Path.Combine(folder2.Folder, subFolder));
                IFolderComparisonReport comparisonReport = subFolder1.Compare(subFolder2, reportOption);
                switch (reportOption)
                {
                    case ReportOption.EqualOnly:
                        if (!comparisonReport.Equal)
                            return new FolderComparisonReport(equal: false);
                        break;
                    case ReportOption.CollectDifferentFiles:
                        uniqInFolder1.AddRange(comparisonReport.Folder1Files.ConvertAll(fileName => subFolder + @"\" + fileName));
                        uniqInFolder2.AddRange(comparisonReport.Folder2Files.ConvertAll(fileName => subFolder + @"\" + fileName));
                        break;
                    default:
                        throw new NotImplementedException(String.Format("reportOption {0} not implemented", reportOption));
                }
            }
            #endregion

            switch (reportOption)
            {
                case ReportOption.EqualOnly:
                    return new FolderComparisonReport(equal: true);
                case ReportOption.CollectDifferentFiles:
                    return new FolderComparisonReport(uniqInFolder1, uniqInFolder2);
                default:
                    throw new NotImplementedException(String.Format("reportOption {0} not implemented", reportOption));
            }            
        }

        static bool Found(IEnumerable<string> files1, string[] files2)
        {
            foreach (string fileName1 in files1)
            {
                bool found = Array.Exists(files2, fileName2 => fileName2 == fileName1);
                if (!found)
                    return false;
            }
            return true;
        }
        static void FoundUniq(IEnumerable<string> files1, IEnumerable<string> files2, List<string> uniqInFolder1, List<string> uniqInFolder2)
        {
            var files1List = new List<string>(files1);
            var files2List = new List<string>(files2);
            uniqInFolder1.AddRange(files1List.Where(file1 => !files2List.Contains(file1)));
            uniqInFolder2.AddRange(files2List.Where(file2 => !files1List.Contains(file2)));
        }

        public static IFolderComparisonReport Compare(this IFolder folder1, string[] folderContent2, ReportOption reportOption = ReportOption.EqualOnly)
        {
            if (folderContent2 == null)
            {
                IFolder folder2 = null;
                return folder1.Compare(folder2);
            }

            using (var temporaryFolder = new TemporaryFolder())
            {
                foreach (string folderEntity in folderContent2)
                {
                    temporaryFolder.CreateFile(folderEntity);
                }
                return folder1.Compare(temporaryFolder, reportOption);
            }
        }
    }
}