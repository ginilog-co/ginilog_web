"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Eye, EyeOff, Mail, Lock, Shield, Loader2 } from "lucide-react";
import { adminLogin, LoginRequest } from "@/lib/api";

export default function AdminAuth() {
  const router = useRouter();
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [formData, setFormData] = useState({ email: "", password: "" });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      const credentials: LoginRequest = { Email_PhoneNo: formData.email, Password: formData.password };
      const data = await adminLogin(credentials);
      if (data.userType === "Super_Admin") {
        router.push("/admin-dashboard");
      } else {
        router.push("/admin-dashboard/company");
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Authentication failed. Check your credentials.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex">
      {/* Brand Panel - Desktop Only */}
      <aside className="hidden lg:flex lg:w-1/2 bg-gray-900 relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-gray-900 to-gray-800" />
        <div className="relative z-10 flex flex-col justify-center items-center w-full px-12 text-white">
          <div className="p-4 bg-primary/20 rounded-xl mb-6">
            <Shield className="h-12 w-12 text-primary" />
          </div>
          <h1 className="text-4xl font-bold mb-4">GINILOG</h1>
          <p className="text-xl text-center mb-6">Admin & Company Portal</p>
          <p className="text-white/80 text-center max-w-md">
            Sign in to access the admin dashboard and manage orders, bookings, and logistics operations.
          </p>
          <div className="mt-12 flex gap-8">
            <div className="text-center">
              <div className="text-3xl font-bold">10K+</div>
              <div className="text-white/70 text-sm">Orders Managed</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">500+</div>
              <div className="text-white/70 text-sm">Properties Listed</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">50+</div>
              <div className="text-white/70 text-sm">Logistics Partners</div>
            </div>
          </div>
        </div>
      </aside>

      {/* Form Panel */}
      <main className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-white py-8">
        <div className="w-full max-w-md">
          <div className="lg:hidden text-center mb-8">
            <div className="inline-flex items-center justify-center p-3 bg-primary/10 rounded-xl mb-4">
              <Shield className="h-8 w-8 text-primary" />
            </div>
            <h1 className="text-2xl font-bold text-gray-900">GINILOG</h1>
            <p className="text-gray-600 mt-2">Admin & Company Portal</p>
          </div>

          <div className="bg-white rounded-lg">
            <div className="border-b border-gray-200 mb-6">
              <div className="py-3 text-center font-medium text-primary border-b-2 border-primary">
                Sign In
              </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              {error && (
                <div className="p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm">
                  {error}
                </div>
              )}

              <div>
                <Label htmlFor="email" className="text-gray-700">Email</Label>
                <div className="relative mt-1">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    id="email"
                    type="email"
                    placeholder="admin@ginilog.com"
                    className="pl-10 h-12"
                    value={formData.email}
                    onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                    required
                  />
                </div>
              </div>

              <div>
                <Label htmlFor="password" className="text-gray-700">Password</Label>
                <div className="relative mt-1">
                  <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    id="password"
                    type={showPassword ? "text" : "password"}
                    placeholder="••••••••"
                    className="pl-10 pr-10 h-12"
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    required
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  >
                    {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                  </button>
                </div>
              </div>

              <div className="flex items-center justify-end">
                <Link
                  href="/admin-dashboard/forgot-password"
                  className="text-sm text-primary hover:text-primary/80"
                >
                  Forgot password?
                </Link>
              </div>

              <Button
                type="submit"
                disabled={isLoading}
                className="w-full h-12 bg-gray-900 hover:bg-gray-800 text-white font-medium"
              >
                {isLoading && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
                {isLoading ? "Signing In..." : "Sign In"}
              </Button>
            </form>

            

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/" className="hover:text-gray-700">← Back to Home</Link>
              {" · "}
              <Link href="/customer-portal/login" className="hover:text-primary">Customer Login</Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
