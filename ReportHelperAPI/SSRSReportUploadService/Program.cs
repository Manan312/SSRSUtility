using SSRSReportUploadService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<UploadBackgroundService>();

var host = builder.Build();
host.Run();
