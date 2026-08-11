"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Mail, Lock, Eye, EyeOff, Loader2, ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { 
  login, 
  requestEmailVerificationToken, 
  getStoredUser,
  isAuthenticated,
  getToken,
  clearAuthData
} from "@/lib/api";
import { signInWithGoogle, signInWithApple } from "@/lib/firebase";

export default function CustomerLoginContent() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showVerification, setShowVerification] = useState(false);
  const [verificationCode, setVerificationCode] = useState("");
  const [verificationLoading, setVerificationLoading] = useState(false);
  const [verificationError, setVerificationError] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [userEmail, setUserEmail] = useState("");

  // Check if user is already logged in
  useEffect(() => {
    if (isAuthenticated()) {
      const user = getStoredUser();
      if (user) {
        router.push("/customer-dashboard");
      }
    }
  }, [router]);

  // Resend timer countdown
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);

    try {
      const response = await login({
        Email_PhoneNo: email,
        Password: password,
      });

      // Check if email is verified
      if (!response.emailVerified) {
        setUserEmail(email);
        setShowVerification(true);
        setIsLoading(false);
        // Send verification code
        await requestEmailVerificationToken(email);
        setResendTimer(60);
        return;
      }

      // If email is verified, redirect to dashboard
      router.push("/customer-dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed. Please try again.");
      setIsLoading(false);
    }
  };

  const handleVerifyEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    setVerificationLoading(true);
    setVerificationError(null);

    try {
      // Call your email verification API
      const response = await fetch("/api/auth/verify-email", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: userEmail,
          token: verificationCode,
        }),
      });

      if (!response.ok) {
        const data = await response.json();
        throw new Error(data.error || "Verification failed");
      }

      // After verification, try to login again
      const loginResponse = await login({
        Email_PhoneNo: userEmail,
        Password: password,
      });

      router.push("/customer-dashboard");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Verification failed");
      setVerificationLoading(false);
    }
  };

  const handleResendCode = async () => {
    if (resendTimer > 0) return;
    setVerificationError(null);
    try {
      await requestEmailVerificationToken(userEmail);
      setResendTimer(60);
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to resend code");
    }
  };

  const handleGoogleSignIn = async () => {
    try {
      setIsLoading(true);
      setError(null);
      await signInWithGoogle();
      // After successful sign-in, redirect to dashboard
      router.push("/customer-dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Google sign in failed");
      setIsLoading(false);
    }
  };

  const handleAppleSignIn = async () => {
    try {
      setIsLoading(true);
      setError(null);
      await signInWithApple();
      router.push("/customer-dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Apple sign in failed");
      setIsLoading(false);
    }
  };

  if (showVerification) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
        <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
          <div className="text-center">
            <h2 className="text-2xl font-bold text-gray-900">Verify Your Email</h2>
            <p className="mt-2 text-sm text-gray-600">
              We sent a verification code to <strong>{userEmail}</strong>
            </p>
          </div>

          {verificationError && (
            <div className="rounded-lg bg-red-50 border border-red-200 p-3 text-red-800 text-sm">
              {verificationError}
            </div>
          )}

          <form onSubmit={handleVerifyEmail} className="space-y-4">
            <div>
              <Label htmlFor="verificationCode">Verification Code</Label>
              <Input
                id="verificationCode"
                type="text"
                placeholder="Enter 6-digit code"
                value={verificationCode}
                onChange={(e) => setVerificationCode(e.target.value.replace(/\D/g, "").slice(0, 6))}
                maxLength={6}
                required
                className="mt-1 text-center text-2xl tracking-widest"
                autoFocus
              />
            </div>

            <div className="flex flex-col gap-3">
              <Button
                type="submit"
                disabled={verificationCode.length !== 6 || verificationLoading}
                className="w-full bg-blue-600 hover:bg-blue-700 text-white"
              >
                {verificationLoading ? (
                  <><Loader2 className="h-4 w-4 animate-spin mr-2" />Verifying...</>
                ) : (
                  "Verify Email"
                )}
              </Button>
              <button
                type="button"
                onClick={handleResendCode}
                disabled={resendTimer > 0}
                className="text-sm text-blue-600 hover:text-blue-800 disabled:text-gray-400"
              >
                {resendTimer > 0 ? `Resend in ${resendTimer}s` : "Resend Code"}
              </button>
            </div>

            <button
              type="button"
              onClick={() => {
                setShowVerification(false);
                setVerificationCode("");
                setVerificationError(null);
              }}
              className="text-sm text-gray-600 hover:text-gray-900 flex items-center gap-1 mx-auto"
            >
              <ArrowLeft className="h-3 w-3" />
              Back to login
            </button>
          </form>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
        <div className="text-center">
          <h2 className="text-3xl font-bold text-gray-900">Welcome Back</h2>
          <p className="mt-2 text-sm text-gray-600">Sign in to your customer account</p>
        </div>

        {error && (
          <div className="rounded-lg bg-red-50 border border-red-200 p-3 text-red-800 text-sm">
            {error}
          </div>
        )}

        <form onSubmit={handleLogin} className="space-y-4">
          <div>
            <Label htmlFor="email">Email or Phone</Label>
            <div className="relative mt-1">
              <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                id="email"
                type="text"
                placeholder="email@example.com or +234..."
                className="pl-10 h-11"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
            </div>
          </div>

          <div>
            <Label htmlFor="password">Password</Label>
            <div className="relative mt-1">
              <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                id="password"
                type={showPassword ? "text" : "password"}
                placeholder="••••••••"
                className="pl-10 pr-10 h-11"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
              </button>
            </div>
          </div>

          <div className="flex items-center justify-between">
            <Link href="/forgot-password" className="text-sm text-blue-600 hover:text-blue-800">
              Forgot password?
            </Link>
          </div>

          <Button type="submit" disabled={isLoading} className="w-full h-11 bg-blue-600 hover:bg-blue-700 text-white">
            {isLoading ? (
              <><Loader2 className="h-4 w-4 animate-spin mr-2" />Signing in...</>
            ) : (
              "Sign In"
            )}
          </Button>
        </form>

        <div className="relative">
          <div className="absolute inset-0 flex items-center">
            <div className="w-full border-t border-gray-300" />
          </div>
          <div className="relative flex justify-center text-sm">
            <span className="px-2 bg-white text-gray-500">Or continue with</span>
          </div>
        </div>

        <div className="flex gap-3">
          <Button
            type="button"
            onClick={handleGoogleSignIn}
            variant="outline"
            className="flex-1 h-11 border-gray-300 hover:bg-gray-50"
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
          </Button>
          <Button
            type="button"
            onClick={handleAppleSignIn}
            variant="outline"
            className="flex-1 h-11 border-gray-300 hover:bg-gray-50"
          >
            <svg className="h-5 w-5 mr-2" viewBox="0 0 24 24" fill="currentColor">
              <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z"/>
            </svg>
            Apple
          </Button>
        </div>

        <p className="text-center text-sm text-gray-600">
          Don't have an account?{" "}
          <Link href="/customer-register" className="text-blue-600 hover:text-blue-800 font-medium">
            Sign up
          </Link>
        </p>

        <p className="text-center text-xs text-gray-500">
          <Link href="/" className="hover:text-gray-700">← Back to Home</Link>
        </p>
      </div>
    </div>
  );
}