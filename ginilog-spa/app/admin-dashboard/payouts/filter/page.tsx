// app/admin-dashboard/payouts/filter/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Filter, DollarSign, Calendar, Users, Building2 } from "lucide-react";

export default function PayoutsFilterPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [filters, setFilters] = useState({
    state: "",
    city: "",
    startDate: "",
    endDate: "",
    recipientType: "",
    status: "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    
    // Build query string
    const params = new URLSearchParams();
    Object.entries(filters).forEach(([key, value]) => {
      if (value) params.append(key, value);
    });
    
    router.push(`/admin-dashboard/payouts?${params.toString()}`);
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/payouts">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Filter Payouts</h1>
          <p className="text-sm text-gray-500">Refine your payout search</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5 text-primary" />
            Filter Criteria
          </CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="state">State</Label>
                <Input
                  id="state"
                  value={filters.state}
                  onChange={(e) => setFilters({ ...filters, state: e.target.value })}
                  placeholder="e.g., Lagos"
                />
              </div>
              <div>
                <Label htmlFor="city">City</Label>
                <Input
                  id="city"
                  value={filters.city}
                  onChange={(e) => setFilters({ ...filters, city: e.target.value })}
                  placeholder="e.g., Ikeja"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="startDate">Start Date</Label>
                <Input
                  id="startDate"
                  type="date"
                  value={filters.startDate}
                  onChange={(e) => setFilters({ ...filters, startDate: e.target.value })}
                />
              </div>
              <div>
                <Label htmlFor="endDate">End Date</Label>
                <Input
                  id="endDate"
                  type="date"
                  value={filters.endDate}
                  onChange={(e) => setFilters({ ...filters, endDate: e.target.value })}
                />
              </div>
            </div>

            <div>
              <Label htmlFor="recipientType">Recipient Type</Label>
              <select
                id="recipientType"
                className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={filters.recipientType}
                onChange={(e) => setFilters({ ...filters, recipientType: e.target.value })}
              >
                <option value="">All Types</option>
                <option value="rider">Rider</option>
                <option value="company">Company</option>
                <option value="gas-station">Gas Station</option>
              </select>
            </div>

            <div>
              <Label htmlFor="status">Status</Label>
              <select
                id="status"
                className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={filters.status}
                onChange={(e) => setFilters({ ...filters, status: e.target.value })}
              >
                <option value="">All Status</option>
                <option value="pending">Pending</option>
                <option value="completed">Completed</option>
                <option value="failed">Failed</option>
                <option value="processing">Processing</option>
              </select>
            </div>

            <div className="flex gap-3 pt-4">
              <Button type="submit" disabled={isLoading} className="flex-1">
                {isLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : <Filter className="h-4 w-4 mr-2" />}
                {isLoading ? "Filtering..." : "Apply Filters"}
              </Button>
              <Button 
                type="button" 
                variant="outline" 
                onClick={() => setFilters({
                  state: "",
                  city: "",
                  startDate: "",
                  endDate: "",
                  recipientType: "",
                  status: "",
                })}
              >
                Clear All
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}