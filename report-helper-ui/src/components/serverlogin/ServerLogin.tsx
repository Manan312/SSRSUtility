import { useState } from "react";
import "./ServerLogin.css";
import type { SsrsConnection } from "../../api/helper/apihelper";
import { serverConnect } from "../../api/services/serverService";

import { toastHelper } from "../../helpers/toastHelper";
interface ServerLoginProps {
  theme: string;
  setIsServerLogin: React.Dispatch<React.SetStateAction<boolean>>;
}

export default function ServerLogin({
  theme,
  setIsServerLogin,
}: ServerLoginProps) {
  const [showPassword, setShowPassword] = useState(false);
  const [ServerUrl, setServerUrl] = useState("");
  const [Username, setUsername] = useState("");
  const [Password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);

  const handleConnect = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    const serverModel: SsrsConnection = {
      Username: Username,
      Password: Password,
      BaseUrl: ServerUrl,
    };
    try {
      var data = await serverConnect(serverModel);
      if (data) {
        if (data.Folders === null || data.DataSources === null) {
          toastHelper("Something went wrong!!", "error", 4000, "🔐");
        } else {
          setIsServerLogin(true);
          localStorage.setItem("Folders", JSON.stringify(data?.Folders));
          localStorage.setItem(
            "DataSources",
            JSON.stringify(data?.DataSources),
          );
          toastHelper("Server Login Successful!!", "success", 4000, "🔐");
        }
      }
    } catch (error) {
      toastHelper(
        "We couldn’t sign you in. Please check your details and try again.",
        "error",
        4000,
        "🔐",
      );
    }

    setTimeout(() => setLoading(false), 1200);
  };

  return (
    <div className={`server-login-wrapper server-login-${theme}`}>
      <div className="server-login-card">
        <h2>Server Login</h2>
        <p className="subtitle">Connect to your SSRS server</p>

        <form onSubmit={handleConnect}>
          <input
            type="text"
            placeholder="Server URL / Name"
            required
            value={ServerUrl}
            onChange={(e) => setServerUrl(e.target.value)}
          />
          <input
            type="text"
            placeholder="Username"
            required
            value={Username}
            onChange={(e) => setUsername(e.target.value)}
          />

          <div className="password-field">
            <input
              type={showPassword ? "text" : "password"}
              placeholder="Password"
              value={Password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
            <button
              type="button"
              className="eye-btn"
              onClick={() => setShowPassword(!showPassword)}
            >
              👁
            </button>
          </div>

          <button className="connect-btn" disabled={loading}>
            {loading ? "Connecting..." : "Connect"}
          </button>
        </form>
      </div>
    </div>
  );
}
