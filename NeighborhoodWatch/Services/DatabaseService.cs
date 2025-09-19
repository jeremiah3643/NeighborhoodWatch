using Microsoft.Data.SqlClient;
using System.Data;
using NeighborhoodWatch.Models;
using Microsoft.Extensions.Options;

namespace NeighborhoodWatch.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly DatabaseSettings _dbSettings;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(IOptions<DatabaseSettings> dbSettings, ILogger<DatabaseService> logger)
        {
            _dbSettings = dbSettings.Value;
            _logger = logger;
        }

        public async Task<List<JobFileResult>> GetJobFileResultAsync(string fileName)
        {
            // Implement database logic to retrieve job file result based on fileName
            var results = new List<JobFileResult>();
            try
            {
                var connectionString = $"Server={_dbSettings.ServerName};Database={_dbSettings.DatabaseName};User Id={_dbSettings.UserId};Password={_dbSettings.Password};TrustServerCertificate=True;";
                var query = @"
                    select jf.originalfilename, jf.trackingkey, jt.description
                    from jobFile jf
                    right join jobtransaction jt on jf.jobid = jt.jobid
                    where jf.filename = @fileName";

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@fileName", fileName);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    results.Add(new JobFileResult
                    {
                        OriginalFileName = reader["originalfilename"] as string,
                        TrackingKey = reader["trackingkey"] as string,
                        Description = reader["description"] as string
                    });
                }
                _logger.LogInformation("Retrieved {Count} job file results for file: {FileName}", results.Count, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job file results for file: {FileName}", fileName);
            }
            return results;
        }

        public async Task<List<JobOutboundResult>> GetJobOutboundResultAsync(string trackingKey)
        {
            // Implement database logic to retrieve job outbound result based on trackingKey
            await Task.CompletedTask;
            return new List<JobOutboundResult>();
        }
    }
}