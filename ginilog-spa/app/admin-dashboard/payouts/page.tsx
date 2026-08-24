// app/admin-dashboard/payouts/page.tsx
"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { 
  Search, Loader2, DollarSign, Calendar, Filter, 
  Download, Eye, TrendingUp, Users, Building2,
  ChevronDown, ArrowUpRight, ArrowDownRight
} from "lucide-react";
import { getPayouts, getPayoutStatistics } from "@/lib/api";

export default function PayoutsPage() {
  const [payouts, setPayouts] = useState<any[]>([]);
  const [filteredPayouts, setFilteredPayouts] = useState<any[]>([]);
  const [stats, setStats] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");

  useEffect(() => {
    fetchData();
  }, []);

  useEffect(() => {
    let filtered = payouts;
    
    if (search.trim()) {
      filtered = filtered.filter((p) =>
        p.transactionReference?.toLowerCase().includes(search.toLowerCase()) ||
        p.recipientType?.toLowerCase().includes(search.toLowerCase()) ||
        p.status?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter !== "all") {
      filtered = filtered.filter((p) => 
        p.status?.toLowerCase() === filter.toLowerCase()
      );
    }
    
    setFilteredPayouts(filtered);
  }, [search, filter, payouts]);

  const fetchData = async () => {
    try {
      setIsLoading(true);
      const [payoutsData, statsData] = await Promise.all([
        getPayouts(),
        getPayoutStatistics(),
      ]);
      setPayouts(payoutsData || []);
      setFilteredPayouts(payoutsData || []);
      setStats(statsData);
    } catch (error) {
      console.error("Failed to fetch payouts:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string }> = {
      completed: { label: "Completed", className: "bg-green-100 text-green-800" },
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800" },
      failed: { label: "Failed", className: "bg-red-100 text-red-800" },
      processing: { label: "Processing", className: "bg-blue-100 text-blue-800" },
    };
    const s = status?.toLowerCase() || "pending";
    return statusMap[s] || statusMap.pending;
  };

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
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Payouts</h1>
          <p className="text-sm text-gray-500">Manage and track all payouts</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" className="gap-2">
            <Download className="h-4 w-4" />
            Export
          </Button>
          <Button className="gap-2">
            <DollarSign className="h-4 w-4" />
            New Payout
          </Button>
        </div>
      </div>

      {/* Stats Cards */}
      {stats && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <Card>
            <CardContent className="p-4">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm text-gray-500">Total Payouts</p>
                  <p className="text-2xl font-bold">{stats.totalPayouts || 0}</p>
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
                  <p className="text-2xl font-bold">{formatCurrency(stats.totalAmount || 0)}</p>
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
                  <p className="text-sm text-gray-500">Riders</p>
                  <p className="text-2xl font-bold">{stats.riderPayouts || 0}</p>
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
                  <p className="text-sm text-gray-500">Companies</p>
                  <p className="text-2xl font-bold">{stats.companyPayouts || 0}</p>
                </div>
                <div className="h-10 w-10 rounded-full bg-orange-100 text-orange-600 flex items-center justify-center">
                  <Building2 className="h-5 w-5" />
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Payouts Table */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Payouts ({filteredPayouts.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search payouts..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
            <div className="flex gap-2">
              <select
                className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
              >
                <option value="all">All Status</option>
                <option value="completed">Completed</option>
                <option value="pending">Pending</option>
                <option value="processing">Processing</option>
                <option value="failed">Failed</option>
              </select>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredPayouts.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              <DollarSign className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>{search || filter !== "all" ? "No payouts match your filters" : "No payouts found"}</p>
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-gray-50">
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Reference</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Recipient</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Amount</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Type</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">Date</th>
                    <th className="text-right py-3 px-4 font-medium text-gray-600">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredPayouts.map((payout) => {
                    const status = getStatusBadge(payout.status);
                    return (
                      <tr key={payout.id} className="border-b hover:bg-gray-50">
                        <td className="py-3 px-4 font-mono text-xs">
                          {payout.transactionReference || payout.id.slice(0, 12)}
                        </td>
                        <td className="py-3 px-4">
                          <div>
                            <p className="font-medium">{payout.recipientName || "N/A"}</p>
                            <p className="text-xs text-gray-500">{payout.recipientEmail || "N/A"}</p>
                          </div>
                        </td>
                        <td className="py-3 px-4 font-medium">
                          {formatCurrency(payout.amount)}
                        </td>
                        <td className="py-3 px-4">
                          <Badge variant="secondary" className="capitalize">
                            {payout.recipientType || "N/A"}
                          </Badge>
                        </td>
                        <td className="py-3 px-4">
                          <Badge className={status.className}>
                            {status.label}
                          </Badge>
                        </td>
                        <td className="py-3 px-4 text-gray-500">
                          {payout.createdAt ? new Date(payout.createdAt).toLocaleDateString() : "N/A"}
                        </td>
                        <td className="py-3 px-4 text-right">
                          <Button size="sm" variant="ghost" className="h-8 w-8 p-0">
                            <Eye className="h-4 w-4" />
                          </Button>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}