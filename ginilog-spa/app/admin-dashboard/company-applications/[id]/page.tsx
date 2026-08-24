// app/admin-dashboard/company-applications/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { 
  Loader2, ArrowLeft, Mail, Phone, Calendar, 
  Building2, User, MapPin, Briefcase, CheckCircle,
  XCircle, Clock, Edit, Trash2, Copy
} from "lucide-react";
import { getCompanyApplicationById, updateCompanyApplication } from "@/lib/api";

export default function CompanyApplicationDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [application, setApplication] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    const fetchApplication = async () => {
      try {
        setIsLoading(true);
        const data = await getCompanyApplicationById(params.id as string);
        setApplication(data);
      } catch (error) {
        console.error("Failed to fetch application:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchApplication();
  }, [params.id]);

  const handleStatusUpdate = async (status: string) => {
    setProcessing(true);
    try {
      await updateCompanyApplication(params.id as string, { status });
      router.push("/admin-dashboard/company-applications");
    } catch (error) {
      console.error("Failed to update application:", error);
    } finally {
      setProcessing(false);
    }
  };

  const getStatusBadge = (status: string) => {
    const statusMap: Record<string, { label: string; className: string; icon: any }> = {
      pending: { label: "Pending", className: "bg-yellow-100 text-yellow-800", icon: Clock },
      approved: { label: "Approved", className: "bg-green-100 text-green-800", icon: CheckCircle },
      rejected: { label: "Rejected", className: "bg-red-100 text-red-800", icon: XCircle },
    };
    const s = status?.toLowerCase() || "pending";
    return statusMap[s] || statusMap.pending;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!application) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Application not found</p>
        <Link href="/admin-dashboard/company-applications">
          <Button className="mt-4">Back to Applications</Button>
        </Link>
      </div>
    );
  }

  const status = getStatusBadge(application.status);
  const StatusIcon = status.icon;

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/admin-dashboard/company-applications">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Application Details</h1>
          <p className="text-sm text-gray-500">Review company application</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Applicant Card */}
        <Card className="lg:col-span-1">
          <CardContent className="p-6 text-center">
            <div className="h-24 w-24 rounded-full bg-blue-100 flex items-center justify-center mx-auto">
              <span className="text-3xl font-bold text-blue-600">
                {application.firstName?.[0]}{application.surName?.[0]}
              </span>
            </div>
            <h2 className="text-xl font-bold text-gray-900 mt-4">
              {application.firstName} {application.surName}
            </h2>
            <p className="text-sm text-gray-500">Applicant</p>
            
            <div className="mt-4 flex justify-center">
              <Badge className={status.className}>
                <StatusIcon className="h-3 w-3 mr-1" />
                {status.label}
              </Badge>
            </div>

            <div className="mt-6 space-y-3 text-left">
              <div className="flex items-center gap-3 text-sm">
                <Mail className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{application.email}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Phone className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">{application.phoneNo || "N/A"}</span>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <Calendar className="h-4 w-4 text-gray-400" />
                <span className="text-gray-600">
                  Applied {new Date(application.createdAt).toLocaleDateString()}
                </span>
              </div>
            </div>

            {application.status?.toLowerCase() === "pending" && (
              <div className="mt-6 space-y-2">
                <Button 
                  className="w-full bg-green-500 hover:bg-green-600 text-white gap-2"
                  onClick={() => handleStatusUpdate("Approved")}
                  disabled={processing}
                >
                  {processing ? <Loader2 className="h-4 w-4 animate-spin" /> : <CheckCircle className="h-4 w-4" />}
                  Approve Application
                </Button>
                <Button 
                  variant="outline" 
                  className="w-full text-red-500 border-red-200 hover:bg-red-50 gap-2"
                  onClick={() => handleStatusUpdate("Rejected")}
                  disabled={processing}
                >
                  {processing ? <Loader2 className="h-4 w-4 animate-spin" /> : <XCircle className="h-4 w-4" />}
                  Reject Application
                </Button>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Company Info */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Building2 className="h-5 w-5 text-primary" />
              Company Information
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Company Name</p>
                <p className="font-semibold text-gray-900">{application.companyName}</p>
              </div>
              <div className="p-4 bg-gray-50 rounded-xl">
                <p className="text-sm text-gray-500">Username</p>
                <p className="font-semibold text-gray-900">{application.companyUserName}</p>
              </div>
            </div>

            <div>
              <p className="text-sm text-gray-500 mb-2">Company Types</p>
              <div className="flex flex-wrap gap-2">
                {application.companyType?.map((type: string) => (
                  <Badge key={type} className="bg-blue-100 text-blue-800">
                    {type}
                  </Badge>
                ))}
              </div>
            </div>

            {application.companyAddress && (
              <div className="flex items-start gap-2 p-4 bg-gray-50 rounded-xl">
                <MapPin className="h-4 w-4 text-gray-400 flex-shrink-0 mt-0.5" />
                <div>
                  <p className="text-sm text-gray-500">Business Address</p>
                  <p className="text-gray-700">{application.companyAddress}</p>
                </div>
              </div>
            )}

            <div className="p-4 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Application Status History</p>
              <div className="mt-2 flex items-center gap-4">
                <div className="flex items-center gap-2">
                  <div className="h-2 w-2 rounded-full bg-yellow-500"></div>
                  <span className="text-xs text-gray-600">Submitted</span>
                </div>
                <div className="flex-1 h-0.5 bg-gray-300"></div>
                <div className={`flex items-center gap-2 ${application.status?.toLowerCase() === "approved" ? "text-green-600" : "text-gray-400"}`}>
                  <div className={`h-2 w-2 rounded-full ${application.status?.toLowerCase() === "approved" ? "bg-green-500" : "bg-gray-300"}`}></div>
                  <span className="text-xs">Reviewed</span>
                </div>
                <div className="flex-1 h-0.5 bg-gray-300"></div>
                <div className={`flex items-center gap-2 ${application.status?.toLowerCase() === "approved" ? "text-green-600" : "text-gray-400"}`}>
                  <div className={`h-2 w-2 rounded-full ${application.status?.toLowerCase() === "approved" ? "bg-green-500" : "bg-gray-300"}`}></div>
                  <span className="text-xs">Decision</span>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}