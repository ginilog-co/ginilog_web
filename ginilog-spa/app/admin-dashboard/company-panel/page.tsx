"use client";

import { useState, useEffect, useCallback } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
Search, User, Eye, Trash2, X, Loader2, AlertCircle,
ChevronLeft, ChevronRight, Filter,
} from "lucide-react";
import { getUsersPaginated, getUserById, deleteUser, type UserProfile } from "@/lib/api";

const PAGE_SIZE_OPTIONS = [10, 25, 50, 100];

export default function UsersPanel() {
const [users, setUsers] = useState<UserProfile[]>([]);
const [totalCount, setTotalCount] = useState(0);
const [totalPages, setTotalPages] = useState(1);
const [page, setPage] = useState(1);
const [pageSize, setPageSize] = useState(25);

const [search, setSearch] = useState("");
const [state, setState] = useState("");
const [locality, setLocality] = useState("");
const [startDate, setStartDate] = useState("");
const [endDate, setEndDate] = useState("");
const [showFilters, setShowFilters] = useState(false);

const [isLoading, setIsLoading] = useState(true);
const [error, setError] = useState<string | null>(null);
const [selectedUser, setSelectedUser] = useState<UserProfile | null>(null);
const [isLoadingDetail, setIsLoadingDetail] = useState(false);
const [userPendingDelete, setUserPendingDelete] = useState<UserProfile | null>(null);
const [deletingUserId, setDeletingUserId] = useState<string | null>(null);

const fetchUsers = useCallback(async () => {
setIsLoading(true);
setError(null);
try {
const result = await getUsersPaginated({
AnyItem: search || undefined,
State: state || undefined,
Locality: locality || undefined,
StartDate: startDate || undefined,
EndDate: endDate || undefined,
Page: page,
PageSize: pageSize,
});
setUsers(result.data || []);
setTotalCount(result.totalCount ?? 0);
setTotalPages(result.totalPages ?? 1);
} catch (err) {
console.error("Failed to fetch users:", err);
setError("Failed to load users.");
setUsers([]);
} finally {
setIsLoading(false);
}
}, [search, state, locality, startDate, endDate, page, pageSize]);

useEffect(() => {
fetchUsers();
}, [fetchUsers]);

const handleSearchSubmit = (e: React.FormEvent) => {
e.preventDefault();
setPage(1);
fetchUsers();
};

const clearFilters = () => {
setSearch("");
setState("");
setLocality("");
setStartDate("");
setEndDate("");
setPage(1);
};

const handleViewUser = async (user: UserProfile) => {
setSelectedUser(user);
setIsLoadingDetail(true);
try {
const fullDetail = await getUserById(user.id);
setSelectedUser(fullDetail);
} catch (err) {
console.error("Failed to load user detail:", err);
// keep showing the row data already in selectedUser as a fallback
} finally {
setIsLoadingDetail(false);
}
};

const confirmDeleteUser = async () => {
if (!userPendingDelete) return;
const userId = userPendingDelete.id;
setDeletingUserId(userId);
try {
await deleteUser(userId);
setUsers((prev) => prev.filter((u) => u.id !== userId));
setTotalCount((prev) => Math.max(0, prev - 1));
if (selectedUser?.id === userId) setSelectedUser(null);
setUserPendingDelete(null);
} catch (err) {
console.error("Failed to delete user:", err);
setError("Failed to delete user. Please try again.");
} finally {
setDeletingUserId(null);
}
};

const getStatusBadge = (user: UserProfile) => {
if (user.archivedAccount) return { label: "Archived", className: "bg-gray-100 text-gray-600" };
if (user.suspendedAccount) return { label: "Suspended", className: "bg-red-100 text-red-800" };
if (user.userStatus) return { label: "Active", className: "bg-green-100 text-green-800" };
return { label: "Inactive", className: "bg-yellow-100 text-yellow-800" };
};

return (
<Card id="users">
<CardHeader className="flex flex-col gap-4">
<div className="flex flex-row items-center justify-between">
<CardTitle>Registered Users ({totalCount})</CardTitle>
<Button
variant="outline"
size="sm"
onClick={() => setShowFilters((v) => !v)}
>
<Filter className="h-4 w-4 mr-2" />
Filters
</Button>
</div>

<form onSubmit={handleSearchSubmit} className="flex flex-col sm:flex-row gap-3">
<div className="relative flex-1">
<Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
<input
type="text"
placeholder="Search by name, email, phone..."
value={search}
onChange={(e) => setSearch(e.target.value)}
className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
/>
</div>
<Button type="submit" size="sm">Search</Button>
</form>

{showFilters && (
<div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3 pt-2 border-t">
<div>
<label className="text-xs text-gray-500 mb-1 block">State</label>
<input
type="text"
value={state}
onChange={(e) => setState(e.target.value)}
placeholder="e.g. Lagos"
className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
/>
</div>
<div>
<label className="text-xs text-gray-500 mb-1 block">Locality</label>
<input
type="text"
value={locality}
onChange={(e) => setLocality(e.target.value)}
placeholder="e.g. Ikeja"
className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
/>
</div>
<div>
<label className="text-xs text-gray-500 mb-1 block">Start Date</label>
<input
type="date"
value={startDate}
onChange={(e) => setStartDate(e.target.value)}
className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
/>
</div>
<div>
<label className="text-xs text-gray-500 mb-1 block">End Date</label>
<input
type="date"
value={endDate}
onChange={(e) => setEndDate(e.target.value)}
className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
/>
</div>
<div className="flex items-end gap-2">
<Button size="sm" onClick={() => { setPage(1); fetchUsers(); }} className="flex-1">
Apply
</Button>
<Button size="sm" variant="outline" onClick={clearFilters}>
Clear
</Button>
</div>
</div>
)}
</CardHeader>

<CardContent>
{error && (
<div className="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg flex items-center gap-2 text-red-600 text-sm">
<AlertCircle className="h-4 w-4" />
<span>{error}</span>
</div>
)}

{isLoading ? (
<div className="flex items-center justify-center py-16">
<Loader2 className="h-6 w-6 animate-spin text-primary" />
</div>
) : users.length === 0 ? (
<p className="text-center text-gray-500 py-10">No users found.</p>
) : (
<>
<div className="overflow-x-auto">
<table className="w-full text-sm">
<thead>
<tr className="border-b bg-gray-50">
<th className="text-left py-3 px-4 font-medium text-gray-600">Name</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">Email</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">Phone</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">State</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">Status</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">Joined</th>
<th className="text-left py-3 px-4 font-medium text-gray-600">Actions</th>
</tr>
</thead>
<tbody>
{users.map((u) => {
const status = getStatusBadge(u);
return (
<tr key={u.id} className="border-b hover:bg-gray-50">
<td className="py-3 px-4 font-medium">
{u.firstName || u.lastName ? `${u.firstName} ${u.lastName}`.trim() : "—"}
</td>
<td className="py-3 px-4 text-gray-500">{u.email}</td>
<td className="py-3 px-4">{u.phoneNo || "—"}</td>
<td className="py-3 px-4">{u.state || "—"}</td>
<td className="py-3 px-4">
<Badge className={status.className}>{status.label}</Badge>
</td>
<td className="py-3 px-4 text-gray-500">
{u.createdAt ? new Date(u.createdAt).toLocaleDateString() : "—"}
</td>
<td className="py-3 px-4">
<div className="flex items-center gap-2">
<button
onClick={() => handleViewUser(u)}
className="p-1.5 rounded hover:bg-blue-50 text-blue-600"
title="View user"
>
<Eye className="h-4 w-4" />
</button>
<button
onClick={() => setUserPendingDelete(u)}
disabled={deletingUserId === u.id}
className="p-1.5 rounded hover:bg-red-50 text-red-600 disabled:opacity-50"
title="Delete user"
>
{deletingUserId === u.id ? (
<Loader2 className="h-4 w-4 animate-spin" />
) : (
<Trash2 className="h-4 w-4" />
)}
</button>
</div>
</td>
</tr>
);
})}
</tbody>
</table>
</div>

{/* Pagination */}
<div className="flex flex-col sm:flex-row items-center justify-between gap-3 pt-4 mt-2 border-t">
<div className="flex items-center gap-2 text-sm text-gray-500">
<span>
Page {page} of {totalPages} · {totalCount} users
</span>
<select
value={pageSize}
onChange={(e) => { setPageSize(Number(e.target.value)); setPage(1); }}
className="border rounded px-2 py-1 text-xs focus:outline-none focus:ring-1 focus:ring-primary"
>
{PAGE_SIZE_OPTIONS.map((size) => (
<option key={size} value={size}>{size} / page</option>
))}
</select>
</div>
<div className="flex items-center gap-2">
<Button
variant="outline"
size="sm"
disabled={page <= 1}
onClick={() => setPage((p) => Math.max(1, p - 1))}
>
<ChevronLeft className="h-4 w-4" />
Prev
</Button>
<Button
variant="outline"
size="sm"
disabled={page >= totalPages}
onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
>
Next
<ChevronRight className="h-4 w-4" />
</Button>
</div>
</div>
</>
)}
</CardContent>

{/* User Detail Modal */}
{selectedUser && (
<div
className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
onClick={() => setSelectedUser(null)}
>
<div
className="bg-white rounded-xl w-full max-w-md max-h-[85vh] overflow-y-auto"
onClick={(e) => e.stopPropagation()}
>
<div className="flex items-center justify-between p-4 border-b sticky top-0 bg-white">
<h3 className="font-semibold text-gray-900">User Details</h3>
<button
onClick={() => setSelectedUser(null)}
className="p-1.5 rounded hover:bg-gray-100 text-gray-500"
>
<X className="h-4 w-4" />
</button>
</div>
<div className="p-5 space-y-4 relative">
{isLoadingDetail && (
<div className="absolute inset-0 bg-white/70 flex items-center justify-center z-10">
<Loader2 className="h-5 w-5 animate-spin text-primary" />
</div>
)}
<div className="flex items-center gap-3">
<div className="h-14 w-14 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden flex-shrink-0">
{selectedUser.profilePicture ? (
<img src={selectedUser.profilePicture} alt="User" className="h-full w-full object-cover" />
) : (
<User className="h-6 w-6 text-primary" />
)}
</div>
<div>
<p className="font-medium text-gray-900">
{selectedUser.firstName || selectedUser.lastName
? `${selectedUser.firstName} ${selectedUser.lastName}`.trim()
: "—"}
</p>
<Badge className={getStatusBadge(selectedUser).className}>
{getStatusBadge(selectedUser).label}
</Badge>
</div>
</div>
<dl className="grid grid-cols-2 gap-3 text-sm">
<div>
<dt className="text-gray-500">Email</dt>
<dd className="font-medium break-all">{selectedUser.email || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">Phone</dt>
<dd className="font-medium">{selectedUser.phoneNo || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">Sex</dt>
<dd className="font-medium">{selectedUser.sex || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">State</dt>
<dd className="font-medium">{selectedUser.state || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">Locality</dt>
<dd className="font-medium">{selectedUser.locality || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">Referral Code</dt>
<dd className="font-medium">{selectedUser.referralCode || "—"}</dd>
</div>
<div className="col-span-2">
<dt className="text-gray-500">Address</dt>
<dd className="font-medium">{selectedUser.address || "—"}</dd>
</div>
<div>
<dt className="text-gray-500">Joined</dt>
<dd className="font-medium">
{selectedUser.createdAt ? new Date(selectedUser.createdAt).toLocaleDateString() : "—"}
</dd>
</div>
<div>
<dt className="text-gray-500">Last Seen</dt>
<dd className="font-medium">
{selectedUser.lastSeenAt ? new Date(selectedUser.lastSeenAt).toLocaleDateString() : "—"}
</dd>
</div>
</dl>
<div className="pt-2 flex justify-end">
<Button
variant="destructive"
size="sm"
onClick={() => setUserPendingDelete(selectedUser)}
>
<Trash2 className="h-4 w-4 mr-2" />
Delete User
</Button>
</div>
</div>
</div>
</div>
)}

{/* Delete Confirmation Dialog */}
{userPendingDelete && (
<div
className="fixed inset-0 z-[60] flex items-center justify-center bg-black/50 p-4"
onClick={() => !deletingUserId && setUserPendingDelete(null)}
>
<div
className="bg-white rounded-xl w-full max-w-sm p-5"
onClick={(e) => e.stopPropagation()}
>
<div className="flex items-start gap-3">
<div className="h-10 w-10 rounded-full bg-red-100 flex items-center justify-center flex-shrink-0">
<AlertCircle className="h-5 w-5 text-red-600" />
</div>
<div>
<h3 className="font-semibold text-gray-900">Delete user?</h3>
<p className="text-sm text-gray-500 mt-1">
{userPendingDelete.firstName || userPendingDelete.lastName
? `${userPendingDelete.firstName} ${userPendingDelete.lastName}`.trim()
: userPendingDelete.email}{" "}
will be permanently removed. This action cannot be undone.
</p>
</div>
</div>
<div className="flex justify-end gap-2 mt-5">
<Button
variant="outline"
size="sm"
onClick={() => setUserPendingDelete(null)}
disabled={!!deletingUserId}
>
Cancel
</Button>
<Button
variant="destructive"
size="sm"
onClick={confirmDeleteUser}
disabled={!!deletingUserId}
>
{deletingUserId ? (
<Loader2 className="h-4 w-4 animate-spin mr-2" />
) : (
<Trash2 className="h-4 w-4 mr-2" />
)}
Delete
</Button>
</div>
</div>
</div>
)}
</Card>
);
}