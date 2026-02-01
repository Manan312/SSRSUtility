using Core.DTOs;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using System.IO;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace Infrastructure.Services
{
    public class SSRSService : ISSRSService
    {
        private readonly ReportDetails _reportDetails;
        private readonly IAuthenticationService _authenticationService;
        private readonly ISSRSClientFactory _ssrsClientFactory;
        private readonly IUploadJobQueue _uploadJobQueue;
        public SSRSService(IOptions<ReportDetails> reportDetails, IAuthenticationService authenticationService, ISSRSClientFactory ssrsClientFactory, IUploadJobQueue uploadJobQueue)
        {
            _reportDetails = reportDetails.Value;
            _authenticationService = authenticationService;
            _ssrsClientFactory = ssrsClientFactory;
            _uploadJobQueue = uploadJobQueue;
        }
        public async Task<SsrsConnectionInfo> ParseConnectionDetails(string token)
        {
            var json = await _authenticationService.Decrypt(token);

            // 2️⃣ Deserialize
            var info = JsonSerializer.Deserialize<SsrsConnectionInfo>(json);

            if (info is null)
                throw new BusinessException("Invalid SSRS token");

            return info;
        }
        public async Task<IReadOnlyList<SsrsFolderDto>> GetFoldersAsync(string token)
        {
            var connection = await ParseConnectionDetails(token);

            using var client = _ssrsClientFactory.Create(connection);

            var response = await client.GetAsync(_reportDetails.Folders);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
            .GetProperty("value")
            .EnumerateArray()
            .Select(x => new SsrsFolderDto
            {
                Name = x.GetProperty("Name").GetString()!,
                Path = x.GetProperty("Path").GetString()!
            })
            .ToList();

        }
        public async Task<IReadOnlyList<SSRSDataSourceDto>> GetAllSharedDataSourcesAsync(string token)
        {
            var connection = await ParseConnectionDetails(token);

            using var client = _ssrsClientFactory.Create(connection);

            var response = await client.GetAsync(_reportDetails.DataSources);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
            .GetProperty("value")
            .EnumerateArray()
            .Select(x => new SSRSDataSourceDto
            {
                Name = x.GetProperty("Name").GetString()!,
                Path = x.GetProperty("Path").GetString()!,
                IsShared = true
            })
            .ToList();

        }
        public async Task<ReportDownloadResultDto> DownloadAllReportAsync(DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            try
            {
                var connection = await ParseConnectionDetails(downloadAllReportRequestDto.token);
                using var client = _ssrsClientFactory.CreateSoapClient(connection);

                // 1️⃣ Get catalog items

                var listXml = await SendSoapAsync(client,
                    _reportDetails.DownloadAllReport.Replace("_SsrsNs", Convert.ToString(_reportDetails.SSRSNs)).Replace("_folderPath", downloadAllReportRequestDto.folderPath)
                    , _reportDetails.DownloadAllReportSubLink.Replace("_SsrsNs", _reportDetails.SSRSNs));

                XDocument doc = XDocument.Parse(listXml);

                XNamespace ns = XNamespace.Get(_reportDetails.SSRSNs);

                var reports = doc
                    .Descendants(ns + "CatalogItem")
                    .Where(x => (string?)x.Element(ns + "TypeName") == "Report")
                    .Select(x => new
                    {
                        Name = (string?)x.Element(ns + "Name"),
                        Path = (string?)x.Element(ns + "Path")
                    })
                    .ToList();

                if (!reports.Any())
                    throw new BusinessException("No reports found in folder.");

                // 2️⃣ Zip reports
                using var zipStream = new MemoryStream();
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    foreach (var report in reports)
                    {
                        var rdlBytes = await DownloadReportBytesAsync(client, report.Path);

                        var entry = archive.CreateEntry(report.Name + ".rdl");
                        using var es = entry.Open();
                        await es.WriteAsync(rdlBytes);
                    }
                }
                return new ReportDownloadResultDto
                {
                    Content = zipStream.ToArray(),
                    FileName = $"{Path.GetFileName(downloadAllReportRequestDto.folderPath)}_{DateTime.Now:yyyyMMdd_HHmm}_Reports.zip",
                    ContentType = "application/zip"
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ReportDetailsDto>> GetAllReportNamesAsync(DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            try
            {
                List<ReportDetailsDto> reportDetailsDtos = new List<ReportDetailsDto>();
                var connection = await ParseConnectionDetails(downloadAllReportRequestDto.token);
                using var client = _ssrsClientFactory.CreateSoapClient(connection);

                // 1️⃣ Get catalog items

                var listXml = await SendSoapAsync(client,
                    _reportDetails.DownloadAllReport.Replace("_SsrsNs", Convert.ToString(_reportDetails.SSRSNs)).Replace("_folderPath", downloadAllReportRequestDto.folderPath)
                    , _reportDetails.DownloadAllReportSubLink.Replace("_SsrsNs", _reportDetails.SSRSNs));

                XDocument doc = XDocument.Parse(listXml);

                XNamespace ns = XNamespace.Get(_reportDetails.SSRSNs);

                var reports = doc
                    .Descendants(ns + "CatalogItem")
                    .Where(x => (string?)x.Element(ns + "TypeName") == "Report")
                    .Select(x => new
                    {
                        Name = (string?)x.Element(ns + "Name"),
                        Path = (string?)x.Element(ns + "Path"),
                        CreatedDate = DateTime.TryParse(
                            (string?)x.Element(ns + "CreationDate"),
                            out var created
                        ) ? created : (DateTime?)null,

                        ModifiedDate = DateTime.TryParse(
                            (string?)x.Element(ns + "ModifiedDate"),
                            out var modified
                        ) ? modified : (DateTime?)null
                    })
                    .ToList();

                if (!reports.Any())
                    throw new BusinessException("No reports found in folder.");
                reportDetailsDtos = reports
                .Select((x, index) => new ReportDetailsDto
                {
                    Id = index,            // or index.ToString()
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate
                })
    .ToList();
                return reportDetailsDtos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<List<ReportParamDto>> DownloadAllReportParamsAsync(DownloadAllReportRequestDto downloadAllReportRequestDto)
        {
            List<ReportParamDto> reportParamDtos = new List<ReportParamDto>();
            try
            {

                var connection = await ParseConnectionDetails(downloadAllReportRequestDto.token);
                using var client = _ssrsClientFactory.CreateSoapClient(connection);

                // 1️⃣ Get catalog items

                var listXml = await SendSoapAsync(client,
                    _reportDetails.DownloadAllReport.Replace("_SsrsNs", Convert.ToString(_reportDetails.SSRSNs)).Replace("_folderPath", downloadAllReportRequestDto.folderPath)
                    , _reportDetails.DownloadAllReportSubLink.Replace("_SsrsNs", _reportDetails.SSRSNs));

                XDocument doc = XDocument.Parse(listXml);

                XNamespace ns = XNamespace.Get(_reportDetails.SSRSNs);

                var reports = doc
                    .Descendants(ns + "CatalogItem")
                    .Where(x => (string?)x.Element(ns + "TypeName") == "Report")
                    .Select(x => new
                    {
                        Name = (string?)x.Element(ns + "Name"),
                        Path = (string?)x.Element(ns + "Path")
                    })
                    .ToList();

                if (!reports.Any())
                    throw new BusinessException("No reports found in folder.");

                foreach (var report in reports)
                {
                    var reportParams = await DownloadReportGetParam(client, report.Path);
                    reportParamDtos.Add(new ReportParamDto
                    {
                        ReportName = report.Name,
                        ReportParams = reportParams
                    });
                }
                return reportParamDtos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<byte[]> DownloadReportBytesAsync(HttpClient client, string path)
        {
            var xml = await SendSoapAsync(client,
                _reportDetails.DownloadReportBytes.Replace("_SsrsNs", _reportDetails.SSRSNs).Replace("_path", SecurityElement.Escape(path)),
                "_SsrsNs/GetItemDefinition".Replace("_SsrsNs", _reportDetails.SSRSNs));

            var base64 = XDocument.Parse(xml)
                .Descendants()
                .First(x => x.Name.LocalName == "Definition")
                .Value;

            return Convert.FromBase64String(base64);
        }

        private async Task<List<ReportParams>> DownloadReportGetParam(HttpClient client, string path)
        {
            var xml = await SendSoapAsync(
                client,
                _reportDetails.DownloadReportBytes
                    .Replace("_SsrsNs", _reportDetails.SSRSNs)
                    .Replace("_path", SecurityElement.Escape(path)),
                "_SsrsNs/GetItemDefinition".Replace("_SsrsNs", _reportDetails.SSRSNs)
            );

            // 1️⃣ Parse SOAP XML
            var soapDoc = XDocument.Parse(xml);

            // 2️⃣ Extract Base64 RDL
            var base64 = soapDoc
                .Descendants()
                .First(x => x.Name.LocalName == "Definition")
                .Value;

            var rdlBytes = Convert.FromBase64String(base64);

            // 3️⃣ Load RDL XML
            using var ms = new MemoryStream(rdlBytes);
            var rdlDoc = XDocument.Load(ms);

            XNamespace rdlNs = rdlDoc.Root!.Name.Namespace;

            // 4️⃣ Extract parameters
            var parameters = rdlDoc
                .Descendants(rdlNs + "ReportParameter")
                .Select(p => new ReportParams
                {
                    ParamName = (string?)p.Attribute("Name"),
                    ParamType = (string?)p.Element(rdlNs + "DataType")
                })
                .ToList();

            return parameters;
        }

        public async Task<ReportDownloadResultDto> DownloadReportAsync(DownloadReportRequestDto downloadReportRequestDto)
        {

            var conn = await ParseConnectionDetails(downloadReportRequestDto.token);
            using var client = _ssrsClientFactory.CreateSoapClient(conn);
            using var zipStream = new MemoryStream();
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var report in downloadReportRequestDto.reportNames)
                {
                    var reportPath = $"{downloadReportRequestDto.folderPath.TrimEnd('/')}/{report}";
                    var rdlBytes = await DownloadReportBytesAsync(client, reportPath);

                    var entry = archive.CreateEntry(report + ".rdl");
                    using var es = entry.Open();
                    await es.WriteAsync(rdlBytes);
                }
            }

            //var reportPath = $"{downloadReportRequestDto.folderPath.TrimEnd('/')}/{downloadReportRequestDto.reportName}";

            //var conn = await ParseConnectionDetails(downloadReportRequestDto.token);
            //using var client = _ssrsClientFactory.CreateSoapClient(conn);

            //var xml = await SendSoapAsync(client,
            //    _reportDetails.DownloadReport.Replace("_SsrsNs", _reportDetails.SSRSNs).Replace("_reportPath", SecurityElement.Escape(reportPath)),
            //    "_SsrsNs/GetItemDefinition".Replace("_SsrsNs", _reportDetails.SSRSNs));

            //var base64 = XDocument.Parse(xml)
            //    .Descendants()
            //    .First(x => x.Name.LocalName == "Definition")
            //    .Value;

            //var rdlBytes = await DownloadReportBytesAsync(client,reportPath);

            return new ReportDownloadResultDto
            {
                Content = zipStream.ToArray(),
                FileName = $"{Path.GetFileName(downloadReportRequestDto.folderPath)}_{DateTime.Now:yyyyMMdd_HHmm}_Reports.zip",
                ContentType = "application/zip"
            };
        }
        private static void ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new BusinessException("Path is required.");

            if (!path.StartsWith("/"))
                throw new BusinessException("Invalid SSRS path.");

            if (path.Contains(".."))
                throw new BusinessException("Invalid SSRS path.");
        }

        public async Task UploadReportsAsync(UploadReportDto job)
        {
            try
            {
                _uploadJobQueue.Update(job.JobId, j =>
                {
                    j.Status = "Processing";
                });

                var uploadReportResponseDto = new UploadReportResponseDto();
                var uploadDetailsReports = new List<UploadDetailsReport>();

                var connection = await ParseConnectionDetails(job.Token);
                using var client = _ssrsClientFactory.CreateSoapClient(connection);

                // 🔐 Create temp folder (once)
                var tempDir = Path.Combine(Path.GetTempPath(), "ssrs-upload-jobs");
                Directory.CreateDirectory(tempDir);

                // 🔐 Save ZIP to disk (detach from request lifecycle)
                var zipPath = Path.Combine(tempDir, $"{job.JobId}.zip");

                await using (var fs = new FileStream(zipPath, FileMode.Create, FileAccess.Write))
                {
                    await job.ZipFile.CopyToAsync(fs);
                }

                var validCount = 0;

                // 🔄 Open ZIP from disk (safe in background job)
                using (var zipStream = File.OpenRead(zipPath))
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!IsValidRdl(entry))
                            continue;

                        validCount++;

                        var reportName = Path.GetFileNameWithoutExtension(entry.Name);

                        try
                        {
                            // ✅ Stream entry (LOW MEMORY)
                            byte[] reportBytes;

                            using (var entryStream = entry.Open())
                            using (var ms = new MemoryStream())
                            {
                                await entryStream.CopyToAsync(ms);
                                reportBytes = ms.ToArray();
                            }
                            await UploadSingleReportAsync(
                                client,
                                job.TargetFolder,
                                reportName,
                                reportBytes
                            );

                            await BindSharedDataSourceAsync(
                                client,
                                job.TargetFolder,
                                reportName,
                                job.DataSourcePath,
                                job.DataSourceName
                            );

                            uploadReportResponseDto.SuccessCount++;
                        }
                        catch (Exception ex)
                        {
                            uploadDetailsReports.Add(new UploadDetailsReport
                            {
                                ReportName = entry.Name,
                                ReportMessage = ex.Message
                            });

                            uploadReportResponseDto.FailureCount++;
                        }
                    }
                }

                if (validCount == 0)
                {
                    uploadReportResponseDto.IsSuccess = false;
                    uploadReportResponseDto.Message = "No valid RDL files found in ZIP";
                }

                uploadReportResponseDto.ErrorDetails = uploadDetailsReports;

                _uploadJobQueue.Update(job.JobId, j =>
                {
                    j.Status = "Completed";
                    j.Result = uploadReportResponseDto;
                });

                // 🧹 Cleanup temp ZIP
                try
                {
                    File.Delete(zipPath);
                }
                catch
                {
                    // swallow cleanup failures (non-critical)
                }
            }
            catch (Exception ex)
            {
                _uploadJobQueue.Update(job.JobId, j =>
                {
                    j.Status = "Failed";
                });

                throw;
            }
        }
        private static bool IsValidRdl(ZipArchiveEntry entry)
        {
            if (!entry.Name.EndsWith(".rdl", StringComparison.OrdinalIgnoreCase))
                return false;

            // Prevent zip-slip
            if (entry.FullName.Contains("/") || entry.FullName.Contains("\\"))
                return false;

            return true;
        }
        public async Task UploadSingleReportAsync(HttpClient client, string folder, string fileName, byte[] filebyte)
        {
            var soap = _reportDetails.UploadReports
                .Replace("_Ssrsns", _reportDetails.SSRSNs)
                .Replace("_reportName", fileName)
                .Replace("_folderPath", folder)
                .Replace("_base64", Convert.ToBase64String(filebyte));
            await SendSoapAsync(
            client,
            soap,
            _reportDetails.SSRSNs + _reportDetails.UploadReportsSubLink);
        }
        private async Task BindSharedDataSourceAsync(HttpClient client, string folderPath, string reportName, string sharedDataSourcePath, string SharedDataSourceName)
        {
            var reportFullPath =
                folderPath.TrimEnd('/') + "/" + reportName;

            var soap = _reportDetails.UploadReportsSetDataSource
                .Replace("_SsrsNs", _reportDetails.SSRSNs)
                .Replace("_ReportFullPath", SecurityElement.Escape(reportFullPath))
                .Replace("_RdlDataSourceName", SecurityElement.Escape(SharedDataSourceName))
                .Replace("_SharedDataSourcePath", SecurityElement.Escape(sharedDataSourcePath));

            await SendSoapAsync(
                client,
                soap,
                _reportDetails.SSRSNs + _reportDetails.UploadReportsSetDataSourceSubLink);
        }

        private async Task<string> SendSoapAsync(HttpClient client, string soapXml, string soapAction)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _reportDetails.ReportSoapURL)
            {
                Content = new StringContent(soapXml, Encoding.UTF8, "text/xml")
            };

            request.Headers.Add("SOAPAction", soapAction);

            var response = await client.SendAsync(request);

            var responseXml = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                // Try to extract SOAP fault message
                throw new BusinessException(ExtractSoapFault(responseXml));
            }
            return responseXml;
        }
        private static string ExtractSoapFault(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);

                return doc.Descendants()
                          .FirstOrDefault(x => x.Name.LocalName == "faultstring")
                          ?.Value
                       ?? xml;
            }
            catch
            {
                return xml;
            }
        }
    }
}
