using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUploadJobQueue
    {
        UploadJobStatusDto Create();
        UploadJobStatusDto? Get(string jobId);
        IReadOnlyCollection<UploadJobStatusDto?> GetAll();
        void Update(string jobId, Action<UploadJobStatusDto> update);
    }
}
