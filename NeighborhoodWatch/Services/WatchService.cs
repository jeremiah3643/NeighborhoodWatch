using NeighborhoodWatch.Models;
using NeighborhoodWatch.Services;
using System.IO;

namespace NeighborhoodWatch.Services
{
    public class WatchService : IWatchService, IDisposable
    {
        private readonly ILogger<WatchService> _logger;
        private readonly IEmailService _emailService;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly string _directoryToWatch;
        private readonly string _filter;
        private readonly IDatabaseService _databaseService;
        private readonly IZipService _zipService;


        public WatchService(ILogger<WatchService> logger, IEmailService emailService, IDatabaseService databaseService, IZipService zipService, IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _databaseService = databaseService;
            _zipService = zipService;

            var watchSettings = configuration.GetSection("watchSettings").Get<WatchSettings>();
            if (watchSettings == null || string.IsNullOrEmpty(watchSettings.DirectoryToWatch))
            {
                throw new ArgumentException("DirectoryToWatch must be specified in configuration.");
            }

            _directoryToWatch = watchSettings.DirectoryToWatch;
            _filter = string.IsNullOrEmpty(watchSettings.Filter) ? "*.*" : watchSettings.Filter;

            _fileSystemWatcher = new FileSystemWatcher
            {
                Path = _directoryToWatch,
                Filter = _filter,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite
            };

            _fileSystemWatcher.Created += OnCreated;
            _fileSystemWatcher.Deleted += OnMoved;
            //_fileSystemWatcher.Error += OnError;
        }

        public void StartWatching()
        {
            _fileSystemWatcher.EnableRaisingEvents = true;
            _logger.LogInformation("Started watching directory: {Directory}", _directoryToWatch);
        }

        public void StopWatching()
        {
            _fileSystemWatcher.EnableRaisingEvents = false;
            _logger.LogInformation("Stopped watching directory: {Directory}", _directoryToWatch);
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            // check to see if file is zip file and get contents
            if(Path.GetExtension(e.FullPath).Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                // If it's a zip file, get its contents
                var zipContents = await _zipService.GetZipContentsListAsync(e.FullPath);
                await _emailService.SendFileEventEmailAsync("ZipCreated", e.FullPath, zipContents: zipContents);
            }
            else
            {
                await _emailService.SendFileEventEmailAsync("Created", e.FullPath);
            }
            _logger.LogInformation("File created: {FilePath}", e.FullPath);
        }

        private async void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File changed: {FilePath}", e.FullPath);
            await _emailService.SendFileEventEmailAsync("Changed", e.FullPath);
        }

        private async void OnMoved(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File moved: {FilePath}", e.FullPath);
            await _emailService.SendFileEventEmailAsync("Moved", e.FullPath);
            _ = Task.Run(async () => await ProcessMovedFileAfterDelay(e.FullPath));
        }
        private async Task ProcessMovedFileAfterDelay(string filePath)
        {
            await Task.Delay(TimeSpan.FromMinutes(20));

            //Query database for results
            var fileName = Path.GetFileName(filePath);
            var results = await _databaseService.GetJobFileResultAsync(fileName);
            //Send results via email
            await _emailService.SendJobResultsEmailAsync(filePath, results);
            if (results.Any() && !string.IsNullOrEmpty(results.First().TrackingKey))
            {
                var trackingKey = results.First().TrackingKey;
                var outboundResults = await _databaseService.GetJobOutboundResultAsync(trackingKey);
            }
            else
            {
                _logger.LogWarning("No tracking key found for file: {FileName}", fileName);
            }
        }
        public void Dispose()
        {
            _fileSystemWatcher?.Dispose(); // Properly cleanup
        }
    }
}