// app/admin-dashboard/notifications/page.tsx

"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Bell,
  CheckCircle,
  Send,
  Loader2,
  AlertCircle,
  Calendar,
  MessageSquare,
  Users,
  Trash2,
  Eye,
  X,
} from "lucide-react";
import { 
  getNotifications, 
  sendNotification, 
  updateNotification,
  isAuthenticated,
  getStoredUser,
  validateSession
} from "@/lib/api";

export default function NotificationsPage() {
  const router = useRouter();
  const [notifications, setNotifications] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [showSendModal, setShowSendModal] = useState(false);
  const [isSending, setIsSending] = useState(false);
  
  const [sendData, setSendData] = useState({
    title: "",
    body: "",
    notificationType: "general",
    userId: "",
  });

  const [filter, setFilter] = useState("all");

  useEffect(() => {
    if (!isAuthenticated() || !validateSession()) {
      router.push("/admin-dashboard/login");
      return;
    }

    fetchNotifications();
  }, [router]);

  const fetchNotifications = async () => {
    setIsLoading(true);
    setError(null);
    try {
      const data = await getNotifications();
      setNotifications(data || []);
    } catch (err) {
      console.error("❌ Failed to fetch notifications:", err);
      setError(err instanceof Error ? err.message : "Failed to load notifications");
    } finally {
      setIsLoading(false);
    }
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await updateNotification(id, { isRead: true });
      setNotifications(prev =>
        prev.map(n => n.id === id ? { ...n, isRead: true } : n)
      );
      setSuccess("Notification marked as read");
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      console.error("❌ Failed to mark as read:", err);
      setError("Failed to update notification");
      setTimeout(() => setError(null), 3000);
    }
  };

  const handleSendNotification = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSending(true);
    setError(null);
    setSuccess(null);

    try {
      const user = getStoredUser();
      if (!user) {
        throw new Error("User not found. Please login again.");
      }

      await sendNotification({
        title: sendData.title,
        body: sendData.body,
        notificationType: sendData.notificationType,
        userId: sendData.userId || user.userId,
        imageUrl: "",
      });

      setSuccess("Notification sent successfully!");
      setShowSendModal(false);
      setSendData({
        title: "",
        body: "",
        notificationType: "general",
        userId: "",
      });
      fetchNotifications();
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      console.error("❌ Failed to send notification:", err);
      setError(err instanceof Error ? err.message : "Failed to send notification");
      setTimeout(() => setError(null), 3000);
    } finally {
      setIsSending(false);
    }
  };

  const handleDelete = async (id: string) => {
    if (!confirm("Are you sure you want to delete this notification?")) return;
    
    try {
      await updateNotification(id, { isDeleted: true });
      setNotifications(prev => prev.filter(n => n.id !== id));
      setSuccess("Notification deleted");
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      console.error("❌ Failed to delete notification:", err);
      setError("Failed to delete notification");
      setTimeout(() => setError(null), 3000);
    }
  };

  const getNotificationIcon = (type: string) => {
    switch (type?.toLowerCase()) {
      case "booking":
      case "reservation":
        return <Calendar className="h-4 w-4" />;
      case "order":
      case "delivery":
        return <MessageSquare className="h-4 w-4" />;
      case "user":
        return <Users className="h-4 w-4" />;
      default:
        return <Bell className="h-4 w-4" />;
    }
  };

  const getStatusBadge = (isRead: boolean) => {
    return isRead ? (
      <Badge variant="outline" className="text-gray-500">Read</Badge>
    ) : (
      <Badge className="bg-blue-500 text-white">Unread</Badge>
    );
  };

  const filteredNotifications = notifications.filter(n => {
    if (filter === "unread") return !n.isRead;
    if (filter === "read") return n.isRead;
    return true;
  });

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Notifications</h1>
          <p className="text-sm text-gray-500 mt-1">
            Manage system notifications and announcements
          </p>
        </div>
        <div className="flex gap-2">
          <Button
            onClick={() => setShowSendModal(true)}
            className="bg-primary hover:bg-primary/90 text-white"
          >
            <Send className="h-4 w-4 mr-2" />
            Send Notification
          </Button>
          <Button
            variant="outline"
            onClick={fetchNotifications}
            disabled={isLoading}
          >
            Refresh
          </Button>
        </div>
      </div>

      {/* Error/Success Messages */}
      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-xl text-red-700 flex items-start gap-3">
          <AlertCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <p className="text-sm">{error}</p>
        </div>
      )}

      {success && (
        <div className="p-4 bg-green-50 border border-green-200 rounded-xl text-green-700 flex items-start gap-3">
          <CheckCircle className="h-5 w-5 flex-shrink-0 mt-0.5" />
          <p className="text-sm">{success}</p>
        </div>
      )}

      {/* Filters */}
      <div className="flex gap-2">
        <Button
          variant={filter === "all" ? "default" : "outline"}
          onClick={() => setFilter("all")}
          size="sm"
        >
          All ({notifications.length})
        </Button>
        <Button
          variant={filter === "unread" ? "default" : "outline"}
          onClick={() => setFilter("unread")}
          size="sm"
        >
          Unread ({notifications.filter(n => !n.isRead).length})
        </Button>
        <Button
          variant={filter === "read" ? "default" : "outline"}
          onClick={() => setFilter("read")}
          size="sm"
        >
          Read ({notifications.filter(n => n.isRead).length})
        </Button>
      </div>

      {/* Notifications List */}
      <Card>
        <CardContent className="p-6">
          {filteredNotifications.length === 0 ? (
            <div className="text-center py-12">
              <Bell className="h-12 w-12 text-gray-300 mx-auto mb-4" />
              <p className="text-gray-500">No notifications found</p>
            </div>
          ) : (
            <div className="space-y-3">
              {filteredNotifications.map((notification) => (
                <div
                  key={notification.id}
                  className={`flex items-start gap-4 p-4 rounded-xl border transition-all ${
                    notification.isRead
                      ? "bg-white border-gray-200"
                      : "bg-blue-50/50 border-blue-200"
                  }`}
                >
                  <div className="flex-shrink-0 mt-1">
                    <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center text-primary">
                      {getNotificationIcon(notification.notificationType)}
                    </div>
                  </div>

                  <div className="flex-1 min-w-0">
                    <div className="flex items-start justify-between gap-2">
                      <div>
                        <p className={`font-medium ${notification.isRead ? "text-gray-700" : "text-gray-900"}`}>
                          {notification.title}
                        </p>
                        <p className="text-sm text-gray-500 mt-1">
                          {notification.body}
                        </p>
                      </div>
                      <div className="flex items-center gap-2 flex-shrink-0">
                        {getStatusBadge(notification.isRead)}
                      </div>
                    </div>

                    <div className="flex items-center gap-4 mt-3 text-xs text-gray-400">
                      <span>{new Date(notification.createdAt).toLocaleString()}</span>
                      {notification.notificationType && (
                        <span className="px-2 py-0.5 bg-gray-100 rounded-full">
                          {notification.notificationType}
                        </span>
                      )}
                    </div>

                    <div className="flex gap-2 mt-3">
                      {!notification.isRead && (
                        <Button
                          size="sm"
                          variant="outline"
                          onClick={() => handleMarkAsRead(notification.id)}
                          className="text-xs"
                        >
                          <Eye className="h-3 w-3 mr-1" />
                          Mark as Read
                        </Button>
                      )}
                      <Button
                        size="sm"
                        variant="ghost"
                        onClick={() => handleDelete(notification.id)}
                        className="text-xs text-red-500 hover:text-red-700 hover:bg-red-50"
                      >
                        <Trash2 className="h-3 w-3 mr-1" />
                        Delete
                      </Button>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Send Notification Modal */}
      {showSendModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-2xl max-w-md w-full max-h-[90vh] overflow-y-auto p-6">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-gray-900">Send Notification</h2>
              <button
                onClick={() => setShowSendModal(false)}
                className="text-gray-400 hover:text-gray-600"
              >
                <X className="h-5 w-5" />
              </button>
            </div>

            <form onSubmit={handleSendNotification} className="space-y-4">
              <div>
                <Label htmlFor="title">Title *</Label>
                <Input
                  id="title"
                  value={sendData.title}
                  onChange={(e) => setSendData({ ...sendData, title: e.target.value })}
                  placeholder="Notification title"
                  required
                  disabled={isSending}
                />
              </div>

              <div>
                <Label htmlFor="body">Message *</Label>
                <textarea
                  id="body"
                  value={sendData.body}
                  onChange={(e) => setSendData({ ...sendData, body: e.target.value })}
                  placeholder="Notification message"
                  rows={4}
                  className="mt-1.5 w-full rounded-lg border border-gray-300 px-3 py-2 focus:border-primary focus:ring-1 focus:ring-primary"
                  required
                  disabled={isSending}
                />
              </div>

              <div>
                <Label htmlFor="notificationType">Type</Label>
                <select
                  id="notificationType"
                  value={sendData.notificationType}
                  onChange={(e) => setSendData({ ...sendData, notificationType: e.target.value })}
                  className="mt-1.5 w-full rounded-lg border border-gray-300 px-3 py-2 focus:border-primary focus:ring-1 focus:ring-primary"
                  disabled={isSending}
                >
                  <option value="general">General</option>
                  <option value="booking">Booking</option>
                  <option value="order">Order</option>
                  <option value="user">User</option>
                  <option value="system">System</option>
                  <option value="promotion">Promotion</option>
                </select>
              </div>

              <div>
                <Label htmlFor="userId">User ID (Optional)</Label>
                <Input
                  id="userId"
                  value={sendData.userId}
                  onChange={(e) => setSendData({ ...sendData, userId: e.target.value })}
                  placeholder="Leave empty to send to all users"
                  disabled={isSending}
                />
              </div>

              <div className="flex gap-3 pt-4 border-t">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => setShowSendModal(false)}
                  className="flex-1"
                  disabled={isSending}
                >
                  Cancel
                </Button>
                <Button
                  type="submit"
                  disabled={isSending || !sendData.title || !sendData.body}
                  className="flex-1 bg-primary hover:bg-primary/90 text-white"
                >
                  {isSending ? (
                    <>
                      <Loader2 className="h-4 w-4 animate-spin mr-2" />
                      Sending...
                    </>
                  ) : (
                    <>
                      <Send className="h-4 w-4 mr-2" />
                      Send
                    </>
                  )}
                </Button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}