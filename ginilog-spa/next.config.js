/** @type {import('next').NextConfig} */

const nextConfig = {
  reactStrictMode: true,
};

async function headers() {
  return [
    {
      source: '/.well-known/assetlinks.json',
      headers: [{ key: 'Content-Type', value: 'application/json' }],
    },
  ];
}

export default {
  ...nextConfig,
  async headers() {
    return headers();
  },
};
