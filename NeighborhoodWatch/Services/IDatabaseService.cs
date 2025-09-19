using NeighborhoodWatch.Models;

namespace NeighborhoodWatch.Services
{
    public interface IDatabaseService
    {
        Task<List<JobFileResult>> GetJobFileResultAsync(string fileName);
        Task<List<JobOutboundResult>> GetJobOutboundResultAsync(string fileName);
    }
}