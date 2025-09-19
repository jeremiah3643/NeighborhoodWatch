# NeighborhoodWatch

A .NET 8 Worker Service for monitoring file system changes and sending automated email notifications with database integration.

## Overview

NeighborhoodWatch is a background service that monitors a specified directory for file system events (create, modify, delete, rename) and sends email notifications with relevant details. It includes support for ZIP file content inspection and database integration for job tracking.

## Features

- **File System Monitoring**: Real-time monitoring of directory changes using FileSystemWatcher
- **Email Notifications**: Automated email alerts with customizable templates
- **ZIP File Support**: Reads and reports contents of ZIP archives
- **Database Integration**: SQL Server connectivity for job tracking and data retrieval
- **Configurable Templates**: Email templates stored in configuration with placeholder replacement
- **Structured Logging**: Comprehensive logging for monitoring and debugging
- **Dependency Injection**: Modern .NET architecture with service registration

## Architecture

The application follows .NET Worker Service patterns with dependency injection:

### Core Services

- **WatchService**: Manages FileSystemWatcher and coordinates file event handling
- **EmailService**: Handles email composition and delivery with template support
- **DatabaseService**: Manages SQL Server connections and query execution
- **ZipService**: Processes ZIP archives and extracts file listings

### Models

- **EmailSettings**: SMTP configuration and credentials
- **WatchSettings**: File monitoring configuration
- **DatabaseSettings**: SQL Server connection settings
- **EmailTemplates**: Template definitions for different notification types

## Configuration

Configuration is managed through `appsettings.json` with environment-specific overrides:

```json
{
  "WatchSettings": {
    "WatchPath": "C:\\path\\to\\monitor",
    "IncludeSubdirectories": true,
    "Filter": "*.*"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 587,
    "FromEmail": "filewatcher@example.com",
    "ToEmail": "notifications@example.com",
    "Username": "your-username",
    "Password": "your-password",
    "EnableSsl": true
  },
  "DatabaseSettings": {
    "Server": "your-server",
    "Database": "your-database",
    "IntegratedSecurity": true
  }
}
```

### Email Templates

The service supports configurable email templates with placeholder replacement:

```json
{
  "EmailTemplates": {
    "FileCreated": {
      "Subject": "New File Alert: {FileName}",
      "Body": "A new file has been created:\n\nFile: {FileName}\nPath: {FilePath}\nSize: {FileSize} bytes\nTimestamp: {Timestamp}"
    },
    "ZipFileCreated": {
      "Subject": "New ZIP File Alert: {FileName}",
      "Body": "A new ZIP file has been created:\n\nFile: {FileName}\nPath: {FilePath}\nSize: {FileSize} bytes\nContents:\n{ZipContents}\nTimestamp: {Timestamp}"
    }
  }
}
```

## Installation

### Prerequisites

- .NET 8.0 Runtime
- SQL Server (if using database features)
- SMTP server access for email notifications

### Setup

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd NeighborhoodWatch
   ```

2. Configure the application:
   - Update `appsettings.json` with your specific settings
   - For production, use `appsettings.Production.json` or environment variables

3. Build the application:
   ```bash
   dotnet build
   ```

4. Run the service:
   ```bash
   dotnet run
   ```

### Windows Service Installation

To run as a Windows Service, you can use the .NET hosting extensions:

```bash
dotnet publish -c Release
sc create "NeighborhoodWatch" binPath="C:\path\to\published\NeighborhoodWatch.exe"
sc start "NeighborhoodWatch"
```

## Usage

Once running, the service will:

1. Monitor the configured directory for file changes
2. Send email notifications based on configured templates
3. Process ZIP files to include content listings
4. Log all activities for monitoring and debugging

### Supported File Events

- **Created**: New files added to the monitored directory
- **Changed**: Existing files modified
- **Deleted**: Files removed from the directory
- **Renamed**: Files renamed within the directory

## Development

### Project Structure

```
NeighborhoodWatch/
├── Models/           # Configuration and data models
├── Services/         # Core business logic services
├── Program.cs        # Application entry point and DI configuration
├── Worker.cs         # Main worker service implementation
└── appsettings.json  # Configuration files
```

### Key Dependencies

- `Microsoft.Extensions.Hosting` - Worker Service framework
- `Microsoft.Data.SqlClient` - SQL Server connectivity
- `System.IO.Compression` - ZIP file processing
- `Microsoft.Extensions.Configuration` - Configuration management
- `Microsoft.Extensions.Logging` - Structured logging

### Contributing

1. Create a feature branch from `develop`:
   ```bash
   git checkout develop
   git checkout -b feature/your-feature
   ```

2. Make your changes and commit:
   ```bash
   git add .
   git commit -m "Add your feature description"
   ```

3. Push and create a pull request:
   ```bash
   git push -u origin feature/your-feature
   ```

## TODO

### Core Features
- [ ] **Complete ZIP File Processing**: Finish implementing ZIP content reading and listing in email notifications
- [ ] **Background Task Processing**: Implement delayed database checks after file deletion events
- [ ] **Enhanced Database Queries**: Complete implementation of all database service methods
- [ ] **File Event Filtering**: Add configurable file type filters (e.g., ignore temp files, specific extensions)

### Enhancements
- [ ] **Multiple Directory Monitoring**: Support monitoring multiple directories simultaneously
- [ ] **Configuration UI**: Web-based configuration management interface

### Performance & Reliability
- [ ] **Retry Logic**: Implement retry mechanisms for failed email sends and database operations
- [ ] **Bulk Processing**: Batch multiple file events for more efficient email notifications
- [ ] **Health Checks**: Add health check endpoints for monitoring service status

### Security & Compliance
- [ ] **Audit Logging**: Comprehensive audit trail of all file events and actions

### Testing & Documentation
- [ ] **Unit Tests**: Comprehensive test coverage for all services
- [ ] **Integration Tests**: End-to-end testing with test databases and email servers
- [ ] **Performance Tests**: Load testing for high-volume file scenarios


## Logging

The application uses structured logging with different log levels:

- **Information**: Normal operations and file events
- **Warning**: Non-critical issues (email send failures, etc.)
- **Error**: Critical errors that require attention
- **Debug**: Detailed diagnostic information

Logs can be configured in `appsettings.json` to output to console, file, or other providers.


## Troubleshooting


### Debugging

Enable debug logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

