"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Eye, EyeOff, Mail, Lock, User, Phone, Building2, Loader2, CheckCircle, XCircle, ArrowLeft } from "lucide-react";
import { 
  loginManager, 
  registerManager, 
  LoginRequest, 
  RegisterManagerRequest,
  registerBrandOwner,
  RegisterBrandOwnerRequest,
  sendCompanyVerificationCode,
  verifyCompanyEmailWithCode,
  resendCompanyVerificationCode
} from "@/lib/api";

type AuthMode = "signin" | "signup" | "brandowner" | "verify";

const SERVICES = ["Logistics", "Accommodation"];

const defaultPermissions = [
  "CanCreateStaff",
  "CanManageStaff",
  "CanViewStaff",
  "CanDeleteStaff",
  "CanViewBrands",
  "CanManageBrands",
  "CanDeleteBrands",
  "CanCreateProduct",
  "CanViewProduct",
  "CanManageProduct",
  "CanDeleteProduct",
  "CanViewWallet",
  "CanManageWallet",
  "CanViewOrders",
  "CanManageOrders",
  "CanDeleteOrders",
  "CanViewBookings",
  "CanManageBookings",
  "CanDeleteBookings",
];

type VerificationStep = 'idle' | 'sending' | 'sent' | 'verifying' | 'verified' | 'failed';

export default function CompanyLogin() {
  const router = useRouter();
  const [mode, setMode] = useState<AuthMode>("signin");
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const [signInData, setSignInData] = useState({ email: "", password: "" });
  const [signUpData, setSignUpData] = useState({
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    companyName: "",
    services: [] as string[],
    password: "",
    confirmPassword: "",
    state: "",
    locality: "",
    branch: "",
    address: "",
  });

  // Brand Owner Registration States
  const [brandOwnerStep, setBrandOwnerStep] = useState<'idle' | 'registering' | 'success' | 'failed'>('idle');
  const [brandOwnerData, setBrandOwnerData] = useState<RegisterBrandOwnerRequest | null>(null);

  // Email verification states
  const [emailVerificationStep, setEmailVerificationStep] = useState<VerificationStep>('idle');
  const [verificationCode, setVerificationCode] = useState("");
  const [verificationError, setVerificationError] = useState<string | null>(null);
  const [verificationSuccess, setVerificationSuccess] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [showCompanyForm, setShowCompanyForm] = useState(false);

  // Resend timer countdown
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  const handleSignIn = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      const credentials: LoginRequest = { Email_PhoneNo: signInData.email, Password: signInData.password };
      await loginManager(credentials);
      router.push("/admin-dashboard/company");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed. Check your credentials.");
    } finally {
      setIsLoading(false);
    }
  };

  const toggleService = (service: string) => {
    setSignUpData((prev) => ({
      ...prev,
      services: prev.services.includes(service)
        ? prev.services.filter((s) => s !== service)
        : [...prev.services, service],
    }));
  };

  // Step 1: Register Brand Owner
  const handleBrandOwnerRegistration = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    setError(null);

    if (signUpData.password !== signUpData.confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    const passwordRegex = /^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9]).{8,}$/;
    if (!passwordRegex.test(signUpData.password)) {
      setError("Password must be at least 8 characters with uppercase, lowercase, and a number.");
      return;
    }

    const cleanedPhone = signUpData.phone.replace(/\s+/g, "");
    if (!/^\+[0-9]+$/.test(cleanedPhone)) {
      setError("Phone number must start with + and contain only digits (e.g. +2348000000000).");
      return;
    }

    if (signUpData.services.length === 0) {
      setError("Please select at least one service.");
      return;
    }

    if (!signUpData.state || !signUpData.locality || !signUpData.branch || !signUpData.address) {
      setError("Please fill in all location fields (State, Locality, Branch, Address).");
      return;
    }

    setIsLoading(true);
    setBrandOwnerStep('registering');

    try {
      const staffCode = signUpData.firstName.slice(0, 3).toUpperCase() + Math.floor(1000 + Math.random() * 9000);
      const companyUserName = signUpData.companyName.toLowerCase().replace(/\s+/g, "_");

      const payload: RegisterBrandOwnerRequest = {
        staffType: "BrandOwner",
        firstName: signUpData.firstName,
        surName: signUpData.lastName,
        email: signUpData.email,
        password: signUpData.password,
        sex: "Male",
        staffCode: staffCode,
        phoneNo: cleanedPhone,
        address: signUpData.address,
        companyName: signUpData.companyName,
        companyUserName: companyUserName,
        companyType: signUpData.services,
        roles: ["BrandOwner"],
        permissions: defaultPermissions,
        state: signUpData.state,
        branch: signUpData.branch,
        locality: signUpData.locality,
      };

      setBrandOwnerData(payload);
      const result = await registerBrandOwner(payload);
      
      setBrandOwnerStep('success');
      setSuccess("Brand Owner registered successfully! Please verify your email to continue.");
      
      // Move to verification step
      setMode('verify');
      setShowCompanyForm(false);
      
      // Send verification code
      await sendCompanyVerificationCode(signUpData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("Verification code sent! Please check your email.");
      
    } catch (err) {
      setBrandOwnerStep('failed');
      setError(err instanceof Error ? err.message : "Brand Owner registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  // Step 2: Verify Email
  const handleSendVerification = async (e: React.FormEvent) => {
    e.preventDefault();
    setVerificationError(null);
    setVerificationSuccess(null);
    
    if (!signUpData.email) {
      setVerificationError("No email found. Please go back and register.");
      return;
    }

    setEmailVerificationStep('sending');
    
    try {
      await sendCompanyVerificationCode(signUpData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("Verification code sent! Please check your email.");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to send verification code. Please try again.");
      setEmailVerificationStep('idle');
    }
  };

  const handleVerifyCode = async (e: React.FormEvent) => {
    e.preventDefault();
    setVerificationError(null);
    setVerificationSuccess(null);
    
    if (!verificationCode || verificationCode.length !== 5) {
      setVerificationError("Please enter a valid 5-digit verification code.");
      return;
    }

    setEmailVerificationStep('verifying');
    
    try {
      const result = await verifyCompanyEmailWithCode(signUpData.email, verificationCode);
      
      if (result.isValid) {
        setEmailVerificationStep('verified');
        setVerificationSuccess("Email verified successfully!");
        setShowCompanyForm(true);
        setMode('verify');
        setTimeout(() => {
          document.getElementById('company-form')?.scrollIntoView({ behavior: 'smooth' });
        }, 300);
      } else {
        throw new Error(result.message || 'Invalid verification code');
      }
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Verification failed. Please try again.");
      setEmailVerificationStep('sent');
      setVerificationCode("");
    }
  };

  const handleResendCode = async () => {
    if (resendTimer > 0) return;
    
    setVerificationError(null);
    setVerificationSuccess(null);
    setEmailVerificationStep('sending');
    
    try {
      await resendCompanyVerificationCode(signUpData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("New verification code sent!");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to resend code.");
      setEmailVerificationStep('sent');
    }
  };

  const goBackToRegistration = () => {
    setMode('signup');
    setEmailVerificationStep('idle');
    setVerificationError(null);
    setVerificationSuccess(null);
    setVerificationCode("");
    setShowCompanyForm(false);
  };

  // Render verification screen
  const renderVerification = () => (
    <div className="space-y-6">
      <div className="text-center">
        <div className="mx-auto w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mb-4">
          <Mail className="h-8 w-8 text-blue-600" />
        </div>
        <h2 className="text-2xl font-semibold text-gray-900">Verify Your Email</h2>
        <p className="mt-2 text-sm text-gray-600">
          We'll send a verification code to <strong>{signUpData.email}</strong>
        </p>
      </div>

      {verificationError && (
        <div className="rounded-lg bg-red-50 border border-red-200 p-4 text-red-800 flex items-start gap-2">
          <XCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <span>{verificationError}</span>
        </div>
      )}

      {verificationSuccess && (
        <div className="rounded-lg bg-green-50 border border-green-200 p-4 text-green-800 flex items-start gap-2">
          <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <span>{verificationSuccess}</span>
        </div>
      )}

      {!showCompanyForm ? (
        <form onSubmit={handleSendVerification} className="space-y-4">
          {emailVerificationStep === 'idle' && (
            <Button 
              type="submit" 
              disabled={isLoading}
              className="w-full h-12 bg-blue-600 hover:bg-blue-700 text-white font-medium"
            >
              Send Verification Code
            </Button>
          )}

          {emailVerificationStep === 'sending' && (
            <Button disabled className="w-full h-12 bg-blue-600 text-white font-medium">
              <Loader2 className="h-4 w-4 animate-spin mr-2" />
              Sending Code...
            </Button>
          )}

          {emailVerificationStep === 'sent' && (
            <div className="space-y-4">
              <div className="rounded-lg bg-blue-50 border border-blue-200 p-4">
                <div className="flex items-start gap-3">
                  <CheckCircle className="h-5 w-5 text-blue-600 flex-shrink-0 mt-0.5" />
                  <div>
                    <p className="font-medium text-blue-800">Verification code sent!</p>
                    <p className="text-sm text-blue-700">Please check your email for the 5-digit code.</p>
                  </div>
                </div>
              </div>

              <div>
                <Label htmlFor="verificationCode" className="text-gray-700">Verification Code</Label>
                <Input
                  id="verificationCode"
                  type="text"
                  value={verificationCode}
                  onChange={(e) => setVerificationCode(e.target.value.replace(/\D/g, '').slice(0, 5))}
                  placeholder="Enter 5-digit code"
                  maxLength={5}
                  className="mt-1 text-center text-2xl tracking-widest"
                  autoFocus
                />
                <p className="text-xs text-gray-500 mt-1">Enter the 5-digit code sent to your email</p>
              </div>

              <div className="flex flex-col gap-3 sm:flex-row">
                <Button 
                  type="button"
                  onClick={handleResendCode}
                  disabled={resendTimer > 0}
                  variant="outline"
                  className="flex-1"
                >
                  {resendTimer > 0 ? `Resend in ${resendTimer}s` : "Resend Code"}
                </Button>
                <Button 
                  type="button"
                  onClick={handleVerifyCode}
                  disabled={verificationCode.length !== 5 || emailVerificationStep === 'verifying'}
                  className="flex-1 bg-blue-600 hover:bg-blue-700 text-white"
                >
                  {emailVerificationStep === 'verifying' ? (
                    <><Loader2 className="h-4 w-4 animate-spin mr-2" />Verifying...</>
                  ) : (
                    "Verify Email"
                  )}
                </Button>
              </div>

              <button
                type="button"
                onClick={goBackToRegistration}
                className="text-sm text-gray-600 hover:text-gray-900 underline flex items-center gap-1 mx-auto"
              >
                <ArrowLeft className="h-3 w-3" />
                Back to registration
              </button>
            </div>
          )}

          {emailVerificationStep === 'verifying' && (
            <div className="w-full h-12 bg-blue-600 text-white font-medium rounded-lg flex items-center justify-center">
              <Loader2 className="h-4 w-4 animate-spin mr-2" />
              Verifying...
            </div>
          )}
        </form>
      ) : (
        // Company Registration Form (Step 3)
        <div id="company-form" className="space-y-4">
          <div className="mb-4 p-4 rounded-lg bg-green-50 border border-green-200">
            <div className="flex items-start gap-3">
              <CheckCircle className="h-5 w-5 text-green-600 flex-shrink-0 mt-0.5" />
              <div>
                <p className="font-medium text-green-800">Email Verified Successfully!</p>
                <p className="text-sm text-green-700">Now complete your company registration.</p>
              </div>
            </div>
          </div>

          <div className="rounded-lg bg-yellow-50 border border-yellow-200 p-4">
            <p className="text-sm text-yellow-800">
              <strong>Brand Owner:</strong> {signUpData.firstName} {signUpData.lastName} ({signUpData.email})
            </p>
            <p className="text-sm text-yellow-700">
              <strong>Company:</strong> {signUpData.companyName}
            </p>
          </div>

          <div className="flex flex-col gap-3 sm:flex-row">
            <Button 
              type="button"
              onClick={() => {
                // Navigate to company dashboard or login
                router.push("/admin-dashboard/company-login");
              }}
              className="flex-1 bg-primary hover:bg-primary/90 text-white h-12"
            >
              Go to Company Login
            </Button>
            <Button 
              type="button"
              onClick={() => {
                // Navigate to company dashboard
                router.push("/admin-dashboard/company");
              }}
              variant="outline"
              className="flex-1 h-12"
            >
              Go to Dashboard
            </Button>
          </div>
          
          <button
            type="button"
            onClick={goBackToRegistration}
            className="text-sm text-gray-600 hover:text-gray-900 underline flex items-center gap-1 mx-auto"
          >
            <ArrowLeft className="h-3 w-3" />
            Back to verification
          </button>
        </div>
      )}
    </div>
  );

  return (
    <div className="min-h-screen flex">
      {/* Brand Panel */}
      <aside className="hidden lg:flex lg:w-1/2 bg-primary relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-primary to-primary/80" />
        <div className="relative z-10 flex flex-col justify-center items-center w-full px-12 text-white">
          <Building2 className="h-14 w-14 mb-6" />
          <h1 className="text-4xl font-bold mb-4">GINILOG</h1>
          <p className="text-xl text-center mb-4">Company &amp; Manager Portal</p>
          <p className="text-white/80 text-center max-w-md">
            Manage your logistics operations, track deliveries, handle bookings, and serve your customers — all in one place.
          </p>
          <div className="mt-12 flex gap-8">
            <div className="text-center">
              <div className="text-3xl font-bold">10K+</div>
              <div className="text-white/70 text-sm">Deliveries</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">500+</div>
              <div className="text-white/70 text-sm">Companies</div>
            </div>
            <div className="text-center">
              <div className="text-3xl font-bold">50+</div>
              <div className="text-white/70 text-sm">Cities</div>
            </div>
          </div>
        </div>
      </aside>

      {/* Form Panel */}
      <main className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-white py-8">
        <div className="w-full max-w-md">
          <div className="lg:hidden text-center mb-8">
            <Building2 className="h-10 w-10 text-primary mx-auto mb-3" />
            <h1 className="text-2xl font-bold text-gray-900">GINILOG</h1>
            <p className="text-gray-600 mt-1">Company &amp; Manager Portal</p>
          </div>

          <div className="bg-white rounded-lg">
            <div className="border-b border-gray-200 mb-6">
              <div className="flex">
                <button
                  onClick={() => { setMode("signin"); setError(null); setSuccess(null); setBrandOwnerStep('idle'); }}
                  className={`flex-1 py-3 text-center font-medium transition-colors ${
                    mode === "signin" ? "text-primary border-b-2 border-primary" : "text-gray-500 hover:text-gray-700"
                  }`}
                >
                  Sign In
                </button>
                <button
                  onClick={() => { setMode("signup"); setError(null); setSuccess(null); setBrandOwnerStep('idle'); }}
                  className={`flex-1 py-3 text-center font-medium transition-colors ${
                    mode === "signup" ? "text-primary border-b-2 border-primary" : "text-gray-500 hover:text-gray-700"
                  }`}
                >
                  Register Company
                </button>
              </div>
            </div>

            {error && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm">{error}</div>
            )}
            {success && (
              <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-md text-green-700 text-sm">{success}</div>
            )}

            {mode === "signin" ? (
              <form onSubmit={handleSignIn} className="space-y-4">
                <div>
                  <Label htmlFor="email">Email</Label>
                  <div className="relative mt-1">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="email"
                      type="email"
                      placeholder="you@company.com"
                      className="pl-10 h-12"
                      value={signInData.email}
                      onChange={(e) => setSignInData({ ...signInData, email: e.target.value })}
                      required
                    />
                  </div>
                </div>
                <div>
                  <Label htmlFor="password">Password</Label>
                  <div className="relative mt-1">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                    <Input
                      id="password"
                      type={showPassword ? "text" : "password"}
                      placeholder="••••••••"
                      className="pl-10 pr-10 h-12"
                      value={signInData.password}
                      onChange={(e) => setSignInData({ ...signInData, password: e.target.value })}
                      required
                    />
                    <button type="button" onClick={() => setShowPassword(!showPassword)}
                      className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                      {showPassword ? <EyeOff className="h-5 w-5" /> : <Eye className="h-5 w-5" />}
                    </button>
                  </div>
                </div>
                <Button type="submit" disabled={isLoading} className="w-full h-12 bg-primary hover:bg-primary/90 text-white">
                  {isLoading && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
                  {isLoading ? "Signing In..." : "Sign In"}
                </Button>
              </form>
            ) : mode === "verify" ? (
              renderVerification()
            ) : (
              // Sign Up Form - Brand Owner Registration
              <form onSubmit={handleBrandOwnerRegistration} className="space-y-4">
                {/* Name */}
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="firstName">First Name</Label>
                    <div className="relative mt-1">
                      <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="firstName" placeholder="John" className="pl-9 h-11"
                        value={signUpData.firstName}
                        onChange={(e) => setSignUpData({ ...signUpData, firstName: e.target.value })} required />
                    </div>
                  </div>
                  <div>
                    <Label htmlFor="lastName">Last Name</Label>
                    <div className="relative mt-1">
                      <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="lastName" placeholder="Doe" className="pl-9 h-11"
                        value={signUpData.lastName}
                        onChange={(e) => setSignUpData({ ...signUpData, lastName: e.target.value })} required />
                    </div>
                  </div>
                </div>

                {/* Phone */}
                <div>
                  <Label htmlFor="phone">Phone Number</Label>
                  <div className="relative mt-1">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="phone" type="tel" placeholder="+2348000000000" className="pl-9 h-11"
                      value={signUpData.phone}
                      onChange={(e) => setSignUpData({ ...signUpData, phone: e.target.value })} required />
                  </div>
                </div>

                {/* Company Email */}
                <div>
                  <Label htmlFor="signUpEmail">Email</Label>
                  <div className="relative mt-1">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="signUpEmail" type="email" placeholder="info@company.com" className="pl-9 h-11"
                      value={signUpData.email}
                      onChange={(e) => setSignUpData({ ...signUpData, email: e.target.value })} required />
                  </div>
                </div>

                {/* Company Name */}
                <div>
                  <Label htmlFor="companyName">Company Name</Label>
                  <div className="relative mt-1">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="companyName" placeholder="Acme Logistics Ltd" className="pl-9 h-11"
                      value={signUpData.companyName}
                      onChange={(e) => setSignUpData({ ...signUpData, companyName: e.target.value })} required />
                  </div>
                </div>

                {/* Location Fields */}
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="state">State</Label>
                    <Input id="state" placeholder="Lagos" className="h-11"
                      value={signUpData.state}
                      onChange={(e) => setSignUpData({ ...signUpData, state: e.target.value })} required />
                  </div>
                  <div>
                    <Label htmlFor="locality">Locality</Label>
                    <Input id="locality" placeholder="Ikeja" className="h-11"
                      value={signUpData.locality}
                      onChange={(e) => setSignUpData({ ...signUpData, locality: e.target.value })} required />
                  </div>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="branch">Branch</Label>
                    <Input id="branch" placeholder="Main" className="h-11"
                      value={signUpData.branch}
                      onChange={(e) => setSignUpData({ ...signUpData, branch: e.target.value })} required />
                  </div>
                  <div>
                    <Label htmlFor="address">Address</Label>
                    <Input id="address" placeholder="123 Main St" className="h-11"
                      value={signUpData.address}
                      onChange={(e) => setSignUpData({ ...signUpData, address: e.target.value })} required />
                  </div>
                </div>

                {/* Services */}
                <div>
                  <Label>Services Offered</Label>
                  <div className="mt-2 flex gap-4">
                    {SERVICES.map((service) => (
                      <label key={service} className="flex items-center gap-2 cursor-pointer">
                        <input
                          type="checkbox"
                          className="h-4 w-4 text-primary border-gray-300 rounded focus:ring-primary"
                          checked={signUpData.services.includes(service)}
                          onChange={() => toggleService(service)}
                        />
                        <span className="text-sm text-gray-700">{service}</span>
                      </label>
                    ))}
                  </div>
                </div>

                {/* Password */}
                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="signUpPassword">Password</Label>
                    <div className="relative mt-1">
                      <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="signUpPassword" type={showPassword ? "text" : "password"} placeholder="••••••••" className="pl-9 pr-9 h-11"
                        value={signUpData.password}
                        onChange={(e) => setSignUpData({ ...signUpData, password: e.target.value })} required />
                      <button type="button" onClick={() => setShowPassword(!showPassword)}
                        className="absolute right-2 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600">
                        {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                      </button>
                    </div>
                  </div>
                  <div>
                    <Label htmlFor="confirmPassword">Confirm</Label>
                    <div className="relative mt-1">
                      <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="confirmPassword" type={showPassword ? "text" : "password"} placeholder="••••••••" className="pl-9 h-11"
                        value={signUpData.confirmPassword}
                        onChange={(e) => setSignUpData({ ...signUpData, confirmPassword: e.target.value })} required />
                    </div>
                  </div>
                </div>

                <Button type="submit" disabled={isLoading || brandOwnerStep === 'registering'} className="w-full h-12 bg-primary hover:bg-primary/90 text-white">
                  {isLoading || brandOwnerStep === 'registering' ? (
                    <><Loader2 className="h-4 w-4 animate-spin mr-2" />Registering Brand Owner...</>
                  ) : (
                    "Register Brand Owner"
                  )}
                </Button>

                {brandOwnerStep === 'success' && (
                  <div className="p-3 bg-green-50 border border-green-200 rounded-md text-green-700 text-sm">
                    ✅ Brand Owner registered successfully! Now verify your email.
                  </div>
                )}
              </form>
            )}

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/" className="hover:text-gray-700">← Back to Home</Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}