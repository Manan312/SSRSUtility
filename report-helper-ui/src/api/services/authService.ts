import api from "../middleware/apiClient";
import { LoginURL } from "../helper/apihelper";
import type { LoginModel, ApiResponse, AccessTokenResponse } from "../helper/apihelper";
import type { AxiosResponse } from "axios";

export const login = async (
  loginModel: LoginModel
): Promise<AccessTokenResponse> => {
  // console.log("here");
  const res: AxiosResponse<ApiResponse<AccessTokenResponse>> =
    await api.post(LoginURL, loginModel);

  if (!res.data.Success || !res.data.Data) {
    throw new Error(res.data.Message ?? "Login failed");
  }
  // console.log(res);

  return res.data.Data; // ✅ clean return
};
