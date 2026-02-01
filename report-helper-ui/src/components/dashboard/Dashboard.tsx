import { useEffect, useState } from "react";
import Sidebar from "../sidebar/Sidebar";
import Upload from "../upload/Upload";
import ViewReport from "../viewreport/ViewReport";
import Download from "../download/Download";
import "./Dashboard.css";
import ServerLogin from "../serverlogin/ServerLogin";
import { toastHelper } from "../../helpers/toastHelper";
import UploadStatus from "../uploadStatus/UploadStatus";

interface DashboardProps {
  setIsLogin: React.Dispatch<React.SetStateAction<boolean>>;
}
export default function Dashboard({ setIsLogin }: DashboardProps) {
  const [activePage, setActivePage] = useState<
    "upload" | "view" | "download" | "upload-report"
  >("upload-report");

  const [theme, setTheme] = useState<"dark" | "light">(
    (localStorage.getItem("theme") as "dark" | "light") || "light",
  );
  const [isServerLogin, setIsServerLogin] = useState(false);
  const userName = "";
  useEffect(() => {
    if (localStorage.getItem("Folders")) {
      setIsServerLogin(true);
    }
    const interval = setInterval(() => {
      const expiry = localStorage.getItem("accesstokenexpiry");
      const now = Date.now();
      if (expiry) {
        const expiryTime = parseInt(expiry);
        const timeLeft = Math.floor((expiryTime - now) / 1000);
        // console.log(now + "   " + expiryTime + "   " + timeLeft);
        if (timeLeft <= 0) handleLogout();
        if (timeLeft > 0 && timeLeft <= 60) {
          toastHelper(
            `You will be logged out in ${timeLeft}!!`,
            "error",
            4000,
            "🔐",
          );
        }
      }
    }, 60000);
    return () => clearInterval(interval); // Clean up when component unmounts
  }, []);
  useEffect(() => {
    document.body.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);

  const handleLogout = () => {
    try {
      setIsLogin(false);
      setIsServerLogin(false);
      localStorage.clear();
      toastHelper("User Logged Out!!", "error", 4000, "🔐");
    } catch (error) {
      toastHelper("Something went wrong!!", "error", 4000, "🔐");
    }
  };

  const renderContent = (theme: string) => {
    let header = "";
    let content = null;

    switch (activePage) {
      case "view":
        header = "View Reports";
        content = !isServerLogin ? (
          <ServerLogin theme={theme} setIsServerLogin={setIsServerLogin} />
        ) : (
          <ViewReport theme={theme} />
        );
        break;

      case "download":
        header = "Download Reports";
        content = !isServerLogin ? (
          <ServerLogin theme={theme} setIsServerLogin={setIsServerLogin} />
        ) : (
          <Download theme={theme} />
        );
        break;

      case "upload":
        header = "Upload Reports";
        content = !isServerLogin ? (
          <ServerLogin theme={theme} setIsServerLogin={setIsServerLogin} />
        ) : (
          <Upload theme={theme} />
        );
        break;
      default:
        header = "Upload Report Status";
        content = <UploadStatus theme={theme} />;
    }

    return (
      <div className="page-wrapper">
        {/* HEADER */}
        <div className="page-header">
          <h1>{header}</h1>
        </div>

        {/* PAGE CONTENT */}
        <div className="page-content">{content}</div>
      </div>
    );
  };

  return (
    <div className="dashboard-layout">
      <Sidebar
        activePage={activePage}
        onChange={setActivePage}
        theme={theme}
        setTheme={setTheme}
        userName={userName}
        onLogout={handleLogout}
      />

      <div
        className={`dashboard-content ${
          theme === "dark" ? "dashboard-dark" : "dashboard-light"
        }`}
      >
        {renderContent(theme)}
      </div>
    </div>
  );
}
