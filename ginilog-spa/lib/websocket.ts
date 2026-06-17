"use client";

import { useEffect, useState } from "react";
import { API_URL, getToken } from "@/lib/api";

// Derive the WebSocket URL from the same API_URL used for REST calls,
// so it can never drift out of sync with the actual backend host.
// Falls back to NEXT_PUBLIC_WS_URL only if explicitly set.
function resolveWsBase(): string {
  const fromEnv = process.env.NEXT_PUBLIC_WS_URL;
  if (fromEnv) return fromEnv.replace(/\/$/, "");

  return API_URL.replace(/^https:/, "wss:").replace(/^http:/, "ws:") + "/ws";
}

const WS_BASE = resolveWsBase();

export function useWebSocket(orderId?: string) {
  const [socket, setSocket] = useState<WebSocket | null>(null);
  const [messages, setMessages] = useState<any[]>([]);
  const [isConnected, setIsConnected] = useState(false);

  useEffect(() => {
    // Attach the JWT as a query param — the backend's JwtBearerEvents
    // .OnMessageReceived specifically reads "access_token" from the
    // query string for paths under /ws, since browsers can't send
    // custom Authorization headers on a WebSocket handshake.
    const token = getToken();
    const url = token ? `${WS_BASE}?access_token=${encodeURIComponent(token)}` : WS_BASE;

    const ws = new WebSocket(url);

    ws.onopen = () => {
      console.log("Connected to WebSocket");
      setIsConnected(true);
      if (orderId) {
        ws.send(JSON.stringify({ action: "JoinOrderTracking", orderId }));
      }
    };

    ws.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data);
        setMessages((prev) => [...prev, data]);
      } catch (err) {
        console.error("Error parsing WS message:", err);
      }
    };

    ws.onclose = () => {
      console.log("Disconnected from WebSocket");
      setIsConnected(false);
    };

    ws.onerror = (err) => {
      console.error("WebSocket error:", err);
    };

    setSocket(ws);

    return () => {
      ws.close();
    };
  }, [orderId]);

  const sendMessage = (action: string, data: any) => {
    if (socket?.readyState === WebSocket.OPEN) {
      socket.send(JSON.stringify({ action, ...data }));
    }
  };

  return { messages, isConnected, sendMessage };
}