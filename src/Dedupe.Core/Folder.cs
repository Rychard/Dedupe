using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Dedupe.Core
{
    public class Folder : IFolder
    {
        DirectoryInfo _directoryInfo;

        public String FolderName => _directoryInfo.Name;

        public String FolderPath => _directoryInfo.FullName;

        public Folder(String path)
        {
            _directoryInfo = new DirectoryInfo(path);
        }

        public Folder(DirectoryInfo directoryInfo)
        {
            _directoryInfo = directoryInfo;
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            var files = _directoryInfo.EnumerateFiles("*.*", SearchOption.AllDirectories);
            foreach(var fileInfo in files)
            {
                yield return new File(fileInfo);
            }
        }

        public IEnumerable<IFolder> EnumerateFolders()
        {
            var folders = _directoryInfo.EnumerateDirectories();
            foreach(var folderInfo in folders)
            {
                yield return new Folder(folderInfo);
            }
        }

        public Task<Boolean> FolderExistsAsync(String folderName)
        {
            String path = Path.Combine(_directoryInfo.FullName, folderName);
            Boolean exists = Directory.Exists(path);
            return Task.FromResult(exists);
        }

        public Task<IFolder> CreateFolderAsync(String folderName)
        {
            String path = Path.Combine(_directoryInfo.FullName, folderName);
            IFolder folder = new Folder(path);
            return Task.FromResult(folder);
        }

        public Task<Boolean> FileExistsAsync(String fileName)
        {
            String path = Path.Combine(_directoryInfo.FullName, fileName);
            Boolean exists = System.IO.File.Exists(path);
            return Task.FromResult(exists);
        }

        public Task<IFile> CreateFileAsync(String fileName)
        {
            String path = Path.Combine(_directoryInfo.FullName, fileName);
            using(var fileStream = System.IO.File.Create(path))
            {
                // Do nothing; immediately close.
            }
            IFile file = new File(path);
            return Task.FromResult(file);
        }
    }
}