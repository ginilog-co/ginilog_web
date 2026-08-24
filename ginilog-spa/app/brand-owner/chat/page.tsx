"use client";

import { useState } from "react";
import Link from "next/link";
import { ArrowLeft, MessageSquare, Search, Inbox } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

export default function BrandOwnerChatPage() {
  const [message, setMessage] = useState("");

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Messages</h1>
          <p className="text-sm text-gray-500">Manage conversations with your team and customers</p>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-[320px_minmax(0,1fr)]">
        <Card>
          <CardContent className="p-0">
            <div className="border-b p-4">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
                <Input placeholder="Search chats" className="pl-9" />
              </div>
            </div>

            <div className="p-6 text-center text-sm text-gray-500">
              <div className="mx-auto mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-gray-100 text-gray-400">
                <Inbox className="h-5 w-5" />
              </div>
              No conversations yet.
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardContent className="p-0">
            <div className="flex items-center justify-between border-b p-4">
              <div>
                <p className="font-semibold text-gray-900">Support Team</p>
                <p className="text-xs text-gray-500">Chat is currently unavailable</p>
              </div>
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-primary">
                <MessageSquare className="h-4 w-4" />
              </div>
            </div>

            <div className="p-6 text-sm text-gray-500">
              Real-time chat will appear here when the support channel is connected.
            </div>

            <div className="border-t p-4">
              <div className="flex items-center gap-2">
                <Input
                  value={message}
                  onChange={(e) => setMessage(e.target.value)}
                  placeholder="Type your message..."
                  className="flex-1"
                  disabled
                />
                <Button className="gap-2" disabled>
                  Send
                </Button>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
