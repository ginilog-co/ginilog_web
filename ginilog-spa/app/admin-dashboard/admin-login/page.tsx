"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Eye, EyeOff, Mail, Lock, User, Shield } from "lucide-react";

type AuthMode = "signin" | "signup";

export default function AdminAuth() {
  const router = useRouter();
  const [mode, setMode] = useState<AuthMode>("signin");
  const [showPassword, setShowPassword] = useState(false);
  const [formData, setFormData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    adminCode: "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    console.log("Admin auth submitted:", { mode, ...formData });
    // Simulate successful login/signup and redirect to admin dashboard
    router.push("/admin-dashboard");
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
          <h1 className="text-4xl font-bold mb-4">GINILOG Admin</h1>
          <p className="text-xl text-center mb-6">
            {mode === "signin" ? "Welcome Back" : "Join Our Team"}
          </p>
          <p className="text-white/80 text-center max-w-md">
            {mode === "signin" 
              ? "Sign in to access the admin dashboard and manage orders, bookings, and users."
              : "Create an admin account to help manage GINILOG operations and serve our customers better."}
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
          {/* Mobile Header */}
          <div className="lg:hidden text-center mb-8">
            <div className="inline-flex items-center justify-center p-3 bg-primary/10 rounded-xl mb-4">
              <Shield className="h-8 w-8 text-primary" />
            </div>
            <h1 className="text-2xl font-bold text-gray-900">GINILOG Admin</h1>
            <p className="text-gray-600 mt-2">
              {mode === "signin" ? "Sign in to continue" : "Create admin account"}
            </p>
          </div>

          <div className="bg-white rounded-lg">
            {/* Tabs */}
            <div className="border-b border-gray-200 mb-6">
              <div className="flex">
                <button
                  onClick={() => setMode("signin")}
                  className={`flex-1 py-3 text-center font-medium transition-colors ${
                    mode === "signin"
                      ? "text-primary border-b-2 border-primary"
                      : "text-gray-500 hover:text-gray-700"
                  }`}
                >
                  Sign In
                </button>
                <button
                  onClick={() => setMode("signup")}
                  className={`flex-1 py-3 text-center font-medium transition-colors ${
                    mode === "signup"
                      ? "text-primary border-b-2 border-primary"
                      : "text-gray-500 hover:text-gray-700"
                  }`}
                >
                  Sign Up
                </button>
              </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              {mode === "signup" && (
                <>
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label htmlFor="firstName" className="text-gray-700">First Name</Label>
                      <div className="relative mt-1">
                        <User className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                        <Input
                          id="firstName"
                          type="text"
                          placeholder="John"
                          className="pl-10 h-12"
                          value={formData.firstName}
                          onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                          required
                        />
                      </div>
                    </div>
                    <div>
                      <Label htmlFor="lastName" className="text-gray-700">Last Name</Label>
                      <div className="relative mt-1">
                        <User className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                        <Input
                          id="lastName"
                          type="text"
                          placeholder="Doe"
                          className="pl-10 h-12"
                          value={formData.lastName}
                          onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                          required
                        />
                      </div>
                    </div>
                  </div>

                  <div>
                    <Label htmlFor="adminCode" className="text-gray-700">Admin Access Code</Label>
                    <div className="relative mt-1">
                      <Shield className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                      <Input
                        id="adminCode"
                        type="password"
                        placeholder="Enter admin access code"
                        className="pl-10 h-12"
                        value={formData.adminCode}
                        onChange={(e) => setFormData({ ...formData, adminCode: e.target.value })}
                        required
                      />
                    </div>
                    <p className="text-xs text-gray-500 mt-1">
                      Contact super admin to get your access code
                    </p>
                  </div>
                </>
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

              {mode === "signup" && (
                <div>
                  <Label htmlFor="confirmPassword" className="text-gray-700">Confirm Password</Label>
                  <div className="relative mt-1">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="confirmPassword"
                      type={showPassword ? "text" : "password"}
                      placeholder="••••••••"
                      className="pl-10 h-12"
                      value={formData.confirmPassword}
                      onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                      required
                    />
                  </div>
                </div>
              )}

              {mode === "signin" && (
                <div className="flex items-center justify-between">
                  <div className="flex items-center">
                    <input
                      type="checkbox"
                      id="remember"
                      className="h-4 w-4 text-primary border-gray-300 rounded focus:ring-primary"
                    />
                    <Label htmlFor="remember" className="ml-2 text-sm text-gray-600 font-normal">
                      Remember me
                    </Label>
                  </div>
                  <Link 
                    href="/forgot-password" 
                    className="text-sm text-primary hover:text-primary/80"
                  >
                    Forgot password?
                  </Link>
                </div>
              )}

              {mode === "signup" && (
                <div className="flex items-start gap-2">
                  <input
                    type="checkbox"
                    id="terms"
                    className="mt-1 h-4 w-4 text-primary border-gray-300 rounded focus:ring-primary"
                    required
                  />
                  <Label htmlFor="terms" className="text-sm text-gray-600 font-normal">
                    I agree to the{" "}
                    <Link href="/terms" className="text-primary hover:text-primary/80">
                      Admin Terms of Service
                    </Link>{" "}
                    and have authorization to access this system
                  </Label>
                </div>
              )}

              <Button 
                type="submit" 
                className="w-full h-12 bg-gray-900 hover:bg-gray-800 text-white font-medium"
              >
                {mode === "signin" ? "Sign In" : "Create Admin Account"}
              </Button>
            </form>

            <div className="mt-6">
              <div className="relative">
                <div className="absolute inset-0 flex items-center">
                  <div className="w-full border-t border-gray-200" />
                </div>
                <div className="relative flex justify-center text-sm">
                  <span className="px-2 bg-white text-gray-500">or continue with</span>
                </div>
              </div>

              <div className="mt-4 grid grid-cols-2 gap-3">
                <button
                  type="button"
                  className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  <svg className="h-5 w-5 mr-2" viewBox="0 0 24 24">
                    <path
                      d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92a5.06 5.06 0 0 1-2.2 3.32v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.1z"
                      fill="#4285F4"
                    />
                    <path
                      d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"
                      fill="#34A853"
                    />
                    <path
                      d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"
                      fill="#FBBC05"
                    />
                    <path
                      d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"
                      fill="#EA4335"
                    />
                  </svg>
                  Google
                </button>
                <button
                  type="button"
                  className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50"
                >
                  <svg className="h-5 w-5 mr-2" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M16.365 1.43c0 1.14-.42 2.25-1.13 3.06-.75.87-1.98 1.54-3.08 1.45-.14-1.08.39-2.23 1.1-3 .78-.86 2.1-1.49 3.11-1.51.03.14.05.29.05.45zm3.36 16.09c-.84 1.22-1.72 2.43-3.08 2.46-1.33.03-1.76-.8-3.29-.8-1.52 0-2 .77-3.26.83-1.33.05-2.35-1.33-3.2-2.55-1.75-2.54-3.08-7.18-1.29-10.31.89-1.55 2.48-2.54 4.2-2.57 1.31-.03 2.55.89 3.29.89.73 0 2.1-1.1 3.54-.94.6.03 2.29.24 3.38 1.84-.09.05-2.02 1.18-2 3.52.02 2.79 2.44 3.72 2.47 3.73-.02.06-.39 1.34-1.26 2.4z" />
                  </svg>
                  Apple
                </button>
              </div>
            </div>

            <p className="mt-6 text-center text-sm text-gray-600">
              {mode === "signin" ? (
                <>
                  Don&apos;t have an admin account?{" "}
                  <button 
                    onClick={() => setMode("signup")}
                    className="text-primary hover:text-primary/80 font-medium"
                  >
                    Sign up
                  </button>
                </>
              ) : (
                <>
                  Already have an admin account?{" "}
                  <button 
                    onClick={() => setMode("signin")}
                    className="text-primary hover:text-primary/80 font-medium"
                  >
                    Sign in
                  </button>
                </>
              )}
            </p>

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/" className="hover:text-gray-700">
                ← Back to Home
              </Link>
            </p>

            <p className="mt-2 text-center text-xs text-gray-400">
              <Link href="/customer-portal/login" className="hover:text-primary">
                Customer Login →
              </Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
