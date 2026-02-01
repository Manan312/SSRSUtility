using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace ReportHelperAPI.Controllers
{
    [ApiController]
    [Route("api/SSRS")]
    [Authorize]
    public class SSRSController : ControllerBase
    {
        private readonly ISSRSService _ssrsService;
        private readonly IUploadJobQueue _uploadJobQueue;
        private readonly IUploadQueue _uploadQueue;
        public SSRSController(ISSRSService ssrsService, IUploadQueue uploadQueue, IUploadJobQueue uploadJobQueue)
        {
            _ssrsService = ssrsService;
            _uploadJobQueue = uploadJobQueue;
            _uploadQueue = uploadQueue;
        }
        [HttpPost("Folders")]
        public async Task<ActionResult> GetFolders([FromBody] DataRequestDto sSRSConnectionRequestDto)
        {
            var data = await _ssrsService.GetFoldersAsync(sSRSConnectionRequestDto.Token);
            return Ok(data);
        }
        [HttpPost("DataSources")]
        public async Task<ActionResult> GetDataSourcesAsync([FromBody] DataRequestDto sSRSConnectionRequestDto)
        {
            var data = await _ssrsService.GetAllSharedDataSourcesAsync(sSRSConnectionRequestDto.Token);
            return Ok(data);
        }
        [HttpPost("GetReportNames")]
        public async Task<ActionResult> GetAllReportNames([FromBody] DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            var data = await _ssrsService.GetAllReportNamesAsync(downloadAllReportRequestDto);
            return Ok(data);
        }
        [HttpPost("DownloadAll")]
        public async Task<ActionResult> DownloadAllReports([FromBody] DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            var data = await _ssrsService.DownloadAllReportAsync(downloadAllReportRequestDto);
            return Ok(data);
        }
        [HttpPost("DownloadAllParam")]
        public async Task<ActionResult> DownloadAllParams([FromBody] DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            var data = await _ssrsService.DownloadAllReportParamsAsync(downloadAllReportRequestDto);
            return Ok(data);
        }
        [HttpPost("DownloadReport")]
        public async Task<ActionResult> DownloadReport([FromBody] DownloadReportRequestDto downloadReportRequestDto)
        {
            var data = await _ssrsService.DownloadReportAsync(downloadReportRequestDto);
            return Ok(data);
        }
        [HttpPost("UploadReport")]
        public async Task<ActionResult> UploadReport([FromForm] UploadReportDto uploadReportDto)
        {
            if (uploadReportDto.ZipFile == null || uploadReportDto.ZipFile.Length == 0)
                return BadRequest("Zip file is missing");

            if (!uploadReportDto.ZipFile.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid zip file");
            var job = _uploadJobQueue.Create();
            uploadReportDto.JobId = job.JobId;
            _uploadQueue.Enqueue(uploadReportDto);
            return Accepted(new
            {
                Message = "Upload started",
                JobId = job.JobId,
                FileCount = "Processing in background"
            });
        }
        [HttpGet("UploadStatus")]
        public async Task<ActionResult> GetUploadJobStatus()
        {
            var jobs = _uploadJobQueue.GetAll();

            var result = jobs.Select(job => new UploadJobResponseDto
            {
                JobId = job.JobId,
                Status = job.Status.ToString(),   // Queued | Processing | Completed | Failed
            })
            .OrderByDescending(j => j.JobId) // latest first
            .ToList();

            return Ok(result);
        }
    }
}
