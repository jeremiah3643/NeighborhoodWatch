namespace NeighborhoodWatch.Services
{
    public interface IZipService
    {
        Task<string> GetZipContentsListAsync(string zipFilePath);
    }
}