import { useEffect, useState } from "react";
import "./ViewReport.css";

interface ReportParam {
  name: string;
  type: "String" | "DateTime" | "Boolean" | "Integer";
  required?: boolean;
  defaultValue?: string;
  allowedValues?: string[];
}

interface ViewReportProps {
  theme: string;
  reportName?: string;
  reportPath?: string;
  serverUrl?: string;
}

export default function ViewReport({
  theme,
  reportName = "Report",
  reportPath,
  serverUrl,
}: ViewReportProps) {
  const [mode, setMode] = useState<"view" | "edit">("view");

  const [paramsMeta, setParamsMeta] = useState<ReportParam[]>([]);
  const [params, setParams] = useState<Record<string, string>>({});
  const [runParams, setRunParams] = useState<Record<string, string>>({});

  const [xml, setXml] = useState("");

  /* ===============================
     FETCH REPORT PARAMETERS
     =============================== */
  useEffect(() => {
    if (mode === "view" && reportPath) {
      fetch(`/api/report/parameters?path=${reportPath}`)
        .then((r) => r.json())
        .then((data: ReportParam[]) => {
          setParamsMeta(data);

          const initial: Record<string, string> = {};
          data.forEach((p) => {
            if (p.defaultValue) initial[p.name] = p.defaultValue;
          });

          setParams(initial);
          setRunParams({}); // reset run state
        });
    }
  }, [mode, reportPath]);

  /* ===============================
     SSRS RENDER URL (RUN ONLY)
     =============================== */
  const renderUrl =
    serverUrl && reportPath && Object.keys(runParams).length > 0
      ? `${serverUrl}/ReportServer?${reportPath}&rs:Format=HTML4.0&${new URLSearchParams(
          runParams
        ).toString()}`
      : "";

  /* ===============================
     REQUIRED PARAM VALIDATION
     =============================== */
  const canRun =
    paramsMeta.length === 0 ||
    paramsMeta.every(
      (p) => !p.required || (params[p.name] && params[p.name].trim() !== "")
    );

  return (
    <div className={`report-viewer report-${theme}`}>
      {/* ================= HEADER ================= */}
      <div className="report-header">
        <h2>{reportName}</h2>

        <div className="report-actions">
          <button
            className={mode === "view" ? "active" : ""}
            onClick={() => setMode("view")}
          >
            View
          </button>
          <button
            className={mode === "edit" ? "active" : ""}
            onClick={() => setMode("edit")}
          >
            Edit XML
          </button>
        </div>
      </div>

      {/* ================= PARAMETER PANEL ================= */}
      {mode === "view" && paramsMeta.length > 0 && (
        <div className="param-panel">
          {paramsMeta.map((p) => (
            <div key={p.name} className="param-row">
              <label>
                {p.name}
                {p.required && " *"}
              </label>

              {p.allowedValues ? (
                <select
                  value={params[p.name] || ""}
                  onChange={(e) =>
                    setParams({ ...params, [p.name]: e.target.value })
                  }
                >
                  <option value="">Select</option>
                  {p.allowedValues.map((v) => (
                    <option key={v} value={v}>
                      {v}
                    </option>
                  ))}
                </select>
              ) : (
                <input
                  type={p.type === "DateTime" ? "date" : "text"}
                  value={params[p.name] || ""}
                  onChange={(e) =>
                    setParams({ ...params, [p.name]: e.target.value })
                  }
                />
              )}
            </div>
          ))}

          <button
            className="run-btn"
            disabled={!canRun}
            onClick={() => setRunParams(params)}
          >
            Run Report
          </button>
        </div>
      )}

      {/* ================= BODY ================= */}
      <div className="report-body">
        {mode === "view" ? (
          renderUrl ? (
            <iframe
              src={renderUrl}
              title="SSRS Report"
              className="report-frame"
            />
          ) : (
            <div className="empty-state">
              <span className="empty-icon">📄</span>
              <p>
                Set parameters and click <strong>Run Report</strong>
              </p>
            </div>
          )
        ) : (
          <textarea
            className="xml-editor"
            value={xml}
            onChange={(e) => setXml(e.target.value)}
            placeholder="RDL XML"
          />
        )}
      </div>
    </div>
  );
}
