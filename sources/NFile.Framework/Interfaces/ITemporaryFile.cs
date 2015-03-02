using System;

namespace NFile.Framework
{
    public interface ITemporaryFile: IDisposable
    {
        string FileName { get; }
    }
}