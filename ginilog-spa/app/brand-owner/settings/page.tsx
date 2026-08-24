// app/brand-owner/settings/page.tsx
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
  Save,
  Loader2,
  CheckCircle
} from "lucide-react";

export default function BrandOwnerSettingsPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [saved, setSaved] = useState(false);

  const handleSave = async () => {
    setIsLoading(true);
    setSaved(false);
    await new Promise(resolve => setTimeout(resolve, 1500));
    setIsLoading(false);
    setSaved(true);
    setTimeout(() => setSaved(false), 3000);
  };

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Settings</h1>
        <p className="text-sm text-gray-500">Manage your brand settings and preferences</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
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

        <div className="lg:col-span-3 space-y-6">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Settings className="h-5 w-5 text-primary" />
                General Settings
              </CardTitle>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <Label htmlFor="brandName">Brand Name</Label>
                <Input id="brandName" defaultValue="Jopachi Group" />
              </div>
              <div>
                <Label htmlFor="brandDescription">Brand Description</Label>
                <Input id="brandDescription" defaultValue="Luxury accommodations and logistics services" />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="contactEmail">Contact Email</Label>
                  <Input id="contactEmail" defaultValue="info@jopachi.com" type="email" />
                </div>
                <div>
                  <Label htmlFor="contactPhone">Contact Phone</Label>
                  <Input id="contactPhone" defaultValue="+234 800 123 4567" />
                </div>
              </div>
            </CardContent>
          </Card>

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
                  <p className="text-sm text-gray-500">Add an extra layer of security</p>
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
            </CardContent>
          </Card>

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