using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Dedupe.Core
{
    public class File : IFile
    {
        private FileInfo _fileInfo;
        
        public String FileName => _fileInfo.Name;

        public String Path => _fileInfo.FullName;

        public File(String path)
        {
            _fileInfo = new FileInfo(path);
        }

        public File(FileInfo fileInfo)
        {
            _fileInfo = fileInfo;
        }

        public Task<IFile> MoveAsync(IFolder targetFolder)
        {
            String newPath = System.IO.Path.Combine(targetFolder.FolderPath, _fileInfo.Name);
            System.IO.File.Move(_fileInfo.FullName, newPath);
            IFile movedFile = new File(newPath);
            return Task.FromResult(movedFile);
        }

        public Task<IFile> CopyAsync(IFolder targetFolder)
        {
            String newPath = System.IO.Path.Combine(targetFolder.FolderPath, _fileInfo.Name);
            System.IO.File.Copy(_fileInfo.FullName, newPath);
            IFile copiedFile = new File(newPath);
            return Task.FromResult(copiedFile);
        }
               
        public Task<Boolean> DeleteAsync()
        {
            System.IO.File.Delete(_fileInfo.FullName);
            return Task.FromResult(true);
        }

        public Task<Stream> OpenReadAsync()
        {
            Stream stream = _fileInfo.OpenRead();
            return Task.FromResult(stream);
        }

        public Task<Stream> OpenWriteAsync()
        {
            Stream stream = _fileInfo.OpenWrite();
            return Task.FromResult(stream);
        }
    }
}