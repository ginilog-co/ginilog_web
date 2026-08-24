// app/brand-owner/properties/[id]/edit/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Upload, X, Plus } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { getAccommodationById, updateAccommodation, uploadImage } from "@/lib/api";

export default function EditPropertyPage() {
  const router = useRouter();
  const params = useParams();
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [imageFiles, setImageFiles] = useState<File[]>([]);
  const [imagePreviews, setImagePreviews] = useState<string[]>([]);
  const [existingImages, setExistingImages] = useState<string[]>([]);
  const [facilityInput, setFacilityInput] = useState("");
  const [formData, setFormData] = useState({
    accommodationName: "",
    accommodationType: "",
    location: "",
    state: "",
    country: "Nigeria",
    bookingAmount: "",
    noOfRooms: "",
    accommodationDescription: "",
    accommodationFacilities: [] as string[],
    available: true,
  });

  useEffect(() => {
    const fetchProperty = async () => {
      try {
        const data = await getAccommodationById(params.id as string);
        setFormData({
          accommodationName: data.accomodationName || "",
          accommodationType: data.accomodationType || "",
          location: data.location || "",
          state: data.state || "",
          country: data.country || "Nigeria",
          bookingAmount: data.bookingAmount?.toString() || "",
          noOfRooms: data.noOfRooms?.toString() || "",
          accommodationDescription: data.accomodationDescription || "",
          accommodationFacilities: data.accomodationFacilities || [],
          available: data.available !== undefined ? data.available : true,
        });
        setExistingImages(data.accomodationImages || []);
        setImagePreviews(data.accomodationImages || []);
      } catch (error) {
        console.error("Failed to fetch property:", error);
        setError("Failed to load property data");
      } finally {
        setIsLoading(false);
      }
    };
    fetchProperty();
  }, [params.id]);

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      const newFiles = Array.from(files);
      setImageFiles([...imageFiles, ...newFiles]);

      newFiles.forEach((file) => {
        const reader = new FileReader();
        reader.onloadend = () => {
          setImagePreviews((prev) => [...prev, reader.result as string]);
        };
        reader.readAsDataURL(file);
      });
    }
  };

  const removeImage = (index: number) => {
    setImageFiles(imageFiles.filter((_, i) => i !== index));
    setImagePreviews(imagePreviews.filter((_, i) => i !== index));
    if (index < existingImages.length) {
      setExistingImages(existingImages.filter((_, i) => i !== index));
    }
  };

  const addFacility = () => {
    if (facilityInput.trim()) {
      setFormData({
        ...formData,
        accommodationFacilities: [...formData.accommodationFacilities, facilityInput.trim()],
      });
      setFacilityInput("");
    }
  };

  const removeFacility = (index: number) => {
    setFormData({
      ...formData,
      accommodationFacilities: formData.accommodationFacilities.filter((_, i) => i !== index),
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setIsSubmitting(true);

    try {
      // Upload new images
      const uploadedImages = [...existingImages];
      for (const file of imageFiles) {
        const result = await uploadImage(file);
        uploadedImages.push(result.url || result.imageUrl);
      }

      const payload = {
        accommodationName: formData.accommodationName,
        accommodationType: formData.accommodationType,
        location: formData.location,
        state: formData.state,
        country: formData.country,
        bookingAmount: parseFloat(formData.bookingAmount),
        noOfRooms: parseInt(formData.noOfRooms),
        accommodationDescription: formData.accommodationDescription,
        accommodationImages: uploadedImages,
        accommodationFacilities: formData.accommodationFacilities,
        available: formData.available,
      };

      await updateAccommodation(params.id as string, payload);
      setSuccess("Property updated successfully!");
      setTimeout(() => router.push(`/brand-owner/properties/${params.id}`), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update property");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-96">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href={`/brand-owner/properties/${params.id}`}>
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Edit Property</h1>
          <p className="text-sm text-gray-500">Update your accommodation details</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Property Details</CardTitle>
        </CardHeader>
        <CardContent>
          {error && (
            <div className="mb-4 bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-xl text-sm">
              {error}
            </div>
          )}
          {success && (
            <div className="mb-4 bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded-xl text-sm">
              {success}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div>
              <Label htmlFor="accommodationName">Property Name *</Label>
              <Input
                id="accommodationName"
                value={formData.accommodationName}
                onChange={(e) => setFormData({ ...formData, accommodationName: e.target.value })}
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="accommodationType">Property Type *</Label>
                <select
                  id="accommodationType"
                  className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                  value={formData.accommodationType}
                  onChange={(e) => setFormData({ ...formData, accommodationType: e.target.value })}
                  required
                >
                  <option value="">Select type</option>
                  <option value="Hotel">Hotel</option>
                  <option value="Apartment">Apartment</option>
                  <option value="Villa">Villa</option>
                  <option value="Resort">Resort</option>
                  <option value="Guest House">Guest House</option>
                  <option value="B&B">Bed & Breakfast</option>
                </select>
              </div>
              <div>
                <Label htmlFor="noOfRooms">Number of Rooms *</Label>
                <Input
                  id="noOfRooms"
                  type="number"
                  value={formData.noOfRooms}
                  onChange={(e) => setFormData({ ...formData, noOfRooms: e.target.value })}
                  required
                  min="1"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="location">Location *</Label>
                <Input
                  id="location"
                  value={formData.location}
                  onChange={(e) => setFormData({ ...formData, location: e.target.value })}
                  required
                />
              </div>
              <div>
                <Label htmlFor="state">State *</Label>
                <Input
                  id="state"
                  value={formData.state}
                  onChange={(e) => setFormData({ ...formData, state: e.target.value })}
                  required
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="bookingAmount">Price per Night (₦) *</Label>
                <Input
                  id="bookingAmount"
                  type="number"
                  value={formData.bookingAmount}
                  onChange={(e) => setFormData({ ...formData, bookingAmount: e.target.value })}
                  required
                  min="0"
                />
              </div>
              <div>
                <Label htmlFor="country">Country</Label>
                <Input
                  id="country"
                  value={formData.country}
                  onChange={(e) => setFormData({ ...formData, country: e.target.value })}
                />
              </div>
            </div>

            <div>
              <Label htmlFor="accommodationDescription">Description *</Label>
              <textarea
                id="accommodationDescription"
                value={formData.accommodationDescription}
                onChange={(e) => setFormData({ ...formData, accommodationDescription: e.target.value })}
                required
                rows={4}
                className="flex min-h-[100px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              />
            </div>

            <div>
              <Label>Facilities</Label>
              <div className="flex gap-2">
                <Input
                  value={facilityInput}
                  onChange={(e) => setFacilityInput(e.target.value)}
                  placeholder="e.g., Swimming pool"
                  onKeyPress={(e) => e.key === 'Enter' && addFacility()}
                />
                <Button type="button" onClick={addFacility} size="sm" className="gap-1">
                  <Plus className="h-4 w-4" />
                  Add
                </Button>
              </div>
              <div className="flex flex-wrap gap-2 mt-2">
                {formData.accommodationFacilities.map((facility, index) => (
                  <Badge key={index} className="bg-blue-100 text-blue-800 flex items-center gap-1">
                    {facility}
                    <button
                      type="button"
                      onClick={() => removeFacility(index)}
                      className="hover:text-red-500"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </Badge>
                ))}
              </div>
            </div>

            <div>
              <Label>Property Images</Label>
              <div className="mt-2">
                <Input
                  type="file"
                  accept="image/*"
                  multiple
                  onChange={handleImageChange}
                  className="cursor-pointer"
                />
                <p className="text-xs text-gray-500 mt-1">Upload additional images (recommended: 1200x800px)</p>
              </div>
              <div className="flex flex-wrap gap-2 mt-3">
                {imagePreviews.map((preview, index) => (
                  <div key={index} className="relative h-20 w-20 rounded-lg overflow-hidden border">
                    <img src={preview} alt={`Property ${index}`} className="h-full w-full object-cover" />
                    <button
                      type="button"
                      onClick={() => removeImage(index)}
                      className="absolute -top-1 -right-1 bg-red-500 text-white rounded-full p-0.5"
                    >
                      <X className="h-3 w-3" />
                    </button>
                  </div>
                ))}
              </div>
            </div>

            <div className="flex items-center gap-3">
              <input
                type="checkbox"
                id="available"
                checked={formData.available}
                onChange={(e) => setFormData({ ...formData, available: e.target.checked })}
                className="h-4 w-4 rounded border-gray-300 text-primary focus:ring-primary"
              />
              <Label htmlFor="available">Property is available for booking</Label>
            </div>

            <div className="flex gap-4 pt-4">
              <Button type="submit" disabled={isSubmitting} className="flex-1">
                {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                {isSubmitting ? "Saving..." : "Save Changes"}
              </Button>
              <Link href={`/brand-owner/properties/${params.id}`} className="flex-1">
                <Button type="button" variant="outline" className="w-full">Cancel</Button>
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}