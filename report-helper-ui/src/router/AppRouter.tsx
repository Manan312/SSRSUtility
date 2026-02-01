import React, { useState } from "react";
import Login from "../components/login/Login";
import Dashboard from "../components/dashboard/Dashboard";

export default function AppRouter() {
  const [isLogin, setIsLogin] = useState(false);
  return <>{!isLogin ? <Login setIsLogin={setIsLogin} /> : <Dashboard setIsLogin={setIsLogin}/>}</>;
}
