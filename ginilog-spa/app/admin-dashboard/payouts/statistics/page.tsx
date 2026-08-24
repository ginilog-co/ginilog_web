// app/admin-dashboard/payouts/statistics/page.tsx
"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { 
  Loader2, ArrowLeft, TrendingUp, DollarSign, 
  Users, Building2, Calendar, Download, PieChart,
  BarChart3, ArrowUpRight, ArrowDownRight
} from "lucide-react";
import { getPayoutStatistics } from "@/lib/api";

export default function PayoutsStatisticsPage() {
  const [stats, setStats] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        setIsLoading(true);
        const data = await getPayoutStatistics();
        setStats(data);
      } catch (error) {
        console.error("Failed to fetch statistics:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchStats();
  }, []);

  const formatCurrency = (amount: number) => {
    return new Intl.NumberFormat('en-NG', { style: 'currency', currency: 'NGN' }).format(amount || 0);
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/payouts">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Payout Statistics</h1>
          <p className="text-sm text-gray-500">Overview of all payout activity</p>
        </div>
        <Button variant="outline" className="ml-auto gap-2">
          <Download className="h-4 w-4" />
          Export Report
        </Button>
      </div>

      {/* Summary Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Payouts</p>
                <p className="text-2xl font-bold">{stats?.totalPayouts || 0}</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  12% from last month
                </div>
              </div>
              <div className="h-10 w-10 rounded-full bg-blue-100 text-blue-600 flex items-center justify-center">
                <DollarSign className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Total Amount</p>
                <p className="text-2xl font-bold">{formatCurrency(stats?.totalAmount || 0)}</p>
                <div className="flex items-center gap-1 text-xs text-green-600 mt-1">
                  <ArrowUpRight className="h-3 w-3" />
                  8.5% from last month
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
                <p className="text-sm text-gray-500">Rider Payouts</p>
                <p className="text-2xl font-bold">{stats?.riderPayouts || 0}</p>
                <p className="text-xs text-gray-400 mt-1">{formatCurrency(stats?.riderAmount || 0)}</p>
              </div>
              <div className="h-10 w-10 rounded-full bg-purple-100 text-purple-600 flex items-center justify-center">
                <Users className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Company Payouts</p>
                <p className="text-2xl font-bold">{stats?.companyPayouts || 0}</p>
                <p className="text-xs text-gray-400 mt-1">{formatCurrency(stats?.companyAmount || 0)}</p>
              </div>
              <div className="h-10 w-10 rounded-full bg-orange-100 text-orange-600 flex items-center justify-center">
                <Building2 className="h-5 w-5" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BarChart3 className="h-5 w-5 text-primary" />
              Payouts by Month
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-64 bg-gray-100 rounded-xl flex items-center justify-center text-gray-400">
              <div className="text-center">
                <BarChart3 className="h-12 w-12 mx-auto mb-2 text-gray-300" />
                <p>Monthly payout chart</p>
                <p className="text-xs">(Chart component integration required)</p>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <PieChart className="h-5 w-5 text-primary" />
              Payout Distribution
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-64 bg-gray-100 rounded-xl flex items-center justify-center text-gray-400">
              <div className="text-center">
                <PieChart className="h-12 w-12 mx-auto mb-2 text-gray-300" />
                <p>Payout distribution chart</p>
                <p className="text-xs">(Chart component integration required)</p>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Breakdown by Status */}
      <Card>
        <CardHeader>
          <CardTitle>Payouts by Status</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Completed</span>
                <span className="text-gray-500">{stats?.completedPayouts || 0}</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-green-500 h-2 rounded-full" style={{ width: `${(stats?.completedPayouts || 0) / (stats?.totalPayouts || 1) * 100}%` }}></div>
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Pending</span>
                <span className="text-gray-500">{stats?.pendingPayouts || 0}</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-yellow-500 h-2 rounded-full" style={{ width: `${(stats?.pendingPayouts || 0) / (stats?.totalPayouts || 1) * 100}%` }}></div>
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Processing</span>
                <span className="text-gray-500">{stats?.processingPayouts || 0}</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-blue-500 h-2 rounded-full" style={{ width: `${(stats?.processingPayouts || 0) / (stats?.totalPayouts || 1) * 100}%` }}></div>
              </div>
            </div>
            <div>
              <div className="flex items-center justify-between text-sm mb-1">
                <span className="font-medium">Failed</span>
                <span className="text-gray-500">{stats?.failedPayouts || 0}</span>
              </div>
              <div className="w-full bg-gray-200 rounded-full h-2">
                <div className="bg-red-500 h-2 rounded-full" style={{ width: `${(stats?.failedPayouts || 0) / (stats?.totalPayouts || 1) * 100}%` }}></div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}