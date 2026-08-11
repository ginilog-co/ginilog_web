// app/customer-portal/verify-email/page.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Mail, CheckCircle, Loader2, ArrowLeft, AlertCircle, Eye, EyeOff, Lock, Key } from "lucide-react";
import { 
  verifyAuthUserEmail,
  requestAuthUserEmailVerificationToken,
  resendAuthUserVerificationCode,
  getStoredUser
} from "@/lib/api";

function VerifyEmailContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const emailParam = searchParams.get("email") || "";
  
  const [email, setEmail] = useState(emailParam);
  const [verificationCode, setVerificationCode] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isResending, setIsResending] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [step, setStep] = useState<'request' | 'verify' | 'complete'>('request');

  // Check if user is already logged in
  useEffect(() => {
    const user = getStoredUser();
    if (user) {
      router.push("/customer-dashboard");
    }
  }, [router]);

  // Resend timer countdown
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  // If email is provided in URL, auto-send verification
  useEffect(() => {
    if (emailParam) {
      setTimeout(() => {
        handleSendVerification();
      }, 500);
    }
  }, [emailParam]);

  const handleSendVerification = async () => {
    if (!email) {
      setError("Please enter your email address.");
      return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      setError("Please enter a valid email address.");
      return;
    }

    setIsResending(true);
    setError(null);
    setSuccess(null);

    try {
      await requestAuthUserEmailVerificationToken(email);
      setSuccess("Verification code sent! Please check your email.");
      setStep('verify');
      setResendTimer(60);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to send verification code.");
    } finally {
      setIsResending(false);
    }
  };

  const handleVerifyEmail = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!verificationCode || verificationCode.length < 4) {
      setError("Please enter a valid verification code.");
      return;
    }

    if (!password) {
      setError("Please enter a password.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    const passwordRegex = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$/;
    if (!passwordRegex.test(password)) {
      setError("Password must be at least 8 characters with uppercase, lowercase, and a number.");
      return;
    }

    setIsLoading(true);

    try {
      // Use auth user verification
      await verifyAuthUserEmail({
        Token: verificationCode,
        Password: password,
      });

      setSuccess("Email verified successfully! Redirecting to dashboard...");
      setStep('complete');
      
      setTimeout(() => {
        router.push("/customer-dashboard");
      }, 2000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Verification failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendCode = async () => {
    if (resendTimer > 0) return;
    
    setError(null);
    setSuccess(null);
    setIsResending(true);

    try {
      await resendAuthUserVerificationCode(email);
      setSuccess("New verification code sent!");
      setResendTimer(60);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to resend code.");
    } finally {
      setIsResending(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full bg-white rounded-lg shadow-md p-8">
        {/* Header */}
        <div className="text-center mb-6">
          <div className="mx-auto w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mb-4">
            <Mail className="h-8 w-8 text-blue-600" />
          </div>
          <h2 className="text-2xl font-bold text-gray-900">
            {step === 'request' ? 'Verify Your Email' : 
             step === 'verify' ? 'Enter Verification Code' : 
             'Email Verified!'}
          </h2>
          <p className="mt-2 text-sm text-gray-600">
            {step === 'request' && 'Enter your email to receive a verification code.'}
            {step === 'verify' && `We sent a code to ${email}`}
            {step === 'complete' && 'Your email has been verified successfully!'}
          </p>
        </div>

        {/* Error/Success Messages */}
        {error && (
          <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md flex items-start gap-2">
            <AlertCircle className="h-5 w-5 text-red-500 flex-shrink-0 mt-0.5" />
            <span className="text-sm text-red-700">{error}</span>
          </div>
        )}

        {success && (
          <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md flex items-start gap-2">
            <CheckCircle className="h-5 w-5 text-green-500 flex-shrink-0 mt-0.5" />
            <span className="text-sm text-green-700">{success}</span>
          </div>
        )}

        {/* Step 1: Request Verification */}
        {step === 'request' && (
          <form onSubmit={(e) => { e.preventDefault(); handleSendVerification(); }} className="space-y-4">
            <div>
              <Label htmlFor="email">Email Address</Label>
              <Input
                id="email"
                type="email"
                placeholder="you@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                className="mt-1"
                disabled={isResending}
              />
            </div>

            <Button
              type="submit"
              disabled={isResending || !email}
              className="w-full h-11 bg-blue-600 hover:bg-blue-700 text-white"
            >
              {isResending ? (
                <><Loader2 className="h-4 w-4 animate-spin mr-2" />Sending...</>
              ) : (
                "Send Verification Code"
              )}
            </Button>

            <div className="text-center">
              <Link href="/customer-portal/login" className="text-sm text-gray-600 hover:text-gray-900">
                ← Back to Login
              </Link>
            </div>
          </form>
        )}

        {/* Step 2: Verify Code */}
        {step === 'verify' && (
          <form onSubmit={handleVerifyEmail} className="space-y-4">
            <div className="rounded-lg bg-blue-50 border border-blue-200 p-4">
              <p className="text-sm text-blue-700">
                A verification code was sent to <strong>{email}</strong>
              </p>
            </div>

            <div>
              <Label htmlFor="verificationCode">Verification Code</Label>
              <div className="relative mt-1">
                <Key className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="verificationCode"
                  type="text"
                  placeholder="Enter code"
                  value={verificationCode}
                  onChange={(e) => setVerificationCode(e.target.value.replace(/\D/g, ''))}
                  className="pl-10 text-center text-2xl tracking-widest"
                  required
                  autoFocus
                />
              </div>
            </div>

            <div>
              <Label htmlFor="password">Set Password</Label>
              <div className="relative mt-1">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  placeholder="Min 8 characters"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="pl-10 pr-10"
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
              <p className="text-xs text-gray-500 mt-1">Must contain uppercase, lowercase, and a number</p>
            </div>

            <div>
              <Label htmlFor="confirmPassword">Confirm Password</Label>
              <div className="relative mt-1">
                <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                <Input
                  id="confirmPassword"
                  type={showConfirmPassword ? "text" : "password"}
                  placeholder="Confirm password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                  className="pl-10 pr-10"
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showConfirmPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                </button>
              </div>
            </div>

            <div className="flex flex-col gap-2">
              <Button
                type="submit"
                disabled={isLoading || !verificationCode || !password || !confirmPassword}
                className="w-full h-11 bg-blue-600 hover:bg-blue-700 text-white"
              >
                {isLoading ? (
                  <><Loader2 className="h-4 w-4 animate-spin mr-2" />Verifying...</>
                ) : (
                  "Verify & Set Password"
                )}
              </Button>

              <button
                type="button"
                onClick={handleResendCode}
                disabled={resendTimer > 0 || isResending}
                className="text-sm text-blue-600 hover:text-blue-800 disabled:text-gray-400"
              >
                {resendTimer > 0 ? `Resend in ${resendTimer}s` : "Resend Code"}
              </button>
            </div>

            <button
              type="button"
              onClick={() => {
                setStep('request');
                setError(null);
                setSuccess(null);
                setVerificationCode("");
                setPassword("");
                setConfirmPassword("");
              }}
              className="text-sm text-gray-600 hover:text-gray-900 flex items-center gap-1 mx-auto"
            >
              <ArrowLeft className="h-3 w-3" />
              Change email address
            </button>
          </form>
        )}

        {/* Step 3: Complete */}
        {step === 'complete' && (
          <div className="text-center space-y-4">
            <div className="mx-auto w-16 h-16 bg-green-100 rounded-full flex items-center justify-center">
              <CheckCircle className="h-8 w-8 text-green-600" />
            </div>
            <p className="text-sm text-gray-600">
              Your email has been verified and your password has been set successfully.
            </p>
            <Button
              onClick={() => router.push("/customer-dashboard")}
              className="w-full h-11 bg-blue-600 hover:bg-blue-700 text-white"
            >
              Go to Dashboard
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}

export default function VerifyEmailPage() {
  return <VerifyEmailContent />;
}