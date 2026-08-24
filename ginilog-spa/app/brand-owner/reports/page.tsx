// app/brand-owner/reports/page.tsx
"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  BarChart3,
  TrendingUp,
  DollarSign,
  Hotel,
  Calendar,
  Users,
  Download,
  Filter,
  ArrowUpRight,
  ArrowDownRight,
  PieChart,
  LineChart
} from "lucide-react";

export default function BrandOwnerReportsPage() {
  const [dateRange, setDateRange] = useState("month");

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Reports & Analytics</h1>
          <p className="text-sm text-gray-500">Brand performance and analytics</p>
        </div>
        <div className="flex gap-2">
          <select
            className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
            value={dateRange}
            onChange={(e) => setDateRange(e.target.value)}
          >
            <option value="day">Today</option>
            <option value="week">This Week</option>
            <option value="month">This Month</option>
            <option value="quarter">This Quarter</option>
            <option value="year">This Year</option>
          </select>
          <Button variant="outline" className="gap-2">
            <Download className="h-4 w-4" />
            Export
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Revenue</p>
                <p className="text-2xl font-bold">₦2,450,000</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  12.5% from last month
                </div>
              </div>
              <div className="h-10 w-10 rounded-full bg-green-100 text-green-600 flex items-center justify-center">
                <TrendingUp className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Bookings</p>
                <p className="text-2xl font-bold">48</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  8.3% from last month
                </div>
              </div>
              <div className="h-10 w-10 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center">
                <Calendar className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Properties</p>
                <p className="text-2xl font-bold">12</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  2 new this month
                </div>
              </div>
              <div className="h-10 w-10 rounded-full bg-purple-100 text-purple-600 flex items-center justify-center">
                <Hotel className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Rating</p>
                <p className="text-2xl font-bold">4.8</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  0.3 increase
                </div>
              </div>
              <div className="h-10 w-10 rounded-full bg-yellow-100 text-yellow-600 flex items-center justify-center">
                <PieChart className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5 text-primary" />
              Revenue Overview
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-64 bg-gray-100 rounded-xl flex items-center justify-center text-gray-400">
              <div className="text-center">
                <BarChart3 className="h-12 w-12 mx-auto mb-2 text-gray-300" />
                <p>Revenue chart will be displayed here</p>
                <p className="text-xs">(Chart component integration required)</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <PieChart className="h-5 w-5 text-primary" />
              Booking Distribution
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-64 bg-gray-100 rounded-xl flex items-center justify-center text-gray-400">
              <div className="text-center">
                <PieChart className="h-12 w-12 mx-auto mb-2 text-gray-300" />
                <p>Booking distribution chart</p>
                <p className="text-xs">(Chart component integration required)</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Top Performing Properties</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Luxury Villa</span>
                <span className="text-gray-500">₦1,245,000</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-blue-500 h-2 rounded-full" style={{ width: "78%" }}></div>
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">City Apartment</span>
                <span className="text-gray-500">₦875,000</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-green-500 h-2 rounded-full" style={{ width: "55%" }}></div>
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Beach Resort</span>
                <span className="text-gray-500">₦330,000</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-purple-500 h-2 rounded-full" style={{ width: "35%" }}></div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}