"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Eye, EyeOff, Mail, Lock, User, Phone, Building2, Loader2, CheckCircle, XCircle, ArrowLeft, Store } from "lucide-react";
import { 
  loginManager, 
  LoginRequest, 
  registerBrandOwner,
  RegisterBrandOwnerRequest,
  sendCompanyVerificationCode,
  verifyCompanyEmailWithCode,
  resendCompanyVerificationCode,
  addCompany,
  getStoredUser,
  isAuthenticated
} from "@/lib/api";

type AuthMode = "signin" | "signup" | "verify" | "company-register";

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

// ✅ FIXED: Added 'verifying' to the type
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

  // Company Registration Data
  const [companyData, setCompanyData] = useState({
    companyName: "",
    companyEmail: "",
    phoneNumber: "",
    companyAddress: "",
    state: "",
    locality: "",
    valueCharge: 0,
    companyInfo: "",
    noOfTrucks: 0,
    nofOfBikes: 0,
    bankName: "",
    accountName: "",
    accountNumber: "",
    deliveryTypes: [] as string[],
    serviceAreas: [] as string[],
  });

  const [deliveryTypeInput, setDeliveryTypeInput] = useState("");
  const [serviceAreaInput, setServiceAreaInput] = useState("");

  // Email verification states
  const [emailVerificationStep, setEmailVerificationStep] = useState<VerificationStep>('idle');
  const [verificationCode, setVerificationCode] = useState("");
  const [verificationError, setVerificationError] = useState<string | null>(null);
  const [verificationSuccess, setVerificationSuccess] = useState<string | null>(null);
  const [resendTimer, setResendTimer] = useState(0);
  const [registeredEmail, setRegisteredEmail] = useState("");

  // Resend timer countdown
  useEffect(() => {
    if (resendTimer > 0) {
      const timer = setTimeout(() => setResendTimer(resendTimer - 1), 1000);
      return () => clearTimeout(timer);
    }
  }, [resendTimer]);

  // Check if user is already authenticated
  useEffect(() => {
    if (isAuthenticated()) {
      const user = getStoredUser();
      if (user?.userType === "BrandOwner" || user?.userType === "Manager") {
        router.push("/admin-dashboard/company");
      }
    }
  }, [router]);

  const handleSignIn = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    setIsLoading(true);
    setError(null);
    try {
      const credentials: LoginRequest = { Email_PhoneNo: signInData.email, Password: signInData.password };
      const result = await loginManager(credentials);
      
      if (result.userType === "BrandOwner" || result.userType === "Manager") {
        router.push("/admin-dashboard/company");
      } else {
        setError("This account is not authorized for company access.");
      }
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

  const addDeliveryType = () => {
    if (deliveryTypeInput.trim() && !companyData.deliveryTypes.includes(deliveryTypeInput.trim())) {
      setCompanyData({
        ...companyData,
        deliveryTypes: [...companyData.deliveryTypes, deliveryTypeInput.trim()]
      });
      setDeliveryTypeInput("");
    }
  };

  const removeDeliveryType = (type: string) => {
    setCompanyData({
      ...companyData,
      deliveryTypes: companyData.deliveryTypes.filter(t => t !== type)
    });
  };

  const addServiceArea = () => {
    if (serviceAreaInput.trim() && !companyData.serviceAreas.includes(serviceAreaInput.trim())) {
      setCompanyData({
        ...companyData,
        serviceAreas: [...companyData.serviceAreas, serviceAreaInput.trim()]
      });
      setServiceAreaInput("");
    }
  };

  const removeServiceArea = (area: string) => {
    setCompanyData({
      ...companyData,
      serviceAreas: companyData.serviceAreas.filter(a => a !== area)
    });
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

      await registerBrandOwner(payload);
      
      setRegisteredEmail(signUpData.email);
      setSuccess("Brand Owner registered successfully! Please verify your email.");
      
      setMode('verify');
      
      await sendCompanyVerificationCode(signUpData.email);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("Verification code sent! Please check your email.");
      
    } catch (err) {
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
    
    if (!registeredEmail) {
      setVerificationError("No email found. Please go back and register.");
      return;
    }

    setEmailVerificationStep('sending');
    
    try {
      await sendCompanyVerificationCode(registeredEmail);
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
      const result = await verifyCompanyEmailWithCode(registeredEmail, verificationCode);
      
      if (result.isValid) {
        setEmailVerificationStep('verified');
        setVerificationSuccess("Email verified successfully! Now complete your company registration.");
        
        setTimeout(() => {
          setMode('company-register');
          setCompanyData({
            ...companyData,
            companyName: signUpData.companyName,
            companyEmail: registeredEmail,
            phoneNumber: signUpData.phone,
            state: signUpData.state,
            locality: signUpData.locality,
          });
          setEmailVerificationStep('idle');
          setVerificationSuccess(null);
        }, 1500);
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
      await resendCompanyVerificationCode(registeredEmail);
      setEmailVerificationStep('sent');
      setResendTimer(60);
      setVerificationSuccess("New verification code sent!");
    } catch (err) {
      setVerificationError(err instanceof Error ? err.message : "Failed to resend code.");
      setEmailVerificationStep('sent');
    }
  };

  // Step 3: Company Registration
  const handleCompanyRegistration = async (e: { preventDefault(): void }) => {
    e.preventDefault();
    setError(null);

    if (!companyData.companyName) {
      setError("Company name is required.");
      return;
    }
    if (!companyData.companyEmail) {
      setError("Company email is required.");
      return;
    }
    if (!companyData.phoneNumber) {
      setError("Phone number is required.");
      return;
    }
    if (companyData.valueCharge <= 0) {
      setError("Value charge must be greater than 0.");
      return;
    }
    if (!companyData.state) {
      setError("State is required.");
      return;
    }
    if (!companyData.locality) {
      setError("Locality is required.");
      return;
    }
    if (!companyData.companyAddress) {
      setError("Company address is required.");
      return;
    }

    setIsLoading(true);

    try {
      await addCompany({
        companyName: companyData.companyName,
        companyEmail: companyData.companyEmail,
        phoneNumber: companyData.phoneNumber,
        companyAddress: companyData.companyAddress,
        state: companyData.state,
        locality: companyData.locality,
        valueCharge: companyData.valueCharge,
        companyInfo: companyData.companyInfo,
        noOfTrucks: companyData.noOfTrucks,
        nofOfBikes: companyData.nofOfBikes,
        bankName: companyData.bankName,
        accountName: companyData.accountName,
        accountNumber: companyData.accountNumber,
        deliveryTypes: companyData.deliveryTypes,
        serviceAreas: companyData.serviceAreas,
        companyStatus: "pending",
      });

      setSuccess("Company registered successfully! Redirecting to dashboard...");
      
      setTimeout(() => {
        router.push("/admin-dashboard/company");
      }, 2000);
      
    } catch (err) {
      setError(err instanceof Error ? err.message : "Company registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  const goBackToRegistration = () => {
    setMode('signup');
    setEmailVerificationStep('idle');
    setVerificationError(null);
    setVerificationSuccess(null);
    setVerificationCode("");
    setRegisteredEmail("");
  };

  const goBackToVerification = () => {
    setMode('verify');
    setEmailVerificationStep('sent');
    setVerificationError(null);
    setVerificationSuccess(null);
    setCompanyData({
      companyName: "",
      companyEmail: "",
      phoneNumber: "",
      companyAddress: "",
      state: "",
      locality: "",
      valueCharge: 0,
      companyInfo: "",
      noOfTrucks: 0,
      nofOfBikes: 0,
      bankName: "",
      accountName: "",
      accountNumber: "",
      deliveryTypes: [],
      serviceAreas: [],
    });
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
          We sent a verification code to <strong>{registeredEmail}</strong>
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

      <div className="space-y-4">
        {emailVerificationStep === 'idle' && (
          <Button 
            type="button"
            onClick={handleSendVerification}
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
  disabled={verificationCode.length !== 5 || (emailVerificationStep as string) === 'verifying'}
  className="flex-1 bg-blue-600 hover:bg-blue-700 text-white"
>
  {(emailVerificationStep as string) === 'verifying' ? (
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

        {emailVerificationStep === 'verified' && (
          <div className="rounded-lg bg-green-50 border border-green-200 p-4 text-green-800 flex items-start gap-2">
            <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
            <div>
              <p className="font-medium">Email Verified Successfully!</p>
              <p className="text-sm">Redirecting to company registration...</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );

  // Render company registration form
  const renderCompanyRegistration = () => (
    <div className="space-y-6">
      <div className="text-center">
        <div className="mx-auto w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mb-4">
          <Store className="h-8 w-8 text-green-600" />
        </div>
        <h2 className="text-2xl font-semibold text-gray-900">Complete Company Registration</h2>
        <p className="mt-2 text-sm text-gray-600">
          Fill in your company details to complete the registration.
        </p>
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
            <p className="font-medium">Company Registered Successfully!</p>
            <p className="text-sm">Redirecting to dashboard...</p>
          </div>
        </div>
      )}

      <form onSubmit={handleCompanyRegistration} className="space-y-4 max-h-[60vh] overflow-y-auto pr-2">
        <div className="grid grid-cols-2 gap-3">
          <div>
            <Label htmlFor="companyName">Company Name *</Label>
            <Input
              id="companyName"
              placeholder="Acme Logistics Ltd"
              value={companyData.companyName}
              onChange={(e) => setCompanyData({ ...companyData, companyName: e.target.value })}
              required
            />
          </div>
          <div>
            <Label htmlFor="companyEmail">Company Email *</Label>
            <Input
              id="companyEmail"
              type="email"
              placeholder="info@company.com"
              value={companyData.companyEmail}
              onChange={(e) => setCompanyData({ ...companyData, companyEmail: e.target.value })}
              required
            />
          </div>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <Label htmlFor="phoneNumber">Phone Number *</Label>
            <Input
              id="phoneNumber"
              type="tel"
              placeholder="+2348000000000"
              value={companyData.phoneNumber}
              onChange={(e) => setCompanyData({ ...companyData, phoneNumber: e.target.value })}
              required
            />
          </div>
          <div>
            <Label htmlFor="valueCharge">Value Charge *</Label>
            <Input
              id="valueCharge"
              type="number"
              step="0.01"
              placeholder="0.00"
              value={companyData.valueCharge || ''}
              onChange={(e) => setCompanyData({ ...companyData, valueCharge: parseFloat(e.target.value) || 0 })}
              required
            />
          </div>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <Label htmlFor="companyState">State *</Label>
            <Input
              id="companyState"
              placeholder="Lagos"
              value={companyData.state}
              onChange={(e) => setCompanyData({ ...companyData, state: e.target.value })}
              required
            />
          </div>
          <div>
            <Label htmlFor="companyLocality">Locality *</Label>
            <Input
              id="companyLocality"
              placeholder="Ikeja"
              value={companyData.locality}
              onChange={(e) => setCompanyData({ ...companyData, locality: e.target.value })}
              required
            />
          </div>
        </div>

        <div>
          <Label htmlFor="companyAddress">Company Address *</Label>
          <Input
            id="companyAddress"
            placeholder="123 Main Street"
            value={companyData.companyAddress}
            onChange={(e) => setCompanyData({ ...companyData, companyAddress: e.target.value })}
            required
          />
        </div>

        <div>
          <Label htmlFor="companyInfo">Company Info (Optional)</Label>
          <Input
            id="companyInfo"
            placeholder="Brief description of your company"
            value={companyData.companyInfo}
            onChange={(e) => setCompanyData({ ...companyData, companyInfo: e.target.value })}
          />
        </div>

        <div className="grid grid-cols-2 gap-3">
          <div>
            <Label htmlFor="noOfTrucks">Number of Trucks</Label>
            <Input
              id="noOfTrucks"
              type="number"
              placeholder="0"
              value={companyData.noOfTrucks || ''}
              onChange={(e) => setCompanyData({ ...companyData, noOfTrucks: parseInt(e.target.value) || 0 })}
            />
          </div>
          <div>
            <Label htmlFor="nofOfBikes">Number of Bikes</Label>
            <Input
              id="nofOfBikes"
              type="number"
              placeholder="0"
              value={companyData.nofOfBikes || ''}
              onChange={(e) => setCompanyData({ ...companyData, nofOfBikes: parseInt(e.target.value) || 0 })}
            />
          </div>
        </div>

        <div className="grid grid-cols-3 gap-3">
          <div>
            <Label htmlFor="bankName">Bank Name</Label>
            <Input
              id="bankName"
              placeholder="GTBank"
              value={companyData.bankName}
              onChange={(e) => setCompanyData({ ...companyData, bankName: e.target.value })}
            />
          </div>
          <div>
            <Label htmlFor="accountName">Account Name</Label>
            <Input
              id="accountName"
              placeholder="John Doe"
              value={companyData.accountName}
              onChange={(e) => setCompanyData({ ...companyData, accountName: e.target.value })}
            />
          </div>
          <div>
            <Label htmlFor="accountNumber">Account Number</Label>
            <Input
              id="accountNumber"
              placeholder="0123456789"
              value={companyData.accountNumber}
              onChange={(e) => setCompanyData({ ...companyData, accountNumber: e.target.value })}
            />
          </div>
        </div>

        <div>
          <Label>Delivery Types</Label>
          <div className="flex gap-2 mt-1">
            <Input
              placeholder="e.g. Express, Standard"
              value={deliveryTypeInput}
              onChange={(e) => setDeliveryTypeInput(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), addDeliveryType())}
            />
            <Button type="button" onClick={addDeliveryType} variant="outline" size="sm">
              Add
            </Button>
          </div>
          <div className="flex flex-wrap gap-2 mt-2">
            {companyData.deliveryTypes.map((type) => (
              <span key={type} className="bg-blue-100 text-blue-800 px-3 py-1 rounded-full text-sm flex items-center gap-1">
                {type}
                <button type="button" onClick={() => removeDeliveryType(type)} className="text-blue-600 hover:text-blue-800">
                  ×
                </button>
              </span>
            ))}
          </div>
        </div>

        <div>
          <Label>Service Areas</Label>
          <div className="flex gap-2 mt-1">
            <Input
              placeholder="e.g. Lagos, Abuja"
              value={serviceAreaInput}
              onChange={(e) => setServiceAreaInput(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && (e.preventDefault(), addServiceArea())}
            />
            <Button type="button" onClick={addServiceArea} variant="outline" size="sm">
              Add
            </Button>
          </div>
          <div className="flex flex-wrap gap-2 mt-2">
            {companyData.serviceAreas.map((area) => (
              <span key={area} className="bg-green-100 text-green-800 px-3 py-1 rounded-full text-sm flex items-center gap-1">
                {area}
                <button type="button" onClick={() => removeServiceArea(area)} className="text-green-600 hover:text-green-800">
                  ×
                </button>
              </span>
            ))}
          </div>
        </div>

        <div className="flex flex-col gap-3 sm:flex-row">
          <Button
            type="button"
            onClick={goBackToVerification}
            variant="outline"
            className="flex-1"
          >
            <ArrowLeft className="h-4 w-4 mr-2" />
            Back to Verification
          </Button>
          <Button
            type="submit"
            disabled={isLoading}
            className="flex-1 bg-green-600 hover:bg-green-700 text-white"
          >
            {isLoading ? (
              <><Loader2 className="h-4 w-4 animate-spin mr-2" />Registering Company...</>
            ) : (
              "Complete Registration"
            )}
          </Button>
        </div>
      </form>
    </div>
  );

  return (
    <div className="min-h-screen flex">
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

      <main className="flex-1 flex items-center justify-center px-4 sm:px-6 lg:px-8 bg-white py-8">
        <div className="w-full max-w-md">
          <div className="lg:hidden text-center mb-8">
            <Building2 className="h-10 w-10 text-primary mx-auto mb-3" />
            <h1 className="text-2xl font-bold text-gray-900">GINILOG</h1>
            <p className="text-gray-600 mt-1">Company &amp; Manager Portal</p>
          </div>

          <div className="bg-white rounded-lg">
            {mode !== 'verify' && mode !== 'company-register' && (
              <div className="border-b border-gray-200 mb-6">
                <div className="flex">
                  <button
                    onClick={() => { setMode("signin"); setError(null); setSuccess(null); }}
                    className={`flex-1 py-3 text-center font-medium transition-colors ${
                      mode === "signin" ? "text-primary border-b-2 border-primary" : "text-gray-500 hover:text-gray-700"
                    }`}
                  >
                    Sign In
                  </button>
                  <button
                    onClick={() => { setMode("signup"); setError(null); setSuccess(null); }}
                    className={`flex-1 py-3 text-center font-medium transition-colors ${
                      mode === "signup" ? "text-primary border-b-2 border-primary" : "text-gray-500 hover:text-gray-700"
                    }`}
                  >
                    Register Company
                  </button>
                </div>
              </div>
            )}

            {error && mode !== 'company-register' && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md text-red-600 text-sm">{error}</div>
            )}
            {success && mode !== 'company-register' && mode !== 'verify' && (
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
            ) : mode === "company-register" ? (
              renderCompanyRegistration()
            ) : (
              <form onSubmit={handleBrandOwnerRegistration} className="space-y-4 max-h-[70vh] overflow-y-auto pr-2">
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

                <div>
                  <Label htmlFor="phone">Phone Number</Label>
                  <div className="relative mt-1">
                    <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="phone" type="tel" placeholder="+2348000000000" className="pl-9 h-11"
                      value={signUpData.phone}
                      onChange={(e) => setSignUpData({ ...signUpData, phone: e.target.value })} required />
                  </div>
                </div>

                <div>
                  <Label htmlFor="signUpEmail">Email</Label>
                  <div className="relative mt-1">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="signUpEmail" type="email" placeholder="info@company.com" className="pl-9 h-11"
                      value={signUpData.email}
                      onChange={(e) => setSignUpData({ ...signUpData, email: e.target.value })} required />
                  </div>
                </div>

                <div>
                  <Label htmlFor="companyName">Company Name</Label>
                  <div className="relative mt-1">
                    <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="companyName" placeholder="Acme Logistics Ltd" className="pl-9 h-11"
                      value={signUpData.companyName}
                      onChange={(e) => setSignUpData({ ...signUpData, companyName: e.target.value })} required />
                  </div>
                </div>

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

                <Button type="submit" disabled={isLoading} className="w-full h-12 bg-primary hover:bg-primary/90 text-white">
                  {isLoading ? (
                    <><Loader2 className="h-4 w-4 animate-spin mr-2" />Registering Brand Owner...</>
                  ) : (
                    "Register Brand Owner"
                  )}
                </Button>

                <div className="text-center text-xs text-gray-500">
                  <p>After registration, you'll verify your email and complete company setup.</p>
                </div>
              </form>
            )}

            {mode !== 'verify' && mode !== 'company-register' && (
              <p className="mt-4 text-center text-xs text-gray-500">
                <Link href="/" className="hover:text-gray-700">← Back to Home</Link>
              </p>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}