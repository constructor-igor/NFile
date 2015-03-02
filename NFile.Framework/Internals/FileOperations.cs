using System.IO;

namespace NFile.Framework.Internals
{
    public interface IFileOperations
    {
        string GetTempFileName();
        void Delete(string fileName);
    }

    internal class FileOperations: IFileOperations
    {
        #region IFileOperations
        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }
        public void Delete(string fileName)
        {
            File.Delete(fileName);
        }
        #endregion
    }
}