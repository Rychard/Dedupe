using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Dedupe.Core
{
    public class Deduplicator
    {
        private const String DEDUPLICATOR_FILENAME = ".deduplicator";
        private IFolder _source;
        private Dictionary<Guid, List<String>> _fileHashes;

        public Deduplicator(IFolder source)
        {
            _source = source;
            _fileHashes = new Dictionary<Guid, List<String>>();
        }

        private async Task ScanAsync()
        {
            var files = _source.EnumerateFiles();

            var result = Parallel.ForEach(files, async (file, state, noIdeaWhatThisIs) => 
            {
                await CacheHash(file);
            });

            // foreach(var file in files)
            // {
            //     await CacheHash(file);
            // }
        }

        private async Task CacheHash(IFile file)
        {
            Guid key = await GetFileHashAsync(file);
            String path = file.Path.Substring(_source.FolderPath.Length);
            lock(_fileHashes)
            {
                Console.WriteLine($"[{key:N}] {path}");
                if (_fileHashes.ContainsKey(key))
                {
                    _fileHashes[key].Add(path);
                }
                else
                {
                    var duplicateList = new List<String>() { path };
                    _fileHashes.Add(key, duplicateList);
                }
            }
        }

        public async Task CompressAsync(IFolder destination)
        {
            await ScanAsync();

            Boolean inPlace = (_source.FolderPath == destination.FolderPath); 

            foreach(var fileItem in _fileHashes)
            {
                var key = fileItem.Key;
                var files = fileItem.Value;

                if(inPlace)
                {
                    // Delete all duplicates
                    throw new NotImplementedException();
                }
                else
                {
                    // Copy ONLY the original to the destination.
                    String relativePath = files[0];
                    String sourcePath = Path.Combine(_source.FolderPath, relativePath);
                    String destinationPath = Path.Combine(destination.FolderPath, relativePath);
                    
                    if(!System.IO.File.Exists(sourcePath))
                    {
                        Console.WriteLine($"[ERROR] File not found: {relativePath}");
                    }

                    var directoryToEnsure = Path.GetDirectoryName(destinationPath);
                    Directory.CreateDirectory(directoryToEnsure);

                    // TODO: Abstract this to not take a hard dependency. 
                    System.IO.File.Copy(sourcePath, destinationPath, overwrite: true);
                }
            }

            var settings = new JsonSerializerSettings();
            settings.TypeNameHandling = TypeNameHandling.Objects;
            settings.Formatting = Formatting.Indented;

            var manifestFile = await destination.CreateFileAsync(DEDUPLICATOR_FILENAME);

            using(var stream = await manifestFile.OpenWriteAsync())
            using(var streamWriter = new System.IO.StreamWriter(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Objects;
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(streamWriter, _fileHashes);
            }
        }


        public async Task ExpandAsync()
        {
            var manifestFile = Path.Combine(_source.FolderPath, DEDUPLICATOR_FILENAME);

            // TODO: Refactor this to not take a hard dependency.
            var file = new Dedupe.Core.File(manifestFile);
            
            using(var stream = await file.OpenReadAsync())
            using(var streamReader = new StreamReader(stream))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.TypeNameHandling = TypeNameHandling.Objects;
                serializer.Formatting = Formatting.Indented;
                _fileHashes = (Dictionary<Guid, List<String>>)serializer.Deserialize(streamReader, typeof(Dictionary<Guid, List<String>>));
            }
                            
            foreach (var fileHash in _fileHashes)
            {
                var key = fileHash.Key;
                var files = fileHash.Value;
                
                var source = Path.Combine(_source.FolderPath, files[0]);
                for(var i = 1; i < files.Count; i++)
                {
                    String targetAbsolutePath = Path.Combine(_source.FolderPath, files[i]);
                    String targetDirectoryPath = Path.GetDirectoryName(targetAbsolutePath);
                    Directory.CreateDirectory(targetDirectoryPath);
                    System.IO.File.Copy(source, targetAbsolutePath, overwrite: true); 
                }
            }       
        }

        private async Task<Guid> GetFileHashAsync(IFile file)
        {
            Byte[] hash;
            using(var md5 = System.Security.Cryptography.MD5.Create())
            {
                hash = md5.ComputeHash(await file.OpenReadAsync());
            }
            
            var str = ByteArrayToString(hash);
            var guid = Guid.Parse(str);
            return guid;
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}