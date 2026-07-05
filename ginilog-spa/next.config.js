/** @type {import('next').NextConfig} */

const API_BASE = (process.env.NEXT_PUBLIC_API_URL || "https://api-data-connection.ginilog.org").replace(/\/$/, "");

const nextConfig = {
  async rewrites() {
    return [
      {
        source: "/api/:path*",
        destination: `${API_BASE}/api/:path*`,
      },
      {
        source: "/api/bookings/:path*",
        destination: `${API_BASE}/api/bookings/:path*`,
      },
    ];
  },
  async headers() {
    return [
      {
        source: '/.well-known/apple-app-site-association',
        headers: [
          { key: 'Content-Type', value: 'application/json' },
        ],
      },
      {
        source: '/.well-known/assetlinks.json',
        headers: [
          { key: 'Content-Type', value: 'application/json' },
        ],
      },
    ];
  },
};

module.exports = nextConfig;
