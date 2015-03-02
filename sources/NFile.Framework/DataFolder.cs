namespace NFile.Framework
{
    public class DataFolder: IFolder
    {
        public string Folder { get; private set; }
        public DataFolder(string folder)
        {
            Folder = folder;
        }
    }
}