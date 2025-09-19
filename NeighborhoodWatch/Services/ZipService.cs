using System.IO.Compression;
using System.Text;

namespace NeighborhoodWatch.Services
{
    public class ZipService : IZipService
    {
        private readonly ILogger<ZipService> _logger;

        public ZipService(ILogger<ZipService> logger)
        {
            _logger = logger;
        }

        public async Task<string> GetZipContentsListAsync(string zipFilePath)
        {
            try
            {
                await Task.CompletedTask; // Make it async for consistency
                
                using var archive = ZipFile.OpenRead(zipFilePath);
                var contents = new StringBuilder();
                contents.AppendLine("ZIP file contents:");
                
                foreach (var entry in archive.Entries)
                {
                    if (!string.IsNullOrEmpty(entry.Name)) // Skip directories
                    {
                        contents.AppendLine($"- {entry.FullName} ({entry.Length} bytes)");
                    }
                }
                
                _logger.LogInformation("Successfully read {EntryCount} entries from ZIP file: {ZipPath}", 
                    archive.Entries.Count, zipFilePath);
                
                return contents.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading ZIP file contents: {ZipPath}", zipFilePath);
                return "Could not read ZIP file contents.";
            }
        }
    }
}