// app/brand-owner/adverts/add/page.tsx
"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Upload, X } from "lucide-react";
import { addAdvert, uploadImage } from "@/lib/api";

export default function CreateBrandOwnerAdvertPage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>("");
  const [formData, setFormData] = useState({
    advertName: "",
    advertType: "Standard",
    advertItemDescription: "",
    advertItemCost: "",
    advertDays4: "",
    advertImage: "",
  });

  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImageFile(file);
      const reader = new FileReader();
      reader.onloadend = () => {
        setImagePreview(reader.result as string);
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setSuccess("");
    setIsLoading(true);

    try {
      let imageUrl = formData.advertImage;

      if (imageFile) {
        const uploadResult = await uploadImage(imageFile);
        imageUrl = uploadResult.url || uploadResult.imageUrl || "";
      }

      await addAdvert({
        ...formData,
        advertItemCost: parseFloat(formData.advertItemCost),
        advertDays4: parseInt(formData.advertDays4),
        advertImage: imageUrl,
      });

      setSuccess("Advert created successfully!");
      setTimeout(() => router.push("/brand-owner/adverts"), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create advert");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/adverts">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Create Advert</h1>
          <p className="text-sm text-gray-500">Promote your brand</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Advert Details</CardTitle>
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
              <Label htmlFor="advertName">Advert Name *</Label>
              <Input
                id="advertName"
                value={formData.advertName}
                onChange={(e) => setFormData({ ...formData, advertName: e.target.value })}
                required
                placeholder="Enter advert name"
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="advertType">Advert Type *</Label>
                <select
                  id="advertType"
                  className="w-full px-3 py-2 border rounded-xl focus:outline-none focus:ring-2 focus:ring-primary/20"
                  value={formData.advertType}
                  onChange={(e) => setFormData({ ...formData, advertType: e.target.value })}
                  required
                >
                  <option value="Standard">Standard</option>
                  <option value="Premium">Premium</option>
                  <option value="Featured">Featured</option>
                  <option value="Sponsored">Sponsored</option>
                </select>
              </div>
              <div>
                <Label htmlFor="advertDays4">Duration (Days) *</Label>
                <Input
                  id="advertDays4"
                  type="number"
                  value={formData.advertDays4}
                  onChange={(e) => setFormData({ ...formData, advertDays4: e.target.value })}
                  required
                  placeholder="30"
                  min="1"
                />
              </div>
            </div>

            <div>
              <Label htmlFor="advertItemCost">Cost (₦) *</Label>
              <Input
                id="advertItemCost"
                type="number"
                value={formData.advertItemCost}
                onChange={(e) => setFormData({ ...formData, advertItemCost: e.target.value })}
                required
                placeholder="50000"
                min="0"
              />
            </div>

            <div>
              <Label htmlFor="advertItemDescription">Description *</Label>
              <textarea
                id="advertItemDescription"
                value={formData.advertItemDescription}
                onChange={(e) => setFormData({ ...formData, advertItemDescription: e.target.value })}
                required
                placeholder="Describe your advert"
                rows={4}
                className="flex min-h-[100px] w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50"
              />
            </div>

            <div>
              <Label>Advert Image</Label>
              <div className="mt-2">
                <div className="flex items-center gap-4">
                  <div className="flex-1">
                    <Input
                      type="file"
                      accept="image/*"
                      onChange={handleImageChange}
                      className="cursor-pointer"
                    />
                  </div>
                  {imagePreview && (
                    <div className="relative h-16 w-16 rounded-lg overflow-hidden flex-shrink-0">
                      <img src={imagePreview} alt="Preview" className="h-full w-full object-cover" />
                      <button
                        type="button"
                        onClick={() => {
                          setImagePreview("");
                          setImageFile(null);
                        }}
                        className="absolute -top-1 -right-1 bg-red-500 text-white rounded-full p-0.5"
                      >
                        <X className="h-3 w-3" />
                      </button>
                    </div>
                  )}
                </div>
                <p className="text-xs text-gray-500 mt-1">Upload an image (recommended: 1200x600px)</p>
              </div>
            </div>

            <div className="flex gap-4 pt-4">
              <Button type="submit" disabled={isLoading} className="flex-1">
                {isLoading ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                {isLoading ? "Creating..." : "Create Advert"}
              </Button>
              <Link href="/brand-owner/adverts" className="flex-1">
                <Button type="button" variant="outline" className="w-full">Cancel</Button>
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}