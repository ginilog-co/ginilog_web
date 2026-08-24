// app/brand-owner/notifications/[id]/page.tsx

"use client";

import { useState, useEffect, use } from "react";
import { useRouter } from "next/navigation";
import { getNotificationById, updateNotification, deleteNotification } from "@/lib/api";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  ArrowLeft,
  Loader2,
  Calendar,
  Bell,
  Mail,
  MessageSquare,
  AlertCircle,
  Trash2,
  CheckCircle,
  Clock,
  DollarSign,
  User,
  Building2,
  Package,
  Truck,
  Hotel,
  Calendar as CalendarIcon,
  CreditCard,
  Users,
  Megaphone,
  FileText,
  Settings,
  Shield,
  Star,
  Gift,
  Heart,
  Info,
  AlertTriangle
} from "lucide-react";

export default function NotificationDetailsPage({ params }: { params: Promise<{ id: string }> }) {
  const router = useRouter();
  const { id } = use(params);
  
  const [loading, setLoading] = useState(true);
  const [notification, setNotification] = useState<any>(null);
  const [error, setError] = useState("");
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    if (id) {
      fetchNotification();
    }
  }, [id]);

  const fetchNotification = async () => {
    setLoading(true);
    setError("");
    
    try {
      const data = await getNotificationById(id);
      setNotification(data);
      
      // Mark as read when viewed - using updateNotification with 2 arguments
      if (data && !data.isRead) {
        try {
          await updateNotification(data.id, { isRead: true });
          data.isRead = true;
          setNotification(data);
        } catch (markError) {
          console.error("Failed to mark notification as read:", markError);
        }
      }
    } catch (err: any) {
      console.error("Error fetching notification:", err);
      setError(err.message || "Failed to load notification");
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this notification?")) return;
    
    setDeleting(true);
    try {
      await deleteNotification(id);
      router.push("/brand-owner/notifications");
    } catch (err: any) {
      console.error("Error deleting notification:", err);
      setError(err.message || "Failed to delete notification");
    } finally {
      setDeleting(false);
    }
  };

  const getNotificationIcon = (type: string) => {
    const iconMap: Record<string, any> = {
      booking: CalendarIcon,
      order: Package,
      payment: CreditCard,
      delivery: Truck,
      reservation: Hotel,
      user: User,
      company: Building2,
      admin: Shield,
      advert: Megaphone,
      staff: Users,
      feedback: MessageSquare,
      alert: AlertCircle,
      success: CheckCircle,
      info: Info,
      warning: AlertTriangle,
      promotion: Gift,
      review: Star,
      system: Settings,
    };
    return iconMap[type?.toLowerCase()] || Bell;
  };

  const getNotificationColor = (type: string) => {
    const colorMap: Record<string, string> = {
      booking: "bg-blue-100 text-blue-600",
      order: "bg-orange-100 text-orange-600",
      payment: "bg-green-100 text-green-600",
      delivery: "bg-purple-100 text-purple-600",
      reservation: "bg-cyan-100 text-cyan-600",
      user: "bg-indigo-100 text-indigo-600",
      company: "bg-slate-100 text-slate-600",
      admin: "bg-red-100 text-red-600",
      advert: "bg-pink-100 text-pink-600",
      staff: "bg-teal-100 text-teal-600",
      feedback: "bg-yellow-100 text-yellow-600",
      alert: "bg-red-100 text-red-600",
      success: "bg-green-100 text-green-600",
      info: "bg-blue-100 text-blue-600",
      warning: "bg-yellow-100 text-yellow-600",
      promotion: "bg-purple-100 text-purple-600",
      review: "bg-amber-100 text-amber-600",
      system: "bg-gray-100 text-gray-600",
    };
    return colorMap[type?.toLowerCase()] || "bg-gray-100 text-gray-600";
  };

  const getTimeAgo = (date: string) => {
    const now = new Date();
    const past = new Date(date);
    const diffMs = now.getTime() - past.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins} min${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    return new Date(date).toLocaleDateString();
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (error || !notification) {
    return (
      <div className="flex flex-col items-center justify-center h-64 gap-4">
        <AlertCircle className="h-12 w-12 text-red-500" />
        <p className="text-red-600 text-center">{error || "Notification not found"}</p>
        <Button onClick={() => router.push("/brand-owner/notifications")}>
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back to Notifications
        </Button>
      </div>
    );
  }

  const Icon = getNotificationIcon(notification.notificationType);
  const colorClass = getNotificationColor(notification.notificationType);

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <Button variant="ghost" onClick={() => router.push("/brand-owner/notifications")}>
          <ArrowLeft className="h-4 w-4 mr-2" />
          Back
        </Button>
        <Button 
          variant="destructive" 
          size="sm" 
          onClick={handleDelete}
          disabled={deleting}
        >
          {deleting ? (
            <Loader2 className="h-4 w-4 animate-spin" />
          ) : (
            <>
              <Trash2 className="h-4 w-4 mr-2" />
              Delete
            </>
          )}
        </Button>
      </div>

      {/* Notification Card */}
      <Card>
        <CardHeader className="border-b">
          <div className="flex items-start justify-between">
            <div className="flex items-start gap-4">
              <div className={`p-3 rounded-full ${colorClass}`}>
                <Icon className="h-6 w-6" />
              </div>
              <div>
                <CardTitle className="text-xl">{notification.title}</CardTitle>
                <div className="flex items-center gap-3 mt-1">
                  <Badge variant="outline" className="text-xs">
                    {notification.notificationType || "General"}
                  </Badge>
                  {notification.isRead ? (
                    <Badge className="bg-gray-100 text-gray-600 text-xs">Read</Badge>
                  ) : (
                    <Badge className="bg-blue-100 text-blue-600 text-xs">Unread</Badge>
                  )}
                </div>
              </div>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-500">
              <Calendar className="h-4 w-4" />
              <span>{getTimeAgo(notification.createdAt)}</span>
            </div>
          </div>
        </CardHeader>
        <CardContent className="pt-6">
          <div className="prose max-w-none">
            <p className="text-gray-700 whitespace-pre-wrap">{notification.body}</p>
          </div>
          
          {notification.imageUrl && (
            <div className="mt-4">
              <img 
                src={notification.imageUrl} 
                alt="Notification" 
                className="rounded-lg max-h-64 w-auto object-cover border border-gray-200"
              />
            </div>
          )}

          {/* Additional metadata if available */}
          {notification.metadata && (
            <div className="mt-4 p-4 bg-gray-50 rounded-lg">
              <h4 className="text-sm font-medium text-gray-700 mb-2">Additional Details</h4>
              <pre className="text-xs text-gray-600 whitespace-pre-wrap">
                {JSON.stringify(notification.metadata, null, 2)}
              </pre>
            </div>
          )}

          <div className="mt-6 pt-4 border-t flex items-center justify-between">
            <div className="text-xs text-gray-400">
              ID: {notification.id}
            </div>
            <div className="flex gap-2">
              <Button 
                variant="outline" 
                size="sm"
                onClick={() => router.push("/brand-owner/notifications")}
              >
                <Mail className="h-4 w-4 mr-2" />
                View All
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}