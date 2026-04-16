export const metadata = {
  title: "GINILOG - Customer Portal",
  description: "Customer portal for GINILOG logistics and accommodation services",
};

export default function CustomerLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <div className="min-h-screen bg-gray-50">
      {children}
    </div>
  );
}
