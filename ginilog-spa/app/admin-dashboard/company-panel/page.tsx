"use client";

import { useState, useEffect, useCallback } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Search,
  Building2,
  Eye,
  X,
  Loader2,
  AlertCircle,
  ChevronLeft,
  ChevronRight,
  Filter,
  Truck,
  Bike,
  Star,
} from "lucide-react";
import { getCompaniesPaginated, type Company } from "@/lib/api";

const PAGE_SIZE_OPTIONS = [10, 25, 50, 100];

export default function CompaniesPanel() {
  const [companies, setCompanies] = useState<Company[]>([]);
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
  const [selectedCompany, setSelectedCompany] = useState<Company | null>(null);

  const fetchCompanies = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const result = await getCompaniesPaginated({
        AnyItem: search || undefined,
        State: state || undefined,
        Locality: locality || undefined,
        StartDate: startDate || undefined,
        EndDate: endDate || undefined,
        Page: page,
        PageSize: pageSize,
      });
      setCompanies(result.data || []);
      setTotalCount(result.totalCount ?? 0);
      setTotalPages(result.totalPages ?? 1);
    } catch (err) {
      console.error("Failed to fetch companies:", err);
      setError("Failed to load companies.");
      setCompanies([]);
    } finally {
      setIsLoading(false);
    }
  }, [search, state, locality, startDate, endDate, page, pageSize]);

  useEffect(() => {
    fetchCompanies();
  }, [fetchCompanies]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    fetchCompanies();
  };

  const clearFilters = () => {
    setSearch("");
    setState("");
    setLocality("");
    setStartDate("");
    setEndDate("");
    setPage(1);
  };

  return (
    <Card id="companies">
      <CardHeader className="flex flex-col gap-4">
        <div className="flex flex-row items-center justify-between">
          <CardTitle>All Companies ({totalCount})</CardTitle>
          <Button
            variant="outline"
            size="sm"
            onClick={() => setShowFilters((v) => !v)}
          >
            <Filter className="h-4 w-4 mr-2" />
            Filters
          </Button>
        </div>

        <form
          onSubmit={handleSearchSubmit}
          className="flex flex-col sm:flex-row gap-3"
        >
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
            <input
              type="text"
              placeholder="Search by company name, email..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-9 pr-3 py-2 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary w-full"
            />
          </div>
          <Button type="submit" size="sm">
            Search
          </Button>
        </form>

        {showFilters && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-3 pt-2 border-t">
            <div>
              <label className="text-xs text-gray-500 mb-1 block">State</label>
              <input
                type="text"
                value={state}
                onChange={(e) => setState(e.target.value)}
                placeholder="e.g. Enugu State"
                className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div>
              <label className="text-xs text-gray-500 mb-1 block">
                Locality
              </label>
              <input
                type="text"
                value={locality}
                onChange={(e) => setLocality(e.target.value)}
                placeholder="e.g. Enugu North"
                className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div>
              <label className="text-xs text-gray-500 mb-1 block">
                Start Date
              </label>
              <input
                type="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
                className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div>
              <label className="text-xs text-gray-500 mb-1 block">
                End Date
              </label>
              <input
                type="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
                className="w-full px-3 py-1.5 border rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary"
              />
            </div>
            <div className="flex items-end gap-2">
              <Button
                size="sm"
                onClick={() => {
                  setPage(1);
                  fetchCompanies();
                }}
                className="flex-1"
              >
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
        ) : companies.length === 0 ? (
          <p className="text-center text-gray-500 py-10">No companies found.</p>
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead>
                  <tr className="border-b bg-gray-50">
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Logo
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Company Name
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Email
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Phone
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      State
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Fleet
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Status
                    </th>
                    <th className="text-left py-3 px-4 font-medium text-gray-600">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {companies.map((c) => (
                    <tr key={c.id} className="border-b hover:bg-gray-50">
                      <td className="py-3 px-4">
                        <div className="h-9 w-9 rounded-full bg-gray-100 overflow-hidden flex items-center justify-center">
                          {c.companyLogo ? (
                            <img
                              src={c.companyLogo}
                              alt={c.companyName}
                              className="h-full w-full object-cover"
                            />
                          ) : (
                            <Building2 className="h-4 w-4 text-gray-400" />
                          )}
                        </div>
                      </td>
                      <td className="py-3 px-4 font-medium">{c.companyName}</td>
                      <td className="py-3 px-4 text-gray-500">
                        {c.companyEmail || "—"}
                      </td>
                      <td className="py-3 px-4">{c.phoneNumber || "—"}</td>
                      <td className="py-3 px-4">{c.state || "—"}</td>
                      <td className="py-3 px-4">
                        <div className="flex items-center gap-3 text-gray-500 text-xs">
                          <span className="flex items-center gap-1">
                            <Truck className="h-3.5 w-3.5" />
                            {c.noOfTrucks ?? 0}
                          </span>
                          <span className="flex items-center gap-1">
                            <Bike className="h-3.5 w-3.5" />
                            {c.nofOfBikes ?? 0}
                          </span>
                        </div>
                      </td>
                      <td className="py-3 px-4">
                        <Badge
                          className={
                            c.available
                              ? "bg-green-100 text-green-800"
                              : "bg-gray-100 text-gray-600"
                          }
                        >
                          {c.available ? "Available" : "Unavailable"}
                        </Badge>
                      </td>
                      <td className="py-3 px-4">
                        <button
                          onClick={() => setSelectedCompany(c)}
                          className="p-1.5 rounded hover:bg-blue-50 text-blue-600"
                          title="View company"
                        >
                          <Eye className="h-4 w-4" />
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Pagination */}
            <div className="flex flex-col sm:flex-row items-center justify-between gap-3 pt-4 mt-2 border-t">
              <div className="flex items-center gap-2 text-sm text-gray-500">
                <span>
                  Page {page} of {totalPages} · {totalCount} companies
                </span>
                <select
                  value={pageSize}
                  onChange={(e) => {
                    setPageSize(Number(e.target.value));
                    setPage(1);
                  }}
                  className="border rounded px-2 py-1 text-xs focus:outline-none focus:ring-1 focus:ring-primary"
                >
                  {PAGE_SIZE_OPTIONS.map((size) => (
                    <option key={size} value={size}>
                      {size} / page
                    </option>
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

      {/* Company Detail Modal */}
      {selectedCompany && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
          onClick={() => setSelectedCompany(null)}
        >
          <div
            className="bg-white rounded-xl w-full max-w-md max-h-[85vh] overflow-y-auto"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-center justify-between p-4 border-b sticky top-0 bg-white">
              <h3 className="font-semibold text-gray-900">Company Details</h3>
              <button
                onClick={() => setSelectedCompany(null)}
                className="p-1.5 rounded hover:bg-gray-100 text-gray-500"
              >
                <X className="h-4 w-4" />
              </button>
            </div>
            <div className="p-5 space-y-4">
              <div className="flex items-center gap-3">
                <div className="h-14 w-14 rounded-full bg-primary/10 flex items-center justify-center overflow-hidden flex-shrink-0">
                  {selectedCompany.companyLogo ? (
                    <img
                      src={selectedCompany.companyLogo}
                      alt="Company"
                      className="h-full w-full object-cover"
                    />
                  ) : (
                    <Building2 className="h-6 w-6 text-primary" />
                  )}
                </div>
                <div>
                  <p className="font-medium text-gray-900">
                    {selectedCompany.companyName}
                  </p>
                  <div className="flex items-center gap-2 mt-1">
                    <Badge
                      className={
                        selectedCompany.available
                          ? "bg-green-100 text-green-800"
                          : "bg-gray-100 text-gray-600"
                      }
                    >
                      {selectedCompany.available ? "Available" : "Unavailable"}
                    </Badge>
                    <span className="flex items-center gap-1 text-xs text-gray-500">
                      <Star className="h-3.5 w-3.5 fill-yellow-400 text-yellow-400" />
                      {selectedCompany.rating ?? 0}
                    </span>
                  </div>
                </div>
              </div>
              <dl className="grid grid-cols-2 gap-3 text-sm">
                <div>
                  <dt className="text-gray-500">Email</dt>
                  <dd className="font-medium break-all">
                    {selectedCompany.companyEmail || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Phone</dt>
                  <dd className="font-medium">
                    {selectedCompany.phoneNumber || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Reg. No.</dt>
                  <dd className="font-medium">
                    {selectedCompany.companyRegNo || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Value Charge</dt>
                  <dd className="font-medium">
                    ₦{(selectedCompany.valueCharge || 0).toLocaleString()}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Trucks</dt>
                  <dd className="font-medium">
                    {selectedCompany.noOfTrucks ?? 0}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Bikes</dt>
                  <dd className="font-medium">
                    {selectedCompany.nofOfBikes ?? 0}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">State</dt>
                  <dd className="font-medium">
                    {selectedCompany.state || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Locality</dt>
                  <dd className="font-medium">
                    {selectedCompany.locality || "—"}
                  </dd>
                </div>
                <div className="col-span-2">
                  <dt className="text-gray-500">Address</dt>
                  <dd className="font-medium">
                    {selectedCompany.companyAddress || "—"}
                  </dd>
                </div>
                <div className="col-span-2">
                  <dt className="text-gray-500">Info</dt>
                  <dd className="font-medium">
                    {selectedCompany.companyInfo || "—"}
                  </dd>
                </div>
                <div className="col-span-2">
                  <dt className="text-gray-500">Delivery Types</dt>
                  <dd className="font-medium">
                    {selectedCompany.deliveryTypes?.length
                      ? selectedCompany.deliveryTypes.join(", ")
                      : "—"}
                  </dd>
                </div>
                <div className="col-span-2">
                  <dt className="text-gray-500">Service Areas</dt>
                  <dd className="font-medium">
                    {selectedCompany.serviceAreas?.length
                      ? selectedCompany.serviceAreas.join(", ")
                      : "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Bank</dt>
                  <dd className="font-medium">
                    {selectedCompany.bankName || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Account No.</dt>
                  <dd className="font-medium">
                    {selectedCompany.accountNumber || "—"}
                  </dd>
                </div>
                <div>
                  <dt className="text-gray-500">Joined</dt>
                  <dd className="font-medium">
                    {selectedCompany.createdAt
                      ? new Date(selectedCompany.createdAt).toLocaleDateString()
                      : "—"}
                  </dd>
                </div>
              </dl>
            </div>
          </div>
        </div>
      )}
    </Card>
  );
}
