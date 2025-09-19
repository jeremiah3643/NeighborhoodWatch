using System.Threading.Tasks;

namespace NeighborhoodWatch.Services
{
    public interface IEmailService
    {
        Task SendFileEventEmailAsync(string eventType, string filePath, string? oldPath = null, string? zipContents = null);
        Task SendJobResultsEmailAsync(string filePath, object results, string? trackingKey = null);
    }
}