"use client";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { QrCode, Download, Smartphone, Monitor } from "lucide-react";
import { useEffect, useRef, useState } from "react";
// Use named import for better compatibility
import * as QRCode from "qrcode";

interface QRCodeModalProps {
  isOpen: boolean;
  onClose: () => void;
  androidAppId?: string;
  iosAppId?: string;
  appDownloadUrl?: string;
}

export function QRCodeModal({
  isOpen,
  onClose,
  androidAppId = "com.app.ginilog_customer_app",
  iosAppId = "6751465248",
  appDownloadUrl = "https://ginilog.com/download",
}: QRCodeModalProps) {
  const [qrDataUrl, setQrDataUrl] = useState<string | null>(null);
  const [qrError, setQrError] = useState(false);
  const [isGenerating, setIsGenerating] = useState(false);
  const [deviceType, setDeviceType] = useState<"android" | "ios" | "other">("other");

  // Clean device detection
  useEffect(() => {
    if (typeof window === "undefined") return;

    const userAgent = navigator.userAgent.toLowerCase();

    if (/android/i.test(userAgent)) {
      setDeviceType("android");
      return;
    }

    if (/iphone|ipad|ipod/i.test(userAgent)) {
      setDeviceType("ios");
      return;
    }

    if (/macintosh/i.test(userAgent) && navigator.maxTouchPoints > 1) {
      setDeviceType("ios");
      return;
    }

    setDeviceType("other");
  }, []);

  // Get smart download URL
  const getSmartDownloadUrl = () => {
    return `${window.location.origin}/download?android=${androidAppId}&ios=${iosAppId}`;
  };

  // Get direct store link based on device
  const getDirectStoreLink = () => {
    if (deviceType === "android") {
      return `https://play.google.com/store/apps/details?id=${androidAppId}`;
    } else if (deviceType === "ios") {
      return `https://apps.apple.com/ng/app/ginilog/id${iosAppId}`;
    } else {
      return appDownloadUrl;
    }
  };

  const generateQRCode = async () => {
    if (typeof window === "undefined") return;

    setIsGenerating(true);
    setQrError(false);

    try {
      const qrDataUrl = await QRCode.toDataURL(getSmartDownloadUrl(), {
        width: 192,
        margin: 2,
        color: {
          dark: "#1a1a1a",
          light: "#ffffff",
        },
        errorCorrectionLevel: "H",
      });

      setQrDataUrl(qrDataUrl);
    } catch (error) {
      console.error("Failed to generate QR code:", error);
      setQrDataUrl(null);
      setQrError(true);
    } finally {
      setIsGenerating(false);
    }
  };

  useEffect(() => {
    if (!isOpen) return;
    generateQRCode();
  }, [isOpen, androidAppId, iosAppId]);

  // Reset when modal closes
  useEffect(() => {
    if (!isOpen) {
      setQrDataUrl(null);
      setQrError(false);
      setIsGenerating(false);
    }
  }, [isOpen]);

  const handleDownload = () => {
    const link = getDirectStoreLink();
    window.open(link, "_blank");
  };

  const getButtonText = () => {
    if (deviceType === "android") return "Download for Android";
    if (deviceType === "ios") return "Download for iOS";
    return "Open Download Page";
  };

  const getStoreBadge = () => {
    if (deviceType === "android") {
      return (
        <div className="flex items-center gap-2 text-sm text-green-600 bg-green-50 px-3 py-1 rounded-full">
          <Smartphone className="h-4 w-4" />
          <span>Android detected</span>
        </div>
      );
    }
    if (deviceType === "ios") {
      return (
        <div className="flex items-center gap-2 text-sm text-blue-600 bg-blue-50 px-3 py-1 rounded-full">
          <Smartphone className="h-4 w-4" />
          <span>iOS detected</span>
        </div>
      );
    }
    return (
      <div className="flex items-center gap-2 text-sm text-gray-600 bg-gray-50 px-3 py-1 rounded-full">
        <Monitor className="h-4 w-4" />
        <span>Desktop detected</span>
      </div>
    );
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <QrCode className="h-5 w-5 text-primary" />
            Scan to Download
          </DialogTitle>
          <DialogDescription>
            Scan this QR code with your phone to download the GINILOG app
          </DialogDescription>
        </DialogHeader>

        <div className="flex flex-col items-center py-6">
          <div className="mb-4">{getStoreBadge()}</div>

          <div className="bg-white p-4 rounded-xl border-2 border-gray-200">
            <div className="w-48 h-48 bg-gray-100 flex items-center justify-center">
              {qrDataUrl && !qrError ? (
                <img
                  src={qrDataUrl}
                  alt="Scan to download Ginilog app"
                  className="w-48 h-48"
                />
              ) : qrError ? (
                <div className="w-48 h-48 bg-gray-100 flex flex-col items-center justify-center text-center px-4">
                  <svg
                    className="h-16 w-16 text-gray-400 mb-2"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M12 8v4m0 4h.01M4.93 4.93l14.14 14.14M4.93 19.07L19.07 4.93"
                    />
                  </svg>
                  <p className="text-xs text-gray-500">Unable to generate QR code.</p>
                </div>
              ) : (
                <div className="w-48 h-48 bg-gray-100 flex items-center justify-center">
                  <span className="text-xs text-gray-400">Generating QR code...</span>
                </div>
              )}
            </div>
          </div>

          <div className="mt-4 text-center">
            <p className="text-sm text-gray-600">
              Or visit: <span className="text-primary font-medium">{appDownloadUrl}</span>
            </p>
          </div>

          <div className="flex flex-wrap gap-3 mt-3 justify-center text-xs">
            <a
              href={`https://play.google.com/store/apps/details?id=${androidAppId}`}
              target="_blank"
              rel="noopener noreferrer"
              className="text-gray-500 hover:text-primary transition-colors"
            >
              Google PlayPlay
            </a>
            <span className="text-gray-300">|</span>
            <a
              href={`https://apps.apple.com/ng/app/ginilog/id${iosAppId}`}
              target="_blank"
              rel="noopener noreferrer"
              className="text-gray-500 hover:text-primary transition-colors"
            >
              App Store
            </a>
          </div>

          {qrError && (
            <div className="mt-3 w-full flex justify-center">
              <Button variant="secondary" className="w-full sm:w-auto" onClick={generateQRCode}>
                Retry QR Code
              </Button>
            </div>
          )}

          <div className="flex gap-3 mt-6 w-full">
            <Button variant="outline" className="flex-1" onClick={onClose}>
              Close
            </Button>
            <Button className="flex-1" onClick={handleDownload}>
              <Download className="h-4 w-4 mr-2" />
              {getButtonText()}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}