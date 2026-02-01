using Core.DTOs;
using System.Collections.Concurrent;
namespace Core.Interfaces
{
    public interface IUploadQueue
    {
        void Enqueue(UploadReportDto job);
        bool TryDequeue(out UploadReportDto job);
    }
}
