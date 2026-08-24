"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { ArrowLeft, CreditCard, DollarSign, TrendingUp, Wallet } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { getPayouts, isAuthenticated, validateSession, clearAuthData } from "@/lib/api";

export default function BrandOwnerPaymentsPage() {
  const router = useRouter();
  const [payouts, setPayouts] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!isAuthenticated() || !validateSession()) {
      clearAuthData();
      router.replace("/brand-owner/login");
      return;
    }

    const fetchPayouts = async () => {
      try {
        const data = await getPayouts();
        setPayouts(data || []);
      } catch (error) {
        console.error("Failed to fetch payouts:", error);
        setPayouts([]);
      } finally {
        setLoading(false);
      }
    };

    fetchPayouts();
  }, [router]);

  const formatCurrency = (value: number) => `₦${(Number(value) || 0).toLocaleString("en-NG")}`;
  const completedTotal = payouts
    .filter((item) => (item.status || "").toLowerCase() === "completed")
    .reduce((sum, item) => sum + Number(item.amount || item.totalAmount || 0), 0);
  const pendingTotal = payouts
    .filter((item) => ["pending", "processing"].includes((item.status || "").toLowerCase()))
    .reduce((sum, item) => sum + Number(item.amount || item.totalAmount || 0), 0);
  const thisMonthTotal = payouts
    .filter((item) => {
      const createdAt = item.createdAt || item.date;
      if (!createdAt) return false;
      const date = new Date(createdAt);
      return !Number.isNaN(date.getTime()) && date.getMonth() === new Date().getMonth() && date.getFullYear() === new Date().getFullYear();
    })
    .reduce((sum, item) => sum + Number(item.amount || item.totalAmount || 0), 0);

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between gap-4">
        <div className="flex items-center gap-4">
          <Link href="/brand-owner">
            <Button variant="ghost" size="sm" className="gap-2">
              <ArrowLeft className="h-4 w-4" />
              Back
            </Button>
          </Link>
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Payments</h1>
            <p className="text-sm text-gray-500">Track payouts, wallet balance, and payment history</p>
          </div>
        </div>
        <Link href="/brand-owner/payments/initiate">
          <Button className="gap-2">Initiate payment</Button>
        </Link>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Wallet Balance</p>
                <p className="mt-2 text-2xl font-bold">{formatCurrency(completedTotal)}</p>
              </div>
              <div className="rounded-xl bg-green-100 p-3 text-green-700">
                <Wallet className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">This Month</p>
                <p className="mt-2 text-2xl font-bold">{formatCurrency(thisMonthTotal)}</p>
              </div>
              <div className="rounded-xl bg-blue-100 p-3 text-blue-700">
                <TrendingUp className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-500">Pending Payouts</p>
                <p className="mt-2 text-2xl font-bold">{formatCurrency(pendingTotal)}</p>
              </div>
              <div className="rounded-xl bg-amber-100 p-3 text-amber-700">
                <DollarSign className="h-6 w-6" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5 text-primary" />
            Recent payouts
          </CardTitle>
        </CardHeader>
        <CardContent className="p-0">
          <div className="overflow-x-auto">
            {loading ? (
              <div className="p-6 text-sm text-gray-500">Loading payouts...</div>
            ) : payouts.length === 0 ? (
              <div className="p-6 text-sm text-gray-500">No payout records found.</div>
            ) : (
              <table className="w-full text-sm">
                <thead className="bg-gray-50 text-left">
                  <tr>
                    <th className="px-4 py-3 font-medium text-gray-600">Reference</th>
                    <th className="px-4 py-3 font-medium text-gray-600">Amount</th>
                    <th className="px-4 py-3 font-medium text-gray-600">Status</th>
                    <th className="px-4 py-3 font-medium text-gray-600">Date</th>
                  </tr>
                </thead>
                <tbody>
                  {payouts.map((item, index) => {
                    const status = String(item.status || "Pending");
                    const amount = Number(item.amount || item.totalAmount || 0);
                    const date = item.createdAt || item.date || new Date().toISOString();
                    return (
                      <tr key={item.id || `${status}-${index}`} className="border-t">
                        <td className="px-4 py-3 font-medium">{item.id || `P-${index + 1}`}</td>
                        <td className="px-4 py-3">{formatCurrency(amount)}</td>
                        <td className="px-4 py-3">
                          <Badge
                            className={
                              status.toLowerCase() === "completed"
                                ? "bg-green-100 text-green-700"
                                : status.toLowerCase() === "pending"
                                  ? "bg-yellow-100 text-yellow-700"
                                  : "bg-blue-100 text-blue-700"
                            }
                          >
                            {status}
                          </Badge>
                        </td>
                        <td className="px-4 py-3">{new Date(date).toLocaleDateString("en-GB", { day: "2-digit", month: "short", year: "numeric" })}</td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            )}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
