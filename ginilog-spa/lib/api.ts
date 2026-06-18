// lib/api.ts

const DEFAULT_PRODUCTION_API = "https://api-data-connection.ginilog.org";
const LOCAL_API = "https://api-data-connection.ginilog.org";

function resolveApiUrl(): string {
  const fromEnv = process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "");
  if (fromEnv) return fromEnv;

  if (typeof window === "undefined") {
    return LOCAL_API;
  }

  // Check if we're on the production domain
  if (window.location.hostname === "www.ginilog.com" || 
      window.location.hostname === "ginilog.com" ||
      window.location.hostname.includes("vercel.app")) {
    return DEFAULT_PRODUCTION_API;
  }

  if (window.location.hostname === "localhost" || window.location.hostname === "127.0.0.1") {
    return LOCAL_API;
  }

  return DEFAULT_PRODUCTION_API;
}

const API_URL = resolveApiUrl();

// Enhanced fetch wrapper with timeout and retry logic
async function fetchWithTimeout(
  url: string, 
  options: RequestInit = {}, 
  timeout: number = 30000
): Promise<Response> {
  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), timeout);

  try {
    const response = await fetch(url, {
      ...options,
      signal: controller.signal,
    });
    clearTimeout(timeoutId);
    return response;
  } catch (error) {
    clearTimeout(timeoutId);
    if (error instanceof Error && error.name === 'AbortError') {
      throw new Error('Request timeout. Please check your connection and try again.');
    }
    throw error;
  }
}

// Retry wrapper for failed requests
async function fetchWithRetry(
  url: string, 
  options: RequestInit = {}, 
  retries: number = 3, 
  delay: number = 1000
): Promise<Response> {
  try {
    return await fetchWithTimeout(url, options);
  } catch (error) {
    if (retries > 0) {
      await new Promise(resolve => setTimeout(resolve, delay));
      return fetchWithRetry(url, options, retries - 1, delay * 2);
    }
    throw error;
  }
}

// Helper function to safely extract array from response
function extractArrayFromResponse(data: any): any[] {
  if (!data) return [];
  if (Array.isArray(data)) return data;
  if (typeof data === 'object') {
    if (data.data && Array.isArray(data.data)) return data.data;
    if (data.items && Array.isArray(data.items)) return data.items;
    if (data.results && Array.isArray(data.results)) return data.results;
    if (data.records && Array.isArray(data.records)) return data.records;
    if (data.list && Array.isArray(data.list)) return data.list;
    if (data.id) return [data];
  }
  console.warn('Unexpected response format, expected array:', data);
  return [];
}

// Check if user is authenticated
export function isAuthenticated(): boolean {
  if (typeof window !== "undefined") {
    return !!localStorage.getItem("token");
  }
  return false;
}

// Get current user type
export function getUserType(): string | null {
  if (typeof window !== "undefined") {
    const user = localStorage.getItem("user");
    if (user) {
      try {
        const userData = JSON.parse(user);
        return userData.userType || null;
      } catch {
        return null;
      }
    }
  }
  return null;
}

// Token helpers
export function getToken(): string | null {
  if (typeof window !== "undefined") {
    return localStorage.getItem("token");
  }
  return null;
}

export function getRefreshToken(): string | null {
  if (typeof window !== "undefined") {
    return localStorage.getItem("refreshToken");
  }
  return null;
}

export function getStoredUser(): LoginResponse | null {
  if (typeof window !== "undefined") {
    const user = localStorage.getItem("user");
    return user ? JSON.parse(user) : null;
  }
  return null;
}

export function setAuthData(data: LoginResponse): void {
  if (typeof window !== "undefined") {
    localStorage.setItem("token", data.token);
    localStorage.setItem("refreshToken", data.refreshToken);
    localStorage.setItem("user", JSON.stringify(data));
    window.dispatchEvent(new StorageEvent('storage', {
      key: 'token',
      newValue: data.token
    }));
  }
}

export function clearAuthData(): void {
  if (typeof window !== "undefined") {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.dispatchEvent(new CustomEvent('auth:logout'));
  }
}

// FIXED: Refresh token function with fallback and better error handling
export async function refreshAccessToken(): Promise<string | null> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    console.warn('No refresh token available');
    return null;
  }
  
  // Try multiple possible refresh endpoints
  const refreshEndpoints = [
    `${API_URL}/api/auth-users/refresh-token`,
    `${API_URL}/api/auth-users/token/refresh`,
    `${API_URL}/api/auth/refresh-token`,
    `${API_URL}/api/auth/token/refresh`,
  ];
  
  let lastError: Error | null = null;
  
  for (const endpoint of refreshEndpoints) {
    try {
      console.log(`Attempting token refresh at: ${endpoint}`);
      
      const response = await fetch(endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ 
          refreshToken,
          // Try different possible payload formats
          refresh_token: refreshToken,
          token: refreshToken
        }),
        credentials: 'include',
      });
      
      if (response.ok) {
        const data = await response.json();
        // Try different possible response formats
        const newToken = data.token || data.accessToken || data.access_token || null;
        
        if (newToken) {
          localStorage.setItem('token', newToken);
          console.log('Access token refreshed successfully');
          return newToken;
        } else {
          console.warn('Refresh response did not contain a token:', data);
        }
      } else {
        console.warn(`Refresh endpoint ${endpoint} returned status: ${response.status}`);
        if (response.status === 405) {
          console.warn(`Endpoint ${endpoint} does not accept POST method`);
        }
      }
    } catch (error) {
      console.warn(`Failed to refresh at ${endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }
  
  console.error('All refresh attempts failed');
  return null;
}

// Health check function
export async function checkApiHealth(): Promise<boolean> {
  try {
    const response = await fetch(`${API_URL}/health`, {
      method: 'GET',
      signal: AbortSignal.timeout(5000),
    });
    return response.ok;
  } catch {
    return false;
  }
}

// FIXED: fetchWithAuth with proper token refresh and retry logic
async function fetchWithAuth(
  endpoint: string,
  options: RequestInit = {},
  isRetry: boolean = false
): Promise<Response> {
  const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
  const url = `${API_URL}/api/${cleanEndpoint}`;

  console.log(`Fetching: ${url}${isRetry ? ' (retry after refresh)' : ''}`);

  const token = getToken();

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  } else if (!isRetry) {
    // Only log warning if not a retry (retry will have token from refresh)
    console.warn(`No token found for request to ${url}`);
  }

  try {
    const response = await fetchWithRetry(url, {
      ...options,
      headers,
      credentials: 'include',
      mode: 'cors',
    });

    // If response is 401, try to refresh token once
    if (response.status === 401 && !isRetry) {
      console.log(`Token expired for ${url}, attempting refresh...`);
      const refreshed = await refreshAccessToken();
      
      if (refreshed) {
        console.log(`Token refreshed, retrying: ${url}`);
        // Retry with the new token
        return fetchWithAuth(endpoint, options, true);
      } else {
        console.warn(`Token refresh failed for ${url}`);
        clearAuthData();
        if (typeof window !== 'undefined') {
          window.dispatchEvent(new CustomEvent('auth:unauthorized'));
        }
        throw new Error('Your session has expired. Please log in again.');
      }
    }

    // If retry also returns 401, session is truly invalid
    if (response.status === 401 && isRetry) {
      clearAuthData();
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      }
      throw new Error('Your session has expired. Please log in again.');
    }

    // Handle other non-200 responses
    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;

      try {
        const errorResponse = response.clone();
        const contentType = errorResponse.headers.get('content-type') || '';

        if (contentType.includes('application/json')) {
          const errorData = await errorResponse.json();
          if (errorData.errors && typeof errorData.errors === "object") {
            const messages = Object.values(errorData.errors as Record<string, string[]>)
              .flat()
              .join(" ");
            errorMessage = messages || errorData.title || errorMessage;
          } else {
            errorMessage = errorData.message || errorData.title || errorMessage;
          }
        } else {
          const text = await errorResponse.text();
          if (text && text.length > 0) {
            errorMessage = text.length > 300 ? text.slice(0, 300) + "…" : text;
          }
        }
      } catch (error) {
        console.warn('Could not read error response body:', error);
      }

      throw new Error(errorMessage);
    }

    return response;
  } catch (error) {
    console.error(`API Error for ${url}:`, error);
    if (error instanceof Error) {
      if (error.message.includes('Network error') || 
          error.message.includes('Request timeout') ||
          error.message.includes('HTTP error') ||
          error.message.includes('session has expired')) {
        throw error;
      }
      throw new Error(`Network error: ${error.message}`);
    }
    throw error;
  }
}

// [Rest of your interfaces remain the same...]

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

export interface GoogleAuthRequest {
  Email: string;
  ExternalId: string;
  FirstName?: string;
  LastName?: string;
  ProfilePicture?: string;
}

export interface FirebaseAuthRequest {
  idToken: string;
  provider: string;
  email: string;
  name: string;
  profilePicture: string;
  firebaseUid: string;
}

export interface UpdateUserRequest {
  FirstName?: string;
  LastName?: string;
  PhoneNo?: string;
  Sex?: string;
  Address?: string;
  ProfilePicture?: string;
  PostCodes?: string;
  Locality?: string;
  State?: string;
  Latitude?: number;
  Longitude?: number;
  UserStatus?: boolean;
}

export interface DeliveryAddress {
  id: string;
  userName: string;
  phoneNo: string;
  address: string;
  addressPostCodes: string;
  houseNo: string;
  locality: string;
  state: string;
  latitude: number;
  longitude: number;
}

export interface AddDeliveryAddressRequest {
  UserName: string;
  PhoneNo: string;
  Address: string;
  AddressPostCodes?: string;
  HouseNo?: string;
  Locality?: string;
  State?: string;
  Latitude?: number;
  Longitude?: number;
}

export interface EmailVerificationRequest {
  Token: string;
  Password: string;
}

export interface ResetPasswordRequest {
  Token: string;
  Password: string;
}

export interface Notification {
  id: string;
  userId: string;
  title: string;
  body: string;
  isRead: boolean;
  notificationType: string;
  imageUrl: string;
  createdAt: string;
  updatedAt: string;
}

export interface UpdateNotificationRequest {
  Title?: string;
  Body?: string;
  IsRead?: boolean;
  NotificationType?: string;
}

export interface AddFeedbackRequest {
  Name: string;
  PhoneNo: string;
  Email: string;
  Feedback: string;
}

export interface Airline {
  id: string;
  airlineName: string;
  airlineLogo: string;
  airlineInfo: string;
  valueCharge: number;
}

export interface FlightTicket {
  id: string;
  flightNumber: string;
  departureLocation: string;
  arrivalLocation: string;
  departureTime: string;
  arrivalTime: string;
  ticketCost: number;
  airlineName: string;
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

export interface RegisterManagerRequest {
  AdminType: "Manager";
  FirstName: string;
  SurName: string;
  Email: string;
  Password: string;
  Sex: string;
  StaffCode: string;
  PhoneNo: string;
  State: string;
  Locality: string;
  Address: string;
  Branch: string;
  CompanyName?: string;
  CompanyUserName?: string;
  CompanyType?: string[];
}

// Auth Functions
export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("auth-users/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function register(userData: RegisterRequest): Promise<RegisterResponse> {
  const response = await fetchWithAuth("auth-users", {
    method: "POST",
    body: JSON.stringify(userData),
  });
  return response.json();
}

export async function getProfile(): Promise<UserProfile> {
  const response = await fetchWithAuth("auth-users/profile", { method: "GET" });
  return response.json();
}

export async function logout(): Promise<void> {
  try {
    await fetchWithAuth("auth-users/logout", { method: "POST" }).catch(() => {});
  } finally {
    clearAuthData();
  }
}

export async function googleAuth(data: GoogleAuthRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("auth-users/auth-login", {
    method: "POST",
    body: JSON.stringify(data),
  });
  const loginData = await response.json();
  setAuthData(loginData);
  return loginData;
}

export async function firebaseAuth(data: FirebaseAuthRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("auth-users/firebase-auth", {
    method: "POST",
    body: JSON.stringify(data),
  });
  const loginData = await response.json();
  setAuthData(loginData);
  return loginData;
}

export async function updateProfile(data: UpdateUserRequest): Promise<UserProfile> {
  const response = await fetchWithAuth("auth-users/update-user", {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function updateDeviceToken(deviceToken: string): Promise<any> {
  const response = await fetchWithAuth("auth-users/update-device-token", {
    method: "PUT",
    body: JSON.stringify({ DeviceToken: deviceToken }),
  });
  return response.json();
}

export async function getDeliveryAddresses(): Promise<DeliveryAddress[]> {
  const response = await fetchWithAuth("auth-users/delivery-address", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function addNewAddress(data: AddDeliveryAddressRequest): Promise<UserProfile> {
  const response = await fetchWithAuth("auth-users/add-new-address", {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function updateDeliveryAddress(id: string, data: AddDeliveryAddressRequest): Promise<UserProfile> {
  const response = await fetchWithAuth(`auth-users/update-delivery-address/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function deleteDeliveryAddress(id: string): Promise<void> {
  await fetchWithAuth(`auth-users/delete-delivery-address/${id}`, { method: "DELETE" });
}

export async function verifyEmail(data: EmailVerificationRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("auth-users/email-verification", {
    method: "POST",
    body: JSON.stringify(data),
  });
  const loginData = await response.json();
  setAuthData(loginData);
  return loginData;
}

export async function requestEmailVerificationToken(email: string): Promise<string> {
  const response = await fetchWithAuth("auth-users/email-verification-request-token", {
    method: "POST",
    body: JSON.stringify({ Email: email }),
  });
  return response.json();
}

export async function forgotPassword(email: string): Promise<string> {
  const response = await fetchWithAuth("auth-users/forgot-password-request-token", {
    method: "POST",
    body: JSON.stringify({ Email: email }),
  });
  return response.json();
}

export async function resetPassword(data: ResetPasswordRequest): Promise<string> {
  const response = await fetchWithAuth("auth-users/reset-password", {
    method: "POST",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function verifyPhoneNumber(otp: string): Promise<string> {
  const response = await fetchWithAuth("auth-users/phone-no-verification", {
    method: "POST",
    body: JSON.stringify({ Otp: otp }),
  });
  return response.json();
}

export async function enableTwoFactor(id: string): Promise<string> {
  const response = await fetchWithAuth(`auth-users/two-factor-enabled/${id}`, { method: "POST" });
  return response.json();
}

// Logistics Functions
export async function getCompanies(): Promise<Company[]> {
  const response = await fetchWithAuth("logistics-controller", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getCompanyById(id: string): Promise<Company> {
  const response = await fetchWithAuth(`logistics-controller/${id}`, { method: "GET" });
  return response.json();
}

export async function createOrder(companyId: string, orderData: AddOrder): Promise<any> {
  const response = await fetchWithAuth("logistics-controller/package-orders", {
    method: "POST",
    headers: { companyId },
    body: JSON.stringify(orderData),
  });
  return response.json();
}

export async function getCustomerOrders(): Promise<any[]> {
  const response = await fetchWithAuth("logistics-controller/package-orders", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getOrderById(id: string): Promise<OrderTrackingResult> {
  const response = await fetchWithAuth(`logistics-controller/package-orders/${id}`, { method: "GET" });
  return response.json();
}

export async function trackOrder(trackingNumber: string): Promise<OrderTrackingResult> {
  const response = await fetchWithAuth(`logistics-controller/track-order?trackingNum=${encodeURIComponent(trackingNumber)}`, { 
    method: "GET" 
  });
  return response.json();
}

// Bookings Functions
export async function getAccommodations(): Promise<Accommodation[]> {
  const response = await fetchWithAuth("Bookings/accomodation", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getAccommodationById(id: string): Promise<Accommodation> {
  const response = await fetchWithAuth(`Bookings/accomodation/${id}`, { method: "GET" });
  return response.json();
}

export async function getRooms(accommodationId: string): Promise<any[]> {
  const response = await fetchWithAuth(`Bookings/accomodation-reservations?id=${accommodationId}`, { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function bookAccommodation(reservationId: string, bookingData: AddCustomerBookedReservation): Promise<any> {
  const response = await fetchWithAuth("bookings/accomodation-reservations-customer", {
    method: "POST",
    headers: { reservationId },
    body: JSON.stringify(bookingData),
  });
  return response.json();
}

export async function getCustomerBookings(): Promise<any[]> {
  const response = await fetchWithAuth("bookings/accomodation-reservations-customer", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getCustomerBookingById(id: string): Promise<BookingTrackingResult> {
  const response = await fetchWithAuth(`bookings/accomodation-reservations-customer/${id}`, { method: "GET" });
  return response.json();
}

export async function cancelCustomerBooking(id: string): Promise<void> {
  await fetchWithAuth(`bookings/accomodation-reservations-customer/${id}`, { method: "DELETE" });
}

export async function getAirlines(): Promise<Airline[]> {
  const response = await fetchWithAuth("bookings/airline", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getAirlineById(id: string): Promise<Airline> {
  const response = await fetchWithAuth(`bookings/airline/${id}`, { method: "GET" });
  return response.json();
}

export async function getFlightTickets(): Promise<FlightTicket[]> {
  const response = await fetchWithAuth("bookings/airline-flight-ticket", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function trackBooking(bookingRef: string): Promise<BookingTrackingResult> {
  const response = await fetchWithAuth(`bookings/track-booking?ticketRef=${encodeURIComponent(bookingRef)}`, { 
    method: "GET" 
  });
  return response.json();
}

export async function trackParcelOrBooking(
  searchId: string
): Promise<{ type: "order" | "booking"; data: OrderTrackingResult | BookingTrackingResult }> {
  try {
    const orderData = await trackOrder(searchId);
    return { type: "order", data: orderData };
  } catch {
    try {
      const bookingData = await trackBooking(searchId);
      return { type: "booking", data: bookingData };
    } catch {
      throw new Error("No parcel or booking found with this tracking/reference number");
    }
  }
}

// Payments Functions
export async function initializePaystackPayment(orderId: string, amount: number, email: string): Promise<any> {
  const response = await fetchWithAuth("Wallet/initialize", {
    method: "POST",
    body: JSON.stringify({ amount, email, orderId }),
  });
  return response.json();
}

export async function verifyPaystackPayment(reference: string): Promise<any> {
  const response = await fetchWithAuth(`Wallet/verify?reference=${reference}`, { method: "GET" });
  return response.json();
}

export async function initializeFlutterwavePayment(amount: number, email: string, fullName: string): Promise<any> {
  const response = await fetchWithAuth("Wallet/initialize-flutterwave", {
    method: "POST",
    body: JSON.stringify({ Amount: amount, Email: email, FullName: fullName }),
  });
  return response.json();
}

// Notifications Functions
export async function getNotifications(): Promise<Notification[]> {
  const response = await fetchWithAuth("Notifications", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getNotificationById(id: string): Promise<Notification> {
  const response = await fetchWithAuth(`Notifications/${id}`, { method: "GET" });
  return response.json();
}

export async function markNotificationRead(id: string, data: UpdateNotificationRequest): Promise<Notification> {
  const response = await fetchWithAuth(`Notifications/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

// Feedback Functions
export async function submitFeedback(data: AddFeedbackRequest): Promise<any> {
  const response = await fetchWithAuth("Info/feedback", {
    method: "POST",
    body: JSON.stringify(data),
  });
  return response.json();
}

// Admin Auth Functions
export async function adminLogin(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("admin-controller/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function loginManager(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchWithAuth("admin-controller/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function registerManager(data: RegisterManagerRequest): Promise<any> {
  if (!isAuthenticated()) {
    throw new Error('You must be logged in as an admin to register a new manager. Please log in first.');
  }
  
  const endpoints = [
    "admin-controller/register",
    "admin-controller/add-manager",
    "Admin/add-manager",
    "auth-users/add-manager",
    "admin/register"
  ];
  
  let lastError: Error | null = null;
  
  for (const endpoint of endpoints) {
    try {
      console.log(`Trying to register manager with endpoint: ${endpoint}`);
      const response = await fetchWithAuth(endpoint, {
        method: "POST",
        body: JSON.stringify(data),
      });
      const result = await response.json();
      console.log(`Successfully registered manager with endpoint: ${endpoint}`);
      return result;
    } catch (error) {
      console.warn(`Failed with endpoint ${endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
      if (error instanceof Error) {
        if (!error.message.includes('404') && !error.message.includes('401')) {
          throw error;
        }
      }
    }
  }
  
  console.error('All registration endpoints failed');
  if (lastError) {
    throw lastError;
  }
  throw new Error('Failed to register manager. Please contact support or use a different method to create an admin account.');
}

export async function adminGetProfile(): Promise<any> {
  const response = await fetchWithAuth("admin-controller/profile", { method: "GET" });
  return response.json();
}

// Admin Data Functions
export async function getAllUsers(): Promise<any[]> {
  const response = await fetchWithAuth("auth-users", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getAllOrders(): Promise<any[]> {
  const response = await fetchWithAuth("logistics-controller/package-orders", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function updateOrderStatus(id: string, data: Record<string, unknown>): Promise<any> {
  const response = await fetchWithAuth(`logistics-controller/package-orders/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function getAllReservations(): Promise<any[]> {
  const response = await fetchWithAuth("Bookings/accomodation-reservations", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function updateReservation(id: string, data: Record<string, unknown>): Promise<any> {
  const response = await fetchWithAuth(`Bookings/accomodation-reservations/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function getAllCustomerReservations(): Promise<any[]> {
  const response = await fetchWithAuth("bookings/accomodation-reservations-customer", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function updateCustomerReservation(id: string, data: Record<string, unknown>): Promise<any> {
  const response = await fetchWithAuth(`bookings/accomodation-reservations-customer/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function getAllStaff(): Promise<any[]> {
  const response = await fetchWithAuth("admin-controller/staff-users", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getAllAdverts(): Promise<any[]> {
  const response = await fetchWithAuth("admin-controller/advert", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export { API_URL };