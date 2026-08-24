// app/brand-owner/notifications/page.tsx
"use client";

import { useState, useEffect } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import {
  Bell,
  Search,
  Loader2,
  Check,
  CheckCheck,
  Clock,
  Filter,
  X,
  AlertCircle,
  Info,
  Calendar,
  MessageSquare,
  Send
} from "lucide-react";
import { getNotifications, markNotificationAsRead, sendNotification } from "@/lib/api";

export default function BrandOwnerNotificationsPage() {
  const [notifications, setNotifications] = useState<any[]>([]);
  const [filteredNotifs, setFilteredNotifs] = useState<any[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [filter, setFilter] = useState("all");
  const [showSendForm, setShowSendForm] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [formData, setFormData] = useState({
    title: "",
    body: "",
    notificationType: "General",
  });

  useEffect(() => {
    fetchNotifications();
  }, []);

  useEffect(() => {
    let filtered = notifications;
    
    if (search.trim()) {
      filtered = filtered.filter((n) =>
        n.title?.toLowerCase().includes(search.toLowerCase()) ||
        n.body?.toLowerCase().includes(search.toLowerCase())
      );
    }
    
    if (filter === "unread") {
      filtered = filtered.filter((n) => !n.isRead);
    } else if (filter === "read") {
      filtered = filtered.filter((n) => n.isRead);
    }
    
    setFilteredNotifs(filtered);
  }, [search, filter, notifications]);

  const fetchNotifications = async () => {
    try {
      setIsLoading(true);
      const data = await getNotifications();
      setNotifications(data || []);
      setFilteredNotifs(data || []);
    } catch (error) {
      console.error("Failed to fetch notifications:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleSend = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSending(true);
    try {
      await sendNotification(formData);
      setFormData({ title: "", body: "", notificationType: "General" });
      setShowSendForm(false);
      await fetchNotifications();
    } catch (error) {
      console.error("Failed to send notification:", error);
    } finally {
      setIsSending(false);
    }
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await markNotificationAsRead(id);
      await fetchNotifications();
    } catch (error) {
      console.error("Failed to mark notification as read:", error);
    }
  };

  const getIcon = (type: string) => {
    const iconMap: Record<string, any> = {
      General: <Bell className="h-4 w-4" />,
      Alert: <AlertCircle className="h-4 w-4" />,
      Info: <Info className="h-4 w-4" />,
      Reminder: <Clock className="h-4 w-4" />,
      Booking: <Calendar className="h-4 w-4" />,
      Message: <MessageSquare className="h-4 w-4" />,
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

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Notifications</h1>
          <p className="text-sm text-gray-500">Manage and send notifications</p>
        </div>
        <Button className="gap-2" onClick={() => setShowSendForm(!showSendForm)}>
          {showSendForm ? <X className="h-4 w-4" /> : <Send className="h-4 w-4" />}
          {showSendForm ? "Cancel" : "Send Notification"}
        </Button>
      </div>

      {showSendForm && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Send className="h-5 w-5 text-primary" />
              Send New Notification
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleSend} className="space-y-4">
              <div>
                <label className="text-sm font-medium text-gray-700">Title *</label>
                <Input
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  placeholder="Notification title"
                  required
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700">Message *</label>
                <textarea
                  value={formData.body}
                  onChange={(e) => setFormData({ ...formData, body: e.target.value })}
                  placeholder="Notification message"
                  rows={4}
                  className="flex min-h-[80px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
                  required
                />
              </div>
              <div>
                <label className="text-sm font-medium text-gray-700">Type</label>
                <select
                  className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                  value={formData.notificationType}
                  onChange={(e) => setFormData({ ...formData, notificationType: e.target.value })}
                >
                  <option value="General">General</option>
                  <option value="Alert">Alert</option>
                  <option value="Info">Info</option>
                  <option value="Reminder">Reminder</option>
                  <option value="Booking">Booking</option>
                  <option value="Message">Message</option>
                </select>
              </div>
              <div className="flex gap-4">
                <Button type="submit" disabled={isSending} className="flex-1">
                  {isSending ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                  {isSending ? "Sending..." : "Send Notification"}
                </Button>
                <Button type="button" variant="outline" onClick={() => setShowSendForm(false)}>
                  Cancel
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row sm:items-center gap-4">
            <CardTitle>All Notifications ({filteredNotifs.length})</CardTitle>
            <div className="relative flex-1 sm:max-w-xs">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Search notifications..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="pl-9"
              />
            </div>
            <div className="flex gap-2">
              <select
                className="px-3 py-2 border rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary/20"
                value={filter}
                onChange={(e) => setFilter(e.target.value)}
              >
                <option value="all">All</option>
                <option value="unread">Unread</option>
                <option value="read">Read</option>
              </select>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {filteredNotifs.length === 0 ? (
            <div className="text-center py-12 text-gray-500">
              <Bell className="h-12 w-12 text-gray-300 mx-auto mb-3" />
              <p>No notifications</p>
            </div>
          ) : (
            <div className="space-y-3">
              {filteredNotifs.map((notif) => (
                <div
                  key={notif.id}
                  className={`p-4 rounded-xl border transition-all ${
                    notif.isRead ? "bg-white border-gray-200" : "bg-blue-50 border-blue-200"
                  }`}
                >
                  <div className="flex items-start gap-4">
                    <div className={`p-2 rounded-full ${getColor(notif.notificationType)}`}>
                      {getIcon(notif.notificationType)}
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-start justify-between">
                        <div>
                          <p className="font-semibold text-gray-900">{notif.title}</p>
                          <p className="text-sm text-gray-600 mt-1">{notif.body}</p>
                        </div>
                        {!notif.isRead && (
                          <div className="flex items-center gap-2 flex-shrink-0">
                            <div className="h-2 w-2 rounded-full bg-blue-500"></div>
                            <Button
                              size="sm"
                              variant="ghost"
                              className="h-7 px-2 text-xs text-gray-500 hover:text-primary"
                              onClick={() => handleMarkAsRead(notif.id)}
                            >
                              <Check className="h-3 w-3 mr-1" />
                              Mark read
                            </Button>
                          </div>
                        )}
                      </div>
                      <div className="flex items-center gap-4 mt-2">
                        <span className="text-xs text-gray-400">
                          {new Date(notif.createdAt).toLocaleString()}
                        </span>
                        <Badge variant="secondary" className="text-xs">
                          {notif.notificationType || "General"}
                        </Badge>
                        {notif.isRead && (
                          <span className="text-xs text-gray-400 flex items-center gap-1">
                            <CheckCheck className="h-3 w-3" />
                            Read
                          </span>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}