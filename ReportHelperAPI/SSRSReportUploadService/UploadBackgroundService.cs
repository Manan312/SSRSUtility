using Core.Interfaces;
using NLog;
using System.Runtime.CompilerServices;
using ILogger = NLog.ILogger;

namespace SSRSReportUploadService
{
    public class UploadBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _errorlogger = LogManager.GetLogger("ErrorLog");
        private readonly ILogger _infologger = LogManager.GetLogger("InfoLog");

        public UploadBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _infologger.Info("SSRS Upload Worker started");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var uploadQueue = scope.ServiceProvider.GetRequiredService<IUploadQueue>();
                    var ssrsService = scope.ServiceProvider.GetRequiredService<ISSRSService>();
                    if (uploadQueue.TryDequeue(out var job))
                    {
                        _infologger.Info(
                            "Processing upload job for folder {Folder}",
                            job.TargetFolder);

                        await ssrsService.UploadReportsAsync(job);

                        _infologger.Info(
                            "Upload job completed for folder {Folder}",
                            job.TargetFolder);
                    }
                }
                catch (Exception ex)
                {
                    _errorlogger.Error(ex, "Upload job failed");
                }

                await Task.Delay(500, stoppingToken);
            }

            _infologger.Info("SSRS Upload Worker stopped");
        }
    }
}

