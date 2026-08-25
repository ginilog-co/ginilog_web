// app/brand-owner/reservations/add-rooms/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Loader2,
  ArrowLeft,
  Plus,
  X,
  UploadCloud,
  LinkIcon,
  AlertCircle,
  CheckCircle2,
  Hotel,
  Users,
  DollarSign,
  Bed,
  Wifi,
  Tv,
  Coffee,
  Car,
  Utensils,
  Dumbbell,
  Waves,
  Sparkles,
  CircleCheck
} from "lucide-react";
import { 
  addReservationRoom, 
  uploadImages, 
  getAccommodationById,
  getStoredUser,
  getToken
} from "@/lib/api";

// ─── Constants ────────────────────────────────────────────────────────────────

const MAX_IMAGES = 6;
const ROOM_TYPES = ["Single", "Double", "Suite", "Flat", "Studio", "Penthouse", "Deluxe", "Executive"];

const FEATURE_OPTIONS = [
  { label: "Wi-Fi", icon: Wifi },
  { label: "TV", icon: Tv },
  { label: "Coffee Maker", icon: Coffee },
  { label: "Air Conditioning", icon: Sparkles },
  { label: "Parking", icon: Car },
  { label: "Restaurant", icon: Utensils },
  { label: "Gym", icon: Dumbbell },
  { label: "Pool", icon: Waves },
  { label: "Desk", icon: CircleCheck },
  { label: "Refrigerator", icon: CircleCheck },
];

// ─── Types ────────────────────────────────────────────────────────────────────

interface ImageFile {
  file: File;
  url: string;
  uploading: boolean;
  uploaded: boolean;
  error: boolean;
}

interface FormData {
  roomNumber: string;
  maximumNoOfGuest: string;
  roomPrice: string;
  roomType: string;
  roomImages: string[];
  roomFeatures: string[];
  isBooked: boolean;
}

// ─── Component ────────────────────────────────────────────────────────────────

export default function AddReservationRoomsPage() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const accommodationId = searchParams.get("accommodationId");

  const [accommodation, setAccommodation] = useState<any>(null);
  const [formData, setFormData] = useState<FormData>({
    roomNumber: "",
    maximumNoOfGuest: "",
    roomPrice: "",
    roomType: "",
    roomImages: [],
    roomFeatures: [],
    isBooked: false,
  });
  
  const [imageFiles, setImageFiles] = useState<ImageFile[]>([]);
  const [imageUrlInput, setImageUrlInput] = useState("");
  const [uploadProgress, setUploadProgress] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [addedRooms, setAddedRooms] = useState<any[]>([]);
  const [isAddingAnother, setIsAddingAnother] = useState(false);

  const totalImages = formData.roomImages.length;
  const isUploadingAny = imageFiles.some((img) => img.uploading);

  // ─── Fetch Accommodation ──────────────────────────────────────────────────

  useEffect(() => {
    if (accommodationId) {
      fetchAccommodation();
    }
  }, [accommodationId]);

  const fetchAccommodation = async () => {
    try {
      const data = await getAccommodationById(accommodationId!);
      setAccommodation(data);
    } catch (error) {
      console.error("Failed to fetch accommodation:", error);
      setError("Failed to load accommodation details");
    }
  };

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
      const results = await uploadImages(toUpload);
      const urls: string[] = results.map((r: any) => (r?.url ?? r?.imageUrl ?? r ?? "")).filter(Boolean);

      setFormData((prev) => ({
        ...prev,
        roomImages: [...prev.roomImages, ...urls],
      }));

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
      setImageFiles((prev) => prev.map((img) => ({ ...img, uploading: false, error: true })));
    } finally {
      setUploadProgress("");
    }
  }

  function addImageUrl() {
    const url = imageUrlInput.trim();
    if (!url || totalImages >= MAX_IMAGES) return;
    setField("roomImages", [...formData.roomImages, url]);
    setImageUrlInput("");
  }

  function removeImage(index: number) {
    const removed = formData.roomImages[index];
    setField("roomImages", formData.roomImages.filter((_, i) => i !== index));
    setImageFiles((prev) => prev.filter((img) => img.url !== removed));
  }

  // ─── Features ─────────────────────────────────────────────────────────────

  function toggleFeature(feature: string) {
    setFormData((prev) => ({
      ...prev,
      roomFeatures: prev.roomFeatures.includes(feature)
        ? prev.roomFeatures.filter((f) => f !== feature)
        : [...prev.roomFeatures, feature],
    }));
  }

  // ─── Submit ───────────────────────────────────────────────────────────────

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    // Validation
    if (!formData.roomNumber) {
      setError("Room number is required.");
      return;
    }
    if (!formData.maximumNoOfGuest) {
      setError("Maximum number of guests is required.");
      return;
    }
    if (!formData.roomPrice) {
      setError("Room price is required.");
      return;
    }
    if (!formData.roomType) {
      setError("Room type is required.");
      return;
    }
    if (formData.roomImages.length === 0) {
      setError("Please add at least one room image.");
      return;
    }
    if (isUploadingAny) {
      setError("Please wait for all images to finish uploading.");
      return;
    }
    if (!accommodationId) {
      setError("Accommodation ID is missing. Please go back and select an accommodation.");
      return;
    }

    setIsSubmitting(true);

    try {
      const payload = {
        roomNumber: Number(formData.roomNumber),
        maximumNoOfGuest: Number(formData.maximumNoOfGuest),
        roomPrice: Number(formData.roomPrice),
        roomType: formData.roomType,
        roomImages: formData.roomImages,
        roomFeatures: formData.roomFeatures,
        isBooked: false,
      };

      const result = await addReservationRoom(accommodationId, payload);

      if (result.success || result.data) {
        setAddedRooms((prev) => [...prev, result.data || { roomNumber: formData.roomNumber }]);
        setSuccess(true);
        
        // Reset form for adding another room
        if (isAddingAnother) {
          resetForm();
          setSuccess(false);
        } else {
          setTimeout(() => {
            router.push(`/brand-owner/reservations/rooms?accommodationId=${accommodationId}`);
          }, 1500);
        }
      } else {
        throw new Error(result.message || "Failed to add room");
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : "Something went wrong.");
      setSuccess(false);
    } finally {
      setIsSubmitting(false);
    }
  }

  function resetForm() {
    setFormData({
      roomNumber: "",
      maximumNoOfGuest: "",
      roomPrice: "",
      roomType: "",
      roomImages: [],
      roomFeatures: [],
      isBooked: false,
    });
    setImageFiles([]);
    setImageUrlInput("");
    setError(null);
    setSuccess(false);
  }

  // ─── Render ───────────────────────────────────────────────────────────────

  if (!accommodationId) {
    return (
      <div className="max-w-2xl mx-auto py-20 px-4 text-center">
        <AlertCircle className="h-12 w-12 text-red-500 mx-auto mb-4" />
        <h2 className="text-xl font-semibold text-gray-900 mb-2">No Accommodation Selected</h2>
        <p className="text-gray-500 mb-6">Please select an accommodation first to add rooms.</p>
        <Button onClick={() => router.push("/brand-owner/reservations")}>
          Go to Reservations
        </Button>
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto py-10 px-4">
      {/* Header */}
      <div className="flex items-center gap-4 mb-8">
        <Link href={`/brand-owner/reservations/rooms?accommodationId=${accommodationId}`}>
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back to Rooms
          </Button>
        </Link>
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Add Reservation Rooms</h2>
          <p className="text-sm text-gray-500 mt-1">
            {accommodation?.accomodationName || "Accommodation"} — Add rooms to this property
          </p>
        </div>
      </div>

      {/* Success Message */}
      {success && !isAddingAnother && (
        <div className="flex items-center gap-2 p-4 mb-6 bg-green-50 border border-green-200 rounded-lg text-green-700">
          <CheckCircle2 className="h-5 w-5 shrink-0" />
          <p className="text-sm font-medium">Room added successfully! Redirecting...</p>
        </div>
      )}

      {success && isAddingAnother && (
        <div className="flex items-center gap-2 p-4 mb-6 bg-green-50 border border-green-200 rounded-lg text-green-700">
          <CheckCircle2 className="h-5 w-5 shrink-0" />
          <p className="text-sm font-medium">Room added successfully! Ready for the next room.</p>
        </div>
      )}

      {error && (
        <div className="flex items-center gap-2 p-4 mb-6 bg-red-50 border border-red-200 rounded-lg text-red-700">
          <AlertCircle className="h-5 w-5 shrink-0" />
          <p className="text-sm">{error}</p>
        </div>
      )}

      {/* Added Rooms Summary */}
      {addedRooms.length > 0 && (
        <Card className="mb-6">
          <CardHeader>
            <CardTitle className="text-sm">Added Rooms ({addedRooms.length})</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex flex-wrap gap-2">
              {addedRooms.map((room, index) => (
                <Badge key={index} variant="secondary" className="flex items-center gap-1">
                  <Hotel className="h-3 w-3" />
                  Room {room.roomNumber || index + 1}
                </Badge>
              ))}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Form */}
      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Add Another Toggle */}
        <div className="flex items-center gap-2 p-3 bg-gray-50 rounded-lg border">
          <input
            type="checkbox"
            id="addAnother"
            checked={isAddingAnother}
            onChange={(e) => setIsAddingAnother(e.target.checked)}
            className="rounded border-gray-300"
          />
          <label htmlFor="addAnother" className="text-sm text-gray-700">
            Add multiple rooms (stay on this page after adding)
          </label>
        </div>

        {/* Room Details */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <Label>Room Number *</Label>
            <Input
              type="number"
              value={formData.roomNumber}
              onChange={(e) => setField("roomNumber", e.target.value)}
              placeholder="e.g. 101"
              disabled={isSubmitting}
              required
            />
          </div>
          <div>
            <Label>Room Type *</Label>
            <Select
              value={formData.roomType}
              onValueChange={(v) => setField("roomType", v)}
              disabled={isSubmitting}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select room type" />
              </SelectTrigger>
              <SelectContent>
                {ROOM_TYPES.map((type) => (
                  <SelectItem key={type} value={type}>{type}</SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <Label>Maximum Guests *</Label>
            <Input
              type="number"
              min={1}
              value={formData.maximumNoOfGuest}
              onChange={(e) => setField("maximumNoOfGuest", e.target.value)}
              placeholder="e.g. 4"
              disabled={isSubmitting}
              required
            />
          </div>
          <div>
            <Label>Room Price (₦) *</Label>
            <Input
              type="number"
              min={0}
              value={formData.roomPrice}
              onChange={(e) => setField("roomPrice", e.target.value)}
              placeholder="e.g. 50000"
              disabled={isSubmitting}
              required
            />
          </div>
        </div>

        {/* Room Features */}
        <div>
          <Label>Room Features</Label>
          <div className="flex flex-wrap gap-2 mt-2">
            {FEATURE_OPTIONS.map(({ label, icon: Icon }) => (
              <button
                key={label}
                type="button"
                onClick={() => toggleFeature(label)}
                className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full text-sm transition-colors ${
                  formData.roomFeatures.includes(label)
                    ? "bg-primary text-white hover:bg-primary/90"
                    : "bg-gray-100 text-gray-600 hover:bg-gray-200"
                }`}
                disabled={isSubmitting}
              >
                <Icon className="h-3.5 w-3.5" />
                {label}
              </button>
            ))}
          </div>
          {formData.roomFeatures.length > 0 && (
            <div className="flex flex-wrap gap-2 mt-2">
              {formData.roomFeatures.map((f) => (
                <Badge key={f} variant="secondary" className="flex items-center gap-1 pr-1">
                  {f}
                  <button
                    type="button"
                    onClick={() => toggleFeature(f)}
                    disabled={isSubmitting}
                    className="ml-1 hover:text-red-500"
                  >
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              ))}
            </div>
          )}
        </div>

        {/* Images */}
        <div>
          <Label>Room Images * ({totalImages}/{MAX_IMAGES})</Label>
          
          <div
            className={`relative flex flex-col items-center justify-center w-full p-4 mt-2 border-2 border-dashed rounded-xl transition-colors ${
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
                  <p className="text-sm font-medium text-primary">{uploadProgress || "Uploading..."}</p>
                </>
              ) : (
                <>
                  <UploadCloud className="h-8 w-8 text-gray-400" />
                  <p className="text-sm font-medium text-gray-600">
                    {totalImages >= MAX_IMAGES ? "Maximum images reached" : "Click or drag images here"}
                  </p>
                  <p className="text-xs text-gray-400">JPEG, PNG · max 10MB each · up to {MAX_IMAGES} images</p>
                </>
              )}
            </div>
            <Input
              type="file"
              accept=".jpg,.jpeg,.png"
              multiple
              onChange={handleFileSelect}
              className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
              disabled={isUploadingAny || isSubmitting || totalImages >= MAX_IMAGES}
            />
          </div>

          {/* URL Input */}
          <div className="flex gap-2 mt-2">
            <div className="relative flex-1">
              <LinkIcon className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                value={imageUrlInput}
                onChange={(e) => setImageUrlInput(e.target.value)}
                placeholder="https://example.com/room-image.jpg"
                className="pl-10"
                disabled={isSubmitting || totalImages >= MAX_IMAGES}
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
              disabled={isSubmitting || totalImages >= MAX_IMAGES || !imageUrlInput.trim()}
            >
              <Plus className="h-4 w-4 mr-1" /> Add
            </Button>
          </div>

          {/* Image Previews */}
          {formData.roomImages.length > 0 && (
            <div className="grid grid-cols-2 sm:grid-cols-4 gap-2 mt-3">
              {formData.roomImages.map((image, index) => {
                const fileEntry = imageFiles.find((img) => img.url === image);
                const isUploading = fileEntry?.uploading ?? false;
                const hasError = fileEntry?.error ?? false;
                return (
                  <div key={index} className="relative group rounded-lg overflow-hidden border border-gray-200 aspect-square">
                    <img
                      src={image}
                      alt={`Room ${index + 1}`}
                      className={`w-full h-full object-cover transition-opacity ${
                        isUploading ? "opacity-50" : "opacity-100"
                      }`}
                      onError={(e) => {
                        (e.target as HTMLImageElement).src = "https://via.placeholder.com/200x200?text=Invalid+Image";
                      }}
                    />
                    {isUploading && (
                      <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                        <Loader2 className="h-5 w-5 animate-spin text-white" />
                      </div>
                    )}
                    {hasError && (
                      <div className="absolute inset-0 bg-red-500/10 flex items-center justify-center">
                        <span className="text-xs text-red-500 font-medium">Failed</span>
                      </div>
                    )}
                    <button
                      type="button"
                      onClick={() => removeImage(index)}
                      className="absolute top-1 right-1 p-1 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-600"
                      disabled={isUploading || isSubmitting}
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        {/* Submit */}
        <div className="flex items-center justify-end gap-3 pt-4 border-t">
          <Button
            type="button"
            variant="outline"
            onClick={() => router.push(`/brand-owner/reservations/rooms?accommodationId=${accommodationId}`)}
            disabled={isSubmitting}
          >
            Cancel
          </Button>
          <Button type="submit" disabled={isSubmitting || isUploadingAny}>
            {isSubmitting ? (
              <>
                <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                Adding Room...
              </>
            ) : (
              <>
                <Plus className="h-4 w-4 mr-2" />
                {isAddingAnother ? "Add Another Room" : "Add Room"}
              </>
            )}
          </Button>
        </div>
      </form>
    </div>
  );
}