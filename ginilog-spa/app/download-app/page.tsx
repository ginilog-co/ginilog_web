"use client";

import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import {
  Smartphone,
  Apple,
  PlayCircle,
  QrCode,
  ArrowLeft,
  Star,
  Zap,
  Shield,
  Clock,
  CheckCircle,
} from "lucide-react";

export default function DownloadAppPage() {
  const features = [
    {
      icon: Zap,
      title: "Real-time Tracking",
      description: "Track your packages and orders in real-time",
    },
    {
      icon: Shield,
      title: "Secure Payments",
      description: "Pay safely with multiple payment options",
    },
    {
      icon: Clock,
      title: "24/7 Support",
      description: "Get help whenever you need it",
    },
    {
      icon: Star,
      title: "Easy Booking",
      description: "Book accommodations with just a few taps",
    },
  ];

  return (
    <div className="min-h-screen bg-gradient-to-b from-gray-50 to-white">
      <div className="container mx-auto px-4 py-8 max-w-4xl">
        {/* Back Button */}
        <Link href="/customer-portal/dashboard">
          <Button variant="ghost" className="mb-6">
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Dashboard
          </Button>
        </Link>

        {/* Hero Section */}
        <div className="text-center mb-12">
          <div className="inline-flex items-center gap-2 bg-primary/10 text-primary px-4 py-2 rounded-full mb-4">
            <Smartphone className="h-4 w-4" />
            <span className="text-sm font-medium">Mobile App</span>
          </div>
          <h1 className="text-4xl md:text-5xl font-bold text-gray-900 mb-4">
            Get the GINILOG App
          </h1>
          <p className="text-xl text-gray-600 max-w-2xl mx-auto">
            Track packages, book accommodations, and manage everything on the go
          </p>
        </div>

        {/* Download Options */}
        <div className="grid md:grid-cols-2 gap-6 mb-12">
          <Card className="border-2 hover:border-primary/50 transition-all hover:shadow-lg">
            <CardContent className="p-6 text-center">
              <div className="h-16 w-16 rounded-full bg-green-100 flex items-center justify-center mx-auto mb-4">
                <PlayCircle className="h-8 w-8 text-green-600" />
              </div>
              <h3 className="text-xl font-semibold mb-2">Android App</h3>
              <p className="text-gray-600 text-sm mb-4">
                Available on Google Play Store
              </p>
              <Button
                className="w-full bg-gradient-to-r from-green-600 to-green-700 hover:from-green-700 hover:to-green-800"
                onClick={() =>
                  window.open(
                    "https://play.google.com/store/apps/details?id=com.app.ginilog_customer_app",
                    "_blank"
                  )
                }
              >
                <PlayCircle className="h-5 w-5 mr-2" />
                Download from Play Store
              </Button>
            </CardContent>
          </Card>

          <Card className="border-2 hover:border-primary/50 transition-all hover:shadow-lg">
            <CardContent className="p-6 text-center">
              <div className="h-16 w-16 rounded-full bg-gray-100 flex items-center justify-center mx-auto mb-4">
                <Apple className="h-8 w-8 text-gray-700" />
              </div>
              <h3 className="text-xl font-semibold mb-2">iOS App</h3>
              <p className="text-gray-600 text-sm mb-4">
                Available on the App Store
              </p>
              <Button
                className="w-full bg-gray-900 hover:bg-gray-800"
                onClick={() =>
                  window.open("https://apps.apple.com/ng/app/ginilog/id6751465248", "_blank")
                }
              >
                <Apple className="h-5 w-5 mr-2" />
                Download from App Store
              </Button>
            </CardContent>
          </Card>
        </div>

        {/* Features */}
        <div className="mb-12">
          <h2 className="text-2xl font-bold text-center mb-8">
            Why Get the App?
          </h2>
          <div className="grid md:grid-cols-4 gap-6">
            {features.map((feature, index) => (
              <div key={index} className="text-center">
                <div className="h-12 w-12 rounded-full bg-primary/10 flex items-center justify-center mx-auto mb-3">
                  <feature.icon className="h-6 w-6 text-primary" />
                </div>
                <h4 className="font-semibold text-gray-900">{feature.title}</h4>
                <p className="text-sm text-gray-600 mt-1">{feature.description}</p>
              </div>
            ))}
          </div>
        </div>

        {/* QR Code Section */}
        <Card className="bg-primary/5 border-primary/20">
          <CardContent className="p-6 text-center">
            <div className="flex flex-col md:flex-row items-center justify-center gap-6">
              <div className="bg-white p-4 rounded-xl shadow-md">
                <div className="w-32 h-32 bg-gray-100 flex items-center justify-center">
                  <QrCode className="h-16 w-16 text-gray-400" />
                </div>
              </div>
              <div className="text-left">
                <h3 className="text-lg font-semibold">Scan to Download</h3>
                <p className="text-gray-600 text-sm max-w-md">
                  Scan this QR code with your phone camera to download the app instantly
                </p>
                <p className="text-xs text-gray-500 mt-2">
                  Or visit: <span className="text-primary">ginilog.com/download</span>
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}