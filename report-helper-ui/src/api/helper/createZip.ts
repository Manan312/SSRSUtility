import JSZip from "jszip";

export const createZip = async (
  files: File[],
  onProgress: (p: number) => void,
): Promise<Blob> => {
  const zip = new JSZip();

  files.forEach((file) => {
    zip.file(file.name, file);
  });

  return zip.generateAsync({ type: "blob" }, (meta) => {
    onProgress(Math.round(meta.percent)); // 0–100
  });
};
