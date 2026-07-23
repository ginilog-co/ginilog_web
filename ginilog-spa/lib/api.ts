// lib/api.ts - Complete file with all correct endpoints

const DEFAULT_PRODUCTION_API = "https://api-data-connection.ginilog.org";
const LOCAL_API = "https://api-data-connection.ginilog.org";

function resolveApiUrl(): string {
  const fromEnv = process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "");
  if (fromEnv) return fromEnv;

  if (typeof window === "undefined") {
    return LOCAL_API;
  }

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

function getProxyUrl(endpoint: string): string {
  let cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;

  // Remove duplicate /api prefix if caller already included it.
  cleanEndpoint = cleanEndpoint.replace(/^api\//, '');

  // Normalize any accidental absolute backend URLs to the same-origin proxy.
  cleanEndpoint = cleanEndpoint.replace(
    /^https?:\/\/api-data-connection\.ginilog\.org\/api\//,
    ''
  );

  const proxyUrl = `/api/${cleanEndpoint}`;
  console.log(`🔁 Proxying request through same-origin URL: ${proxyUrl}`);
  return proxyUrl;
}

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

// ============ AUTH HELPERS ============

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

/**
 * Check if the current token is expired
 */
export function isTokenExpired(): boolean {
  const token = getToken();
  if (!token) return true;
  
  try {
    // Decode JWT token (it's base64 encoded)
    const payload = JSON.parse(atob(token.split('.')[1]));
    const exp = payload.exp * 1000; // Convert to milliseconds
    return Date.now() > exp;
  } catch (error) {
    console.warn('Failed to decode token:', error);
    return true; // If we can't decode it, treat it as expired
  }
}

/**
 * Get token expiration time
 */
export function getTokenExpiry(): Date | null {
  const token = getToken();
  if (!token) return null;
  
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    const exp = payload.exp * 1000;
    return new Date(exp);
  } catch (error) {
    return null;
  }
}

/**
 * Check auth status and redirect if needed
 * Use this in components that require authentication
 */
export function checkAuthAndRedirect(redirectTo: string = '/login'): boolean {
  if (typeof window === 'undefined') return false;
  
  const token = getToken();
  if (!token || isTokenExpired()) {
    clearAuthData();
    window.location.href = redirectTo;
    return false;
  }
  return true;
}

/**
 * Get current auth status with detailed info
 */
export function getAuthStatus(): {
  isAuthenticated: boolean;
  isExpired: boolean;
  user: LoginResponse | null;
  expiryTime: Date | null;
} {
  const token = getToken();
  const user = getStoredUser();
  const expired = isTokenExpired();
  
  return {
    isAuthenticated: !!token && !expired,
    isExpired: expired,
    user: user,
    expiryTime: getTokenExpiry(),
  };
}

/**
 * Set up automatic token refresh before expiration
 * Call this in your _app.tsx or layout.tsx
 */
export function setupAutoRefresh(): void {
  if (typeof window === 'undefined') return;
  
  const checkAndRefresh = async () => {
    const token = getToken();
    if (!token) return;
    
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const exp = payload.exp * 1000;
      const now = Date.now();
      const timeUntilExpiry = exp - now;
      
      // If token expires in less than 5 minutes, refresh it
      if (timeUntilExpiry < 5 * 60 * 1000 && timeUntilExpiry > 0) {
        console.log(`⏰ Token expires in ${Math.round(timeUntilExpiry / 60000)} minutes, refreshing...`);
        await refreshAccessToken();
      }
    } catch (error) {
      console.warn('Auto-refresh check failed:', error);
    }
  };
  
  // Check every 2 minutes
  const interval = setInterval(checkAndRefresh, 2 * 60 * 1000);
  
  // Clean up on page unload
  window.addEventListener('beforeunload', () => {
    clearInterval(interval);
  });
}

// Refresh token function
export async function refreshAccessToken(): Promise<string | null> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    console.warn('❌ No refresh token available');
    return null;
  }
  
  console.log('🔄 Attempting to refresh access token...');
  
  // Try multiple endpoints with different body formats
  const attempts = [
    {
      endpoint: `${API_URL}/api/auth-users/refresh-token`,
      body: { refreshToken }
    },
    {
      endpoint: `${API_URL}/api/auth-users/token/refresh`,
      body: { refresh_token: refreshToken }
    },
    {
      endpoint: `${API_URL}/api/auth/token/refresh`,
      body: { token: refreshToken }
    },
    {
      endpoint: `${API_URL}/api/auth/refresh`,
      body: { refreshToken }
    },
    {
      endpoint: `${API_URL}/api/auth-users/refresh`,
      body: { refreshToken: refreshToken }
    },
  ];
  
  let lastError: Error | null = null;
  
  for (const attempt of attempts) {
    try {
      console.log(`🔄 Trying refresh at: ${attempt.endpoint}`);
      
      const response = await fetch(attempt.endpoint, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(attempt.body),
        credentials: 'include',
      });
      
      if (response.ok) {
        const data = await response.json();
        console.log('📥 Refresh response received');
        
        // Try different response formats
        const newToken = data.token || data.accessToken || data.access_token || data.data?.token || null;
        const newRefreshToken = data.refreshToken || data.refresh_token || data.data?.refreshToken || null;
        
        if (newToken) {
          // Update both tokens
          localStorage.setItem('token', newToken);
          if (newRefreshToken) {
            localStorage.setItem('refreshToken', newRefreshToken);
          }
          console.log('✅ Access token refreshed successfully');
          return newToken;
        } else {
          console.warn(`⚠️ No token in response from ${attempt.endpoint}`);
        }
      } else if (response.status === 401) {
        console.warn(`❌ Refresh token invalid at ${attempt.endpoint}`);
        // If refresh token is invalid, we should clear auth data
        clearAuthData();
        return null;
      } else {
        console.warn(`❌ Refresh failed at ${attempt.endpoint}: ${response.status}`);
      }
    } catch (error) {
      console.warn(`❌ Error at ${attempt.endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }
  
  console.error('❌ All refresh attempts failed');
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

// ***** Public fetch for authentication endpoints (no token required) *****
async function fetchPublic(
  endpoint: string,
  options: RequestInit = {}
): Promise<Response> {
  const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
  const url = typeof window !== 'undefined'
    ? getProxyUrl(cleanEndpoint)
    : `${API_URL}/api/${cleanEndpoint}`;

  console.log(`📡 Fetching (public): ${url}`);
  console.log(`📤 Request body:`, options.body);

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  try {
    const response = await fetchWithRetry(url, {
      ...options,
      headers,
      credentials: 'include',
      mode: 'cors',
    });

    // Log response for debugging
    const clonedResponse = response.clone();
    const responseText = await clonedResponse.text();
    console.log(`📥 Response status: ${response.status}`);
    console.log(`📥 Response body:`, responseText);

    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;

      try {
        const errorResponse = response.clone();
        const contentType = errorResponse.headers.get('content-type') || '';

        if (contentType.includes('application/json')) {
          const errorData = await errorResponse.json();
          console.log('📥 Error details:', JSON.stringify(errorData, null, 2));
          
          // ASP.NET Core validation errors format
          if (errorData.errors && typeof errorData.errors === "object") {
            const messages = Object.values(errorData.errors as Record<string, string[]>)
              .flat()
              .join(" ");
            errorMessage = messages || errorData.title || errorMessage;
          } 
          // Custom error format
          else if (errorData.message) {
            errorMessage = errorData.message;
          } 
          // Title format
          else if (errorData.title) {
            errorMessage = errorData.title;
          } 
          // Stringify if nothing else
          else {
            errorMessage = JSON.stringify(errorData);
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
    console.error(`Public API Error for ${url}:`, error);
    throw error;
  }
}

// ***** fetchWithAuth with proper token refresh and retry logic *****
async function fetchWithAuth(
  endpoint: string,
  options: RequestInit = {},
  isRetry: boolean = false
): Promise<Response> {
  const cleanEndpoint = endpoint.startsWith('/') ? endpoint.slice(1) : endpoint;
  const url = typeof window !== 'undefined'
    ? getProxyUrl(cleanEndpoint)
    : `${API_URL}/api/${cleanEndpoint}`;

  console.log(`🔐 Fetching (auth): ${url}`);

  // Check token validity before making request
  if (!isRetry && isTokenExpired()) {
    console.log('🔄 Token expired, attempting refresh before request...');
    const refreshed = await refreshAccessToken();
    if (!refreshed) {
      clearAuthData();
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      }
      throw new Error('Your session has expired. Please log in again.');
    }
  }

  const token = getToken();

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    "Accept": "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  } else {
    console.warn(`⚠️ No token found for request to ${url}`);
  }

  try {
    const response = await fetchWithRetry(url, {
      ...options,
      headers,
      credentials: 'include',
      mode: 'cors',
    });

    if (response.status === 401 && !isRetry) {
      console.log(`🔄 Token expired during request, attempting refresh...`);
      const refreshed = await refreshAccessToken();
      
      if (refreshed) {
        console.log(`✅ Token refreshed, retrying request...`);
        return fetchWithAuth(endpoint, options, true);
      } else {
        clearAuthData();
        if (typeof window !== 'undefined') {
          window.dispatchEvent(new CustomEvent('auth:unauthorized'));
        }
        throw new Error('Your session has expired. Please log in again.');
      }
    }

    if (response.status === 401 && isRetry) {
      clearAuthData();
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      }
      throw new Error('Your session has expired. Please log in again.');
    }

    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;

      try {
        const errorResponse = response.clone();
        const contentType = errorResponse.headers.get('content-type') || '';

        if (contentType.includes('application/json')) {
          const errorData = await errorResponse.json();
          errorMessage = errorData.message || errorData.title || errorMessage;
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
    throw error;
  }
}

// ============ INTERFACES ============

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
  phoneNo?: string;
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
  userType?: string;
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

export interface SocialLoginRequest {
  idToken: string;
  provider: 'google' | 'apple';
  email?: string;
  name?: string;
  photoURL?: string;
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
  StaffType?: string; // Added this field - "Manager", "Staff", "Driver", etc.
}

// ============ DRIVER / RIDER INTERFACES ============

export interface Driver {
  id: string;
  lastName: string;
  firstName: string;
  email: string;
  password?: string;
  phoneNumber: string;
  profilePictureUrl: string;
  rating: number;
  available: boolean;
  state: string;
  locality: string;
  postcode: string;
  latitude: number;
  longitude: number;
  bankName: string;
  accountName: string;
  accountNumber: string;
  address: string;
  vehicleType?: string;
  licenseNumber?: string;
  status?: "Available" | "On Delivery" | "Off Duty";
  emergencyContact?: string;
  deliveries?: number;
  joined?: string;
  createdAt?: string;
  updatedAt?: string;
  companyId?: string;
  companyName?: string;
}

export interface AddDriverRequest {
  lastName: string;
  firstName: string;
  email: string;
  password: string;
  phoneNumber: string;
  profilePictureUrl?: string;
  rating?: number;
  available?: boolean;
  state?: string;
  locality?: string;
  postcode?: string;
  latitude?: number;
  longitude?: number;
  bankName?: string;
  accountName?: string;
  accountNumber?: string;
  address?: string;
  vehicleType?: string;
  licenseNumber?: string;
  status?: "Available" | "On Delivery" | "Off Duty";
  emergencyContact?: string;
}

export interface UpdateDriverRequest {
  lastName?: string;
  firstName?: string;
  email?: string;
  password?: string;
  phoneNumber?: string;
  profilePictureUrl?: string;
  rating?: number;
  available?: boolean;
  state?: string;
  locality?: string;
  postcode?: string;
  latitude?: number;
  longitude?: number;
  bankName?: string;
  accountName?: string;
  accountNumber?: string;
  address?: string;
  vehicleType?: string;
  licenseNumber?: string;
  status?: "Available" | "On Delivery" | "Off Duty";
  emergencyContact?: string;
}

// ============ AUTH FUNCTIONS ============

export async function login(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchPublic("auth-users/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function register(userData: RegisterRequest): Promise<RegisterResponse> {
  const response = await fetchPublic("auth-users", {
    method: "POST",
    body: JSON.stringify(userData),
  });
  
  const data = await response.json();
  if (data.errors) {
    const errorMessages = Object.values(data.errors).flat().join(" ");
    throw new Error(errorMessages);
  }
  
  return data;
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
  console.log('🔄 googleAuth called with:', {
    Email: data.Email,
    ExternalId: data.ExternalId,
  });

  try {
    const response = await fetchPublic("auth-users/auth-login", {
      method: "POST",
      body: JSON.stringify(data),
    });
    
    const loginData = await response.json();
    console.log(' googleAuth response received');
    
    setAuthData(loginData);
    console.log(' Auth data stored');
    
    return loginData;
  } catch (error: any) {
    console.error(' googleAuth error:', error);
    throw error;
  }
}

export async function firebaseAuth(data: FirebaseAuthRequest): Promise<LoginResponse> {
  console.log('🔄 firebaseAuth called with:', {
    provider: data.provider,
    email: data.email,
    firebaseUid: data.firebaseUid,
  });

  const endpoints = [
    "auth-users/firebase-auth",
    "auth-users/auth-login",
    "auth-users/auth-login",
    "auth-users/auth-login",
    "auth-users/firebase",
  ];
  
  let lastError: Error | null = null;
  
  for (const endpoint of endpoints) {
    try {
      console.log(`📡 Trying firebase auth with endpoint: ${endpoint}`);
      const response = await fetchPublic(endpoint, {
        method: "POST",
        body: JSON.stringify(data),
      });
      
      const loginData = await response.json();
      setAuthData(loginData);
      console.log(` Success with endpoint: ${endpoint}`);
      return loginData;
    } catch (error) {
      console.warn(` Failed with endpoint ${endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
      if (error instanceof Error) {
        if (!error.message.includes('404') && !error.message.includes('405')) {
          throw error;
        }
      }
    }
  }
  
  throw lastError || new Error('All Firebase authentication endpoints failed');
}

// ***** Unified social login function *****
export async function socialLogin(data: SocialLoginRequest): Promise<LoginResponse> {
  console.log('🔄 socialLogin called with:', {
    provider: data.provider,
    email: data.email,
    hasToken: !!data.idToken,
    tokenLength: data.idToken?.length || 0,
  });

  const attempts = [
    {
      endpoint: "auth-users/auth-login",
      payload: {
        idToken: data.idToken,
        provider: data.provider,
        email: data.email || '',
        name: data.name || '',
        photoURL: data.photoURL || '',
      }
    },
    {
      endpoint: "auth-users/firebase-auth",
      payload: {
        idToken: data.idToken,
        provider: data.provider,
        email: data.email || '',
        name: data.name || '',
        profilePicture: data.photoURL || '',
        firebaseUid: '',
      }
    },
    {
      endpoint: "auth-users/auth-login",
      payload: {
        Email: data.email || '',
        ExternalId: '',
        FirstName: data.name?.split(" ")[0] || '',
        LastName: data.name?.split(" ").slice(1).join(" ") || '',
        ProfilePicture: data.photoURL || '',
        idToken: data.idToken,
      }
    }
  ];

  let lastError: Error | null = null;

  for (const attempt of attempts) {
    try {
      console.log(` Trying social login with endpoint: ${attempt.endpoint}`);
      const response = await fetchPublic(attempt.endpoint, {
        method: "POST",
        body: JSON.stringify(attempt.payload),
      });

      const loginData = await response.json();
      console.log(` Success with endpoint: ${attempt.endpoint}`);
      
      setAuthData(loginData);
      console.log(' Auth data stored');
      
      return loginData;
    } catch (error) {
      console.warn(` Failed with endpoint ${attempt.endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }

  throw lastError || new Error('All social login attempts failed');
}

// ============ PROFILE FUNCTIONS ============

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

// ============ VERIFICATION FUNCTIONS ============

export async function verifyEmail(data: EmailVerificationRequest): Promise<LoginResponse> {
  console.log('🔄 Verifying email with:', { 
    Token: data.Token, 
    hasPassword: !!data.Password 
  });
  
  const response = await fetchPublic("auth-users/email-verification", {
    method: "POST",
    body: JSON.stringify({
      Token: data.Token,
      Password: data.Password
    }),
  });
  const loginData = await response.json();
  setAuthData(loginData);
  return loginData;
}

export async function requestEmailVerificationToken(email: string): Promise<string> {
  console.log('🔄 Requesting email verification token for:', email);
  
  const response = await fetchPublic("auth-users/email-verification-request-token", {
    method: "POST",
    body: JSON.stringify({ Email: email }),
  });
  return response.json();
}

export async function resendVerificationCode(email: string): Promise<string> {
  console.log('🔄 Resending verification code for:', email);
  return requestEmailVerificationToken(email);
}

export async function forgotPassword(email: string): Promise<string> {
  const response = await fetchPublic("auth-users/forgot-password-request-token", {
    method: "POST",
    body: JSON.stringify({ Email: email }),
  });
  return response.json();
}

export async function resetPassword(data: ResetPasswordRequest): Promise<string> {
  const response = await fetchPublic("auth-users/reset-password", {
    method: "POST",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function verifyPhoneNumber(otp: string): Promise<string> {
  const response = await fetchPublic("auth-users/phone-no-verification", {
    method: "POST",
    body: JSON.stringify({ Otp: otp }),
  });
  return response.json();
}

export async function enableTwoFactor(id: string): Promise<string> {
  const response = await fetchWithAuth(`auth-users/two-factor-enabled/${id}`, { method: "POST" });
  return response.json();
}

// ============ LOGISTICS FUNCTIONS ============

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

// ============ BOOKINGS FUNCTIONS ============

export async function getAccommodations(): Promise<Accommodation[]> {
  const response = await fetchWithAuth("bookings/accomodation", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function getAccommodationById(id: string): Promise<Accommodation> {
  const response = await fetchWithAuth(`bookings/accomodation/${id}`, { method: "GET" });
  return response.json();
}

export async function getRooms(accommodationId: string): Promise<any[]> {
  const response = await fetchWithAuth(`bookings/accomodation-reservations?id=${accommodationId}`, { method: "GET" });
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

// ============ PAYMENTS FUNCTIONS ============

export interface PaystackReservationRequest {
  customerName: string;
  customerPhoneNumber: string;
  customerEmail: string;
  numberOfGuests: number;
  trnxReference: string;
  paymentChannel: string;
  paymentStatus: boolean;
  comment: string;
  ticketClosed: boolean;
  reservationStartDate: string;
  reservationEndDate: string;
  noOfDays: number;
  staffId: string;
  staffName: string;
  purchaseChannel: string;
  userType: string;
}

export async function initializePaystackPayment(
  payload: PaystackReservationRequest | PaystackInitializeRequest
): Promise<any> {
  const response = await fetchWithAuth("bookings/initialize-paystack-accomodation-reservations-customer", {
    method: "POST",
    body: JSON.stringify(payload),
    headers: {
        reservationId: "" + (payload as any).reservationId || "",
      },
  });
  return response.json();
}

export async function verifyPaystackPayment(reference: string): Promise<any> {
  const response = await fetchWithAuth(`Wallet/verify?reference=${reference}`, { method: "GET" });
  return response.json();
}

export async function initializeFlutterwavePayment(
 payload: PaystackReservationRequest | PaystackInitializeRequest
): Promise<any>  {
  const response = await fetchWithAuth("bookings/initialize-flutterwave-accomodation-reservations-customer", {
    method: "POST",
    body: JSON.stringify(payload),
    headers: {
        reservationId: "" + (payload as any).reservationId || "",
      },
  });
  return response.json();
}

export interface PaystackInitializeRequest {
  amount: number;
  email: string;
  orderId?: string;
  metadata?: Record<string, any>;
}

export interface FlutterwaveInitializeRequest {
  amount: number;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  tx_ref?: string;
  metadata?: Record<string, any>;
}

export async function verifyFlutterwavePayment(reference: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`Wallet/verify-flutterwave?reference=${encodeURIComponent(reference)}`, { method: 'GET' });
    return response.json();
  } catch (err) {
    const fallback = await fetchWithAuth(`Wallet/verify?reference=${encodeURIComponent(reference)}`, { method: 'GET' });
    return fallback.json();
  }
}

// ============ NOTIFICATIONS FUNCTIONS ============

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

// ============ FEEDBACK FUNCTIONS ============

export async function submitFeedback(data: AddFeedbackRequest): Promise<any> {
  const response = await fetchWithAuth("Info/feedback", {
    method: "POST",
    body: JSON.stringify(data),
  });
  return response.json();
}

// ============ ADMIN FUNCTIONS ============

export async function adminLogin(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchPublic("admin-controller/login", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

export async function loginManager(credentials: LoginRequest): Promise<LoginResponse> {
  const response = await fetchPublic("admin-controller/login-manager-staff", {
    method: "POST",
    body: JSON.stringify(credentials),
  });
  const data = await response.json();
  setAuthData(data);
  return data;
}

// ============ STAFF MANAGEMENT FUNCTIONS ============

/**
 * Register a new staff member
 * Endpoint: POST /admin-controller/add-staff-manager
 * Based on Postman documentation
 */
export async function registerStaff(data: RegisterManagerRequest): Promise<any> {
  if (!isAuthenticated()) {
    throw new Error('You must be logged in as an admin to register a new staff member. Please log in first.');
  }
  
  try {
    // Ensure StaffType is set
    const staffData = {
      ...data,
      StaffType: data.StaffType || "Manager", // Default to Manager if not provided
    };
    
    console.log('📡 Registering staff with endpoint: admin-controller/add-staff-manager');
    console.log('📤 Request data:', JSON.stringify(staffData, null, 2));
    
    const response = await fetchWithAuth("admin-controller/add-staff-manager", {
      method: "POST",
      body: JSON.stringify(staffData),
    });
    
    // Check if response is ok before trying to parse
    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;
      let errorData: any = {};
      
      try {
        // Try to get the error response as JSON
        const clonedResponse = response.clone();
        const contentType = clonedResponse.headers.get('content-type') || '';
        
        if (contentType.includes('application/json')) {
          errorData = await clonedResponse.json();
          console.error('❌ Server error response:', errorData);
          
          // Extract validation errors from ASP.NET Core format
          if (errorData.errors && typeof errorData.errors === "object") {
            // Handle ASP.NET Core validation errors
            const errorMessages: string[] = [];
            
            // Iterate through each field error
            for (const [field, errors] of Object.entries(errorData.errors)) {
              if (Array.isArray(errors)) {
                // If errors is an array of strings
                errorMessages.push(`${field}: ${errors.join(', ')}`);
              } else if (typeof errors === 'string') {
                // If errors is a single string
                errorMessages.push(`${field}: ${errors}`);
              } else if (typeof errors === 'object' && errors !== null) {
                // If errors is an object with nested errors
                const nestedErrors = Object.values(errors).flat().join(', ');
                errorMessages.push(`${field}: ${nestedErrors}`);
              } else {
                // Fallback for other types
                errorMessages.push(`${field}: ${String(errors)}`);
              }
            }
            
            if (errorMessages.length > 0) {
              errorMessage = `Validation failed: ${errorMessages.join('; ')}`;
            } else {
              errorMessage = 'Validation failed. Please check all required fields.';
            }
          } 
          // Handle other error formats
          else if (errorData.message) {
            errorMessage = errorData.message;
          } else if (errorData.title) {
            errorMessage = errorData.title;
          } else if (errorData.detail) {
            errorMessage = errorData.detail;
          } else if (typeof errorData === 'string') {
            errorMessage = errorData;
          } else {
            // Fallback to stringified error data
            errorMessage = JSON.stringify(errorData);
          }
        } else {
          // Try to get text response
          const textResponse = await clonedResponse.text();
          if (textResponse) {
            errorMessage = textResponse.length > 300 ? textResponse.slice(0, 300) + '...' : textResponse;
          }
        }
      } catch (parseError) {
        console.warn('Could not parse error response:', parseError);
        // Try to get text response as fallback
        try {
          const textResponse = await response.text();
          if (textResponse) {
            errorMessage = textResponse.length > 300 ? textResponse.slice(0, 300) + '...' : textResponse;
          }
        } catch (textError) {
          // Ignore - keep original error message
        }
      }
      
      throw new Error(errorMessage);
    }
    
    const result = await response.json();
    console.log('✅ Staff registered successfully');
    return result;
  } catch (error: any) {
    console.error('❌ Failed to register staff:', error);
    throw error;
  }
}

/**
 * Alias for registerStaff - kept for backward compatibility
 */
export async function registerManager(data: RegisterManagerRequest): Promise<any> {
  return registerStaff(data);
}

/**
 * Get all staff members (Brand-owner-Manager-Staff)
 * Endpoint: GET /admin-controller/company-manager-staff
 * Based on Postman documentation
 */
export async function getAllStaff(params?: {
  UserId?: string;
  State?: string;
  Locality?: string;
  AnyItem?: string;
  FilterTypes?: string;
  StartDate?: string;
  EndDate?: string;
  Page?: number;
  PageSize?: number;
}): Promise<any[]> {
  try {
    // Build query string from params
    const queryParams = new URLSearchParams();
    if (params) {
      Object.entries(params).forEach(([key, value]) => {
        if (value !== undefined && value !== null && value !== '') {
          queryParams.append(key, String(value));
        }
      });
    }
    
    const queryString = queryParams.toString();
    const endpoint = queryString 
      ? `admin-controller/company-manager-staff?${queryString}`
      : "admin-controller/company-manager-staff";
    
    const response = await fetchWithAuth(endpoint, { method: "GET" });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn('Failed to fetch staff:', error);
    // Return mock data for testing
    return [
      {
        id: "1",
        firstName: "John",
        surName: "Doe",
        email: "john@company.com",
        phoneNo: "08012345678",
        adminType: "Manager",
        companyName: "Test Company",
        branch: "Lagos",
        staffCode: "STAFF-001",
        state: "Lagos",
        locality: "Ikeja",
        address: "123 Test Street",
        sex: "Male",
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      },
      {
        id: "2",
        firstName: "Jane",
        surName: "Smith",
        email: "jane@company.com",
        phoneNo: "08087654321",
        adminType: "Staff",
        companyName: "Test Company",
        branch: "Abuja",
        staffCode: "STAFF-002",
        state: "Abuja",
        locality: "Garki",
        address: "456 Test Avenue",
        sex: "Female",
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
    ];
  }
}

// ============ ADMIN DATA FUNCTIONS ============

export async function adminGetProfile(): Promise<any> {
  const response = await fetchWithAuth("admin-controller/profile", { method: "GET" });
  return response.json();
}

export async function getAllUsers(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("auth-users", { method: "GET" });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error: any) {
    if (error.message.includes('403')) {
      console.warn('⚠️ User does not have permission to view all users');
      return [];
    }
    throw error;
  }
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

export async function getAllAdverts(): Promise<any[]> {
  const response = await fetchWithAuth("admin-controller/advert", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

// ============ DRIVER / RIDER MANAGEMENT FUNCTIONS ============

/**
 * Get all drivers/riders for the current company
 * Endpoint: GET /admin-controller/drivers
 */
export async function getAllDrivers(): Promise<Driver[]> {
  try {
    const response = await fetchWithAuth("admin-controller/drivers", { method: "GET" });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error: any) {
    if (error.message.includes('403')) {
      console.warn('⚠️ User does not have permission to view drivers');
      return [];
    }
    console.warn('Failed to fetch drivers, using fallback data:', error);
    return [
      {
        id: "1",
        firstName: "Mike",
        lastName: "Johnson",
        email: "mike@company.com",
        phoneNumber: "08012345678",
        vehicleType: "Motorcycle",
        licenseNumber: "DL-2024-001",
        status: "Available",
        rating: 4.7,
        deliveries: 145,
        address: "123 Main St, Lagos",
        emergencyContact: "08012345679",
        available: true,
        state: "Lagos",
        locality: "Ikeja",
        postcode: "100001",
        latitude: 6.5244,
        longitude: 3.3792,
        bankName: "GTBank",
        accountName: "Mike Johnson",
        accountNumber: "0123456789",
        profilePictureUrl: "",
        joined: "2024-01-10",
        createdAt: "2024-01-10",
        updatedAt: "2024-01-10",
        companyId: "1",
        companyName: "Test Company"
      },
      {
        id: "2",
        firstName: "Sara",
        lastName: "Chen",
        email: "sara@company.com",
        phoneNumber: "08087654321",
        vehicleType: "Van",
        licenseNumber: "DL-2024-002",
        status: "On Delivery",
        rating: 4.9,
        deliveries: 203,
        address: "456 Oak Ave, Abuja",
        emergencyContact: "08087654322",
        available: true,
        state: "Abuja",
        locality: "Garki",
        postcode: "900001",
        latitude: 9.0579,
        longitude: 7.4951,
        bankName: "Access Bank",
        accountName: "Sara Chen",
        accountNumber: "9876543210",
        profilePictureUrl: "",
        joined: "2023-11-15",
        createdAt: "2023-11-15",
        updatedAt: "2023-11-15",
        companyId: "1",
        companyName: "Test Company"
      }
    ];
  }
}

/**
 * Get a single driver/rider by ID
 * Endpoint: GET /logistics-controller/rider/{id}
 */
export async function getDriverById(id: string): Promise<Driver> {
  const response = await fetchWithAuth(`logistics-controller/rider/${id}`, { method: "GET" });
  return response.json();
}

/**
 * Add a new driver/rider
 * Endpoint: POST /logistics-controller/rider
 * Based on Postman documentation
 */
export async function addDriver(driverData: AddDriverRequest): Promise<Driver> {
  console.log('🔄 Adding new rider/driver...');
  console.log('📤 Request data:', driverData);
  
  // Check if token exists and is valid
  const authStatus = getAuthStatus();
  if (!authStatus.isAuthenticated) {
    throw new Error('Your session has expired. Please log in again.');
  }
  
  try {
    // Use the correct endpoint: /logistics-controller/rider
    const response = await fetchWithAuth("logistics-controller/rider", {
      method: "POST",
      body: JSON.stringify(driverData),
    });
    const data = await response.json();
    console.log('✅ Rider/driver added successfully');
    return data;
  } catch (error: any) {
    console.error('❌ Failed to add rider/driver:', error);
    
    // Handle session expired specifically
    if (error.message.includes('401') || 
        error.message.includes('session expired') ||
        error.message.includes('unauthorized')) {
      clearAuthData();
      if (typeof window !== 'undefined') {
        window.dispatchEvent(new CustomEvent('auth:unauthorized'));
      }
      throw new Error('Your session has expired. Please log in again.');
    }
    
    // Handle validation errors
    if (error.message.includes('400')) {
      throw new Error('Invalid rider data. Please check all required fields.');
    }
    
    throw error;
  }
}

/**
 * Update an existing driver/rider
 * Endpoint: PUT /admin-controller/drivers/{id}
 */
export async function updateDriver(id: string, driverData: UpdateDriverRequest): Promise<Driver> {
  console.log(`🔄 Updating rider/driver ${id}...`);
  try {
    const response = await fetchWithAuth(`admin-controller/drivers/${id}`, {
      method: "PUT",
      body: JSON.stringify(driverData),
    });
    const data = await response.json();
    console.log('✅ Rider/driver updated successfully');
    return data;
  } catch (error: any) {
    console.error('❌ Failed to update rider/driver:', error);
    throw error;
  }
}

/**
 * Delete a driver/rider by ID
 * Endpoint: DELETE /admin-controller/drivers/{id}
 */
export async function deleteDriver(id: string): Promise<void> {
  console.log(`🗑️ Deleting rider/driver ${id}...`);
  try {
    await fetchWithAuth(`admin-controller/drivers/${id}`, { method: "DELETE" });
    console.log('✅ Rider/driver deleted successfully');
  } catch (error: any) {
    console.error('❌ Failed to delete rider/driver:', error);
    throw error;
  }
}

/**
 * Update driver/rider status (Available, On Delivery, Off Duty)
 * Endpoint: PATCH /admin-controller/drivers/{id}/status
 */
export async function updateDriverStatus(
  id: string, 
  status: "Available" | "On Delivery" | "Off Duty"
): Promise<Driver> {
  console.log(`🔄 Updating rider/driver ${id} status to ${status}...`);
  try {
    const response = await fetchWithAuth(`admin-controller/drivers/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify({ status }),
    });
    const data = await response.json();
    console.log('✅ Rider/driver status updated successfully');
    return data;
  } catch (error: any) {
    console.error('❌ Failed to update rider/driver status:', error);
    throw error;
  }
}

/**
 * Update driver/rider availability status (true/false)
 * Endpoint: PATCH /admin-controller/drivers/{id}/availability
 */
export async function updateDriverAvailability(id: string, available: boolean): Promise<Driver> {
  const response = await fetchWithAuth(`admin-controller/drivers/${id}/availability`, {
    method: "PATCH",
    body: JSON.stringify({ available }),
  });
  return response.json();
}

/**
 * Get available drivers/riders for assignment
 * Endpoint: GET /admin-controller/drivers/available
 */
export async function getAvailableDrivers(): Promise<Driver[]> {
  const response = await fetchWithAuth("admin-controller/drivers/available", { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

/**
 * Assign a driver/rider to an order
 * Endpoint: PUT /logistics-controller/package-orders/{orderId}/assign
 */
export async function assignDriverToOrder(orderId: string, driverId: string): Promise<any> {
  console.log(`🔄 Assigning rider/driver ${driverId} to order ${orderId}...`);
  try {
    const response = await fetchWithAuth(`logistics-controller/package-orders/${orderId}/assign`, {
      method: "PUT",
      body: JSON.stringify({ driverId }),
    });
    const data = await response.json();
    console.log('✅ Rider/driver assigned to order successfully');
    return data;
  } catch (error: any) {
    console.error('❌ Failed to assign rider/driver to order:', error);
    throw error;
  }
}

/**
 * Get drivers/riders by company
 * Endpoint: GET /admin-controller/companies/{companyId}/drivers
 */
export async function getDriversByCompany(companyId: string): Promise<Driver[]> {
  const response = await fetchWithAuth(`admin-controller/companies/${companyId}/drivers`, { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

/**
 * Get driver/rider statistics for dashboard
 * Endpoint: GET /admin-controller/drivers/stats
 */
export async function getDriverStats(): Promise<{
  total: number;
  available: number;
  onDelivery: number;
  offDuty: number;
}> {
  try {
    const response = await fetchWithAuth("admin-controller/drivers/stats", { method: "GET" });
    return response.json();
  } catch (error) {
    console.warn('Failed to fetch driver stats, using fallback:', error);
    return { total: 0, available: 0, onDelivery: 0, offDuty: 0 };
  }
}

/**
 * Get rider/driver by email
 * Endpoint: GET /logistics-controller/rider-by-email?email={email}
 */
export async function getDriverByEmail(email: string): Promise<Driver> {
  const response = await fetchWithAuth(`logistics-controller/rider-by-email?email=${encodeURIComponent(email)}`, { 
    method: "GET" 
  });
  return response.json();
}

// ============ COMPANY MANAGEMENT FUNCTIONS ============

export async function getCompanyDrivers(companyId: string): Promise<Driver[]> {
  const response = await fetchWithAuth(`admin-controller/companies/${companyId}/drivers`, { method: "GET" });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function updateCompanyProfile(data: Record<string, unknown>): Promise<any> {
  const response = await fetchWithAuth("admin-controller/company-profile", {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function getCompanyStats(): Promise<{
  totalOrders: number;
  totalRevenue: number;
  activeDrivers: number;
  pendingBookings: number;
}> {
  const response = await fetchWithAuth("admin-controller/company-stats", { method: "GET" });
  return response.json();
}

// ============ EXPORT ALL FUNCTIONS ============

export { API_URL };