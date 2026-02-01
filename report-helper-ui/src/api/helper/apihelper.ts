export interface SsrsConnection {
  BaseUrl?: string;
  Username?: string;
  Password?: string;
  Domain?: string;
}
export interface SsrsFolder {
  Name?: string;
  Path?: string;
}

export interface SsrsDataSource {
  Name: string;
  Path: string;
  IsShared: boolean;
}

export interface EncryptDecrpytModel {
  Token?:string
}

export interface SsrsDetails {
  Token?:string
}

export interface UploadJob {
  Token?: string;
  ZipFile?: Blob;
  FolderPath?: string;
  DataSourceName?: string;
  DataSourcePath?: string;
}

export interface UploadReports {
  Files:File[];
  DataSource:string;
  FolderPath:string;
  DataSourcePath:string;
}

export interface UploadReportsResponse {
  Message:string;
  JobId:string;
  FileCount:string;
}

export interface LoginModel {
  UserName?: string;
  Password?: string;
}

export interface ApiResponse<T> {
  Success?: boolean;
  Data?: T;
  Message?: string;
  TraceId?: string;
}
export interface AccessTokenResponse {
  IsSuccess?: boolean;
  AccessToken?: string;
  ExpiryDate?: string;
  UserRole?: string | "User";
}

export interface SsrsConnectionResponse{
  Folders?:SsrsFolder[]
  DataSources?:SsrsDataSource[]
}

export interface ReportItem {
  Id: string;
  Name: string;
  CreatedDate:Date;
  ModifiedDate:Date;
}

export interface DownloadReports{
  token:string;
  folderPath:string;
  reportNames?:string[];
}


export interface DownloadReportResponse {
  Content?: string;
  FileName?: string;
  ContentType?: string;
}

export interface UploadJobStatus {
  JobId: string;
  Status: string;
  Message:string;
}

export const ApiURL = import.meta.env.VITE_API_URL;
export const AESSecretKey = import.meta.env.VITE_SECRET_KEY;
export const LoginURL = import.meta.env.VITE_Login_URL;
export const FolderUrl = import.meta.env.VITE_Folders_URL;
export const EncryptUrl = import.meta.env.VITE_Encrypt_URL;
export const DecryptUrl = import.meta.env.VITE_Decrypt_URL;
export const DataSourceUrl = import.meta.env.VITE_DataSources_URL;
export const ReportNameUrl = import.meta.env.VITE_ReportName_URL;
export const DownloadAllUrl = import.meta.env.VITE_DownloadAll_URL;
export const DownloadReportUrl = import.meta.env.VITE_DownloadReport_URL;
export const UploadUrl = import.meta.env.VITE_Upload_URL;
export const UploadStatusUrl = import.meta.env.VITE_UploadStatus_URL;
