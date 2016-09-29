using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dedupe.Core
{
    public interface IFolder
    {
        // The name of the current instance.
        String FolderName { get; }

        // The absolute path to the current instance.
        String FolderPath { get; }

        // Expose toggle to specify top-level vs recursive.
        IEnumerable<IFile> EnumerateFiles();

        // Expose toggle to specify top-level vs recursive 
        IEnumerable<IFolder> EnumerateFolders();
 
        // Parameter should be relative to the current instance's path. 
        Task<Boolean> FolderExistsAsync(String folderName);

        // Parameter should be relative to the current instance's path.
        Task<IFolder> CreateFolderAsync(String folderName);

        // Parameter should be relative to the current instance's path.
        Task<Boolean> FileExistsAsync(String fileName);

        // Parameter should be relative to the current instance's path.
        Task<IFile> CreateFileAsync(String fileName);
    }
}