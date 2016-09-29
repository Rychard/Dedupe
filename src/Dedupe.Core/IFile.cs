using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dedupe.Core
{
    public interface IFile
    {
        // The filename of the current instance.
        String FileName { get; }

        // The absolute path to the current instance.
        String Path { get; }

        // Moves the current instance
        Task<IFile> MoveAsync(IFolder targetFolder);

        // Creates a copy of the current instance
        Task<IFile> CopyAsync(IFolder targetFolder);
        
        // Deletes the current instance.
        Task<Boolean> DeleteAsync();

        // Opens a stream that allows reading from the current instance.
        Task<Stream> OpenReadAsync();        

        // Opens a stream that allows writing to the current instance.
        Task<Stream> OpenWriteAsync();
    }
}