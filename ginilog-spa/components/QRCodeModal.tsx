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
  const qrContainerRef = useRef<HTMLDivElement>(null);
  const qrGeneratedRef = useRef<boolean>(false);
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

  // Generate QR code
  useEffect(() => {
    if (!isOpen || !qrContainerRef.current || qrGeneratedRef.current) return;

    const generateQR = async () => {
      try {
        // Clear previous QR code
        qrContainerRef.current!.innerHTML = "";

        // Generate QR code with proper error handling
        const qrDataUrl = await QRCode.toDataURL(getSmartDownloadUrl(), {
          width: 192,
          margin: 2,
          color: {
            dark: "#1a1a1a",
            light: "#ffffff",
          },
          errorCorrectionLevel: "H",
        });

        // Create and append the QR image
        const img = document.createElement("img");
        img.src = qrDataUrl;
        img.alt = "Scan to download Ginilog app";
        img.className = "w-48 h-48";
        qrContainerRef.current!.appendChild(img);
        qrGeneratedRef.current = true;
      } catch (error) {
        console.error("Failed to generate QR code:", error);
        // Show fallback with better visual
        qrContainerRef.current!.innerHTML = `
          <div class="w-48 h-48 bg-gray-100 flex items-center justify-center">
            <div class="text-center">
              <svg class="h-24 w-24 text-gray-400 mx-auto" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v1m6 11h2m-6 0h-2v4m0-11v3m0 0h.01M12 12h4.01M16 20h4M4 12h4m12 0h.01M5 8h2a1 1 0 001-1V5a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1zm12 0h2a1 1 0 001-1V5a1 1 0 00-1-1h-2a1 1 0 00-1 1v2a1 1 0 001 1zM5 20h2a1 1 0 001-1v-2a1 1 0 00-1-1H5a1 1 0 00-1 1v2a1 1 0 001 1z"/>
              </svg>
              <p class="text-xs text-gray-500 mt-2">Tap to retry</p>
            </div>
          </div>
        `;
      }
    };

    const timer = setTimeout(generateQR, 100);
    return () => clearTimeout(timer);
  }, [isOpen, androidAppId, iosAppId]);

  // Reset when modal closes
  useEffect(() => {
    if (!isOpen) {
      qrGeneratedRef.current = false;
      if (qrContainerRef.current) {
        qrContainerRef.current.innerHTML = "";
      }
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
            <div ref={qrContainerRef} className="w-48 h-48 bg-gray-100 flex items-center justify-center">
              {/* QR code generated here */}
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
              Google Play
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