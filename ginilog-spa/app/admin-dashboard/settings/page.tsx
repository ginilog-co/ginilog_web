// app/admin-dashboard/settings/page.tsx
"use client";

import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Settings,
  Shield,
  Bell,
  Mail,
  Globe,
  Lock,
  Database,
  Users,
  Building2,
  DollarSign,
  Save,
  Loader2,
  AlertTriangle,
  CheckCircle
} from "lucide-react";

export default function SettingsPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [saved, setSaved] = useState(false);

  const handleSave = async () => {
    setIsLoading(true);
    setSaved(false);
    // Simulate save
    await new Promise(resolve => setTimeout(resolve, 1500));
    setIsLoading(false);
    setSaved(true);
    setTimeout(() => setSaved(false), 3000);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Settings</h1>
        <p className="text-sm text-gray-500">Manage platform settings and configurations</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
        {/* Sidebar */}
        <Card className="lg:col-span-1">
          <CardContent className="p-4 space-y-1">
            <button className="w-full text-left px-3 py-2 rounded-lg bg-primary/10 text-primary font-medium">
              <Settings className="h-4 w-4 inline mr-2" />
              General
            </button>
            <button className="w-full text-left px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700">
              <Shield className="h-4 w-4 inline mr-2" />
              Security
            </button>
            <button className="w-full text-left px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700">
              <Bell className="h-4 w-4 inline mr-2" />
              Notifications
            </button>
            <button className="w-full text-left px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700">
              <Mail className="h-4 w-4 inline mr-2" />
              Email
            </button>
            <button className="w-full text-left px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700">
              <Globe className="h-4 w-4 inline mr-2" />
              Localization
            </button>
            <button className="w-full text-left px-3 py-2 rounded-lg hover:bg-gray-100 text-gray-700">
              <Lock className="h-4 w-4 inline mr-2" />
              Privacy
            </button>
          </CardContent>
        </Card>

        {/* Main Settings */}
        <div className="lg:col-span-3 space-y-6">
          {/* General Settings */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Settings className="h-5 w-5 text-primary" />
                General Settings
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="siteName">Site Name</Label>
                <Input id="siteName" defaultValue="Ginilog" />
              </div>
              <div>
                <Label htmlFor="siteDescription">Site Description</Label>
                <Input id="siteDescription" defaultValue="Logistics & Accommodation Platform" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="supportEmail">Support Email</Label>
                  <Input id="supportEmail" defaultValue="support@ginilog.com" type="email" />
                </div>
                <div>
                  <Label htmlFor="supportPhone">Support Phone</Label>
                  <Input id="supportPhone" defaultValue="+234 800 123 4567" />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Security Settings */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Shield className="h-5 w-5 text-primary" />
                Security Settings
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-gray-900">Two-Factor Authentication</p>
                  <p className="text-sm text-gray-500">Require 2FA for all admin accounts</p>
                </div>
                <input type="checkbox" defaultChecked className="h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary" />
              </div>
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-gray-900">Session Timeout</p>
                  <p className="text-sm text-gray-500">Auto-logout after inactivity</p>
                </div>
                <select className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20">
                  <option value="15">15 minutes</option>
                  <option value="30">30 minutes</option>
                  <option value="60" selected>1 hour</option>
                  <option value="120">2 hours</option>
                </select>
              </div>
              <div className="flex items-center justify-between">
                <div>
                  <p className="font-medium text-gray-900">Password Policy</p>
                  <p className="text-sm text-gray-500">Enforce strong password requirements</p>
                </div>
                <input type="checkbox" defaultChecked className="h-5 w-5 rounded border-gray-300 text-primary focus:ring-primary" />
              </div>
            </CardContent>
          </Card>

          {/* Save Button */}
          <div className="flex items-center justify-between">
            {saved && (
              <div className="flex items-center gap-2 text-sm text-green-600">
                <CheckCircle className="h-4 w-4" />
                Settings saved successfully!
              </div>
            )}
            <Button onClick={handleSave} disabled={isLoading} className="ml-auto gap-2">
              {isLoading ? <Loader2 className="h-4 w-4 animate-spin" /> : <Save className="h-4 w-4" />}
              {isLoading ? "Saving..." : "Save Settings"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}