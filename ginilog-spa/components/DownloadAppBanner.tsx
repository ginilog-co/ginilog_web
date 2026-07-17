"use client";

import { useState, useEffect } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Smartphone,
  Download,
  Apple,
  PlayCircle,
  QrCode,
  X,
  Star,
  Sparkles,
  ArrowRight,
  CheckCircle,
  Monitor,
  Clock,
  Shield,
  Zap,
} from "lucide-react";
import Image from "next/image";
import Link from "next/link";

interface DownloadAppBannerProps {
  onClose?: () => void;
  isDismissible?: boolean;
  variant?: "banner" | "card" | "floating" | "popup";
}

export function DownloadAppBanner({
  onClose,
  isDismissible = true,
  variant = "banner",
}: DownloadAppBannerProps) {
  const [isVisible, setIsVisible] = useState(true);
  const [isMobile, setIsMobile] = useState(false);

  useEffect(() => {
    // Check if user is on mobile
    const checkMobile = () => {
      setIsMobile(window.innerWidth < 768);
    };
    checkMobile();
    window.addEventListener("resize", checkMobile);
    return () => window.removeEventListener("resize", checkMobile);
  }, []);

  // Check if user is on mobile device (user agent)
  const isMobileDevice = /Android|iPhone|iPad|iPod/i.test(navigator.userAgent);

  const handleClose = () => {
    setIsVisible(false);
    if (onClose) onClose();
    // Store dismissal in localStorage
    localStorage.setItem("appBannerDismissed", "true");
  };

  // Check if banner was dismissed before
  useEffect(() => {
    const dismissed = localStorage.getItem("appBannerDismissed");
    if (dismissed === "true") {
      setIsVisible(false);
    }
  }, []);

  // Get appropriate app store links
  const appStoreLink = "https://apps.apple.com/app/ginilog";
  const playStoreLink = "https://play.google.com/store/apps/details?id=com.ginilog";

  const features = [
    { icon: Zap, text: "Real-time tracking" },
    { icon: Shield, text: "Secure payments" },
    { icon: Clock, text: "24/7 support" },
    { icon: Star, text: "Easy booking" },
  ];

  if (!isVisible) return null;

  // Floating variant - bottom right
  if (variant === "floating") {
    return (
      <div className="fixed bottom-4 right-4 z-50 max-w-sm animate-in slide-in-from-bottom-5 duration-300">
        <Card className="shadow-2xl border-2 border-primary/20 bg-gradient-to-br from-white to-primary/5">
          <CardContent className="p-4">
            <div className="flex items-start gap-3">
              <div className="h-12 w-12 rounded-xl bg-primary flex items-center justify-center flex-shrink-0">
                <Smartphone className="h-6 w-6 text-white" />
              </div>
              <div className="flex-1">
                <h4 className="font-semibold text-sm">Get the App</h4>
                <p className="text-xs text-gray-600">
                  Better experience on mobile
                </p>
                <div className="flex gap-2 mt-2">
                  <Link
                    href={isMobileDevice ? playStoreLink : "#"}
                    target="_blank"
                    rel="noopener noreferrer"
                  >
                    <Button size="sm" className="h-8 text-xs">
                      <Download className="h-3 w-3 mr-1" />
                      Download
                    </Button>
                  </Link>
                  {isDismissible && (
                    <Button
                      size="sm"
                      variant="ghost"
                      className="h-8 px-2"
                      onClick={handleClose}
                    >
                      <X className="h-4 w-4" />
                    </Button>
                  )}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  // Popup variant
  if (variant === "popup") {
    return (
      <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 animate-in fade-in duration-200">
        <div className="bg-white rounded-2xl max-w-lg w-full mx-4 p-6 relative animate-in zoom-in-95 duration-200 shadow-2xl">
          {isDismissible && (
            <button
              onClick={handleClose}
              className="absolute top-4 right-4 p-2 hover:bg-gray-100 rounded-full transition-colors"
              aria-label="Close"
            >
              <X className="h-5 w-5 text-gray-500" />
            </button>
          )}

          <div className="text-center mb-6">
            <div className="h-20 w-20 rounded-full bg-primary/10 flex items-center justify-center mx-auto mb-4">
              <Smartphone className="h-10 w-10 text-primary" />
            </div>
            <h2 className="text-2xl font-bold text-gray-900">
              Get the GINILOG App
            </h2>
            <p className="text-gray-600 mt-2">
              Track packages, book accommodations, and manage everything on the go
            </p>
          </div>

          {/* Features */}
          <div className="grid grid-cols-2 gap-3 mb-6">
            {features.map((feature, index) => (
              <div key={index} className="flex items-center gap-2 text-sm">
                <feature.icon className="h-4 w-4 text-primary" />
                <span className="text-gray-700">{feature.text}</span>
              </div>
            ))}
          </div>

          {/* Download Buttons */}
          <div className="space-y-3">
            <Link
              href={playStoreLink}
              target="_blank"
              rel="noopener noreferrer"
              className="block"
            >
              <Button className="w-full h-12 text-base gap-3 bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800">
                <PlayCircle className="h-6 w-6" />
                Download for Android
              </Button>
            </Link>
            <Link
              href={appStoreLink}
              target="_blank"
              rel="noopener noreferrer"
              className="block"
            >
              <Button variant="outline" className="w-full h-12 text-base gap-3">
                <Apple className="h-6 w-6" />
                Download for iOS
              </Button>
            </Link>
          </div>

          <p className="text-center text-xs text-gray-500 mt-4">
            Scan QR code or download from official stores
          </p>
        </div>
      </div>
    );
  }

  // Card variant
  if (variant === "card") {
    return (
      <Card className="bg-gradient-to-r from-primary to-primary/80 text-white shadow-xl">
        <CardContent className="p-6">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <div className="h-14 w-14 rounded-2xl bg-white/20 flex items-center justify-center">
                <Smartphone className="h-7 w-7 text-white" />
              </div>
              <div>
                <h3 className="text-lg font-bold">Get the GINILOG App</h3>
                <p className="text-white/80 text-sm">
                  Track, book, and manage everything on the go
                </p>
              </div>
            </div>
            <div className="flex gap-2">
              <Link
                href={isMobileDevice ? playStoreLink : "#"}
                target="_blank"
                rel="noopener noreferrer"
              >
                <Button
                  variant="secondary"
                  className="bg-white text-primary hover:bg-white/90"
                >
                  <Download className="h-4 w-4 mr-2" />
                  Download
                </Button>
              </Link>
              {isDismissible && (
                <Button
                  variant="ghost"
                  className="text-white hover:bg-white/20"
                  onClick={handleClose}
                >
                  <X className="h-4 w-4" />
                </Button>
              )}
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  // Banner variant (default)
  return (
    <div className="bg-gradient-to-r from-primary via-primary/90 to-primary/70 rounded-xl overflow-hidden shadow-lg">
      <div className="container mx-auto px-4 py-4">
        <div className="flex flex-col md:flex-row items-center justify-between gap-4">
          {/* Left - Content */}
          <div className="flex-1 flex items-center gap-4">
            <div className="hidden sm:flex h-12 w-12 rounded-xl bg-white/20 items-center justify-center flex-shrink-0">
              <Smartphone className="h-6 w-6 text-white" />
            </div>
            <div>
              <div className="flex items-center gap-2">
                <h3 className="text-white font-semibold text-lg">
                  📱 Get the App
                </h3>
                <Badge variant="secondary" className="bg-white/20 text-white border-0">
                  New
                </Badge>
              </div>
              <p className="text-white/80 text-sm">
                Track packages, book stays, and manage everything on the go
              </p>
            </div>
          </div>

          {/* Right - Actions */}
          <div className="flex items-center gap-3 w-full md:w-auto">
            {/* Download Buttons */}
            <div className="flex gap-2">
              {isMobileDevice ? (
                <>
                  <Link
                    href={playStoreLink}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex-1"
                  >
                    <Button
                      variant="secondary"
                      className="w-full bg-white/20 text-white hover:bg-white/30 border-0"
                    >
                      <PlayCircle className="h-4 w-4 mr-2" />
                      Android
                    </Button>
                  </Link>
                  <Link
                    href={appStoreLink}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="flex-1"
                  >
                    <Button
                      variant="secondary"
                      className="w-full bg-white/20 text-white hover:bg-white/30 border-0"
                    >
                      <Apple className="h-4 w-4 mr-2" />
                      iOS
                    </Button>
                  </Link>
                </>
              ) : (
                <>
                  <Button
                    variant="secondary"
                    className="bg-white text-primary hover:bg-white/90"
                    onClick={() => {
                      // Show QR code or redirect to download page
                      window.open("/download-app", "_blank");
                    }}
                  >
                    <Download className="h-4 w-4 mr-2" />
                    Download App
                  </Button>
                  <Button
                    variant="ghost"
                    className="text-white hover:bg-white/20"
                  >
                    <QrCode className="h-4 w-4" />
                  </Button>
                </>
              )}
            </div>

            {isDismissible && (
              <Button
                variant="ghost"
                size="sm"
                className="text-white/80 hover:text-white hover:bg-white/20 flex-shrink-0"
                onClick={handleClose}
                aria-label="Close"
              >
                <X className="h-4 w-4" />
              </Button>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}