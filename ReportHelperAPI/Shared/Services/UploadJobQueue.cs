using Core.DTOs;
using Core.Interfaces;
using System.Collections.Concurrent;

namespace Infrastructure.Services
{
    public class UploadJobQueue : IUploadJobQueue
    {
        private readonly ConcurrentDictionary<string, UploadJobStatusDto> _store = new();

        public UploadJobStatusDto Create()
        {
            var job = new UploadJobStatusDto();
            _store[job.JobId] = job;
            return job;
        }

        public UploadJobStatusDto? Get(string jobId)
            => _store.TryGetValue(jobId, out var job) ? job : null;

        public IReadOnlyCollection<UploadJobStatusDto>? GetAll()
            => _store.Values.ToList();

        public void Update(string jobId, Action<UploadJobStatusDto> update)
        {
            if (_store.TryGetValue(jobId, out var job))
                update(job);
        }
    }
}
