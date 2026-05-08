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

export interface Accommodation {
  id: string;
  accomodationName: string;
  accomodationType: string;
  location: string;
  bookingAmount: number;
  accomodationImages: string[];
  accomodationDescription: string;
  rating: number;
}

export interface Company {
  id: string;
  companyName: string;
  companyLogo: string;
  companyInfo: string;
  valueCharge: number;
}

export interface AddCustomerBookedReservation {
  userId: string;
  customerName: string;
  customerPhoneNumber: string;
  customerEmail: string;
  numberOfGuests: number;
  reservationStartDate: string;
  reservationEndDate: string;
  comment?: string;
  userType?: string;
}

export interface AddOrder {
  itemName: string;
  itemDescription: string;
  itemCost: number;
  itemQuantity: number;
  itemWeight: number;
  packageType: string;
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
  riderType?: string;
  shippingType?: string;
  userType?: string;
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

// Logistics API functions
export async function getCompanies(): Promise<Company[]> {
  const response = await fetchWithAuth("Logistics", {
    method: "GET",
  });
  return response.json();
}

export async function createOrder(companyId: string, orderData: AddOrder): Promise<any> {
  const response = await fetchWithAuth(`Logistics/package-orders`, {
    method: "POST",
    headers: {
      "companyId": companyId
    },
    body: JSON.stringify(orderData),
  });
  return response.json();
}

export async function getCustomerOrders(): Promise<any[]> {
  const response = await fetchWithAuth(`Logistics/package-orders`, {
    method: "GET",
  });
  return response.json();
}

// Bookings API functions
export async function getAccommodations(): Promise<Accommodation[]> {
  const response = await fetchWithAuth("Bookings/accomodation", {
    method: "GET",
  });
  return response.json();
}

export async function getRooms(accommodationId: string): Promise<any[]> {
  const response = await fetchWithAuth(`Bookings/accomodation-reservations?id=${accommodationId}`, {
    method: "GET",
  });
  return response.json();
}

export async function bookAccommodation(reservationId: string, bookingData: AddCustomerBookedReservation): Promise<any> {
  const response = await fetchWithAuth(`Bookings/accomodation-reservations-customer`, {
    method: "POST",
    headers: {
      "reservationId": reservationId
    },
    body: JSON.stringify(bookingData),
  });
  return response.json();
}

export async function getCustomerBookings(): Promise<any[]> {
  const response = await fetchWithAuth(`Bookings/accomodation-reservations-customer`, {
    method: "GET",
  });
  return response.json();
}

// Payment API functions
export async function initializePaystackPayment(orderId: string, amount: number, email: string): Promise<any> {
  const response = await fetchWithAuth(`Wallet/initialize`, {
    method: "POST",
    body: JSON.stringify({ amount, email, orderId }),
  });
  return response.json();
}

export async function verifyPaystackPayment(reference: string): Promise<any> {
  const response = await fetchWithAuth(`Wallet/verify?reference=${reference}`, {
    method: "GET",
  });
  return response.json();
}

// Tracking API functions
export async function trackOrder(trackingNumber: string): Promise<OrderTrackingResult> {
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

export async function trackParcelOrBooking(searchId: string): Promise<{ type: 'order' | 'booking'; data: OrderTrackingResult | BookingTrackingResult }> {
  try {
    const orderData = await trackOrder(searchId);
    return { type: 'order', data: orderData };
  } catch (orderError) {
    try {
      const bookingData = await trackBooking(searchId);
      return { type: 'booking', data: bookingData };
    } catch (bookingError) {
      throw new Error("No parcel or booking found with this tracking/reference number");
    }
  }
}
