import { useState, useEffect } from "react";
import type { SsrsFolder, SsrsDataSource } from "../../api/helper/apihelper";
import "./FolderDataSource.css";
export interface FolderDataSourcePropos {
  theme: string;
  showDataSource?: boolean;
  onChange?: (folderPath: string, dataSourcePath?: string,dataSourceName?:string) => void;
}
export default function FolderDataSource({
  theme,
  showDataSource = true,
  onChange,
}: FolderDataSourcePropos) {
  const [folders, setFolders] = useState<SsrsFolder[]>([]);

  const [dataSources, setDataSources] = useState<SsrsDataSource[]>([]);

  const [selectedFolder, setSelectedFolder] = useState("");

  const [selectedDataSource, setSelectedDataSource] = useState("");
  const [selectedDataSourcePath, setSelectedDataSourcePath] = useState("");

  useEffect(() => {
    debugger;
    const storedFolders = localStorage.getItem("Folders");
    const storedDataSources = localStorage.getItem("DataSources");

    if (storedFolders) setFolders(JSON.parse(storedFolders));
    if (storedDataSources) {
      const parsed = JSON.parse(storedDataSources);
      setDataSources(parsed);
    }
  }, []);
  return (
    <div className={`fds-container fds-${theme}`}>
      <div className="fds-field">
        <label>Folder</label>
        <select
          value={selectedFolder}
          onChange={(e) => {
            const folderPath = e.target.value;

            setSelectedFolder(folderPath);
            setSelectedDataSource("");
            setSelectedDataSourcePath("");

            onChange?.(folderPath);
          }}
        >
          <option value="">Select folder</option>
          {folders.map((folder) => (
            <option key={folder.Path} value={folder.Path}>
              {folder.Name}
            </option>
          ))}
        </select>
      </div>
      {showDataSource && (
        <div className="fds-field">
          <label>Data Source</label>
          <select
            value={selectedDataSource}
            onChange={(e) => {
              const dsPath = e.target.value;
              const ds = dataSources.find((d) => d.Path === dsPath);

              setSelectedDataSource(dsPath);
              setSelectedDataSourcePath(dsPath);

              onChange?.(
                selectedFolder,
                dsPath,
                ds?.Name
              );
            }}
          >
            <option value="">Select data source</option>
            {dataSources.map((ds) => (
              <option key={ds.Path} value={ds.Path}>
                {ds.Name} {ds.IsShared ? "(Shared)" : ""}
              </option>
            ))}
          </select>
        </div>
      )}
    </div>
  );
}
