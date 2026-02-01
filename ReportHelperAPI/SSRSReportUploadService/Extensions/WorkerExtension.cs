namespace SSRSReportUploadService.Extensions
{
    public static class WorkerExtension
    {
        public static IServiceCollection AddWorker(this IServiceCollection services)
        {
            services.AddHostedService<UploadBackgroundService>();
            return services;
        }
    }
}
