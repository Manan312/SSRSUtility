import { useEffect, useState } from "react";
import "./UploadStatus.css";
import type { UploadJobStatus } from "../../api/helper/apihelper";
import { UploadSSRSJobStatus } from "../../api/services/serverService";
import { toastHelper } from "../../helpers/toastHelper";

export interface UploadStatusProps {
  theme: string;
}

export default function UploadStatus({ theme }: UploadStatusProps) {
  const [jobs, setJobs] = useState<UploadJobStatus[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(false);
  useEffect(() => {
    loadJobs();
  }, []);

  const loadJobs = async () => {
    try {
      setLoading(true);
      const data = await UploadSSRSJobStatus();
      if (data) setJobs(data);
    } catch (error) {
      toastHelper("Something Went Wrong", "error", 4000, "❌");
    } finally {
      setLoading(false);
    }
  };

  const filteredJobs = jobs.filter(
    (j) =>
      j.JobId.toLowerCase().includes(search.toLowerCase()) ||
      j.Status.toLowerCase().includes(search.toLowerCase()),
  );

  return (
    <div className={`upload-status upload-status-${theme}`}>
      <div className="upload-status-header">
        <h2>Upload Status</h2>

        <button
          className="refresh-btn"
          onClick={loadJobs}
          disabled={loading}
          title="Refresh"
        >
          {loading ? "Refreshing..." : "Refresh"}
        </button>
      </div>

      {/* SEARCH */}
      <input
        className="search-input"
        placeholder="Find by Job ID or Status..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
      />

      {/* TABLE */}
      <div className="status-table-wrapper">
        <table className="status-table">
          <thead>
            <tr>
              <th>Job ID</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {filteredJobs.length === 0 ? (
              <tr>
                <td colSpan={5} className="empty">
                  No jobs found
                </td>
              </tr>
            ) : (
              filteredJobs.map((job) => (
                <tr key={job.JobId}>
                  <td className="mono">{job.JobId}</td>
                  <td>
                    <span className={`status ${job.Status.toLowerCase()}`}>
                      {job.Status}
                    </span>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
