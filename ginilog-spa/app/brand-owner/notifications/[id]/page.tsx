// app/brand-owner/notifications/[id]/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import {
  Loader2,
  ArrowLeft,
  Bell,
  AlertCircle,
  Info,
  Clock,
  Calendar,
  MessageSquare,
  Mail,
  CheckCircle,
  Trash2,
  Copy,
  Share2,
  User,
  Package,
  DollarSign,
} from "lucide-react";
import { getNotificationById, markNotificationAsRead, deleteNotification } from "@/lib/api";

export default function NotificationDetailsPage() {
  const router = useRouter();
  const params = useParams();
  const [notification, setNotification] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    const fetchNotification = async () => {
      try {
        setIsLoading(true);
        const data = await getNotificationById(params.id as string);
        setNotification(data);
        
        // Mark as read when viewed
        if (data && !data.isRead) {
          await markNotificationAsRead(data.id);
          data.isRead = true;
          setNotification(data);
        }
      } catch (error) {
        console.error("Failed to fetch notification:", error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchNotification();
  }, [params.id]);

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this notification?")) return;
    setDeleting(true);
    try {
      await deleteNotification(params.id as string);
      router.push("/brand-owner/notifications");
    } catch (error) {
      console.error("Failed to delete notification:", error);
    } finally {
      setDeleting(false);
    }
  };

  const getIcon = (type: string) => {
    const iconMap: Record<string, any> = {
      General: <Bell className="h-6 w-6" />,
      Alert: <AlertCircle className="h-6 w-6" />,
      Info: <Info className="h-6 w-6" />,
      Reminder: <Clock className="h-6 w-6" />,
      Booking: <Calendar className="h-6 w-6" />,
      Message: <MessageSquare className="h-6 w-6" />,
      Order: <Package className="h-6 w-6" />,
      Payment: <DollarSign className="h-6 w-6" />,
    };
    return iconMap[type] || iconMap.General;
  };

  const getColor = (type: string) => {
    const colorMap: Record<string, string> = {
      General: "bg-blue-100 text-blue-600",
      Alert: "bg-red-100 text-red-600",
      Info: "bg-cyan-100 text-cyan-600",
      Reminder: "bg-yellow-100 text-yellow-600",
      Booking: "bg-green-100 text-green-600",
      Message: "bg-purple-100 text-purple-600",
      Order: "bg-orange-100 text-orange-600",
      Payment: "bg-emerald-100 text-emerald-600",
    };
    return colorMap[type] || colorMap.General;
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (!notification) {
    return (
      <div className="text-center py-12">
        <p className="text-gray-500">Notification not found</p>
        <Link href="/brand-owner/notifications">
          <Button className="mt-4">Back to Notifications</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/notifications">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Notification Details</h1>
          <p className="text-sm text-gray-500">View notification information</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <div className="flex items-start justify-between">
            <div className="flex items-center gap-3">
              <div className={`p-3 rounded-full ${getColor(notification.notificationType)}`}>
                {getIcon(notification.notificationType)}
              </div>
              <div>
                <CardTitle className="text-lg">{notification.title}</CardTitle>
                <div className="flex items-center gap-3 mt-1">
                  <Badge variant="secondary">{notification.notificationType || "General"}</Badge>
                  {notification.isRead ? (
                    <Badge className="bg-gray-100 text-gray-600">Read</Badge>
                  ) : (
                    <Badge className="bg-blue-100 text-blue-600">Unread</Badge>
                  )}
                </div>
              </div>
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                className="text-red-500 border-red-200 hover:bg-red-50"
                onClick={handleDelete}
                disabled={deleting}
              >
                {deleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
              </Button>
            </div>
          </div>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="p-4 bg-gray-50 rounded-xl">
            <p className="text-sm text-gray-500">Message</p>
            <p className="text-gray-800 mt-1 whitespace-pre-wrap">{notification.body}</p>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Sent</p>
              <p className="font-medium">{new Date(notification.createdAt).toLocaleString()}</p>
            </div>
            <div className="p-3 bg-gray-50 rounded-xl">
              <p className="text-sm text-gray-500">Last Updated</p>
              <p className="font-medium">{new Date(notification.updatedAt).toLocaleString()}</p>
            </div>
          </div>

          <div className="p-3 bg-gray-50 rounded-xl">
            <p className="text-sm text-gray-500">Notification ID</p>
            <p className="font-mono text-xs">{notification.id}</p>
          </div>

          <div className="flex gap-3 pt-4 border-t">
            <Button variant="outline" className="gap-2" onClick={() => {
              navigator.clipboard.writeText(window.location.href);
            }}>
              <Copy className="h-4 w-4" />
              Copy Link
            </Button>
            <Button variant="outline" className="gap-2">
              <Share2 className="h-4 w-4" />
              Share
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}