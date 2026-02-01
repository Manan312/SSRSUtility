using Core.DTOs;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISSRSService
    {
        public Task<SsrsConnectionInfo> ParseConnectionDetails(string token);
        public Task<IReadOnlyList<SsrsFolderDto>> GetFoldersAsync(string token);
        public Task<IReadOnlyList<SSRSDataSourceDto>> GetAllSharedDataSourcesAsync(string token);
        public Task<ReportDownloadResultDto> DownloadAllReportAsync(DownloadAllReportRequestDto downloadAllReportRequestDto);
        public Task<List<ReportDetailsDto>> GetAllReportNamesAsync(DownloadAllReportRequestDto downloadAllReportRequestDto);
        public Task<ReportDownloadResultDto> DownloadReportAsync(DownloadReportRequestDto downloadReportRequestDto);
        public Task UploadReportsAsync(UploadReportDto job);
        public Task UploadSingleReportAsync(HttpClient client,string folder, string fileName, byte[] filebyte);
        Task<List<ReportParamDto>> DownloadAllReportParamsAsync(DownloadAllReportRequestDto downloadAllReportRequestDto);
    }
}
