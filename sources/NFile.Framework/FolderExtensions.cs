using System.IO;

namespace NFile.Framework
{
    public static class FolderExtensions
    {
        public static bool Exists(this IFolder folder)
        {
            return Directory.Exists(folder.Folder);
        }
        public static void CreateFile(this IFolder folder, string fileName, string fileContent = "")
        {
            string creatingFileName = Path.Combine(folder.Folder, fileName);
            string directory = Path.GetDirectoryName(creatingFileName);
            Directory.CreateDirectory(directory);
            File.WriteAllText(creatingFileName, fileContent);
        }

        public static IFolder CreateFolder(this IFolder folder, string folderName)
        {
            DirectoryInfo createdDirectory = Directory.CreateDirectory(Path.Combine(folder.Folder, folderName));
            return new DataFolder(createdDirectory.FullName);
        }
    }
}