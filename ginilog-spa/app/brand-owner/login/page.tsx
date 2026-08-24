// app/brand-owner/login/page.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, Mail, Lock, Eye, EyeOff, Building2, AlertCircle, CheckCircle } from "lucide-react";
import { brandOwnerLogin, getStoredUser, isAuthenticated, clearAuthData } from "@/lib/api";

export default function BrandOwnerLoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isCheckingAuth, setIsCheckingAuth] = useState(true);

  useEffect(() => {
    const checkAuth = () => {
      try {
        // Clear any stale auth data first
        const token = localStorage.getItem("token");
        const user = getStoredUser();
        
        // If token exists but user is not a Brand Owner, clear it
        if (token && user) {
          const isBrandOwner = 
            user.userType === "BrandOwner" || 
            (user as any).staffType === "BrandOwner" ||
            (user as any).role === "BrandOwner" ||
            (user as any).roles?.includes("BrandOwner") ||
            (user as any).adminType === "BrandOwner";

          if (isBrandOwner) {
            console.log("👤 User already logged in as Brand Owner, redirecting...");
            router.push("/brand-owner");
            return;
          } else {
            // User exists but is not a Brand Owner - clear data
            console.log("❌ User is not a Brand Owner, clearing session");
            clearAuthData();
          }
        }
      } catch (error) {
        console.error("Auth check error:", error);
        clearAuthData();
      }
      setIsCheckingAuth(false);
    };
    
    checkAuth();
  }, [router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");
    setSuccess("");

    // Validate inputs
    if (!email || !password) {
      setError("Please enter both email and password.");
      setIsLoading(false);
      return;
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setError("Please enter a valid email address.");
      setIsLoading(false);
      return;
    }

    // Validate password length
    if (password.length < 6) {
      setError("Password must be at least 6 characters long.");
      setIsLoading(false);
      return;
    }

    try {
      console.log("🔄 Attempting Brand Owner login...");
      
      const response = await brandOwnerLogin({
        Email_PhoneNo: email,
        Password: password,
      } as any);

      console.log("✅ Login successful:", {
        userId: response.userId,
        email: response.email,
        userType: response.userType,
      });

      setSuccess("Login successful! Redirecting to dashboard...");
      
      // Small delay before redirect to show success message
      setTimeout(() => {
        router.push("/brand-owner");
      }, 1000);

    } catch (err) {
      console.error("❌ Login error:", err);
      
      const errorMessage = err instanceof Error ? err.message : "Login failed. Please try again.";
      
      // Parse specific error messages
      if (errorMessage.toLowerCase().includes("invalid") || 
          errorMessage.toLowerCase().includes("incorrect") ||
          errorMessage.toLowerCase().includes("wrong")) {
        setError("Invalid email or password. Please try again.");
      } else if (errorMessage.toLowerCase().includes("not found") || 
                 errorMessage.toLowerCase().includes("doesn't exist")) {
        setError("Account not found. Please check your email or register.");
      } else if (errorMessage.toLowerCase().includes("verify") || 
                 errorMessage.toLowerCase().includes("verification")) {
        setError("Please verify your email before logging in. Check your inbox.");
      } else if (errorMessage.toLowerCase().includes("network") || 
                 errorMessage.toLowerCase().includes("connection")) {
        setError("Network error. Please check your internet connection.");
      } else {
        setError(errorMessage);
      }
      
      setIsLoading(false);
    } finally {
      // Only set loading false if not redirected
      if (!success) {
        setIsLoading(false);
      }
    }
  };

  // Show loading state while checking auth
  if (isCheckingAuth) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin text-primary mx-auto" />
          <p className="mt-3 text-gray-500 text-sm">Checking session...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-gray-50 to-gray-100 px-4 py-8">
      <div className="w-full max-w-md">
        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 mb-4">
            <Building2 className="h-8 w-8 text-primary" />
          </div>
          <h1 className="text-3xl font-bold text-gray-900">GINILOG</h1>
          <p className="text-gray-500 text-sm mt-1">Brand Owner Portal</p>
        </div>

        {/* Login Card */}
        <div className="bg-white rounded-2xl shadow-xl border border-gray-100 p-8">
          {/* Success Message */}
          {success && (
            <div className="mb-4 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl text-sm flex items-start gap-2">
              <CheckCircle className="h-4 w-4 flex-shrink-0 mt-0.5" />
              <span>{success}</span>
            </div>
          )}

          {/* Error Message */}
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm flex items-start gap-2">
              <AlertCircle className="h-4 w-4 flex-shrink-0 mt-0.5" />
              <span>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            {/* Email Field */}
            <div>
              <Label htmlFor="email" className="text-sm font-medium text-gray-700">
                Email Address
              </Label>
              <div className="relative mt-1.5">
                <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="email"
                  type="email"
                  value={email}
                  onChange={(e) => {
                    setEmail(e.target.value);
                    if (error) setError("");
                  }}
                  className={`pl-10 h-11 rounded-xl border-gray-200 focus:border-primary focus:ring-primary/20 ${
                    error ? 'border-red-300 focus:border-red-500 focus:ring-red-500/20' : ''
                  }`}
                  placeholder="brand@ginilog.com"
                  required
                  disabled={isLoading}
                  autoComplete="email"
                />
              </div>
            </div>

            {/* Password Field */}
            <div>
              <div className="flex items-center justify-between">
                <Label htmlFor="password" className="text-sm font-medium text-gray-700">
                  Password
                </Label>
                <Link
                  href="/brand-owner/forgot-password"
                  className="text-xs text-primary hover:underline"
                >
                  Forgot password?
                </Link>
              </div>
              <div className="relative mt-1.5">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  value={password}
                  onChange={(e) => {
                    setPassword(e.target.value);
                    if (error) setError("");
                  }}
                  className={`pl-10 pr-10 h-11 rounded-xl border-gray-200 focus:border-primary focus:ring-primary/20 ${
                    error ? 'border-red-300 focus:border-red-500 focus:ring-red-500/20' : ''
                  }`}
                  placeholder="••••••••"
                  required
                  disabled={isLoading}
                  autoComplete="current-password"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                  disabled={isLoading}
                >
                  {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                </button>
              </div>
              <p className="text-xs text-gray-400 mt-1">
                Password must be at least 6 characters
              </p>
            </div>

            {/* Submit Button */}
            <Button 
              type="submit" 
              className="w-full h-11 rounded-xl font-semibold bg-primary hover:bg-primary/90 text-white transition-all duration-200"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  Signing in...
                </>
              ) : (
                "Sign In"
              )}
            </Button>
          </form>

          {/* Register Link */}
          <div className="mt-6 text-center">
            <p className="text-sm text-gray-500">
              Don't have a brand account?{" "}
              <Link
                href="/brand-owner/register"
                className="text-primary hover:underline font-medium"
              >
                Register here
              </Link>
            </p>
          </div>

          {/* Back to Home */}
          <div className="mt-4 text-center">
            <Link
              href="/"
              className="text-xs text-gray-400 hover:text-gray-600 transition-colors"
            >
              ← Back to Home
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}