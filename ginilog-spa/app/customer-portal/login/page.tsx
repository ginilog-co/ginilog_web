"use client";

import Link from "next/link";
import { useState, useEffect, Suspense } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Eye, EyeOff, Mail, Lock, Loader2, AlertCircle, CheckCircle } from "lucide-react";
import { 
  login, 
  requestAuthUserEmailVerificationToken,
  getStoredUser,
  isAuthenticated 
} from "@/lib/api";
import { signInWithGoogle, signInWithApple } from "@/lib/firebase";

function CustomerLoginContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [isSocialLoading, setIsSocialLoading] = useState<'google' | 'apple' | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [resendLoading, setResendLoading] = useState(false);
  const [resendSuccess, setResendSuccess] = useState(false);
  const [resendCountdown, setResendCountdown] = useState(0);
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const [verificationEmailSent, setVerificationEmailSent] = useState(false);

  // Check if user came from registration/verification
  useEffect(() => {
    const verified = searchParams.get("verified");
    const email = searchParams.get("email");
    const fromRegistration = searchParams.get("from") === "registration";
    
    if (verified === "true" && email) {
      setFormData(prev => ({ ...prev, email }));
      setVerificationEmailSent(true);
      setTimeout(() => {
        setVerificationEmailSent(false);
      }, 5000);
    }
    
    if (fromRegistration && email) {
      setFormData(prev => ({ ...prev, email }));
      setError("Please verify your email before logging in. A verification code has been sent to your email.");
    }
  }, [searchParams]);

  // Countdown timer for resend
  useEffect(() => {
    if (resendCountdown > 0) {
      const timer = setTimeout(() => setResendCountdown(resendCountdown - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendCountdown]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    setResendSuccess(false);

    try {
      const credentials = {
        Email_PhoneNo: formData.email,
        Password: formData.password,
      };

      const result = await login(credentials);
      console.log('✅ Login successful:', result);
      
      if (!result.emailVerified) {
        setError("Please verify your email before logging in. A verification code has been sent to your email.");
        setIsLoading(false);
        return;
      }
      
      router.push("/customer-portal/dashboard");
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : "Login failed. Please try again.";
      setError(errorMessage);
      setResendSuccess(false);
    } finally {
      setIsLoading(false);
    }
  };

  const handleResendVerification = async () => {
    if (resendCountdown > 0) return;
    
    setResendLoading(true);
    setResendSuccess(false);
    setError(null);
    
    try {
      const userEmail = formData.email || getStoredUser()?.email;
      
      if (!userEmail) {
        setError("Please enter your email address to resend the verification code.");
        setResendLoading(false);
        return;
      }

      console.log('📤 Requesting verification token for:', userEmail);
      // Use the correct function name
      await requestAuthUserEmailVerificationToken(userEmail);
      
      setResendSuccess(true);
      setResendCountdown(60);
      setVerificationEmailSent(true);
      
      setTimeout(() => {
        setVerificationEmailSent(false);
      }, 5000);
      
      router.push(`/customer-portal/verify-email?email=${encodeURIComponent(userEmail)}&from=login`);
    } catch (err) {
      console.error('❌ Resend verification error:', err);
      setError(err instanceof Error ? err.message : "Failed to resend verification email.");
      setResendSuccess(false);
    } finally {
      setResendLoading(false);
    }
  };

  const handleGoogleLogin = async () => {
    setIsSocialLoading('google');
    setError(null);
    try {
      console.log('🔄 Starting Google login from page...');
      const result = await signInWithGoogle();
      console.log('✅ Google login result:', result);
      
      if (result && (result.token || result.refreshToken)) {
        const user = getStoredUser();
        if (user?.emailVerified) {
          console.log('✅ Email verified, redirecting to dashboard...');
          router.push("/customer-portal/dashboard");
        } else {
          console.log('⚠️ Email not verified, redirecting to verification...');
          router.push(`/customer-portal/verify-email?email=${encodeURIComponent(user?.email || "")}&from=login`);
        }
      } else {
        throw new Error('Authentication failed - no token received');
      }
    } catch (err: any) {
      console.error('❌ Google login error in page:', err);
      
      let errorMessage = err.message || "Google login failed. Please try again.";
      
      if (errorMessage.includes('HTTP 401') || errorMessage.includes('Unauthorized')) {
        errorMessage = 'Authentication failed. Please make sure your account exists or contact support.';
      } else if (errorMessage.includes('HTTP 404')) {
        errorMessage = 'The login service is currently unavailable. Please try again later.';
      } else if (errorMessage.includes('HTTP 500')) {
        errorMessage = 'Server error. Please try again later.';
      } else if (errorMessage.includes('email not verified') || errorMessage.includes('verify your email')) {
        errorMessage = 'Please verify your email before logging in.';
      }
      
      setError(errorMessage);
    } finally {
      setIsSocialLoading(null);
    }
  };

  const handleAppleLogin = async () => {
    setIsSocialLoading('apple');
    setError(null);
    try {
      console.log('🔄 Starting Apple login from page...');
      const result = await signInWithApple();
      console.log('✅ Apple login result:', result);
      
      if (result && (result.token || result.refreshToken)) {
        const user = getStoredUser();
        if (user?.emailVerified) {
          console.log('✅ Email verified, redirecting to dashboard...');
          router.push("/customer-portal/dashboard");
        } else {
          console.log('⚠️ Email not verified, redirecting to verification...');
          router.push(`/customer-portal/verify-email?email=${encodeURIComponent(user?.email || "")}&from=login`);
        }
      } else {
        throw new Error('Authentication failed - no token received');
      }
    } catch (err: any) {
      console.error('❌ Apple login error in page:', err);
      
      let errorMessage = err.message || "Apple login failed. Please try again.";
      
      if (errorMessage.includes('HTTP 401') || errorMessage.includes('Unauthorized')) {
        errorMessage = 'Authentication failed. Please make sure your account exists or contact support.';
      } else if (errorMessage.includes('HTTP 404')) {
        errorMessage = 'The login service is currently unavailable. Please try again later.';
      } else if (errorMessage.includes('HTTP 500')) {
        errorMessage = 'Server error. Please try again later.';
      } else if (errorMessage.includes('email not verified') || errorMessage.includes('verify your email')) {
        errorMessage = 'Please verify your email before logging in.';
      }
      
      setError(errorMessage);
    } finally {
      setIsSocialLoading(null);
    }
  };

  const isEmailUnverified = error && (
    /not yet verify|not verified|verify your email|email not verified|please verify your email/i.test(error)
  );

  return (
    <div className="min-h-screen flex">
      {/* Background with Image */}
      <div 
        className="hidden lg:block lg:w-1/2 bg-cover bg-center relative"
        style={{ backgroundImage: "url('/carousel-2.jpg')" }}
      >
        <div className="absolute inset-0 bg-primary/20 mix-blend-multiply" />
        <div className="absolute inset-0 flex flex-col justify-center px-12 text-white">
          <h1 className="text-4xl font-bold mb-4">Welcome to GINILOG</h1>
          <p className="text-xl mb-12">Your one-stop shop for logistics and accommodation.</p>
        </div>
      </div>

      {/* Form Panel */}
      <main className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-white">
        <div className="w-full max-w-md">
          {/* Mobile Header */}
          <div className="lg:hidden text-center mb-8">
            <h1 className="text-2xl font-bold text-primary">GINILOG</h1>
            <p className="text-gray-600 mt-2">Welcome back</p>
          </div>

          <div className="bg-white rounded-lg">
            <div className="border-b border-gray-200 mb-6">
              <div className="flex">
                <button className="flex-1 py-3 text-center text-primary border-b-2 border-primary font-medium">
                  Sign In
                </button>
                <Link 
                  href="/customer-portal/register" 
                  className="flex-1 py-3 text-center text-gray-500 hover:text-gray-700"
                >
                  Sign Up
                </Link>
              </div>
            </div>

            {/* Success message for verified email */}
            {verificationEmailSent && (
              <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md text-green-700 text-sm flex items-start gap-2">
                <CheckCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                <span>Verification email sent! Please check your inbox and spam folder.</span>
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-5">
              {error && (
                <div className="p-3 bg-red-50 border border-red-200 rounded-md">
                  <div className="flex items-start gap-2">
                    <AlertCircle className="h-4 w-4 text-red-600 mt-0.5 flex-shrink-0" />
                    <div className="flex-1">
                      <p className="text-sm text-red-700">{error}</p>
                      {isEmailUnverified && (
                        <div className="mt-2 pt-2 border-t border-red-200">
                          <p className="text-xs text-red-600 mb-1">
                            Please verify your email address to continue.
                          </p>
                          <button
                            type="button"
                            disabled={resendLoading || resendCountdown > 0}
                            onClick={handleResendVerification}
                            className="text-sm text-primary hover:text-primary/80 font-medium disabled:opacity-60 disabled:cursor-not-allowed"
                          >
                            {resendLoading ? (
                              <>
                                <Loader2 className="inline h-3 w-3 animate-spin mr-1" />
                                Sending...
                              </>
                            ) : resendCountdown > 0 ? (
                              `Resend in ${resendCountdown}s`
                            ) : resendSuccess ? (
                              "✓ Verification email sent!"
                            ) : (
                              "Resend verification email"
                            )}
                          </button>
                        </div>
                      )}
                    </div>
                  </div>
                </div>
              )}

              <div>
                <Label htmlFor="email" className="text-gray-700">Email or Phone</Label>
                <div className="relative mt-1">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    id="email"
                    type="text"
                    placeholder="you@example.com or +2348000000000"
                    className="pl-10 h-12"
                    value={formData.email}
                    onChange={(e) => {
                      setFormData({ ...formData, email: e.target.value });
                      if (error) setError(null);
                      setResendSuccess(false);
                    }}
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
                    onChange={(e) => {
                      setFormData({ ...formData, password: e.target.value });
                      if (error) setError(null);
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
              </div>

              <div className="flex items-center justify-between">
                <Link 
                  href="/customer-portal/forgot-password"
                  className="text-sm text-primary hover:text-primary/80"
                >
                  Forgot password?
                </Link>
              </div>

              <Button 
                type="submit" 
                disabled={isLoading || isSocialLoading !== null}
                className="w-full h-12 bg-primary hover:bg-primary/90 text-white font-medium"
              >
                {isLoading ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Signing In...
                  </>
                ) : (
                  "Sign In"
                )}
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
                  onClick={handleGoogleLogin}
                  disabled={isSocialLoading !== null}
                  className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-60 disabled:cursor-not-allowed transition-colors"
                >
                  {isSocialLoading === 'google' ? (
                    <Loader2 className="h-5 w-5 mr-2 animate-spin" />
                  ) : (
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
                  )}
                  {isSocialLoading === 'google' ? 'Signing in...' : 'Google'}
                </button>
                <button
                  type="button"
                  onClick={handleAppleLogin}
                  disabled={isSocialLoading !== null}
                  className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-60 disabled:cursor-not-allowed transition-colors"
                >
                  {isSocialLoading === 'apple' ? (
                    <Loader2 className="h-5 w-5 mr-2 animate-spin" />
                  ) : (
                    <svg className="h-5 w-5 mr-2" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M16.365 1.43c0 1.14-.42 2.25-1.13 3.06-.75.87-1.98 1.54-3.08 1.45-.14-1.08.39-2.23 1.1-3 .78-.86 2.1-1.49 3.11-1.51.03.14.05.29.05.45zm3.36 16.09c-.84 1.22-1.72 2.43-3.08 2.46-1.33.03-1.76-.8-3.29-.8-1.52 0-2 .77-3.26.83-1.33.05-2.35-1.33-3.2-2.55-1.75-2.54-3.08-7.18-1.29-10.31.89-1.55 2.48-2.54 4.2-2.57 1.31-.03 2.55.89 3.29.89.73 0 2.1-1.1 3.54-.94.6.03 2.29.24 3.38 1.84-.09.05-2.02 1.18-2 3.52.02 2.79 2.44 3.72 2.47 3.73-.02.06-.39 1.34-1.26 2.4z" />
                    </svg>
                  )}
                  {isSocialLoading === 'apple' ? 'Signing in...' : 'Apple'}
                </button>
              </div>
            </div>

            <p className="mt-6 text-center text-sm text-gray-600">
              Don't have an account?{" "}
              <Link href="/customer-portal/register" className="text-primary hover:text-primary/80 font-medium">
                Sign up
              </Link>
            </p>

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/" className="hover:text-gray-700">
                ← Back to Home
              </Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}

// Main component with Suspense wrapper for useSearchParams
export default function CustomerLogin() {
  return (
    <Suspense fallback={
      <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin mx-auto text-primary" />
          <p className="mt-2 text-gray-500">Loading...</p>
        </div>
      </div>
    }>
      <CustomerLoginContent />
    </Suspense>
  );
}