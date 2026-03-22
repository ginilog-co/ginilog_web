import Link from "next/link";
import { Button } from "@/components/ui/button";

export default function AdminPage() {
  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b">
        <div className="container mx-auto px-4 py-4 flex items-center justify-between">
          <Link href="/admin-dashboard" className="text-2xl font-bold text-primary">
            GINILOG Admin
          </Link>
          <div className="flex items-center gap-4">
            <span className="text-gray-600">Welcome, Admin</span>
            <Button variant="outline" size="sm">Logout</Button>
          </div>
        </div>
      </header>

      {/* Dashboard Content */}
      <main className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-8">Dashboard</h1>
        
        {/* Stats Cards */}
        <div className="grid md:grid-cols-4 gap-6 mb-8">
          <div className="bg-white p-6 rounded-lg shadow-sm">
            <h3 className="text-gray-600 text-sm">Total Orders</h3>
            <p className="text-3xl font-bold text-primary">1,234</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-sm">
            <h3 className="text-gray-600 text-sm">Active Bookings</h3>
            <p className="text-3xl font-bold text-primary">567</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-sm">
            <h3 className="text-gray-600 text-sm">Total Users</h3>
            <p className="text-3xl font-bold text-primary">8,901</p>
          </div>
          <div className="bg-white p-6 rounded-lg shadow-sm">
            <h3 className="text-gray-600 text-sm">Revenue</h3>
            <p className="text-3xl font-bold text-primary">₦2.4M</p>
          </div>
        </div>

        {/* Quick Actions */}
        <div className="bg-white p-6 rounded-lg shadow-sm">
          <h2 className="text-xl font-semibold mb-4">Quick Actions</h2>
          <div className="flex flex-wrap gap-4">
            <Button>Manage Orders</Button>
            <Button variant="outline">View Bookings</Button>
            <Button variant="outline">User Management</Button>
            <Button variant="outline">Reports</Button>
          </div>
        </div>
      </main>
    </div>
  );
}
