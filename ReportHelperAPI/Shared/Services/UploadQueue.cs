using Core.DTOs;
using Core.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Services
{
    public class UploadQueue:IUploadQueue
    {
        private readonly ConcurrentQueue<UploadReportDto> _queue = new();

        public void Enqueue(UploadReportDto job)
            => _queue.Enqueue(job);

        public bool TryDequeue(out UploadReportDto job)
            => _queue.TryDequeue(out job);
    }
}
