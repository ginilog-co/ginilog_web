"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Loader2, Mail, CheckCircle, XCircle, ArrowLeft } from "lucide-react";
import { 
  registerBrandOwner, 
  RegisterBrandOwnerRequest,
  sendCompanyVerificationCode,
  verifyCompanyEmailWithCode,
  resendCompanyVerificationCode
} from "@/lib/api";

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

// Define the type properly
type VerificationStep = 'idle' | 'sending' | 'sent' | 'verifying' | 'verified' | 'failed';

export default function CompanyRegister() {
  const router = useRouter();
  
  // Registration form state
  const [formData, setFormData] = useState({
    firstName: "",
    surName: "",
    email: "",
    password: "",
    confirmPassword: "",
    sex: "Male",
    phoneNo: "",
    address: "",
    staffCode: "",
    companyName: "",
    companyUserName: "",
    companyType: "",
    state: "",
    branch: "",
    locality: "",
    companyInfo: "",
  });
  
  // Email verification states - Using the proper type
  const [emailVerificationStep, setEmailVerificationStep] = useState<VerificationStep>('idle');
  const [verificationCode, setVerificationCode] = useState("");
  const [verificationError, setVerificationError] = useState<string | null>(null);
  const [verificationSuccess, setVerificationSuccess] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [showCompanyForm, setShowCompanyForm] = useState(false);
  
  // Registration states
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  // Resend timer countdown
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  // Step 1: Send verification email
  const handleSendVerification = async (e: React.FormEvent) => {
    e.preventDefault();
    setVerificationError(null);
    setVerificationSuccess(null);
    
    if (!formData.email) {
      setVerificationError("Please enter your email address.");
      return;
    }

    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.email)) {
      setVerificationError("Please enter a valid email address.");
      return;
    }

    setEmailVerificationStep('sending');
    
    try {
      // Use company-specific verification
      await sendCompanyVerificationCode(formData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("Verification code sent! Please check your email.");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to send verification code. Please try again.");
      setEmailVerificationStep('idle');
    }
  };

  // Step 2: Verify the code (5-digit code)
  const handleVerifyCode = async (e: React.FormEvent) => {
    e.preventDefault();
    setVerificationError(null);
    setVerificationSuccess(null);
    
    // Check for exactly 5 digits
    if (!verificationCode || verificationCode.length !== 5) {
      setVerificationError("Please enter a valid 5-digit verification code.");
      return;
    }

    setEmailVerificationStep('verifying');
    
    try {
      // Use company-specific verification
      const result = await verifyCompanyEmailWithCode(formData.email, verificationCode);
      
      if ((result as any).isValid) {
        setEmailVerificationStep('verified');
        setVerificationSuccess("Email verified successfully!");
        setShowCompanyForm(true);
        setTimeout(() => {
          document.getElementById('company-form')?.scrollIntoView({ behavior: 'smooth' });
        }, 300);
      } else {
        throw new Error((result as any).message || 'Invalid verification code');
      }
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Verification failed. Please try again.");
      setEmailVerificationStep('sent');
      setVerificationCode("");
    }
  };

  // Resend verification code
  const handleResendCode = async () => {
    if (resendTimer > 0) return;
    
    setVerificationError(null);
    setVerificationSuccess(null);
    setEmailVerificationStep('sending');
    
    try {
      // Use company-specific resend
      await resendCompanyVerificationCode(formData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("New verification code sent!");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to resend code.");
      setEmailVerificationStep('sent');
    }
  };

  // Go back to verification
  const goBackToVerification = () => {
    setShowCompanyForm(false);
    setEmailVerificationStep('sent');
    setVerificationError(null);
    setVerificationSuccess(null);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  // Step 3: Register the brand owner with company details
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(false);

    if (formData.password !== formData.confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    if (formData.password.length < 8) {
      setError("Password must be at least 8 characters long.");
      return;
    }

    if (!formData.companyType) {
      setError("Please enter a company type.");
      return;
    }

    setIsLoading(true);

    try {
      const companyUserName =
        formData.companyUserName ||
        formData.companyName.toLowerCase().replace(/\s+/g, "");

      const payload: RegisterBrandOwnerRequest = {
        staffType: "BrandOwner",
        firstName: formData.firstName,
        surName: formData.surName,
        email: formData.email,
        password: formData.password,
        sex: formData.sex as "Male" | "Female" | "Other",
        staffCode: formData.staffCode || `${formData.firstName}${Date.now().toString().slice(-4)}`,
        phoneNo: formData.phoneNo,
        address: formData.address || "N/A",
        companyName: formData.companyName,
        companyUserName,
        companyType: [formData.companyType],
        roles: ["BrandOwner"],
        permissions: defaultPermissions,
        state: formData.state,
        locality: formData.locality,
        branch: formData.branch,
      };

      await registerBrandOwner(payload);
      setSuccess(true);
      
      const email = formData.email;
      setFormData({
        firstName: "",
        surName: "",
        email: email,
        password: "",
        confirmPassword: "",
        sex: "Male",
        phoneNo: "",
        address: "",
        staffCode: "",
        companyName: "",
        companyUserName: "",
        companyType: "",
        state: "",
        branch: "",
        locality: "",
        companyInfo: "",
      });
      
      setVerificationCode("");
      
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  // Render email verification screen
  const renderEmailVerification = () => (
    <div className="space-y-6">
      <div className="text-center">
        <div className="mx-auto w-16 h-16 bg-blue-100 rounded-full flex items-center justify-center mb-4">
          <Mail className="h-8 w-8 text-blue-600" />
        </div>
        <h2 className="text-2xl font-semibold text-gray-900">Verify Your Email</h2>
        <p className="mt-2 text-sm text-gray-600">
          We'll send a verification code to <strong>{formData.email || "your email"}</strong>
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

      <form onSubmit={handleSendVerification} className="space-y-4">
        <div>
          <Label htmlFor="email" className="text-gray-700">Email Address</Label>
          <Input
            id="email"
            name="email"
            type="email"
            value={formData.email}
            onChange={handleChange}
            placeholder="Enter your email"
            disabled={emailVerificationStep === 'sending' || emailVerificationStep === 'sent' || emailVerificationStep === 'verifying' || emailVerificationStep === 'verified'}
            required
            className="mt-1"
          />
        </div>

        {emailVerificationStep === 'idle' && (
          <Button 
            type="submit" 
            disabled={!formData.email}
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
                disabled={verificationCode.length !== 5}
                className="flex-1 bg-blue-600 hover:bg-blue-700 text-white"
              >
                Verify Email
              </Button>
            </div>

            <button
              type="button"
              onClick={() => {
                setEmailVerificationStep('idle');
                setVerificationError(null);
                setVerificationSuccess(null);
                setVerificationCode("");
              }}
              className="text-sm text-gray-600 hover:text-gray-900 underline flex items-center gap-1 mx-auto"
            >
              <ArrowLeft className="h-3 w-3" />
              Change email address
            </button>
          </div>
        )}

        {/* Verifying state */}
        {emailVerificationStep === 'verifying' && (
          <div className="w-full h-12 bg-blue-600 text-white font-medium rounded-lg flex items-center justify-center">
            <Loader2 className="h-4 w-4 animate-spin mr-2" />
            Verifying...
          </div>
        )}
      </form>

      <div className="text-center border-t pt-4 mt-4">
        <Link href="/brand-owner/login" className="text-sm text-gray-600 hover:text-gray-900">
          ← Back to login
        </Link>
      </div>
    </div>
  );

  // Render company registration form
  const renderCompanyForm = () => (
    <div id="company-form">
      <div className="mb-6 p-4 rounded-lg bg-green-50 border border-green-200">
        <div className="flex items-start gap-3">
          <CheckCircle className="h-5 w-5 text-green-600 flex-shrink-0 mt-0.5" />
          <div>
            <p className="font-medium text-green-800">Email Verified Successfully!</p>
            <p className="text-sm text-green-700">Now complete your company registration.</p>
          </div>
        </div>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">
        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="firstName" className="text-gray-700">First Name *</Label>
            <Input
              id="firstName"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
          <div>
            <Label htmlFor="surName" className="text-gray-700">Surname *</Label>
            <Input
              id="surName"
              name="surName"
              value={formData.surName}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="phoneNo" className="text-gray-700">Phone Number *</Label>
            <Input
              id="phoneNo"
              name="phoneNo"
              type="tel"
              value={formData.phoneNo}
              onChange={handleChange}
              required
              className="mt-1"
              placeholder="08012345678"
            />
          </div>
          <div>
            <Label htmlFor="staffCode" className="text-gray-700">Staff Code</Label>
            <Input
              id="staffCode"
              name="staffCode"
              value={formData.staffCode}
              onChange={handleChange}
              className="mt-1"
              placeholder="Optional"
            />
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="password" className="text-gray-700">Password *</Label>
            <Input
              id="password"
              name="password"
              type="password"
              value={formData.password}
              onChange={handleChange}
              required
              className="mt-1"
              placeholder="Min 8 characters"
            />
          </div>
          <div>
            <Label htmlFor="confirmPassword" className="text-gray-700">Confirm Password *</Label>
            <Input
              id="confirmPassword"
              name="confirmPassword"
              type="password"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="companyName" className="text-gray-700">Company Name *</Label>
            <Input
              id="companyName"
              name="companyName"
              value={formData.companyName}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
          <div>
            <Label htmlFor="companyType" className="text-gray-700">Company Type *</Label>
            <Input
              id="companyType"
              name="companyType"
              value={formData.companyType}
              onChange={handleChange}
              required
              className="mt-1"
              placeholder="e.g. Retail, Manufacturing"
            />
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="state" className="text-gray-700">State *</Label>
            <Input
              id="state"
              name="state"
              value={formData.state}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
          <div>
            <Label htmlFor="locality" className="text-gray-700">Locality *</Label>
            <Input
              id="locality"
              name="locality"
              value={formData.locality}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
        </div>

        <div className="grid gap-4 md:grid-cols-2">
          <div>
            <Label htmlFor="branch" className="text-gray-700">Branch *</Label>
            <Input
              id="branch"
              name="branch"
              value={formData.branch}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
          <div>
            <Label htmlFor="address" className="text-gray-700">Address *</Label>
            <Input
              id="address"
              name="address"
              value={formData.address}
              onChange={handleChange}
              required
              className="mt-1"
            />
          </div>
        </div>

        <div>
          <Label htmlFor="companyInfo" className="text-gray-700">Company Info (Optional)</Label>
          <Input
            id="companyInfo"
            name="companyInfo"
            value={formData.companyInfo}
            onChange={handleChange}
            className="mt-1"
            placeholder="Brief description of your company"
          />
        </div>

        <div>
          <Label htmlFor="sex" className="text-gray-700">Gender *</Label>
          <select
            id="sex"
            name="sex"
            value={formData.sex}
            onChange={handleChange}
            className="mt-1 w-full rounded-lg border border-gray-300 px-3 py-2 focus:ring-2 focus:ring-blue-500 focus:border-transparent"
            required
          >
            <option value="Male">Male</option>
            <option value="Female">Female</option>
            <option value="Other">Other</option>
          </select>
        </div>

        {error && (
          <div className="rounded-lg bg-red-50 border border-red-200 p-4 text-red-800 flex items-start gap-2">
            <XCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
            <span>{error}</span>
          </div>
        )}

        {success && (
          <div className="rounded-lg bg-green-50 border border-green-200 p-4 text-green-800 flex items-start gap-2">
            <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium">Registration Successful!</p>
              <p className="text-sm">Please wait for admin approval, then you can log in.</p>
            </div>
          </div>
        )}

        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <button
            type="button"
            onClick={goBackToVerification}
            className="text-sm text-gray-600 hover:text-gray-900 underline flex items-center gap-1"
          >
            <ArrowLeft className="h-3 w-3" />
            Back to verification
          </button>
          <Button 
            type="submit" 
            disabled={isLoading || success} 
            className="w-full sm:w-auto h-12 bg-gray-900 hover:bg-gray-800 text-white font-medium"
          >
            {isLoading ? <><Loader2 className="h-4 w-4 animate-spin mr-2" />Registering...</> : "Complete Registration"}
          </Button>
        </div>
      </form>
    </div>
  );

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-3xl w-full bg-white shadow-sm rounded-2xl border border-gray-200 p-8">
        <div className="mb-8 text-center">
          <h1 className="text-3xl font-bold text-gray-900">Company Registration</h1>
          <p className="mt-2 text-sm text-gray-600">
            {!showCompanyForm 
              ? "Verify your email address to get started."
              : "Complete your company profile to register."
            }
          </p>
        </div>

        {!showCompanyForm ? renderEmailVerification() : renderCompanyForm()}
      </div>
    </div>
  );
}