// app/brand-owner/profile/edit/page.tsx
"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Loader2, ArrowLeft, Upload, X } from "lucide-react";
import { getStoredUser, getBrandOwnerById, updateStaff, uploadImage } from "@/lib/api";

export default function EditBrandOwnerProfilePage() {
  const router = useRouter();
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [formData, setFormData] = useState({
    firstName: "",
    surName: "",
    phoneNo: "",
    address: "",
    profilePicture: "",
  });
  const [imageFile, setImageFile] = useState<File | null>(null);
  const [imagePreview, setImagePreview] = useState<string>("");

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const user = getStoredUser();
        if (!user) {
          router.push("/brand-owner/login");
          return;
        }
        
        const data = await getBrandOwnerById(user.userId || user.companyId || "");
        setFormData({
          firstName: data.firstName || "",
          surName: data.surName || "",
          phoneNo: data.phoneNo || "",
          address: data.address || "",
          profilePicture: data.profilePicture || "",
        });
        if (data.profilePicture) {
          setImagePreview(data.profilePicture);
        }
      } catch (error) {
        console.error("Failed to fetch profile:", error);
        setError("Failed to load profile data");
      } finally {
        setIsLoading(false);
      }
    };
    fetchProfile();
  }, [router]);

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
    setIsSubmitting(true);

    try {
      let profilePicture = formData.profilePicture;
      
      if (imageFile) {
        const uploadResult = await uploadImage(imageFile);
        profilePicture = uploadResult.url || uploadResult.imageUrl || "";
      }

      await updateStaff({
        firstName: formData.firstName,
        surName: formData.surName,
        phoneNo: formData.phoneNo,
        address: formData.address,
        imagePath: profilePicture,
      });

      setSuccess("Profile updated successfully!");
      setTimeout(() => router.push("/brand-owner/profile"), 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to update profile");
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
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="flex items-center gap-4">
        <Link href="/brand-owner/profile">
          <Button variant="ghost" size="sm" className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Back
          </Button>
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Edit Profile</h1>
          <p className="text-sm text-gray-500">Update your personal information</p>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Profile Details</CardTitle>
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
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="firstName">First Name *</Label>
                <Input
                  id="firstName"
                  value={formData.firstName}
                  onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                  required
                />
              </div>
              <div>
                <Label htmlFor="surName">Last Name *</Label>
                <Input
                  id="surName"
                  value={formData.surName}
                  onChange={(e) => setFormData({ ...formData, surName: e.target.value })}
                  required
                />
              </div>
            </div>

            <div>
              <Label htmlFor="phoneNo">Phone Number</Label>
              <Input
                id="phoneNo"
                type="tel"
                value={formData.phoneNo}
                onChange={(e) => setFormData({ ...formData, phoneNo: e.target.value })}
              />
            </div>

            <div>
              <Label htmlFor="address">Business Address</Label>
              <Input
                id="address"
                value={formData.address}
                onChange={(e) => setFormData({ ...formData, address: e.target.value })}
              />
            </div>

            <div>
              <Label>Profile Picture</Label>
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
                    <div className="relative h-16 w-16 rounded-full overflow-hidden flex-shrink-0">
                      <img src={imagePreview} alt="Preview" className="h-full w-full object-cover" />
                      <button
                        type="button"
                        onClick={() => {
                          setImagePreview("");
                          setImageFile(null);
                          setFormData({ ...formData, profilePicture: "" });
                        }}
                        className="absolute -top-1 -right-1 bg-red-500 text-white rounded-full p-0.5"
                      >
                        <X className="h-3 w-3" />
                      </button>
                    </div>
                  )}
                </div>
              </div>
            </div>

            <div className="flex gap-4 pt-4">
              <Button type="submit" disabled={isSubmitting} className="flex-1">
                {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin mr-2" /> : null}
                {isSubmitting ? "Saving..." : "Save Changes"}
              </Button>
              <Link href="/brand-owner/profile" className="flex-1">
                <Button type="button" variant="outline" className="w-full">Cancel</Button>
              </Link>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}