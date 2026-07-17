// lib/appUtils.ts

export const isAppInstalled = (): boolean => {
  // Check if app is installed via deep link or custom scheme
  if (typeof window === "undefined") return false;

  // Check for app-specific cookie or localStorage
  const appInstalled = localStorage.getItem("appInstalled");
  if (appInstalled === "true") return true;

  // Check if user agent has app-specific identifier
  const ua = navigator.userAgent;
  return ua.includes("GINILOGApp") || ua.includes("GINILOG/");
};

export const openAppOrDownload = () => {
  const appDeepLink = "ginilog://dashboard";
  const downloadUrl = "/download-app";

  if (isAppInstalled()) {
    // Try to open the app
    window.location.href = appDeepLink;
  } else {
    // Open download page
    window.open(downloadUrl, "_blank");
  }
};

export const getDeviceType = (): "mobile" | "tablet" | "desktop" => {
  if (typeof window === "undefined") return "desktop";
  const ua = navigator.userAgent;
  if (/Mobi|Android|iPhone|iPad|iPod/i.test(ua)) return "mobile";
  if (/Tablet|iPad/i.test(ua)) return "tablet";
  return "desktop";
};