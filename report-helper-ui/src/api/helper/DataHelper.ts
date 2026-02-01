import CryptoJS from "crypto-js";
import type { DownloadReportResponse } from "./apihelper";
import { toastHelper } from "../../helpers/toastHelper";

export function encryptJson(payload: any, secretKey: string): string {
  const json = JSON.stringify(payload);

  const key = CryptoJS.enc.Utf8.parse(secretKey.padEnd(32, " "));
  const iv = CryptoJS.enc.Hex.parse("00000000000000000000000000000000");

  const encrypted = CryptoJS.AES.encrypt(json, key, {
    iv,
    mode: CryptoJS.mode.CBC,
    padding: CryptoJS.pad.Pkcs7,
  });

  // IMPORTANT: ciphertext only
  return encrypted.ciphertext.toString(CryptoJS.enc.Base64);
}
export function decryptJson(cipherText: string, secretKey: string): string {
  const key = CryptoJS.enc.Utf8.parse(secretKey.padEnd(32, " "));
  const iv = CryptoJS.enc.Hex.parse("00000000000000000000000000000000");

  const decrypted = CryptoJS.AES.decrypt(
    { ciphertext: CryptoJS.enc.Base64.parse(cipherText) } as any,
    key,
    {
      iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7,
    },
  );

  return decrypted.toString(CryptoJS.enc.Utf8);
}
export const downloadZip = (response: DownloadReportResponse) => {
  try {
    if (response.Content && response.FileName && response.ContentType) {
      const blob = base64ToBlob(response.Content, response.ContentType);

      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");

      a.href = url;
      a.download = response.FileName;
      document.body.appendChild(a);
      a.click();

      a.remove();
      window.URL.revokeObjectURL(url);
    } else {
      toastHelper("Something went wrong!!", "error", 4000, "🔐");
    }
  } catch (error) {
    toastHelper("Something went wrong!!", "error", 4000, "🔐");
  }
};

export const base64ToBlob = (base64: string, contentType: string): Blob => {
  const byteCharacters = atob(base64);
  const byteNumbers = new Array(byteCharacters.length);

  for (let i = 0; i < byteCharacters.length; i++) {
    byteNumbers[i] = byteCharacters.charCodeAt(i);
  }

  const byteArray = new Uint8Array(byteNumbers);
  return new Blob([byteArray], { type: contentType });
};
