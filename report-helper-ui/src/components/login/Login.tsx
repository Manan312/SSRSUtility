import React, { useEffect, useState } from "react";
import "./Login.css";
import { login } from "../../api/services/authService";
import toast from "react-hot-toast";
import type { LoginModel } from "../../api/helper/apihelper";
import { toastHelper } from "../../helpers/toastHelper";

interface LoginProps {
  setIsLogin: React.Dispatch<React.SetStateAction<boolean>>;
}
export default function Login({ setIsLogin }: LoginProps) {
  const [viewPassword, setViewPassword] = useState(false);
  const [username, setUserName] = useState("");
  const [password, setPassword] = useState("");
  useEffect(() => {
    try {
      if (localStorage.getItem("accesstoken")) {
        setIsLogin(true);
      }
    } catch (error) {
      toast.error("Something Went Wrong!", {
        duration: 4000,
        icon: "🔐",
      });
    }
  }, []);
  const handleLogin = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const loginModel: LoginModel = {
      UserName: username,
      Password: password,
    };
    try {
      var data = await login(loginModel);
      if (data.IsSuccess == false) {
        toastHelper("We couldn’t sign you in. Please check your details and try again.","error",4000,"🔐");
      } else {
        if (!data?.AccessToken) {
          throw new Error("Access token missing");
        }
        setIsLogin(true);
        localStorage.setItem("accesstoken", data?.AccessToken);
        const expiryValue = data?.ExpiryDate ? new Date(data.ExpiryDate) : null;
        if(expiryValue)
        localStorage.setItem("accesstokenexpiry",expiryValue.getTime().toString());
      
        toastHelper("Welcome back! You have successfully logged in.","success",4000,"🔐");
      }
    } catch (error) {
      toastHelper("We couldn’t sign you in. Please check your details and try again.","error",4000,"🔐");
    }
  };

  return (
    <div className="auth-container">
      <div className="auth-card">
        {/* LEFT IMAGE SECTION */}
        <div className="auth-left">
          <div className="logo">SSRS Utility</div>

          <div className="left-content">
            <h2>
              Helping with SSRS,
              <br />
              Upload, Edit and Download
            </h2>
          </div>
        </div>

        {/* RIGHT FORM SECTION */}
        <div className="auth-right">
          <h1>Login</h1>

          <form className="auth-form" onSubmit={handleLogin}>
            <input
              type="text"
              placeholder="Username"
              value={username}
              onChange={(e) => setUserName(e.target.value)}
            />

            <div className="password-field">
              <input
                type={viewPassword ? "text" : "password"}
                placeholder="Enter your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />

              <button
                type="button" // 🔥 VERY IMPORTANT
                className="eye"
                onClick={() => setViewPassword(!viewPassword)}
              >
                👁
              </button>
            </div>

            <button type="submit" className="primary-btn">
              Login
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
