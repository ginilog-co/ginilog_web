"use client";

import Link from "next/link";
import { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Mail, CheckCircle, Loader2, ArrowLeft, AlertCircle, Eye, EyeOff, Lock, Key } from "lucide-react";
import { verifyEmail, resendVerificationCode, getStoredUser } from "@/lib/api";

function VerifyEmailContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const emailParam = searchParams.get("email") || "";
  
  const [verificationCode, setVerificationCode] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isResending, setIsResending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [resendSuccess, setResendSuccess] = useState(false);
  const [countdown, setCountdown] = useState(0);
  const [email, setEmail] = useState(emailParam);
  const [verificationError, setVerificationError] = useState<string | null>(null);

  // Auto-fill email from stored user data if not in URL
  useEffect(() => {
    if (!email && typeof window !== "undefined") {
      const user = getStoredUser();
      if (user?.email) {
        setEmail(user.email);
      }
      // Also try to get email from sessionStorage
      const storedEmail = sessionStorage.getItem("tempEmail");
      if (storedEmail) {
        setEmail(storedEmail);
      }
    }
  }, [email]);

  // Countdown timer for resend
  useEffect(() => {
    if (countdown > 0) {
      const timer = setTimeout(() => setCountdown(countdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [countdown]);

  // Auto-fill password from session storage
  useEffect(() => {
    if (!password && typeof window !== "undefined") {
      const storedPassword = sessionStorage.getItem("tempPassword");
      if (storedPassword) {
        setPassword(storedPassword);
      }
    }
  }, [password]);

  const handleVerify = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setVerificationError(null);
    setIsLoading(true);

    try {
      // Validate code length
      if (verificationCode.length !== 5) {
        setVerificationError("Please enter a valid 5-digit verification code.");
        setIsLoading(false);
        return;
      }

      // Get password from state or session storage
      let userPassword = password;
      if (!userPassword) {
        const storedPassword = sessionStorage.getItem("tempPassword");
        if (storedPassword) {
          userPassword = storedPassword;
        }
      }

      if (!userPassword) {
        setVerificationError("Please enter your password to verify your account.");
        setIsLoading(false);
        return;
      }

      const verificationData = {
        Token: verificationCode,
        Password: userPassword,
      };

      console.log('📤 Verifying email with:', { 
        Token: verificationCode, 
        hasPassword: !!verificationData.Password 
      });

      const result = await verifyEmail(verificationData);
      console.log('✅ Verification successful:', result);
      
      setSuccess(true);
      // Clear temp data from session storage
      sessionStorage.removeItem("tempPassword");
      sessionStorage.removeItem("tempEmail");
      
      // Redirect to dashboard after successful verification
      setTimeout(() => {
        router.push("/customer-portal/dashboard");
      }, 2000);
    } catch (err) {
      console.error('❌ Verification error:', err);
      const errorMessage = err instanceof Error ? err.message : "Verification failed. Please try again.";
      
      // Parse specific error messages
      if (errorMessage.toLowerCase().includes("invalid") || 
          errorMessage.toLowerCase().includes("incorrect") ||
          errorMessage.toLowerCase().includes("wrong")) {
        setVerificationError("Invalid verification code. Please check and try again.");
      } else if (errorMessage.toLowerCase().includes("expired")) {
        setVerificationError("Verification code has expired. Please request a new one.");
      } else if (errorMessage.toLowerCase().includes("password")) {
        setVerificationError("Incorrect password. Please try again.");
      } else {
        setError(errorMessage);
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendCode = async () => {
    if (countdown > 0) return;
    
    setError(null);
    setVerificationError(null);
    setResendSuccess(false);
    setIsResending(true);

    try {
      const userEmail = email || getStoredUser()?.email || sessionStorage.getItem("tempEmail");
      
      if (!userEmail) {
        throw new Error("No email address found. Please register again.");
      }

      console.log('📤 Resending verification code to:', userEmail);
      await resendVerificationCode(userEmail);
      setResendSuccess(true);
      setCountdown(60); // 60 second cooldown
      
      // Clear any old verification errors
      setVerificationError(null);
    } catch (err) {
      console.error('❌ Resend error:', err);
      setError(err instanceof Error ? err.message : "Failed to resend verification code.");
    } finally {
      setIsResending(false);
    }
  };

  if (success) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="w-full max-w-md bg-white rounded-lg shadow-sm border p-8 text-center">
          <div className="mx-auto w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mb-4">
            <CheckCircle className="h-8 w-8 text-green-600" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900 mb-2">Email Verified!</h1>
          <p className="text-gray-600 mb-6">
            Your email has been successfully verified. You will be redirected to your dashboard.
          </p>
          <Loader2 className="h-6 w-6 animate-spin mx-auto text-primary" />
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4 py-8">
      <div className="w-full max-w-md bg-white rounded-lg shadow-sm border p-8">
        <Link
          href="/customer-portal/login"
          className="inline-flex items-center text-sm text-gray-500 hover:text-gray-700 mb-6"
        >
          <ArrowLeft className="h-4 w-4 mr-1" />
          Back to Sign In
        </Link>

        <div className="text-center mb-6">
          <div className="mx-auto w-14 h-14 bg-primary/10 rounded-full flex items-center justify-center mb-3">
            <Mail className="h-7 w-7 text-primary" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">Verify Your Email</h1>
          <p className="text-sm text-gray-500 mt-1">
            We sent a verification code to
            <br />
            <span className="font-medium text-gray-700">{email || "your email"}</span>
          </p>
        </div>

        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-start gap-2">
            <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
            <span>{error}</span>
          </div>
        )}

        {verificationError && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm flex items-start gap-2">
            <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
            <span>{verificationError}</span>
          </div>
        )}

        {resendSuccess && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md text-green-700 text-sm flex items-start gap-2">
            <CheckCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
            <span>New verification code sent successfully! Please check your email.</span>
          </div>
        )}

        <form onSubmit={handleVerify} className="space-y-4">
          <div>
            <Label htmlFor="verificationCode" className="text-gray-700">Verification Code</Label>
            <div className="relative mt-1">
              <Key className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input
                id="verificationCode"
                type="text"
                placeholder="Enter 5-digit code"
                className={`pl-10 h-12 text-center text-lg tracking-widest ${
                  verificationError?.includes("code") ? 'border-red-500 focus:ring-red-500' : ''
                }`}
                value={verificationCode}
                onChange={(e) => {
                  const value = e.target.value.replace(/\D/g, "").slice(0, 5);
                  setVerificationCode(value);
                  if (verificationError) {
                    setVerificationError(null);
                  }
                }}
                required
                maxLength={5}
                autoFocus
              />
            </div>
            <p className="text-xs text-gray-400 mt-1 text-center">
              Enter the 5-digit code sent to your email
            </p>
          </div>

          {/* Password field - required for verification */}
          <div>
            <Label htmlFor="password" className="text-gray-700">Password</Label>
            <div className="relative mt-1">
              <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input
                id="password"
                type={showPassword ? "text" : "password"}
                placeholder="Enter your password"
                className={`pl-10 pr-10 h-12 ${
                  verificationError?.includes("password") ? 'border-red-500 focus:ring-red-500' : ''
                }`}
                value={password}
                onChange={(e) => {
                  setPassword(e.target.value);
                  if (verificationError) {
                    setVerificationError(null);
                  }
                }}
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
            <p className="text-xs text-gray-400 mt-1">
              Enter your password to confirm your identity
            </p>
          </div>

          <Button
            type="submit"
            disabled={isLoading || verificationCode.length < 5 || !password}
            className="w-full h-12 bg-primary hover:bg-primary/90 text-white font-medium"
          >
            {isLoading ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Verifying...
              </>
            ) : (
              "Verify Email"
            )}
          </Button>
        </form>

        <div className="mt-6 text-center">
          <p className="text-sm text-gray-500">
            Didn&apos;t receive the code?{" "}
            <button
              type="button"
              onClick={handleResendCode}
              disabled={isResending || countdown > 0}
              className="text-primary hover:text-primary/80 font-medium disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isResending ? (
                <>
                  <Loader2 className="inline h-3 w-3 animate-spin mr-1" />
                  Sending...
                </>
              ) : countdown > 0 ? (
                `Resend in ${countdown}s`
              ) : (
                "Resend Code"
              )}
            </button>
          </p>
        </div>

        <div className="mt-4 pt-4 border-t border-gray-100">
          <p className="text-center text-xs text-gray-400">
            Check your spam folder if you don&apos;t see the email in your inbox.
          </p>
          <p className="text-center text-xs text-gray-400 mt-1">
            The code expires in 10 minutes.
          </p>
        </div>
      </div>
    </div>
  );
}

// Main component with Suspense wrapper
export default function VerifyEmail() {
  return (
    <Suspense fallback={
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin mx-auto text-primary" />
          <p className="mt-2 text-gray-500">Loading...</p>
        </div>
      </div>
    }>
      <VerifyEmailContent />
    </Suspense>
  );
} 