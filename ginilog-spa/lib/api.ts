const API_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

// Types matching backend models
export interface LoginRequest {
  Email_PhoneNo: string;
  Password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  refreshTokenExpiryTime: string;
  userId: string;
  email: string;
  userType: string;
  emailVerified: boolean;
  phoneVerified: boolean;
  fullName: string;
  profileImage: string;
}

export interface RegisterRequest {
  FirstName: string;
  LastName: string;
  Email: string;
  PhoneNo: string;
  Password: string;
}

export interface RegisterResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNo: string;
  sex: string;
  userStatus: boolean;
  profilePicture: string;
  referralCode: string;
  createdAt: string;
  address: string;
  locality: string;
  state: string;
  postCodes: string;
  latitude: number;
  longitude: number;
  moneyBoxBalance: number;
  accountName: string;
  accountNumber: string;
  bankName: string;
  lastLoginAt: string;
  lastSeenAt: string;
}

export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNo: string;
  sex: string;
  userStatus: boolean;
  profilePicture: string;
  referralCode: string;
  createdAt: string;
  address: string;
  locality: string;
  state: string;
  postCodes: string;
  latitude: number;
  longitude: number;
  moneyBoxBalance: number;
  accountName: string;
  accountNumber: string;
  bankName: string;
  lastLoginAt: string;
  lastSeenAt: string;
  deviceTokenModels: Array<{
    deviceTokenId: string;
    userId: string;
    userType: string;
  }>;
}

export interface ApiError {
  message: string;
  status: boolean;
}

// Helper to get stored token
export function getToken(): string | null {
  if (typeof window !== "undefined") {
    return localStorage.getItem("token");
  }
  return null;
}

// Helper to get stored user
export function getStoredUser(): LoginResponse | null {
  if (typeof window !== "undefined") {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  }
  return null;
}

// Helper to set auth data
export function setAuthData(data: LoginResponse): void {
  if (typeof window !== "undefined") {
    localStorage.setItem("token", data.token);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("user", JSON.stringify(data));
  }
}

// Helper to clear auth data
export function clearAuthData(): void {
  if (typeof window !== "undefined") {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
  }
}

// Generic fetch with auth
async function fetchWithAuth(endpoint: string, options: RequestInit = {}): Promise<Response> {
  const url = `${API_URL}/api/${endpoint}`;
  const token = getToken();

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(url, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "An error occurred" }));
    throw new Error(error.message || `HTTP error! status: ${response.status}`);
  }

  return response;
}

// Auth API functions
export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("AuthUsers/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function register(userData: RegisterRequest): Promise<RegisterResponse> {
  const response = await fetchWithAuth("AuthUsers", {
    method: "POST",
    body: JSON.stringify(userData),
  });
  return response.json();
}

export async function getProfile(): Promise<UserProfile> {
  const response = await fetchWithAuth("AuthUsers/profile", {
    method: "GET",
  });
  return response.json();
}

export async function logout(): Promise<void> {
  clearAuthData();
}

// Tracking Types
export interface OrderTrackingResult {
  id: string;
  trackingNum: string;
  itemName: string;
  itemDescription: string;
  itemCost: number;
  itemWeight: number;
  itemQuantity: number;
  packageType: string;
  expectedDeliveryTime: string;
  orderStatus: string;
  senderName: string;
  senderPhoneNo: string;
  senderEmail: string;
  senderAddress: string;
  senderState: string;
  senderLocality: string;
  recieverName: string;
  recieverPhoneNo: string;
  recieverEmail: string;
  recieverAddress: string;
  recieverState: string;
  recieverLocality: string;
  companyName: string;
  companyPhoneNo: string;
  riderName: string;
  currentLocation: string;
  currentLatitude: number;
  currentLongitude: number;
  shippingCost: number;
  vatCost: number;
  paymentStatus: boolean;
  orderStatusDate: string;
  createdAt: string;
  updatedAt: string;
  packageImageLists: string[];
  orderDeliveryFlows: Array<{
    id: string;
    orderStatus: string;
    currentLocation: string;
    updatedAt: string;
  }>;
}

export interface BookingTrackingResult {
  id: string;
  bookingRefNo: string;
  accomodationName: string;
  accomodationType: string;
  roomType: string;
  roomNumber: number;
  bookingStatus: string;
  checkInDate: string;
  checkOutDate: string;
  numberOfNights: number;
  numberOfGuests: number;
  guestName: string;
  guestEmail: string;
  guestPhoneNo: string;
  totalAmount: number;
  paymentStatus: boolean;
  accomodationAddress: string;
  accomodationLocality: string;
  accomodationState: string;
  companyName: string;
  companyPhoneNo: string;
  qrCode: string;
  createdAt: string;
  updatedAt: string;
  accomodationImages: string[];
}

// Tracking API functions - Public (no auth required)
export async function trackOrder(trackingNumber: string): Promise<OrderTrackingResult> {
  // Try to get order by tracking number from the public endpoint
  const response = await fetch(`${API_URL}/api/Logistics/track-order?trackingNum=${encodeURIComponent(trackingNumber)}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Tracking number not found" }));
    throw new Error(error.message || "Order not found");
  }

  return response.json();
}

export async function trackBooking(bookingRef: string): Promise<BookingTrackingResult> {
  // Try to get booking by reference number from the public endpoint
  const response = await fetch(`${API_URL}/api/Bookings/track-booking?bookingRef=${encodeURIComponent(bookingRef)}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: "Booking reference not found" }));
    throw new Error(error.message || "Booking not found");
  }

  return response.json();
}

// Search both orders and bookings by tracking/reference number
export async function trackParcelOrBooking(searchId: string): Promise<{ type: 'order' | 'booking'; data: OrderTrackingResult | BookingTrackingResult }> {
  try {
    // First try to find as order
    const orderData = await trackOrder(searchId);
    return { type: 'order', data: orderData };
  } catch (orderError) {
    // If not found as order, try as booking
    try {
      const bookingData = await trackBooking(searchId);
      return { type: 'booking', data: bookingData };
    } catch (bookingError) {
      // If neither found, throw error
      throw new Error("No parcel or booking found with this tracking/reference number");
    }
  }
}
