using InterViewV2.Models.DAL;
using InterViewV2.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace InterViewV2.Services.Extension
{
    public class FileService : IFileService
    {
        private readonly string filesLocation;
        private readonly Repo r;

        public FileService(Repo repo, IConfiguration configuration)
        {
            r = repo;
            filesLocation = configuration["FilesLocation"];
        }

        public async Task<List<Models.File>> Upload(IEnumerable<IFormFile> files, Guid? filesId = null, string filePath = null)
        {
            var list = new List<Models.File>();
            foreach (var file in files)
            {
                list.Add(await Upload(file, filesId, filePath));
            }
            return list;
        }

        public async Task<Models.File> Upload(IFormFile file, Guid? filesId = null, string filePath = null)
        {
            var f = new Models.File
            {
                Name = file.FileName,
                Extension = Path.GetExtension(file.FileName),
                FilesId = filesId
            };
            f = await r.Add(f);

            Create(file, f, filePath);
            return f;
        }

        public async Task<Models.File> Update(Guid id, IFormFile file, string filePath = null)
        {
            var f = await r.Get<Models.File>(x => x.Id == id).SingleAsync();
            f.Name = file.FileName;
            f.Extension = Path.GetExtension(file.FileName);
            f = await r.Update(f);

            Create(file, f, filePath);
            return f;
        }

        public async Task Delete(Guid id, string filePath = null)
        {
            var f = await r.Get<Models.File>(x => x.Id == id).SingleOrDefaultAsync();
            if (f != null)
            {
                await r.Remove(f);
                System.IO.File.Delete(GetFullName(f, filePath));
            }
        }

        public async Task DeleteFiles(Guid filesId, string filePath = null)
        {
            var files = await r.Get<Models.Files>(x => x.Id == filesId).SingleOrDefaultAsync();

            if (files != null)
            {
                var fs = await GetFiles(filesId);
                foreach (var f in fs)
                {
                    await r.Remove(f);
                    System.IO.File.Delete(GetFullName(f, filePath));
                }

                await r.Remove(files);
            }
        }

        public async Task<byte[]> Get(Guid id, string filePath = null)
        {
            var f = await GetFile(id);
            return Get(f, filePath);
        }

        public byte[] Get(Models.File f, string filePath = null)
        {
            var name = GetName(f, filePath);
            if (name == null)
            {
                return null;
            }
            return System.IO.File.ReadAllBytes(name);
        }

        public async Task<Models.File> GetFile(Guid id)
        {
            return await r.Get<Models.File>(x => x.Id == id).SingleOrDefaultAsync();
        }

        public async Task<List<Models.File>> GetFiles(Guid filesId)
        {
            return await r.Get<Models.File>(x => x.FilesId == filesId).ToListAsync();
        }

        private string GetName(Models.File f, string filePath = null)
        {
            return f == null ? null : GetFullName(f, filePath);
        }

        private string GetFullName(Models.File f, string filePath = null)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                return filePath + $@"\{f.Id}{f.Extension}";
            }
            return filesLocation + $@"\{f.Id}{f.Extension}";
        }

        private void Create(IFormFile file, Models.File f, string filePath = null)
        {
            if (filePath != null)
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            }
            if (!Directory.Exists(filesLocation))
            {
                Directory.CreateDirectory(filesLocation);
            }

            using var fs = System.IO.File.Create(GetFullName(f, filePath));
            file.CopyTo(fs);
            fs.Flush();
        }

        public MemoryStream GetFilesZip(List<Models.File> files, string filePath = null)
        {
            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var fileName = $"{file.Name}";
                    var zipEntry = zipArchive.CreateEntry(fileName);
                    using (var entryStream = zipEntry.Open())
                    {
                        using (var dosyaStream = new MemoryStream(Get(file, filePath)))
                        {
                            dosyaStream.CopyTo(entryStream);
                        }
                    }
                }
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        public byte[] Get(List<Models.File> filesList)
        {
            throw new NotImplementedException();
        }
    }
}
