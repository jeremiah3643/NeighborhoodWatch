using NeighborhoodWatch.Services;

namespace NeighborhoodWatch;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEmailService _emailService;
    private readonly IDatabaseService _databaseService;

 //    public Worker(ILogger<Worker> logger, IEmailService emailService)
    public Worker(ILogger<Worker> logger, IEmailService emailService, IDatabaseService databaseService)
    {
        _logger = logger;
        _emailService = emailService;
        _databaseService = databaseService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started - testing email service...");

        //Testing database service
       try
       {
          var results = await _databaseService.GetJobFileResultAsync("EBCDIC_PREMIER.TNI.605_3b26d_20250912.zip");
            _logger.LogInformation("Database returned {Count} results", results.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve data from database");
        }

        // Test the email service once when the worker starts
        try
        {
            await _emailService.SendFileEventEmailAsync(
                "Created",
                @"C:\test\sample.txt",
                zipContents: null);

            _logger.LogInformation("Test email sent successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test email");
        }

        // Keep the service running
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(60000, stoppingToken); // Check every minute instead of every second
        }
    }
}
