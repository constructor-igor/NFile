using System;
using System.Collections.Generic;

namespace NFile.Framework
{
    public class TemporaryCollection: IDisposable
    {
        readonly List<IDisposable> temporaryList = new List<IDisposable>();
        public ITemporaryFile AddFile()
        {
            return AddFile(new TemporaryFile());
        }
        public ITemporaryFile AddFile(string extension)
        {
            return AddFile(new TemporaryFile(extension));
        }
        public ITemporaryFile AddFile(ITemporaryFile temporaryFile)
        {
            temporaryList.Add(temporaryFile);
            return temporaryFile;
        }
        public ITemporaryFolder AddFolder()
        {
            return AddFolder(new TemporaryFolder());
        }
        public ITemporaryFolder AddFolder(ITemporaryFolder temporaryFolder)
        {
            temporaryList.Add(temporaryFolder);
            return temporaryFolder;
        }
        #region IDisposable
        public void Dispose()
        {
            foreach (IDisposable temporaryItem in temporaryList)
            {
                temporaryItem.Dispose();
            }
            temporaryList.Clear();
        }
        #endregion
    }
}