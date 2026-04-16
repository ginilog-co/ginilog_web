"use client";

import Link from "next/link";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Eye, EyeOff, Mail, Lock } from "lucide-react";

export default function CustomerLogin() {
  const [showPassword, setShowPassword] = useState(false);
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Handle login logic here
    console.log("Login submitted:", formData);
  };

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

            <form onSubmit={handleSubmit} className="space-y-5">
              <div>
                <Label htmlFor="email" className="text-gray-700">Email</Label>
                <div className="relative mt-1">
                  <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-5 w-5 text-gray-400" />
                  <Input
                    id="email"
                    type="email"
                    placeholder="you@example.com"
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

              <div className="flex items-center justify-between">
                <Link 
                  href="/forgot-password" 
                  className="text-sm text-primary hover:text-primary/80"
                >
                  Forgot password?
                </Link>
              </div>

              <Button 
                type="submit" 
                className="w-full h-12 bg-primary hover:bg-primary/90 text-white font-medium"
              >
                Sign In
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
              Don't have an account?{" "}
              <Link href="/customer-portal/register" className="text-primary hover:text-primary/80 font-medium">
                Sign up
              </Link>
            </p>

            <p className="mt-4 text-center text-xs text-gray-500">
              <Link href="/customer-portal" className="hover:text-gray-700">
                ← Back to Home
              </Link>
            </p>
          </div>
        </div>
      </main>
    </div>
  );
}
