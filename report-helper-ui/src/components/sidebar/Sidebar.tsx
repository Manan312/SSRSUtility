import "./Sidebar.css";

interface SidebarProps {
  activePage: string;
  onChange: (page: "upload" | "view" | "download" | "upload-report") => void;
  theme: "dark" | "light";
  setTheme: React.Dispatch<React.SetStateAction<"dark" | "light">>;
  userName: string;
  onLogout: () => void;
}

export default function Sidebar({
  activePage,
  onChange,
  theme,
  setTheme,
  userName,
  onLogout,
}: SidebarProps) {
  return (
    <div
      className={`sidebar ${
        theme === "dark" ? "sidebar-dark" : "sidebar-light"
      }`}
    >
      <div className="sidebar-logo">SSRS Utility</div>

      {/* MENU */}
      <div className="sidebar-menu">
        <button
          className={activePage === "upload-report" ? "active" : ""}
          onClick={() => onChange("upload-report")}
        >
          Upload Reports Status
        </button>

        <button
          className={activePage === "upload" ? "active" : ""}
          onClick={() => onChange("upload")}
        >
          Upload Reports
        </button>

        <button
          className={activePage === "download" ? "active" : ""}
          onClick={() => onChange("download")}
        >
          Download Reports
        </button>

        <button
          className={activePage === "view" ? "active" : ""}
          onClick={() => onChange("view")}
        >
          View Reports
        </button>
      </div>

      {/* FOOTER */}
      <div className="sidebar-footer">
        {/* THEME TOGGLE */}
        <button
          className="theme-btn"
          onClick={() => setTheme(theme === "dark" ? "light" : "dark")}
        >
          {theme === "dark" ? "☀️ Light Mode" : "🌙 Dark Mode"}
        </button>

        {/* USER BOX */}
        <div className="user-box">
          <div className="user-name">{userName}</div>
          <button className="logout-btn" onClick={onLogout}>
            Sign Out
          </button>
        </div>
      </div>
    </div>
  );
}
