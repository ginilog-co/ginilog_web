"use client";

import { useState } from "react";
import Link from "next/link";
import { ArrowLeft, CreditCard, CheckCircle, AlertCircle } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export default function BrandOwnerPaymentInitiatePage() {
  const [amount, setAmount] = useState("50000");
  const [loading, setLoading] = useState(false);
  const [status, setStatus] = useState<"idle" | "success" | "error">("idle");

  const handleSubmit = async () => {
    setLoading(true);
    setStatus("idle");

    await new Promise((resolve) => setTimeout(resolve, 800));

    setLoading(false);
    setStatus("success");
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/payments">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Initiate Payment</h1>
          <p className="text-sm text-gray-500">Create a withdrawal or payout request</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <CreditCard className="h-5 w-5 text-primary" />
            Payout request
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-5">
          <div className="space-y-2">
            <Label htmlFor="amount">Amount</Label>
            <Input
              id="amount"
              type="number"
              value={amount}
              onChange={(e) => setAmount(e.target.value)}
              placeholder="50000"
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="account">Bank account</Label>
            <Input id="account" defaultValue="Sterling Bank • 0000000000" />
          </div>

          <Button onClick={handleSubmit} disabled={loading} className="w-full">
            {loading ? "Submitting..." : "Submit payout request"}
          </Button>

          {status === "success" && (
            <div className="flex items-center gap-2 rounded-xl border border-green-200 bg-green-50 p-3 text-sm text-green-700">
              <CheckCircle className="h-4 w-4" />
              Payment request submitted successfully.
            </div>
          )}

          {status === "error" && (
            <div className="flex items-center gap-2 rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-700">
              <AlertCircle className="h-4 w-4" />
              We could not process this payment request.
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
