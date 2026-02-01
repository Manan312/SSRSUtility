import "./Upload.css";
import { useState } from "react";
import FolderDataSource from "../folderDataSource/FolderDataSource";
import { toastHelper } from "../../helpers/toastHelper";
import { UploadSSRSReports } from "../../api/services/serverService";
import type { UploadReports } from "../../api/helper/apihelper";

interface UploadProps {
  theme: string;
}
export default function Upload({ theme }: UploadProps) {
  const [files, setFiles] = useState<File[]>([]);
  const [isUploading, setIsUploading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [folder, setFolderPath] = useState("");
  const [dataSourceName, setDataSourceName] = useState("");
  const [dataSourcePath, setDataSourcePath] = useState("");
  const uploadFiles = async (e: React.FormEvent<HTMLButtonElement>) => {
    e.preventDefault();
    try {
      if (files.length === 0) {
        toastHelper("Please select .rdl files", "warning", 4000, "📂");
        return;
      }
      setIsUploading(true);
      setProgress(0);
      var uploadReports: UploadReports = {
        Files: files,
        DataSource: dataSourceName,
        DataSourcePath: dataSourcePath,
        FolderPath: folder,
      };
      const job = await UploadSSRSReports(uploadReports);
      if (job?.JobId) {
        toastHelper(
          `Job scheduled`,
          "success",
          4000,
          "✅",
        );
      }
    } catch (error) {
      toastHelper("Something Went Wrong", "error", 4000, "❌");
    } finally {
      setIsUploading(false);
      setProgress(0);
    }
  };
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selected = Array.from(e.target.files || []);

    const invalid = selected.filter(
      (f) => !f.name.toLowerCase().endsWith(".rdl"),
    );

    if (invalid.length > 0) {
      toastHelper("Only .rdl files allowed", "error", 4000, "📄");
      e.target.value = "";
      return;
    }

    setFiles(selected);
  };
  return (
    <>
      {isUploading && (
        <div className="fullscreen-loader">
          <div className="loader-box">
            <div className="spinner"></div>
            <p>Uploading reports…</p>
            <strong>{progress}%</strong>
          </div>
        </div>
      )}
      <FolderDataSource
        theme={theme}
        showDataSource={true}
        onChange={(folder, dataSourcePath, dataSourceName) => {
          setFolderPath(folder);
          if (dataSourcePath) setDataSourcePath(dataSourcePath);
          if (dataSourceName) setDataSourceName(dataSourceName);
        }}
      />
      <div className={`upload-wrapper upload-${theme}`}>
        {/* SERVER STATUS */}
        <div className="upload-info">
          <span className="status-dot connected"></span>
          Connected to SSRS Server
        </div>

        {/* UPLOAD CARD */}
        <div className="upload-card">
          <h2>Upload Reports</h2>
          <p className="subtitle">Upload SSRS reports (.rdl) or zip files</p>

          <label className="upload-area">
            <input
              type="file"
              accept=".rdl"
              onChange={handleFileChange}
              multiple
            />
            <p>Drag & drop files here or click to browse</p>
            <span>.rdl, .zip supported</span>
          </label>

          <div className="upload-actions">
            <button className="primary-btn" onClick={uploadFiles}>
              Upload
            </button>
            <button className="secondary-btn">Clear</button>
          </div>
        </div>
      </div>
    </>
  );
}
