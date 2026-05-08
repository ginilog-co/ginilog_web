"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useRouter } from "next/navigation";
import { Eye, EyeOff, Mail, Lock, User, Phone, Building2, MapPin, Loader2 } from "lucide-react";
import { loginManager, registerManager, LoginRequest, RegisterManagerRequest } from "@/lib/api";

type AuthMode = "signin" | "signup";

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
    password: "",
    confirmPassword: "",
    phone: "",
    sex: "",
    companyName: "",
    companyUserName: "",
    state: "",
    locality: "",
    address: "",
  });

  const handleSignIn = async (e: React.FormEvent) => {
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

  const handleSignUp = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    if (signUpData.password !== signUpData.confirmPassword) {
      setError("Passwords do not match.");
      return;
    }
    setIsLoading(true);
    try {
      const staffCode = signUpData.firstName.slice(0, 3).toUpperCase() + Math.floor(1000 + Math.random() * 9000);
      const payload: RegisterManagerRequest = {
        AdminType: "Manager",
        FirstName: signUpData.firstName,
        SurName: signUpData.lastName,
        Email: signUpData.email,
        Password: signUpData.password,
        Sex: signUpData.sex,
        StaffCode: staffCode,
        PhoneNo: signUpData.phone,
        State: signUpData.state,
        Locality: signUpData.locality,
        Address: signUpData.address,
        Branch: "Main",
        CompanyName: signUpData.companyName,
        CompanyUserName: signUpData.companyUserName,
      };
      await registerManager(payload);
      setSuccess("Account created! You can now sign in.");
      setMode("signin");
      setSignInData({ email: signUpData.email, password: "" });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed. Please try again.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex">
      {/* Brand Panel */}
      <aside className="hidden lg:flex lg:w-1/2 bg-primary relative overflow-hidden">
        <div className="absolute inset-0 bg-gradient-to-br from-primary to-primary/80" />
        <div className="relative z-10 flex flex-col justify-center items-center w-full px-12 text-white">
          <Building2 className="h-14 w-14 mb-6" />
          <h1 className="text-4xl font-bold mb-4">GINILOG</h1>
          <p className="text-xl text-center mb-4">Company & Manager Portal</p>
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
            <p className="text-gray-600 mt-1">Company & Manager Portal</p>
          </div>

          <div className="bg-white rounded-lg">
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
            ) : (
              <form onSubmit={handleSignUp} className="space-y-4">
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

                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="signUpEmail">Email</Label>
                    <div className="relative mt-1">
                      <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="signUpEmail" type="email" placeholder="you@company.com" className="pl-9 h-11"
                        value={signUpData.email}
                        onChange={(e) => setSignUpData({ ...signUpData, email: e.target.value })} required />
                    </div>
                  </div>
                  <div>
                    <Label htmlFor="phone">Phone (+234...)</Label>
                    <div className="relative mt-1">
                      <Phone className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="phone" type="tel" placeholder="+2348000000000" className="pl-9 h-11"
                        value={signUpData.phone}
                        onChange={(e) => setSignUpData({ ...signUpData, phone: e.target.value })} required />
                    </div>
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

                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="companyName">Company Name</Label>
                    <div className="relative mt-1">
                      <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="companyName" placeholder="Acme Logistics" className="pl-9 h-11"
                        value={signUpData.companyName}
                        onChange={(e) => setSignUpData({ ...signUpData, companyName: e.target.value })} required />
                    </div>
                  </div>
                  <div>
                    <Label htmlFor="companyUserName">Company Handle</Label>
                    <div className="relative mt-1">
                      <Building2 className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="companyUserName" placeholder="acme_logistics" className="pl-9 h-11"
                        value={signUpData.companyUserName}
                        onChange={(e) => setSignUpData({ ...signUpData, companyUserName: e.target.value })} required />
                    </div>
                  </div>
                </div>

                <div>
                  <Label htmlFor="sex">Sex</Label>
                  <select id="sex" required
                    className="w-full mt-1 h-11 px-3 border border-gray-300 rounded-md text-sm focus:outline-none focus:ring-2 focus:ring-primary"
                    value={signUpData.sex}
                    onChange={(e) => setSignUpData({ ...signUpData, sex: e.target.value })}>
                    <option value="">Select...</option>
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                  </select>
                </div>

                <div className="grid grid-cols-2 gap-3">
                  <div>
                    <Label htmlFor="state">State</Label>
                    <div className="relative mt-1">
                      <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="state" placeholder="Lagos" className="pl-9 h-11"
                        value={signUpData.state}
                        onChange={(e) => setSignUpData({ ...signUpData, state: e.target.value })} required />
                    </div>
                  </div>
                  <div>
                    <Label htmlFor="locality">Locality</Label>
                    <div className="relative mt-1">
                      <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                      <Input id="locality" placeholder="Victoria Island" className="pl-9 h-11"
                        value={signUpData.locality}
                        onChange={(e) => setSignUpData({ ...signUpData, locality: e.target.value })} required />
                    </div>
                  </div>
                </div>

                <div>
                  <Label htmlFor="address">Address</Label>
                  <div className="relative mt-1">
                    <MapPin className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
                    <Input id="address" placeholder="123 Main Street" className="pl-9 h-11"
                      value={signUpData.address}
                      onChange={(e) => setSignUpData({ ...signUpData, address: e.target.value })} required />
                  </div>
                </div>

                <Button type="submit" disabled={isLoading} className="w-full h-12 bg-primary hover:bg-primary/90 text-white">
                  {isLoading && <Loader2 className="h-4 w-4 animate-spin mr-2" />}
                  {isLoading ? "Creating Account..." : "Create Company Account"}
                </Button>
              </form>
            )}

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/" className="hover:text-gray-700">← Back to Home</Link>
              {" · "}
              <Link href="/admin-dashboard/admin-login" className="hover:text-primary">Super Admin Login</Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
