import { useEffect, useState } from "react";
import "./Download.css";
import FolderDataSource from "../folderDataSource/FolderDataSource";
import type { ReportItem } from "../../api/helper/apihelper";
import {
  downloadAllReports,
  downloadSpecificReports,
  fetchReportName,
} from "../../api/services/serverService";
import { toastHelper } from "../../helpers/toastHelper";
import { downloadZip } from "../../api/helper/DataHelper";

interface DownloadProps {
  theme: string;
}

export default function Download({ theme }: DownloadProps) {
  const [reports, setReports] = useState<ReportItem[]>([]);
  const [FolderPath, setFolderPath] = useState("");
  const [search, setSearch] = useState("");
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [isDownloading, setIsDownloading] = useState(false);
  const [progress, setProgress] = useState(0);

  const filteredReports = reports.filter((r) =>
    r.Name.toLowerCase().includes(search.toLowerCase()),
  );
  type SortKey = "Name" | "CreatedDate" | "ModifiedDate";
  type SortOrder = "asc" | "desc";

  const [sortKey, setSortKey] = useState<SortKey>("Name");
  const [sortOrder, setSortOrder] = useState<SortOrder>("asc");
  const sortedReports = [...filteredReports].sort((a, b) => {
    const aVal = a[sortKey];
    const bVal = b[sortKey];

    if (!aVal || !bVal) return 0;

    if (aVal < bVal) return sortOrder === "asc" ? -1 : 1;
    if (aVal > bVal) return sortOrder === "asc" ? 1 : -1;
    return 0;
  });
  const handleSort = (key: SortKey) => {
    if (sortKey === key) {
      setSortOrder((prev) => (prev === "asc" ? "desc" : "asc"));
    } else {
      setSortKey(key);
      setSortOrder("asc");
    }
  };
  /* TOGGLE SELECT */
  const toggleSelect = (id: string) => {
    const next = new Set(selected);
    next.has(id) ? next.delete(id) : next.add(id);
    setSelected(next);
  };

  /* DOWNLOAD HANDLERS */
  const downloadSelected = async (e: React.FormEvent<HTMLButtonElement>) => {
    e.preventDefault();
    try {
      const selectedNames: string[] = reports
        .filter((r) => selected.has(r.Id))
        .map((r) => r.Name);
      setIsDownloading(true);
      setProgress(0);
      const zipFile = await downloadSpecificReports(FolderPath, selectedNames);
      if (zipFile) downloadZip(zipFile);
      else toastHelper("Something went wrong!!", "error", 4000, "🔐");
      setIsDownloading(false);
    } catch (error) {
      toastHelper("Something went wrong!!", "error", 4000, "🔐");
    }
  };

  const downloadAll = async (e: React.FormEvent<HTMLButtonElement>) => {
    e.preventDefault();
    try {
      setIsDownloading(true);
      setProgress(0);
      const zipFile = await downloadAllReports(FolderPath);
      if (zipFile) downloadZip(zipFile);
      else toastHelper("Something went wrong!!", "error", 4000, "🔐");
      setIsDownloading(false);
    } catch (error) {
      toastHelper("Something went wrong!!", "error", 4000, "🔐");
    }
  };
  useEffect(() => {
    console.log(FolderPath);
    if (!FolderPath) return; // guard
    setReports([]);
    setIsDownloading(true);
    setProgress(0);
    const loadReports = async () => {
      const reports = await fetchReportName(FolderPath);
      if (reports) {
        setReports(reports); // assuming you want to store them
      }
    };
    setIsDownloading(false);
    loadReports();
  }, [FolderPath]);
  return (
    <div className={`download-page download-${theme}`}>
      {isDownloading && (
        <div className="fullscreen-loader">
          <div className="loader-box">
            <div className="spinner"></div>
            <p>Downloading reports...</p>
            <strong>{progress}%</strong>
          </div>
        </div>
      )}
      {/* HEADER */}
      <div className="download-header">
        <h2>Download Reports </h2>
        <h2>Total Reports - {reports.length}</h2>
        <div className="download-actions">
          <button className="primary" onClick={downloadSelected}>
            Download Selected
          </button>

          <button className="primary" onClick={downloadAll}>
            Download All
          </button>
        </div>
      </div>
      <FolderDataSource
        theme={theme}
        showDataSource={false}
        onChange={(folder) => {
          setFolderPath(folder);
        }}
      />
      {/* SEARCH */}
      <input
        className="search-input"
        placeholder="Search reports by name..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      {/* LIST */}
      <div className="report-list">
        {sortedReports.length === 0 ? (
          <div className="empty-state">No reports found</div>
        ) : (
          <table className="report-table">
            <thead>
              <tr>
                <th></th>
                <th onClick={() => handleSort("Name")}>
                  Name {sortKey === "Name" && (sortOrder === "asc" ? "▲" : "▼")}
                </th>
                <th onClick={() => handleSort("CreatedDate")}>
                  Created{" "}
                  {sortKey === "CreatedDate" &&
                    (sortOrder === "asc" ? "▲" : "▼")}
                </th>
                <th onClick={() => handleSort("ModifiedDate")}>
                  Modified{" "}
                  {sortKey === "ModifiedDate" &&
                    (sortOrder === "asc" ? "▲" : "▼")}
                </th>
              </tr>
            </thead>

            <tbody>
              {sortedReports.map((r) => (
                <tr key={r.Id}>
                  <td>
                    <input
                      type="checkbox"
                      checked={selected.has(r.Id)}
                      onChange={() => toggleSelect(r.Id)}
                    />
                  </td>
                  <td className="report-name">{r.Name}</td>
                  <td>
                    {r.CreatedDate
                      ? new Date(r.CreatedDate).toLocaleDateString()
                      : "-"}
                  </td>
                  <td>
                    {r.ModifiedDate
                      ? new Date(r.ModifiedDate).toLocaleDateString()
                      : "-"}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}
