"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
// ❌ Remove this import: import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  UploadCloud,
  Loader2,
  X,
  Plus,
  LinkIcon,
  AlertCircle,
  CheckCircle2,
} from "lucide-react";
import { uploadImage, uploadImages } from "@/lib/api";

// ─── Constants ────────────────────────────────────────────────────────────────

const MAX_IMAGES = 8;

const ACCOMMODATION_TYPES = [
  "ComfortBnB",
  "Hotel",
  "Motel",
  "Hostel",
  "Apartment",
  "Villa",
  "Resort",
  "GuestHouse",
];

const DAYS = [
  "monday",
  "tuesday",
  "wednesday",
  "thursday",
  "friday",
  "saturday",
  "sunday",
];

const DEFAULT_SCHEDULE = Object.fromEntries(
  DAYS.map((day) => [day, { start: "9:00 AM", end: "5:00 PM", isClosed: false }])
);

// ─── Types ────────────────────────────────────────────────────────────────────

interface ImageFile {
  file: File;
  url: string;
  uploading: boolean;
  uploaded: boolean;
  error: boolean;
}

interface TimeSlot {
  start: string;
  end: string;
  isClosed: boolean;
}

interface FormData {
  accomodationName: string;
  accomodationLogo: string;
  accomodationEmail: string;
  accomodationDescription: string;
  accomodationType: string;
  checkInTime: string;
  checkOutTime: string;
  accomodationWebsite: string;
  accomodationPhoneNo: string;
  location: string;
  state: string;
  country: string;
  locality: string;
  postcode: string;
  latitude: string;
  longitude: string;
  bookingAmount: string;
  noOfRooms: string;
  accomodationImages: string[];
  accomodationFacilities: string[];
  timeSchedule: Record<string, TimeSlot>;
}

// ─── Helper Functions ────────────────────────────────────────────────────────

const getToken = (): string | null => {
  if (typeof window !== "undefined") {
    return localStorage.getItem("token");
  }
  return null;
};

const getUserId = (): string | null => {
  if (typeof window !== "undefined") {
    const user = localStorage.getItem("user");
    if (user) {
      try {
        const userData = JSON.parse(user);
        return userData.userId || userData.id || null;
      } catch {
        return null;
      }
    }
  }
  return null;
};

// ─── Component ────────────────────────────────────────────────────────────────

export default function AddAccommodation() {
  const router = useRouter();

  const [formData, setFormData] = useState<FormData>({
    accomodationName: "",
    accomodationLogo: "",
    accomodationEmail: "",
    accomodationDescription: "",
    accomodationType: "",
    checkInTime: "",
    checkOutTime: "",
    accomodationWebsite: "",
    accomodationPhoneNo: "",
    location: "",
    state: "",
    country: "",
    locality: "",
    postcode: "",
    latitude: "",
    longitude: "",
    bookingAmount: "",
    noOfRooms: "",
    accomodationImages: [],
    accomodationFacilities: [],
    timeSchedule: DEFAULT_SCHEDULE,
  });

  const [imageFiles, setImageFiles] = useState<ImageFile[]>([]);
  const [imageUrlInput, setImageUrlInput] = useState("");
  const [facilityInput, setFacilityInput] = useState("");
  const [uploadProgress, setUploadProgress] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isUploadingAny = imageFiles.some((img) => img.uploading);
  const totalImages = formData.accomodationImages.length;

  // ─── Helpers ──────────────────────────────────────────────────────────────

  function setField<K extends keyof FormData>(key: K, value: FormData[K]) {
    setFormData((prev) => ({ ...prev, [key]: value }));
  }

  // ─── Image Upload ─────────────────────────────────────────────────────────

  async function handleFileSelect(e: React.ChangeEvent<HTMLInputElement>) {
    const selected = Array.from(e.target.files ?? []);
    e.target.value = "";

    const remaining = MAX_IMAGES - totalImages;
    const toUpload = selected.slice(0, remaining);

    if (!toUpload.length) return;

    // Create preview entries
    const entries: ImageFile[] = toUpload.map((file) => ({
      file,
      url: URL.createObjectURL(file),
      uploading: true,
      uploaded: false,
      error: false,
    }));

    setImageFiles((prev) => [...prev, ...entries]);

    setUploadProgress(`Uploading ${toUpload.length} image(s)...`);

    try {
      // Use batch upload helper for reliability
      const results = await uploadImages(toUpload);

      // Normalize results to array of URLs/objects
      const urls: string[] = results.map((r: any) => (r?.url ?? r?.imageUrl ?? r ?? "")).filter(Boolean);

      // Append uploaded URLs to form data
      setFormData((prev) => ({
        ...prev,
        accomodationImages: [...prev.accomodationImages, ...urls],
      }));

      // Replace preview URLs with real URLs in imageFiles state
      setImageFiles((prev) =>
        prev.map((img) => {
          const match = entries.find((e) => e.url === img.url);
          if (!match) return img;
          const idx = entries.indexOf(match);
          const uploaded = urls[idx];
          if (uploaded) return { ...img, url: uploaded, uploading: false, uploaded: true };
          return { ...img, uploading: false, error: true };
        })
      );
    } catch (err) {
      const message = err instanceof Error ? err.message : "Image upload failed";
      setError(message);
      // mark previews as errored
      setImageFiles((prev) => prev.map((img) => ({ ...img, uploading: false, error: true })));
    } finally {
      setUploadProgress("");
    }
  }

  function addImageUrl() {
    const url = imageUrlInput.trim();
    if (!url || totalImages >= MAX_IMAGES) return;

    setField("accomodationImages", [...formData.accomodationImages, url]);
    setImageUrlInput("");
  }

  function removeImage(index: number) {
    const removed = formData.accomodationImages[index];
    setField(
      "accomodationImages",
      formData.accomodationImages.filter((_, i) => i !== index)
    );
    // Clean up file entry if it was an uploaded file
    setImageFiles((prev) => prev.filter((img) => img.url !== removed));
  }

  // ─── Facilities ───────────────────────────────────────────────────────────

  function addFacility() {
    const val = facilityInput.trim();
    if (!val || formData.accomodationFacilities.includes(val)) return;
    setField("accomodationFacilities", [...formData.accomodationFacilities, val]);
    setFacilityInput("");
  }

  function removeFacility(item: string) {
    setField(
      "accomodationFacilities",
      formData.accomodationFacilities.filter((f) => f !== item)
    );
  }

  // ─── Time Schedule ────────────────────────────────────────────────────────

  function updateSchedule(
    day: string,
    field: keyof TimeSlot,
    value: string | boolean
  ) {
    setFormData((prev) => ({
      ...prev,
      timeSchedule: {
        ...prev.timeSchedule,
        [day]: { ...prev.timeSchedule[day], [field]: value },
      },
    }));
  }

  // ─── Submit ───────────────────────────────────────────────────────────────

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    // Basic validation
    if (!formData.accomodationName.trim()) {
      setError("Accommodation name is required.");
      return;
    }
    if (!formData.accomodationType) {
      setError("Please select an accommodation type.");
      return;
    }
    if (formData.accomodationImages.length === 0) {
      setError("Please upload at least one image.");
      return;
    }
    if (isUploadingAny) {
      setError("Please wait for all images to finish uploading.");
      return;
    }

    setIsLoading(true);

    try {
      const token = getToken();
      const userId = getUserId();

      if (!token) {
        throw new Error("Authentication required. Please login again.");
      }

      const payload = {
        ...formData,
        bookingAmount: Number(formData.bookingAmount),
        noOfRooms: Number(formData.noOfRooms),
        latitude: parseFloat(formData.latitude) || 0,
        longitude: parseFloat(formData.longitude) || 0,
      };

      const baseUrl = process.env.NEXT_PUBLIC_API_URL || "https://api-data-connection.ginilog.org";
      const endpoint = `${baseUrl}/api/bookings/accomodation`;

      console.log("📤 Adding accommodation with payload:", payload);

      const response = await fetch(endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
          ...(userId ? { userId } : {}),
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        const errorMessage = errorData.message || errorData.title || `Failed to add accommodation (${response.status})`;
        throw new Error(errorMessage);
      }

      const result = await response.json();
      console.log("✅ Accommodation added successfully:", result);

      setSuccess(true);
      setTimeout(() => router.push("/brand-owner/properties"), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
    } finally {
      setIsLoading(false);
    }
  }

  // ─── Render ───────────────────────────────────────────────────────────────

  return (
    <div className="max-w-3xl mx-auto py-10 px-4">
      <div className="mb-8">
        <h2 className="text-2xl font-bold text-gray-900">Add Accommodation</h2>
        <p className="text-sm text-gray-500 mt-1">
          Fill in the details below to list a new accommodation.
        </p>
      </div>

      {success && (
        <div className="flex items-center gap-2 p-4 mb-6 bg-green-50 border border-green-200 rounded-lg text-green-700">
          <CheckCircle2 className="h-5 w-5 shrink-0" />
          <p className="text-sm font-medium">Accommodation added successfully! Redirecting...</p>
        </div>
      )}

      {error && (
        <div className="flex items-center gap-2 p-4 mb-6 bg-red-50 border border-red-200 rounded-lg text-red-700">
          <AlertCircle className="h-5 w-5 shrink-0" />
          <p className="text-sm">{error}</p>
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-8">
        {/* ── Basic Info ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Basic Information</h3>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <Label>Accommodation Name *</Label>
              <Input
                value={formData.accomodationName}
                onChange={(e) => setField("accomodationName", e.target.value)}
                placeholder="e.g. Saints Comfort"
                disabled={isLoading || success}
                required
              />
            </div>
            <div>
              <Label>Type *</Label>
              <Select
                value={formData.accomodationType}
                onValueChange={(v) => setField("accomodationType", v)}
                disabled={isLoading || success}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select type" />
                </SelectTrigger>
                <SelectContent>
                  {ACCOMMODATION_TYPES.map((t) => (
                    <SelectItem key={t} value={t}>{t}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Email</Label>
              <Input
                type="email"
                value={formData.accomodationEmail}
                onChange={(e) => setField("accomodationEmail", e.target.value)}
                placeholder="contact@hotel.com"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Phone Number</Label>
              <Input
                value={formData.accomodationPhoneNo}
                onChange={(e) => setField("accomodationPhoneNo", e.target.value)}
                placeholder="e.g. 08100784540"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Website</Label>
              <Input
                value={formData.accomodationWebsite}
                onChange={(e) => setField("accomodationWebsite", e.target.value)}
                placeholder="e.g. luxury.com"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Logo URL</Label>
              <Input
                value={formData.accomodationLogo}
                onChange={(e) => setField("accomodationLogo", e.target.value)}
                placeholder="https://..."
                disabled={isLoading || success}
              />
            </div>
          </div>

          <div>
            <Label>Description</Label>
            {/* ✅ Replaced Textarea with native HTML textarea */}
            <textarea
              value={formData.accomodationDescription}
              onChange={(e) => setField("accomodationDescription", e.target.value)}
              placeholder="Describe the accommodation..."
              rows={3}
              disabled={isLoading || success}
              className="flex w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
            />
          </div>
        </section>

        {/* ── Location ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Location</h3>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="sm:col-span-2">
              <Label>Street Address</Label>
              <Input
                value={formData.location}
                onChange={(e) => setField("location", e.target.value)}
                placeholder="e.g. Chime Avenue New Haven"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>State</Label>
              <Input
                value={formData.state}
                onChange={(e) => setField("state", e.target.value)}
                placeholder="e.g. Enugu"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Country</Label>
              <Input
                value={formData.country}
                onChange={(e) => setField("country", e.target.value)}
                placeholder="e.g. Nigeria"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Locality</Label>
              <Input
                value={formData.locality}
                onChange={(e) => setField("locality", e.target.value)}
                placeholder="e.g. Enugu North"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Postcode</Label>
              <Input
                value={formData.postcode}
                onChange={(e) => setField("postcode", e.target.value)}
                placeholder="e.g. 400102"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Latitude</Label>
              <Input
                type="number"
                step="any"
                value={formData.latitude}
                onChange={(e) => setField("latitude", e.target.value)}
                placeholder="e.g. 6.4582"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Longitude</Label>
              <Input
                type="number"
                step="any"
                value={formData.longitude}
                onChange={(e) => setField("longitude", e.target.value)}
                placeholder="e.g. 7.5265"
                disabled={isLoading || success}
              />
            </div>
          </div>
        </section>

        {/* ── Pricing & Rooms ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Pricing & Rooms</h3>

          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div>
              <Label>Booking Amount (₦)</Label>
              <Input
                type="number"
                min={0}
                value={formData.bookingAmount}
                onChange={(e) => setField("bookingAmount", e.target.value)}
                placeholder="e.g. 100"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Number of Rooms</Label>
              <Input
                type="number"
                min={1}
                value={formData.noOfRooms}
                onChange={(e) => setField("noOfRooms", e.target.value)}
                placeholder="e.g. 150"
                disabled={isLoading || success}
              />
            </div>
          </div>
        </section>

        {/* ── Check-in / Check-out ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Check-in & Check-out</h3>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <Label>Check-in Time</Label>
              <Input
                value={formData.checkInTime}
                onChange={(e) => setField("checkInTime", e.target.value)}
                placeholder="e.g. 9:00 AM"
                disabled={isLoading || success}
              />
            </div>
            <div>
              <Label>Check-out Time</Label>
              <Input
                value={formData.checkOutTime}
                onChange={(e) => setField("checkOutTime", e.target.value)}
                placeholder="e.g. 12:00 PM"
                disabled={isLoading || success}
              />
            </div>
          </div>
        </section>

        {/* ── Facilities ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Facilities</h3>

          <div className="flex gap-2">
            <Input
              value={facilityInput}
              onChange={(e) => setFacilityInput(e.target.value)}
              placeholder="e.g. Swimming Pool"
              disabled={isLoading || success}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  addFacility();
                }
              }}
            />
            <Button
              type="button"
              variant="outline"
              onClick={addFacility}
              disabled={isLoading || success || !facilityInput.trim()}
            >
              <Plus className="h-4 w-4 mr-1" /> Add
            </Button>
          </div>

          {formData.accomodationFacilities.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {formData.accomodationFacilities.map((f) => (
                <Badge key={f} variant="secondary" className="flex items-center gap-1 pr-1">
                  {f}
                  <button
                    type="button"
                    onClick={() => removeFacility(f)}
                    disabled={isLoading || success}
                    className="ml-1 hover:text-red-500"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              ))}
            </div>
          )}
        </section>

        {/* ── Images ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">
            Images * ({totalImages}/{MAX_IMAGES})
          </h3>

          {/* Drop zone */}
          <div
            className={`relative flex flex-col items-center justify-center w-full p-6 border-2 border-dashed rounded-xl transition-colors ${
              isUploadingAny
                ? "border-primary/40 bg-primary/5"
                : totalImages >= MAX_IMAGES
                ? "border-gray-200 bg-gray-50 opacity-60 cursor-not-allowed"
                : "border-gray-300 hover:border-primary/50 hover:bg-gray-50 cursor-pointer"
            }`}
          >
            <div className="flex flex-col items-center gap-2 text-center pointer-events-none">
              {isUploadingAny ? (
                <>
                  <Loader2 className="h-8 w-8 animate-spin text-primary" />
                  <p className="text-sm font-medium text-primary">
                    {uploadProgress || "Uploading..."}
                  </p>
                  <p className="text-xs text-gray-400">
                    {imageFiles.filter((img) => img.uploaded).length} of{" "}
                    {imageFiles.length} uploaded
                  </p>
                </>
              ) : (
                <>
                  <UploadCloud className="h-10 w-10 text-gray-400" />
                  <p className="text-sm font-medium text-gray-600">
                    {totalImages >= MAX_IMAGES
                      ? `Maximum of ${MAX_IMAGES} images reached`
                      : "Click or drag images here to upload"}
                  </p>
                  <p className="text-xs text-gray-400">
                    JPEG, PNG · max 10MB each · up to {MAX_IMAGES} images
                  </p>
                </>
              )}
            </div>
            <Input
              type="file"
              accept=".jpg,.jpeg,.png"
              multiple
              onChange={handleFileSelect}
              className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
              disabled={isUploadingAny || isLoading || success || totalImages >= MAX_IMAGES}
            />
          </div>

          {/* OR divider */}
          <div className="relative my-2">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-gray-200" />
            </div>
            <div className="relative flex justify-center">
              <span className="px-2 bg-white text-xs font-medium text-gray-400 uppercase tracking-wide">
                or paste a URL
              </span>
            </div>
          </div>

          {/* URL input */}
          <div className="flex gap-2">
            <div className="relative flex-1">
              <LinkIcon className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                value={imageUrlInput}
                onChange={(e) => setImageUrlInput(e.target.value)}
                placeholder="https://example.com/image.jpg"
                className="pl-10"
                disabled={isLoading || success || totalImages >= MAX_IMAGES}
                onKeyDown={(e) => {
                  if (e.key === "Enter") {
                    e.preventDefault();
                    addImageUrl();
                  }
                }}
              />
            </div>
            <Button
              type="button"
              variant="outline"
              onClick={addImageUrl}
              disabled={
                isLoading || success || totalImages >= MAX_IMAGES || !imageUrlInput.trim()
              }
            >
              <Plus className="h-4 w-4 mr-1" /> Add
            </Button>
          </div>

          {/* Previews */}
          {formData.accomodationImages.length > 0 && (
            <>
              <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mt-2">
                {formData.accomodationImages.map((image, index) => {
                  const fileEntry = imageFiles.find((img) => img.url === image);
                  const isUploading = fileEntry?.uploading ?? false;
                  const hasError = fileEntry?.error ?? false;
                  const isUploadedFile = fileEntry?.uploaded ?? false;
                  const isUrlEntry = !fileEntry;

                  return (
                    <div
                      key={index}
                      className="relative group rounded-lg overflow-hidden border border-gray-200"
                    >
                      <img
                        src={image}
                        alt={`Accommodation image ${index + 1}`}
                        className={`w-full h-28 object-cover transition-opacity ${
                          isUploading ? "opacity-50" : "opacity-100"
                        }`}
                        onError={(e) => {
                          (e.target as HTMLImageElement).src =
                            "https://via.placeholder.com/400x300?text=Invalid+Image";
                        }}
                      />
                      {isUploading && (
                        <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                          <Loader2 className="h-5 w-5 animate-spin text-white" />
                        </div>
                      )}
                      {hasError && (
                        <div className="absolute inset-0 bg-red-500/10 flex flex-col items-center justify-center gap-1">
                          <AlertCircle className="h-5 w-5 text-red-500" />
                          <span className="text-xs text-red-500 font-medium">Failed</span>
                        </div>
                      )}
                      {!isUploading && !hasError && (
                        <div className="absolute top-1 left-1">
                          {isUrlEntry ? (
                            <Badge className="text-xs bg-blue-100 text-blue-700 border-0">URL</Badge>
                          ) : isUploadedFile ? (
                            <Badge className="text-xs bg-green-100 text-green-700 border-0">Uploaded</Badge>
                          ) : null}
                        </div>
                      )}
                      <div className="absolute bottom-1 left-1">
                        <Badge className="text-xs bg-black/50 text-white border-0">
                          {index + 1}
                        </Badge>
                      </div>
                      <button
                        type="button"
                        onClick={() => removeImage(index)}
                        className="absolute top-1 right-1 p-1 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-600"
                        disabled={isUploading || isLoading || success}
                        aria-label={`Remove image ${index + 1}`}
                      >
                        <X className="h-3 w-3" />
                      </button>
                    </div>
                  );
                })}
              </div>
              <p className="text-xs text-gray-400">
                {MAX_IMAGES - totalImages > 0
                  ? `You can add ${MAX_IMAGES - totalImages} more image${MAX_IMAGES - totalImages === 1 ? "" : "s"}`
                  : "Maximum images reached"}
              </p>
            </>
          )}
        </section>

        {/* ── Time Schedule ── */}
        <section className="space-y-4">
          <h3 className="text-base font-semibold text-gray-800 border-b pb-2">Weekly Schedule</h3>

          <div className="space-y-3">
            {DAYS.map((day) => {
              const slot = formData.timeSchedule[day];
              return (
                <div
                  key={day}
                  className={`grid grid-cols-[120px_1fr_1fr_auto] items-center gap-3 p-3 rounded-lg border ${
                    slot.isClosed ? "bg-gray-50 opacity-60" : "bg-white"
                  }`}
                >
                  <span className="text-sm font-medium capitalize text-gray-700">{day}</span>
                  <Input
                    value={slot.start}
                    onChange={(e) => updateSchedule(day, "start", e.target.value)}
                    placeholder="9:00 AM"
                    disabled={slot.isClosed || isLoading || success}
                    className="text-sm"
                  />
                  <Input
                    value={slot.end}
                    onChange={(e) => updateSchedule(day, "end", e.target.value)}
                    placeholder="5:00 PM"
                    disabled={slot.isClosed || isLoading || success}
                    className="text-sm"
                  />
                  <label className="flex items-center gap-1.5 text-xs text-gray-500 cursor-pointer select-none">
                    <input
                      type="checkbox"
                      checked={slot.isClosed}
                      onChange={(e) => updateSchedule(day, "isClosed", e.target.checked)}
                      disabled={isLoading || success}
                      className="rounded"
                    />
                    Closed
                  </label>
                </div>
              );
            })}
          </div>
        </section>

        {/* ── Submit ── */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t">
          <Button
            type="button"
            variant="outline"
            onClick={() => router.back()}
            disabled={isLoading || success}
          >
            Cancel
          </Button>
          <Button type="submit" disabled={isLoading || success || isUploadingAny}>
            {isLoading ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Saving...
              </>
            ) : success ? (
              <>
                <CheckCircle2 className="h-4 w-4 mr-2" />
                Saved!
              </>
            ) : (
              "Add Accommodation"
            )}
          </Button>
        </div>
      </form>
    </div>
  );
}