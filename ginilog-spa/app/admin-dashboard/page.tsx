"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
LayoutDashboard,
Package,
Hotel,
Users,
Settings,
LogOut,
Bell,
Search,
ArrowUpRight,
MessageSquare,
User,
Building2,
Truck,
Loader2,
AlertCircle,
} from "lucide-react";
import {
getStoredUser,
logout,
adminGetProfile,
getAllUsers,
getAllOrders,
getAllReservations,
updateOrderStatus,
updateReservation,
} from "@/lib/api";
import UsersPanel from "./registered-users/page";
import CompaniesPanel from "./company-panel/page";

const ORDER_STATUSES = [
"Pending",
"Processing",
"In_Transit",
"Delivered",
"Cancelled",
];
const BOOKING_STATUSES = ["Pending", "Confirmed", "Completed", "Cancelled"];

export default function AdminDashboard() {
const router = useRouter();
const [isSidebarOpen, setIsSidebarOpen] = useState(true);
const [adminProfile, setAdminProfile] = useState<any>(null);
const [users, setUsers] = useState<any[]>([]);
const [orders, setOrders] = useState<any[]>([]);
const [reservations, setReservations] = useState<any[]>([]);
const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);
const [orderSearch, setOrderSearch] = useState("");
const [bookingSearch, setBookingSearch] = useState("");
const [updatingId, setUpdatingId] = useState<string | null>(null);

useEffect(() => {
const stored = getStoredUser();
if (!stored) {
router.push("/admin-dashboard/admin-login");
return;
}
const fetchData = async () => {
try {
const [profile, allUsers, allOrders, allReservations] =
await Promise.all([
adminGetProfile().catch(() => null),
getAllUsers().catch(() => []),
getAllOrders().catch(() => []),
getAllReservations().catch(() => []),
]);
setAdminProfile(profile);
setUsers(allUsers || []);
setOrders(allOrders || []);
setReservations(allReservations || []);
} catch (err) {
setError("Failed to load dashboard data.");
console.error(err);
} finally {
setIsLoading(false);
}
};
fetchData();
}, [router]);

const handleOrderStatusChange = async (
orderId: string,
newStatus: string,
) => {
setUpdatingId(orderId);
try {
await updateOrderStatus(orderId, { OrderStatus: newStatus });
setOrders((prev) =>
prev.map((o) =>
o.id === orderId ? { ...o, orderStatus: newStatus } : o,
),
);
} catch (err) {
console.error("Failed to update order:", err);
} finally {
setUpdatingId(null);
}
};

const handleBookingStatusChange = async (
bookingId: string,
newStatus: string,
) => {
setUpdatingId(bookingId);
try {
await updateReservation(bookingId, { BookingStatus: newStatus });
setReservations((prev) =>
prev.map((r) =>
r.id === bookingId ? { ...r, bookingStatus: newStatus } : r,
),
);
} catch (err) {
console.error("Failed to update booking:", err);
} finally {
setUpdatingId(null);
}
};

const totalRevenue =
orders.reduce(
(sum, o) => sum + (o.shippingCost || 0) + (o.itemCost || 0),
0,
) + reservations.reduce((sum, r) => sum + (r.totalAmount || 0), 0);

const stats = [
{
title: "Total Users",
value: users.length,
icon: Users,
color: "bg-blue-100 text-blue-600",
},
{
title: "Total Orders",
value: orders.length,
icon: Truck,
color: "bg-primary/10 text-primary",
},
{
title: "Total Bookings",
value: reservations.length,
icon: Building2,
color: "bg-purple-100 text-purple-600",
},
{
title: "Total Revenue",
value: `₦${totalRevenue.toLocaleString()}`,
icon: ArrowUpRight,
color: "bg-green-100 text-green-600",
},
];

const filteredOrders = orders.filter(
(o) =>
!orderSearch ||
(o.trackingNum || "").toLowerCase().includes(orderSearch.toLowerCase()) ||
(o.itemName || "").toLowerCase().includes(orderSearch.toLowerCase()) ||
(o.senderName || "").toLowerCase().includes(orderSearch.toLowerCase()),
);

const filteredReservations = reservations.filter(
(r) =>
!bookingSearch ||
(r.bookingRefNo || "")
.toLowerCase()
.includes(bookingSearch.toLowerCase()) ||
(r.accomodationName || "")
.toLowerCase()
.includes(bookingSearch.toLowerCase()) ||
(r.guestName || "").toLowerCase().includes(bookingSearch.toLowerCase()),
);

const getStatusBadge = (status: string) => {
const s = (status || "").toLowerCase();
if (s === "delivered" || s === "confirmed" || s === "completed")
return "bg-green-100 text-green-800";
if (s === "in_transit" || s === "processing")
return "bg-blue-100 text-blue-800";
if (s === "cancelled") return "bg-red-100 text-red-800";
return "bg-yellow-100 text-yellow-800";
};

const navItems = [
{
icon: LayoutDashboard,
label: "Dashboard",
href: "/admin-dashboard",
active: true,
},
{ icon: Package, label: "Orders", href: "#orders" },
{ icon: Hotel, label: "Bookings", href: "#bookings" },
{ icon: Users, label: "Users", href: "#users" },
{ icon: Building2, label: "Companies", href: "#companies" },
{
icon: Building2,
label: "Company Portal",
href: "/admin-dashboard/company",
},
{ icon: Settings, label: "Settings", href: "#" },
];

if (isLoading) {
return (
<div className="min-h-screen flex items-center justify-center bg-gray-50">
<Loader2 className="h-8 w-8 animate-spin text-primary" />
</div>
);
}

return (
<div className="min-h-screen bg-gray-50 flex">
{/* Sidebar */}
<aside
className={`bg-gray-900 text-white transition-all duration-300 ${
isSidebarOpen ? "w-64" : "w-20"
} fixed h-full z-40`}
>
<div className="h-16 flex items-center justify-center border-b border-gray-800">
<Link href="/admin-dashboard" className="text-xl font-bold">
{isSidebarOpen ? "GINILOG Admin" : "GNL"}
</Link>
</div>
<nav className="mt-6 px-3">
{navItems.map((item) => (
<Link
key={item.label}
href={item.href}
className={`flex items-center gap-3 px-3 py-3 rounded-lg mb-1 transition-colors ${
item.active
? "bg-primary text-white"
: "text-gray-400 hover:bg-gray-800 hover:text-white"
}`}
>
<item.icon className="h-5 w-5 flex-shrink-0" />
{isSidebarOpen && (
<span className="text-sm font-medium">{item.label}</span>
)}
</Link>
))}
</nav>
<div className="absolute bottom-0 left-0 right-0 p-3 border-t border-gray-800">
<button
onClick={() => {
logout();
router.push("/admin-dashboard/admin-login");
}}
className="flex items-center gap-3 px-3 py-3 rounded-lg text-gray-400 hover:bg-gray-800 hover:text-white transition-colors w-full"
>
<LogOut className="h-5 w-5 flex-shrink-0" />
{isSidebarOpen && (
<span className="text-sm font-medium">Logout</span>
)}
</button>
</div>
</aside>

{/* Main Content */}
<div
className={`flex-1 transition-all duration-300 ${isSidebarOpen ? "ml-64" : "ml-20"}`}
>
{/* Header */}
<header className="bg-white border-b sticky top-0 z-30">
<div className="flex items-center justify-between px-6 py-4">
<div className="flex items-center gap-4">
<button
onClick={() => setIsSidebarOpen(!isSidebarOpen)}
className="p-2 hover:bg-gray-100 rounded-lg"
>
<LayoutDashboard className="h-5 w-5 text-gray-600" />
</button>
</div>
<div className="flex items-center gap-4">
<button className="relative p-2 hover:bg-gray-100 rounded-lg">
<Bell className="h-5 w-5 text-gray-600" />
<span className="absolute top-1 right-1 h-2 w-2 bg-primary rounded-full" />
</button>
<button className="relative p-2 hover:bg-gray-100 rounded-lg">
<MessageSquare className="h-5 w-5 text-gray-600" />
</button>
<div className="flex items-center gap-3 pl-4 border-l">
<div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden">
{adminProfile?.profilePicture ? (
<img
src={adminProfile.profilePicture}
alt="Admin"
className="h-full w-full object-cover"
/>
) : (
<User className="h-5 w-5 text-primary" />
)}
</div>
<div className="hidden md:block">
<p className="text-sm font-medium text-gray-900">
{adminProfile?.firstName || "Admin"}{" "}
{adminProfile?.lastName || ""}
</p>
<p className="text-xs text-gray-500">Super Admin</p>
</div>
</div>
</div>
</div>
</header>

<main className="p-6">
{error && (
<div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
<AlertCircle className="h-4 w-4" />
<span>{error}</span>
</div>
)}

{/* Stats */}
<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
{stats.map((stat) => (
<Card
key={stat.title}
className="hover:shadow-md transition-shadow"
>
<CardContent className="p-6">
<div className="flex items-start justify-between">
<div>
<p className="text-gray-500 text-sm">{stat.title}</p>
<h3 className="text-2xl font-bold text-gray-900 mt-1">
{stat.value}
</h3>
</div>
<div className={`p-3 rounded-lg ${stat.color}`}>
<stat.icon className="h-5 w-5" />
</div>
</div>
</CardContent>
</Card>
))}
</div>

{/* Users - self-fetching paginated panel */}
<div className="mb-8">
<UsersPanel />
</div>

{/* Companies - self-fetching paginated panel */}
<div className="mb-8">
<CompaniesPanel />
</div>

{/* Orders Table */}
<Card className="mb-8" id="orders">
<CardHeader className="flex flex-row items-center justify-between">
<CardTitle>All Orders ({orders.length})</CardTitle>
<div className="relative w-52">
<Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
<input
type="text"
placeholder="Search orders..."
value={orderSearch}
onChange={(e) => setOrderSearch(e.target.value)}
className="pl-9 pr-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
/>
</div>
</CardHeader>
<CardContent>
{filteredOrders.length === 0 ? (
<p className="text-center text-gray-500 py-8">
No orders found.
</p>
) : (
<div className="overflow-x-auto">
<table className="w-full text-sm">
<thead>
<tr className="border-b bg-gray-50">
<th className="text-left py-3 px-4 font-medium text-gray-600">
Tracking #
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Item
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Sender
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Receiver
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Company
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Shipping
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Item Cost
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Status
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Date
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Update
</th>
</tr>
</thead>
<tbody>
{filteredOrders.map((order) => (
<tr
key={order.id}
className="border-b hover:bg-gray-50"
>
<td className="py-3 px-4 font-mono text-xs">
{order.trackingNum || order.id?.slice(0, 8)}
</td>
<td className="py-3 px-4">{order.itemName}</td>
<td className="py-3 px-4">{order.senderName}</td>
<td className="py-3 px-4">{order.recieverName}</td>
<td className="py-3 px-4">
{order.companyName || "—"}
</td>
<td className="py-3 px-4">
₦{(order.shippingCost || 0).toLocaleString()}
</td>
<td className="py-3 px-4">
₦{(order.itemCost || 0).toLocaleString()}
</td>
<td className="py-3 px-4">
<Badge
className={getStatusBadge(order.orderStatus)}
>
{order.orderStatus || "Pending"}
</Badge>
</td>
<td className="py-3 px-4 text-gray-500">
{order.createdAt
? new Date(order.createdAt).toLocaleDateString()
: "—"}
</td>
<td className="py-3 px-4">
{updatingId === order.id ? (
<Loader2 className="h-4 w-4 animate-spin text-gray-400" />
) : (
<select
value={order.orderStatus || "Pending"}
onChange={(e) =>
handleOrderStatusChange(
order.id,
e.target.value,
)
}
className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
>
{ORDER_STATUSES.map((s) => (
<option key={s} value={s}>
{s}
</option>
))}
</select>
)}
</td>
</tr>
))}
</tbody>
</table>
</div>
)}
</CardContent>
</Card>

{/* Reservations Table */}
<Card id="bookings">
<CardHeader className="flex flex-row items-center justify-between">
<CardTitle>All Reservations ({reservations.length})</CardTitle>
<div className="relative w-52">
<Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
<input
type="text"
placeholder="Search bookings..."
value={bookingSearch}
onChange={(e) => setBookingSearch(e.target.value)}
className="pl-9 pr-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
/>
</div>
</CardHeader>
<CardContent>
{filteredReservations.length === 0 ? (
<p className="text-center text-gray-500 py-8">
No reservations found.
</p>
) : (
<div className="overflow-x-auto">
<table className="w-full text-sm">
<thead>
<tr className="border-b bg-gray-50">
<th className="text-left py-3 px-4 font-medium text-gray-600">
Accommodation
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Guest
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Ref #
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Check-in
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Check-out
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Nights
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Amount
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Status
</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">
Update
</th>
</tr>
</thead>
<tbody>
{filteredReservations.map((res) => (
<tr key={res.id} className="border-b hover:bg-gray-50">
<td className="py-3 px-4">{res.accomodationName}</td>
<td className="py-3 px-4">{res.guestName}</td>
<td className="py-3 px-4 font-mono text-xs">
{res.bookingRefNo || res.id?.slice(0, 8)}
</td>
<td className="py-3 px-4">
{res.checkInDate
? new Date(res.checkInDate).toLocaleDateString()
: "—"}
</td>
<td className="py-3 px-4">
{res.checkOutDate
? new Date(res.checkOutDate).toLocaleDateString()
: "—"}
</td>
<td className="py-3 px-4">
{res.numberOfNights || "—"}
</td>
<td className="py-3 px-4 font-medium text-primary">
₦{(res.totalAmount || 0).toLocaleString()}
</td>
<td className="py-3 px-4">
<Badge
className={getStatusBadge(res.bookingStatus)}
>
{res.bookingStatus || "Pending"}
</Badge>
</td>
<td className="py-3 px-4">
{updatingId === res.id ? (
<Loader2 className="h-4 w-4 animate-spin text-gray-400" />
) : (
<select
value={res.bookingStatus || "Pending"}
onChange={(e) =>
handleBookingStatusChange(
res.id,
e.target.value,
)
}
className="text-xs border rounded px-2 py-1 focus:outline-none focus:ring-1 focus:ring-primary"
>
{BOOKING_STATUSES.map((s) => (
<option key={s} value={s}>
{s}
</option>
))}
</select>
)}
</td>
</tr>
))}
</tbody>
</table>
</div>
)}
</CardContent>
</Card>
</main>
</div>
</div>
);
}
