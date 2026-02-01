using Infrastructure.Helper;
using SSRSReportUploadService.Extensions;
using NLog;
using NLog.Web;

var logger = LogManager.GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddWorker();

    var app = builder.Build();

    app.WebApplicationConfiguration();
}
catch (Exception ex)
{
    logger.Error(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    LogManager.Shutdown();
}