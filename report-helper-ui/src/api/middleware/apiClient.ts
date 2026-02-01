import axios from "axios";
import { ApiURL } from "../helper/apihelper";
const api = axios.create({
  baseURL: ApiURL,
  headers: {
    "Content-Type": "application/json",
  },
});

export default api;