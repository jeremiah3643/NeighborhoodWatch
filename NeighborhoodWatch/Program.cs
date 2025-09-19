using NeighborhoodWatch;
using NeighborhoodWatch.Services;
using NeighborhoodWatch.Models;


var builder = Host.CreateApplicationBuilder(args);



// Build configurations for services from appsettings.json
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("emailSettings"));
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("dataBaseSettings"));
builder.Services.Configure<WatchSettings>(builder.Configuration.GetSection("watchSettings"));
builder.Services.Configure<EmailTemplates>(builder.Configuration.GetSection("emailTemplates"));

// Register the services
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddSingleton<IWatchService, WatchService>();
builder.Services.AddSingleton<IZipService, ZipService>();

// Register the main worker
builder.Services.AddHostedService<Worker>();



var host = builder.Build();
host.Run();


// Tasks:
// - Create a .NET Worker Service project.
// - Create info for appsettings for mail and stuff
// - Implement file watching functionality.
// - Log file changes to the console or a log file.
// - Ensure the service can run in the background and start on system boot.
// - Test the service to ensure it works as expected.

// - Provide documentation on how to deploy and manage the service.