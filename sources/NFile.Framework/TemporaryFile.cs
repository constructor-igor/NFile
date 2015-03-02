using System;
using System.Diagnostics;
using System.IO;
using NFile.Framework.Internals;

namespace NFile.Framework
{
    public class TemporaryFile: ITemporaryFile
    {
        bool destroyed;
        readonly IFileOperations fileOperations;
        readonly string fileName;
        public string FileName
        {
            get
            {
                if (destroyed)
                    throw new ObjectDisposedException(GetType().ToString());
                return fileName;
            }
        }        
        public TemporaryFile(): this(Path.GetTempFileName(), new FileOperations())
        {
        }
        public TemporaryFile(string fileExtension): this(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + fileExtension), new FileOperations())
        {
        }
        protected TemporaryFile(string fileName, IFileOperations fileOperations)
        {
            this.fileName = fileName;
            this.fileOperations = fileOperations;
        }
        #region IDisposable
        public void Dispose()
        {
            Dispose(disposing: true);
        }
        #endregion
        ~TemporaryFile()
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
                fileOperations.Delete(fileName);
            }
            else
            {
                try
                {
                    fileOperations.Delete(fileName);
                }
                catch (Exception exp)
                {
                    Trace.Fail(String.Format("File.Delete({0}) failed", fileName), String.Format("Error message {0} in {1}", exp.Message, exp.StackTrace));
                }
            }
        }

        public static ITemporaryFile Create(IFileOperations fileOperations)
        {
            return new TemporaryFile(fileOperations.GetTempFileName(), fileOperations);    
        }
    }
}
