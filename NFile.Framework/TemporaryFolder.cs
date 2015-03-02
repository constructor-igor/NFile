using System;
using System.Diagnostics;
using System.IO;

namespace NFile.Framework
{
    public class TemporaryFolder: ITemporaryFolder
    {
        bool destroyed;
        readonly string folder;
        public string Folder
        {
            get
            {
                if (destroyed)
                    throw new ObjectDisposedException(GetType().ToString()); 
                return folder;
            }
        }
        public TemporaryFolder()
        {
            folder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(folder);
        }
        #region IDisposable
        public void Dispose()
        {
            Dispose(disposing: true);
        }
        #endregion
        ~TemporaryFolder()
        {
            Dispose(disposing: false);
        }
        void Dispose(bool disposing)
        {            
            if (destroyed)
                return;

            destroyed = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
                Empty(new DirectoryInfo(folder));
            }
            else
            {
                try
                {
                    Empty(new DirectoryInfo(folder));
                }
                catch (Exception exp)
                {
                    Trace.Fail(String.Format("Empty({0}) failed", folder), String.Format("Error message {0} in {1}", exp.Message, exp.StackTrace));
                }
            }
        }
        void Empty(DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.GetFiles()) file.Delete();
            foreach (DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(recursive: true);
            directory.Delete(recursive: true);
        }
    }
}