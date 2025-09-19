using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NeighborhoodWatch.Models;

namespace NeighborhoodWatch.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly IOptions<EmailTemplates> _emailTemplates;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger, IOptions<EmailTemplates> emailTemplates)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _emailTemplates = emailTemplates;
        }

        private EmailTemplate GetTemplateForEventType(string eventType)
        {
            return eventType switch
            {
                "Created" => _emailTemplates.Value.FileCreated,
                "ZipCreated" => _emailTemplates.Value.FileCreatedZip,
                "Moved" => _emailTemplates.Value.FileProcessed,   // or maybe create a FileMoved template?
                _ => _emailTemplates.Value.Testing // fallback
            };
        }
        public async Task SendFileEventEmailAsync(string eventType, string filePath, string? oldPath = null, string? zipContents = null)
        {
            var template = GetTemplateForEventType(eventType);
            try
            {
                var subject = template.Subject.Replace("{FileName}", Path.GetFileName(filePath));
                var body = template.Body.Replace("{FileName}", Path.GetFileName(filePath))
                                        .Replace("{JobDetails}", BuildFileEventBody(eventType, filePath, oldPath, zipContents))
                                        .Replace("{ZipContents}", zipContents ?? string.Empty);
                                        

                await SendEmailAsync(subject, body);
                _logger.LogInformation("Email sent for {EventType} event: {FilePath}", eventType, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email for {EventType} event: {FilePath}", eventType, filePath);
            }
        }

        public async Task SendJobResultsEmailAsync(string filePath, object results, string? trackingKey = null)
        {
            try
            {
                var subject = trackingKey != null ? $"Job Results for {filePath}" : $"File Processed: {filePath}";
                var body = results?.ToString() ?? "No results found.";
                
                await SendEmailAsync(subject, body);
                _logger.LogInformation("Job results email sent for: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending job results email for: {FilePath}", filePath);
            }
        }

        private async Task SendEmailAsync(string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Credentials.Username, _emailSettings.Credentials.Password)
            };

            using var message = new MailMessage(_emailSettings.From, _emailSettings.To, subject, body);
            
            await client.SendMailAsync(message);
        }

        private string GetSubjectForEventType(string eventType) => eventType switch
        {
            "Created" => "File Created",
            "Changed" => "File Changed", 
            "Deleted" => "File Deleted",
            "Renamed" => "File Renamed",
            _ => "File Event"
        };

        private string BuildFileEventBody(string eventType, string filePath, string? oldPath, string? zipContents)
        {
            var fileSize = GetFileSize(filePath, eventType);
            
            var body = eventType switch
            {
                "Created" => $"A file was created: {filePath}\nSize: {fileSize} bytes",
                "Changed" => $"A file was changed: {filePath}\nSize: {fileSize} bytes",
                "Deleted" => $"A file was deleted: {filePath}\nSize: N/A",
                "Renamed" => $"A file was renamed from {oldPath} to {filePath}\nSize: {fileSize} bytes",
                _ => $"A file event occurred: {filePath}"
            };

            if (!string.IsNullOrEmpty(zipContents))
            {
                body += $"\nContents of ZIP:\n{zipContents}";
            }

            return body;
        }

        private string GetFileSize(string filePath, string eventType)
        {
            if (eventType == "Deleted" || !File.Exists(filePath))
                return "N/A";

            try
            {
                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length.ToString();
            }
            catch
            {
                return "N/A";
            }
        }
    }
}