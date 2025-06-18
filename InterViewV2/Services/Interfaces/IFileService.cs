namespace InterViewV2.Services.Interfaces
{
    public interface IFileService
    {
        Task<List<Models.File>> Upload(IEnumerable<IFormFile> files, Guid? filesId = null, string filePath = null);
        Task<Models.File> Upload(IFormFile file, Guid? filesId = null, string filePath = null);
        Task<Models.File> Update(Guid id, IFormFile file, string filePath = null);

        Task Delete(Guid id, string filePath = null);
        Task DeleteFiles(Guid filesId, string filePath = null);

        Task<byte[]> Get(Guid id, string filePath = null);
        byte[] Get(Models.File file, string filePath = null);

        Task<Models.File> GetFile(Guid id);
        Task<List<Models.File>> GetFiles(Guid filesId);
        MemoryStream GetFilesZip(List<Models.File> files, string filePath = null);
        byte[] Get(List<Models.File> filesList);
    }
}
