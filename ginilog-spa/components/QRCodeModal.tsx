"use client";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { QrCode, Download, X } from "lucide-react";
import Image from "next/image";

interface QRCodeModalProps {
  isOpen: boolean;
  onClose: () => void;
  appDownloadUrl?: string;
}

export function QRCodeModal({
  isOpen,
  onClose,
  appDownloadUrl = "https://ginilog.com/download",
}: QRCodeModalProps) {
  // In a real app, you would generate a QR code using a library like qrcode.react
  // For now, we'll show a placeholder

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
          <div className="bg-white p-4 rounded-xl border-2 border-gray-200">
            <div className="w-48 h-48 bg-gray-100 flex items-center justify-center">
              {/* QR Code placeholder */}
              <div className="text-center">
                <QrCode className="h-24 w-24 text-gray-400 mx-auto" />
                <p className="text-xs text-gray-500 mt-2">QR Code</p>
              </div>
            </div>
          </div>

          <div className="mt-4 text-center">
            <p className="text-sm text-gray-600">
              Or visit: <span className="text-primary font-medium">{appDownloadUrl}</span>
            </p>
          </div>

          <div className="flex gap-3 mt-6 w-full">
            <Button variant="outline" className="flex-1" onClick={onClose}>
              Close
            </Button>
            <Button className="flex-1" onClick={() => window.open(appDownloadUrl, "_blank")}>
              <Download className="h-4 w-4 mr-2" />
              Open Download
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}