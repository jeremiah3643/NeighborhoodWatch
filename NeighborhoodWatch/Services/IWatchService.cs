using NeighborhoodWatch.Models;
using NeighborhoodWatch.Services;

namespace NeighborhoodWatch.Services
{
    public interface IWatchService
    {
        void StartWatching();
        void StopWatching();
    }
}