import type {
  SsrsConnection,
  SsrsConnectionResponse,
  SsrsDataSource,
  SsrsDetails,
  ApiResponse,
  SsrsFolder,
  EncryptDecrpytModel,
  ReportItem,
  DownloadReports,
  DownloadReportResponse,
  UploadReports,
  UploadReportsResponse,
  UploadJobStatus,
} from "../helper/apihelper";
import api from "../middleware/apiClient";
import type { AxiosResponse } from "axios";
import {
  DataSourceUrl,
  UploadUrl,
  DownloadAllUrl,
  DownloadReportUrl,
  EncryptUrl,
  FolderUrl,
  ReportNameUrl,
  UploadStatusUrl,
} from "../helper/apihelper";
import { toastHelper } from "../../helpers/toastHelper";
import { createZip } from "../helper/createZip";

export const encryptJson = async (payload: string): Promise<string> => {
  try {
    var token: EncryptDecrpytModel = {
      Token: payload,
    };
    const encryptedstring: AxiosResponse<string> = await api.post(
      EncryptUrl,
      token,
      {
        headers: {
          "Content-Type": "application/json",
        },
      },
    );
    return encryptedstring.data;
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "🔐");
    return "";
  }
};

export const serverConnect = async (
  serverModel: SsrsConnection,
): Promise<SsrsConnectionResponse | null> => {
  try {
    var token = await encryptJson(JSON.stringify(serverModel));
    localStorage.setItem("ssrsdetails", token);
    var ssrsdetails: SsrsDetails = {
      Token: token,
    };
    var AccessToken = localStorage.getItem("accesstoken");
    const folders: AxiosResponse<ApiResponse<SsrsFolder[]>> = await api.post(
      FolderUrl,
      ssrsdetails,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${AccessToken}`, // if needed
        },
      },
    );

    if (!folders.data.Success || !folders.data.Data) {
      throw new Error(folders.data.Message ?? "Something Went Wrong");
    }
    // console.log(folders);

    var Folder = folders.data.Data;
    const dataSources: AxiosResponse<ApiResponse<SsrsDataSource[]>> =
      await api.post(DataSourceUrl, ssrsdetails, {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${AccessToken}`, // if needed
        },
      });
    if (!dataSources.data.Success || !dataSources.data.Data) {
      throw new Error(dataSources.data.Message ?? "Something Went Wrong");
    }
    // console.log(dataSources);

    var ssrsConnectionResponse: SsrsConnectionResponse = {
      Folders: Folder,
      DataSources: dataSources.data.Data,
    };
    return ssrsConnectionResponse;
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "🔐");
    return null;
  }
};

export const fetchReportName = async (
  folderPath: string,
  onProgress?: (percent: number) => void,
): Promise<ReportItem[] | null> => {
  try {
    var token = localStorage.getItem("ssrsdetails");
    var AccessToken = localStorage.getItem("accesstoken");
    if (token) {
      var downloadReports: DownloadReports = {
        token: token,
        folderPath: folderPath,
      };

      const reportNames: AxiosResponse<ApiResponse<ReportItem[]>> =
        await api.post(ReportNameUrl, downloadReports, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${AccessToken}`, // if needed
          },
          onDownloadProgress: (event) => {
            if (event.total && onProgress) {
              const percent = Math.round((event.loaded * 100) / event.total);
              onProgress(percent);
            }
          },
        });
      if (reportNames.data.Data) {
        const newList: ReportItem[] = reportNames.data.Data.map((item) => ({
          Id: item.Id.toString(),
          Name: item.Name,
          CreatedDate: item.CreatedDate,
          ModifiedDate: item.ModifiedDate,
        }));
        return newList;
      } else {
        toastHelper("Something went wrong!!", "error", 4000, "🔐");
        return null;
      }
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "❌");
      return null;
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "❌");
    return null;
  }
};

export const downloadSpecificReports = async (
  folderPath: string,
  payload: string[],
  onProgress?: (percent: number) => void,
): Promise<DownloadReportResponse | null> => {
  try {
    var token = localStorage.getItem("ssrsdetails");
    var AccessToken = localStorage.getItem("accesstoken");
    if (token) {
      var downloadReports: DownloadReports = {
        token: token,
        folderPath: folderPath,
        reportNames: payload,
      };

      const reportNames: AxiosResponse<ApiResponse<DownloadReportResponse>> =
        await api.post(DownloadReportUrl, downloadReports, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${AccessToken}`, // if needed
          },
          onDownloadProgress: (event) => {
            if (event.total && onProgress) {
              const percent = Math.round((event.loaded * 100) / event.total);
              onProgress(percent);
            }
          },
        });
      if (reportNames.data.Data) {
        return reportNames.data.Data;
      } else {
        toastHelper("Something went wrong!!", "error", 4000, "❌");
      }
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "❌");
      return null;
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "❌");
  }
  return null;
};

export const downloadAllReports = async (
  folderPath: string,
  onProgress?: (percent: number) => void,
): Promise<DownloadReportResponse | null> => {
  try {
    var token = localStorage.getItem("ssrsdetails");
    var AccessToken = localStorage.getItem("accesstoken");
    if (token) {
      var downloadReports: DownloadReports = {
        token: token,
        folderPath: folderPath,
      };

      const reportNames: AxiosResponse<ApiResponse<DownloadReportResponse>> =
        await api.post(DownloadAllUrl, downloadReports, {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${AccessToken}`, // if needed
          },
          onDownloadProgress: (event) => {
            if (event.total && onProgress) {
              const percent = Math.round((event.loaded * 100) / event.total);
              onProgress(percent);
            }
          },
        });
      if (reportNames.data.Data) {
        return reportNames.data.Data;
      } else {
        toastHelper("Something went wrong!!", "error", 4000, "❌");
      }
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "❌");
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "❌");
  }
  return null;
};

export const UploadSSRSReports = async (
  UploadReports: UploadReports,
  onProgress?: (percent: number) => void,
): Promise<UploadReportsResponse | null> => {
  try {
    const zipBlob = await createZip(UploadReports.Files, (p: number) => {
      onProgress?.(Math.floor(p * 0.4));
    });
    onProgress?.(50);
    var token = localStorage.getItem("ssrsdetails");
    var AccessToken = localStorage.getItem("accesstoken");
    if (token && AccessToken) {
      const formData = new FormData();
      formData.append("ZipFile", zipBlob, "reports.zip");
      formData.append("Token", token);
      formData.append("TargetFolder", UploadReports.FolderPath);
      formData.append("DataSourceName", UploadReports.DataSource);
      formData.append("DataSourcePath", UploadReports.DataSourcePath ?? "");
      const response = await api.post<ApiResponse<UploadReportsResponse>>(
        UploadUrl,
        formData,
        {
          headers: {
            Authorization: `Bearer ${AccessToken}`,
          },
          onUploadProgress: (e) => {
            if (!e.total) return;
            const percent = 40 + Math.round((e.loaded * 60) / e.total);
            onProgress?.(percent);
          },
          timeout: 120_000,
          transformRequest: (data) => data,
        },
      );
      return response.data.Data ?? null; // jobId
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "❌");
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "❌");
  }
  return null;
};

export const UploadSSRSJobStatus = async (): Promise<
  UploadJobStatus[] | null
> => {
  try {
    var AccessToken = localStorage.getItem("accesstoken");
    if (AccessToken) {
      const response = await api.get<ApiResponse<UploadJobStatus[]>>(
        UploadStatusUrl,
        {
          headers: {
            Authorization: `Bearer ${AccessToken}`,
          },
        },
      );
      if (response.data.Data) return response.data.Data;
      else {
        toastHelper("No Data Found!!", "error", 4000, "❌");
      }
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "❌");
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "❌");
  }
  return null;
};
