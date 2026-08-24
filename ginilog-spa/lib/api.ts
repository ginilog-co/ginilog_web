// lib/api.ts

const DEFAULT_PRODUCTION_API = "https://api-data-connection.ginilog.org";
const LOCAL_API = "https://api-data-connection.ginilog.org";

function resolveApiUrl(): string {
  const fromEnv = process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "");
  if (fromEnv) return fromEnv;

  if (typeof window === "undefined") {
    return LOCAL_API;
  }

  if (
    window.location.hostname === "www.ginilog.com" ||
    window.location.hostname === "ginilog.com" ||
    window.location.hostname.includes("vercel.app")
  ) {
    return DEFAULT_PRODUCTION_API;
  }

  if (
    window.location.hostname === "localhost" ||
    window.location.hostname === "127.0.0.1"
  ) {
    return LOCAL_API;
  }

  return DEFAULT_PRODUCTION_API;
}

const API_URL = resolveApiUrl();

// FIXED: Improved proxy URL construction
function getProxyUrl(endpoint: string): string {
  let cleanEndpoint = endpoint.startsWith("/") ? endpoint.slice(1) : endpoint;

  // Remove duplicate /api prefix if caller already included it
  cleanEndpoint = cleanEndpoint.replace(/^api\//, "");

  // Normalize any accidental absolute backend URLs to the same-origin proxy
  cleanEndpoint = cleanEndpoint.replace(
    /^https?:\/\/api-data-connection\.ginilog\.org\/api\//,
    "",
  );

  const proxyUrl = `/api/${cleanEndpoint}`;
  console.log(`Proxying request through same-origin URL: ${proxyUrl}`);
  return proxyUrl;
}

// Enhanced fetch wrapper with timeout and retry logic
async function fetchWithTimeout(
  url: string,
  options: RequestInit = {},
  timeout: number = 30000,
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
    if (error instanceof Error && error.name === "AbortError") {
      throw new Error(
        "Request timeout. Please check your connection and try again.",
      );
    }
    throw error;
  }
}

// Retry wrapper for failed requests
async function fetchWithRetry(
  url: string,
  options: RequestInit = {},
  retries: number = 3,
  delay: number = 1000,
): Promise<Response> {
  try {
    return await fetchWithTimeout(url, options);
  } catch (error) {
    if (retries > 0) {
      await new Promise((resolve) => setTimeout(resolve, delay));
      return fetchWithRetry(url, options, retries - 1, delay * 2);
    }
    throw error;
  }
}

// Helper function to safely extract array from response
function extractArrayFromResponse(data: any): any[] {
  if (!data) return [];
  if (Array.isArray(data)) return data;
  if (typeof data === "object") {
    if (data.data && Array.isArray(data.data)) return data.data;
    if (data.items && Array.isArray(data.items)) return data.items;
    if (data.results && Array.isArray(data.results)) return data.results;
    if (data.records && Array.isArray(data.records)) return data.records;
    if (data.list && Array.isArray(data.list)) return data.list;
    if (data.id) return [data];
  }
  console.warn("Unexpected response format, expected array:", data);
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
    window.dispatchEvent(
      new StorageEvent("storage", {
        key: "token",
        newValue: data.token,
      }),
    );
  }
}

export function clearAuthData(): void {
  if (typeof window !== "undefined") {
    localStorage.removeItem("token");
    localStorage.removeItem("refreshToken");
    localStorage.removeItem("user");
    window.dispatchEvent(new CustomEvent("auth:logout"));
  }
}

/**
 * Check if the current token is expired
 */
export function isTokenExpired(): boolean {
  const token = getToken();
  if (!token) return true;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;
    return Date.now() > exp;
  } catch (error) {
    console.warn("Failed to decode token:", error);
    return true;
  }
}

/**
 * Get token expiration time
 */
export function getTokenExpiry(): Date | null {
  const token = getToken();
  if (!token) return null;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;
    return new Date(exp);
  } catch (error) {
    return null;
  }
}

/**
 * Get time remaining until token expires in minutes
 */
export function getTimeRemaining(): number {
  const token = getToken();
  if (!token) return 0;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;
    const now = Date.now();
    const remaining = exp - now;
    return Math.floor(remaining / 60000);
  } catch {
    return 0;
  }
}

/**
 * Check if token is near expiry (less than 5 minutes)
 */
export function isTokenNearExpiry(): boolean {
  const token = getToken();
  if (!token) return true;

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;
    const now = Date.now();
    const remaining = exp - now;
    return remaining < 5 * 60 * 1000;
  } catch {
    return true;
  }
}

/**
 * Validate session and redirect if expired
 */
export function validateSession(): boolean {
  if (typeof window === "undefined") return false;

  const token = getToken();
  if (!token) {
    return false;
  }

  try {
    const payload = JSON.parse(atob(token.split(".")[1]));
    const exp = payload.exp * 1000;
    const now = Date.now();
    
    if (now > exp - 30000) {
      clearAuthData();
      return false;
    }
    
    return true;
  } catch {
    clearAuthData();
    return false;
  }
}

/**
 * Check session and redirect if expired
 */
export function checkSessionAndRedirect(): boolean {
  if (typeof window === "undefined") return false;

  const token = getToken();
  if (!token || isTokenExpired()) {
    clearAuthData();
    window.location.href = "/brand-owner/login";
    return false;
  }

  if (isTokenNearExpiry()) {
    refreshAccessToken();
  }

  return true;
}

/**
 * Check auth status and redirect if needed
 */
export function checkAuthAndRedirect(redirectTo: string = "/login"): boolean {
  if (typeof window === "undefined") return false;

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
 */
export function setupAutoRefresh(): void {
  if (typeof window === "undefined") return;

  const checkAndRefresh = async () => {
    const token = getToken();
    if (!token) return;

    try {
      const payload = JSON.parse(atob(token.split(".")[1]));
      const exp = payload.exp * 1000;
      const now = Date.now();
      const timeUntilExpiry = exp - now;

      if (timeUntilExpiry < 5 * 60 * 1000 && timeUntilExpiry > 0) {
        console.log(
          `Token expires in ${Math.round(timeUntilExpiry / 60000)} minutes, refreshing...`,
        );
        await refreshAccessToken();
      }
    } catch (error) {
      console.warn("Auto-refresh check failed:", error);
    }
  };

  const interval = setInterval(checkAndRefresh, 2 * 60 * 1000);

  window.addEventListener("beforeunload", () => {
    clearInterval(interval);
  });
}

// FIXED: Improved refresh token function with more endpoints and better error handling
export async function refreshAccessToken(): Promise<string | null> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    console.warn("No refresh token available");
    return null;
  }

  console.log("Attempting to refresh access token...");

  const refreshEndpoints = [
    `${API_URL}/api/auth-users/tokens/refresh`,
    `${API_URL}/api/auth-users/refresh-token`,
    `${API_URL}/api/auth-users/token/refresh`,
    `${API_URL}/api/auth/refresh-token`,
    `${API_URL}/api/auth/token/refresh`,
    `${API_URL}/api/auth-users/refresh`,
    `${API_URL}/api/auth/refresh`,
  ];

  let lastError: Error | null = null;

  for (const endpoint of refreshEndpoints) {
    try {
      console.log(`Attempting token refresh at: ${endpoint}`);

      const response = await fetch(endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          refreshToken: refreshToken,
          refresh_token: refreshToken,
          token: refreshToken,
        }),
        credentials: "include",
      });

      if (response.ok) {
        const data = await response.json();
        const newToken =
          data.token ||
          data.accessToken ||
          data.access_token ||
          data.data?.token ||
          data.data?.accessToken ||
          null;

        if (newToken) {
          localStorage.setItem("token", newToken);
          if (data.refreshToken || data.refresh_token) {
            localStorage.setItem("refreshToken", data.refreshToken || data.refresh_token);
          }
          console.log("Access token refreshed successfully");
          return newToken;
        } else {
          console.warn("No token in refresh response:", Object.keys(data));
        }
      } else {
        console.warn(`Refresh failed with status ${response.status} at ${endpoint}`);
      }
    } catch (error) {
      console.warn(`Failed to refresh at ${endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }

  console.error("All refresh attempts failed");
  clearAuthData();
  return null;
}

// Health check function
export async function checkApiHealth(): Promise<boolean> {
  try {
    const response = await fetch(`${API_URL}/health`, {
      method: "GET",
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
  options: RequestInit = {},
): Promise<Response> {
  const cleanEndpoint = endpoint.startsWith("/") ? endpoint.slice(1) : endpoint;
  const url =
    typeof window !== "undefined"
      ? getProxyUrl(cleanEndpoint)
      : `${API_URL}/api/${cleanEndpoint}`;

  console.log(`Fetching (public): ${url}`);

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    Accept: "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  try {
    const response = await fetchWithRetry(url, {
      ...options,
      headers,
      credentials: "include",
      mode: "cors",
    });

    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;

      try {
        const errorResponse = response.clone();
        const contentType = errorResponse.headers.get("content-type") || "";

        if (contentType.includes("application/json")) {
          const errorData = await errorResponse.json();

          if (errorData.errors && typeof errorData.errors === "object") {
            const messages = Object.values(
              errorData.errors as Record<string, string[]>,
            )
              .flat()
              .join(" ");
            errorMessage = messages || errorData.title || errorMessage;
          } else if (errorData.message) {
            errorMessage = errorData.message;
          } else if (errorData.title) {
            errorMessage = errorData.title;
          } else {
            errorMessage = JSON.stringify(errorData);
          }
        } else {
          const text = await errorResponse.text();
          if (text && text.length > 0) {
            errorMessage = text.length > 300 ? text.slice(0, 300) + "..." : text;
          }
        }
      } catch (error) {
        console.warn("Could not read error response body:", error);
      }

      throw new Error(errorMessage);
    }

    return response;
  } catch (error) {
    console.error(`Public API Error for ${url}:`, error);
    throw error;
  }
}

// FIXED: ***** fetchWithAuth with proper token refresh and 500 handling *****
async function fetchWithAuth(
  endpoint: string,
  options: RequestInit = {},
  isRetry: boolean = false,
): Promise<Response> {
  const cleanEndpoint = endpoint.startsWith("/") ? endpoint.slice(1) : endpoint;
  const url =
    typeof window !== "undefined"
      ? getProxyUrl(cleanEndpoint)
      : `${API_URL}/api/${cleanEndpoint}`;

  console.log(`Fetching (auth): ${url}`);

  // Check token expiration BEFORE making the request
  if (!isRetry && isTokenExpired()) {
    console.log("Token expired, attempting refresh before request...");
    const refreshed = await refreshAccessToken();
    if (!refreshed) {
      clearAuthData();
      if (typeof window !== "undefined") {
        window.dispatchEvent(new CustomEvent("auth:unauthorized"));
      }
      throw new Error("Your session has expired. Please log in again.");
    }
  }

  const token = getToken();

  if (!token) {
    console.warn(`No token found for request to ${url}`);
    throw new Error("Authentication required. Please log in.");
  }

  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    Accept: "application/json",
    ...((options.headers as Record<string, string>) || {}),
  };

  headers["Authorization"] = `Bearer ${token}`;

  try {
    const response = await fetchWithRetry(url, {
      ...options,
      headers,
      credentials: "include",
      mode: "cors",
    });

    // Handle 500 Internal Server Error with more detail
    if (response.status === 500) {
      console.error(`Server Error (500) for ${url}`);
      
      let errorMessage = "Internal server error. Please try again later.";
      let errorDetails: any = null;
      
      try {
        const errorData = await response.clone().json();
        errorDetails = errorData;
        
        // Try to extract meaningful error message
        if (errorData.message) {
          errorMessage = errorData.message;
        } else if (errorData.error) {
          errorMessage = errorData.error;
        } else if (errorData.errors) {
          const messages = Object.values(errorData.errors).flat().join(" ");
          errorMessage = messages;
        } else if (errorData.title) {
          errorMessage = errorData.title;
        } else {
          errorMessage = JSON.stringify(errorData);
        }
      } catch (e) {
        try {
          const text = await response.clone().text();
          if (text && text.length > 0 && text.length < 500) {
            errorMessage = text;
          }
        } catch (e2) {
          // Ignore
        }
      }
      
      // Log the full error details for debugging
      console.error("Server Error Details:", {
        status: 500,
        url: url,
        endpoint: endpoint,
        method: options.method || 'GET',
        body: options.body,
        error: errorDetails,
        message: errorMessage
      });
      
      throw new Error(`Server error: ${errorMessage}`);
    }

    // Handle 403 Forbidden specifically
    if (response.status === 403) {
      console.warn(`403 Forbidden for ${url}`);

      let errorMessage = "You don't have permission to perform this action.";
      try {
        const errorData = await response.clone().json();
        errorMessage = errorData.message || errorData.title || errorMessage;
      } catch (e) {
        try {
          const text = await response.clone().text();
          if (text && text.length > 0 && text.length < 500) {
            errorMessage = text;
          }
        } catch (e2) {
          // Ignore
        }
      }

      // If it's a session expiry misclassified as 403, clear auth
      const lowerMessage = errorMessage.toLowerCase();
      if (
        lowerMessage.includes("expired") ||
        lowerMessage.includes("session") ||
        lowerMessage.includes("unauthorized") ||
        lowerMessage.includes("invalid token") ||
        lowerMessage.includes("token expired")
      ) {
        clearAuthData();
        if (typeof window !== "undefined") {
          window.dispatchEvent(new CustomEvent("auth:unauthorized"));
        }
        throw new Error("Your session has expired. Please log in again.");
      }

      throw new Error(errorMessage);
    }

    // Handle 401 Unauthorized - Token expired
    if (response.status === 401 && !isRetry) {
      console.log(`Token expired (401), attempting refresh...`);
      const refreshed = await refreshAccessToken();

      if (refreshed) {
        console.log(`Token refreshed, retrying request...`);
        return fetchWithAuth(endpoint, options, true);
      } else {
        clearAuthData();
        if (typeof window !== "undefined") {
          window.dispatchEvent(new CustomEvent("auth:unauthorized"));
        }
        throw new Error("Your session has expired. Please log in again.");
      }
    }

    if (response.status === 401 && isRetry) {
      clearAuthData();
      if (typeof window !== "undefined") {
        window.dispatchEvent(new CustomEvent("auth:unauthorized"));
      }
      throw new Error("Your session has expired. Please log in again.");
    }

    if (!response.ok) {
      let errorMessage = `HTTP error! status: ${response.status}`;

      try {
        const errorResponse = response.clone();
        const contentType = errorResponse.headers.get("content-type") || "";

        if (contentType.includes("application/json")) {
          const errorData = await errorResponse.json();
          errorMessage = errorData.message || errorData.title || errorMessage;
        } else {
          const text = await errorResponse.text();
          if (text && text.length > 0) {
            errorMessage = text.length > 300 ? text.slice(0, 300) + "..." : text;
          }
        }
      } catch (error) {
        console.warn("Could not read error response body:", error);
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
  companyName?: string;
  companyId?: string;
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
  suspendedAccount?: boolean;
  archivedAccount?: boolean;
  deviceTokenModels: Array<{
    deviceTokenId: string;
    userId: string;
    userType: string;
  }>;
}

export interface PaginatedUsersResponse {
  data: UserProfile[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface GetUsersParams {
  UserId?: string;
  State?: string;
  Locality?: string;
  AnyItem?: string;
  FilterTypes?: string;
  StartDate?: string;
  EndDate?: string;
  Page?: number;
  PageSize?: number;
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
  provider: "google" | "apple";
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
  state?: string;
  country?: string;
  bookingAmount: number;
  noOfRooms?: number;
  accomodationImages: string[];
  accomodationDescription: string;
  accomodationFacilities?: string[];
  available?: boolean;
  rating: number;
  createdAt?: string;
  updatedAt?: string;
  userId?: string;
  companyId?: string;
  // Allow additional dynamic properties from API
  [key: string]: any;
}

export interface Company {
  id: string;
  adminId: string;
  companyEmail: string;
  companyName: string;
  phoneNumber: string;
  companyLogo: string;
  companyRegNo: string;
  companyInfo: string;
  rating: number;
  valueCharge: number;
  noOfTrucks: number;
  nofOfBikes: number;
  available: boolean;
  companyAddress: string;
  postCodes: string;
  locality: string;
  state: string;
  latitude: number;
  longitude: number;
  bankName: string;
  accountName: string;
  accountNumber: string;
  deliveryTypes: string[];
  serviceAreas: string[];
  companyReviewModels: any[];
  companyStatus?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface PaginatedCompaniesResponse {
  data: Company[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface GetCompaniesParams {
  UserId?: string;
  State?: string;
  Locality?: string;
  AnyItem?: string;
  FilterTypes?: string;
  StartDate?: string;
  EndDate?: string;
  Page?: number;
  PageSize?: number;
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
  riderId?: string;        // ✅ Added - Rider ID
  riderPhoneNo?: string;   // ✅ Added - Rider Phone Number
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
  ValueCharge?: number;
  CompanyInfo?: string;
  NoOfTrucks?: number;
  NofOfBikes?: number;
  BankName?: string;
  AccountName?: string;
  AccountNumber?: string;
  DeliveryTypes?: string[];
  ServiceAreas?: string[];
}

// ============ BRAND OWNER & STAFF REGISTRATION INTERFACES ============

export interface RegisterBrandOwnerRequest {
  staffType: "BrandOwner";
  firstName: string;
  surName: string;
  email: string;
  password: string;
  sex: "Male" | "Female" | "Other";
  staffCode: string;
  phoneNo: string;
  address: string;
  companyName: string;
  companyUserName: string;
  companyType: string[];
  roles: string[];
  permissions: string[];
  state: string;
  branch: string;
  locality: string;
}

export interface RegisterStaffRequest {
  staffType: "Staff";
  firstName: string;
  surName: string;
  email: string;
  password: string;
  sex: "Male" | "Female" | "Other";
  staffCode: string;
  phoneNo: string;
  address: string;
  companyId: string;
  roles: string[];
  permissions: string[];
}

// ============ DRIVER INTERFACES ============

export interface Driver {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  profilePictureUrl?: string;
  rating: number;
  available: boolean;
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
  status: "Available" | "On Delivery" | "Off Duty";
  emergencyContact?: string;
  deliveries?: number;
  joined?: string;
  createdAt?: string;
  updatedAt?: string;
  companyId?: string;
  companyName?: string;
  currentLocation?: string;
  currentLatitude?: number;
  currentLongitude?: number;
}

export interface AddDriverRequest {
  firstName: string;
  lastName: string;
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
  companyId?: string;
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

export async function register(
  userData: RegisterRequest,
): Promise<RegisterResponse> {
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
    await fetchWithAuth("auth-users/logout", { method: "POST" }).catch(
      () => {},
    );
  } finally {
    clearAuthData();
  }
}

export async function googleAuth(
  data: GoogleAuthRequest,
): Promise<LoginResponse> {
  console.log("googleAuth called with:", {
    Email: data.Email,
    ExternalId: data.ExternalId,
  });

  try {
    const response = await fetchPublic("auth-users/auth-login", {
      method: "POST",
      body: JSON.stringify(data),
    });

    const loginData = await response.json();

    setAuthData(loginData);

    return loginData;
  } catch (error: any) {
    console.error("googleAuth error:", error);
    throw error;
  }
}

export async function firebaseAuth(
  data: FirebaseAuthRequest,
): Promise<LoginResponse> {
  console.log("firebaseAuth called with:", {
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
      console.log(`Trying firebase auth with endpoint: ${endpoint}`);
      const response = await fetchPublic(endpoint, {
        method: "POST",
        body: JSON.stringify(data),
      });

      const loginData = await response.json();
      setAuthData(loginData);
      console.log(`Success with endpoint: ${endpoint}`);
      return loginData;
    } catch (error) {
      console.warn(`Failed with endpoint ${endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
      if (error instanceof Error) {
        if (!error.message.includes("404") && !error.message.includes("405")) {
          throw error;
        }
      }
    }
  }

  throw lastError || new Error("All Firebase authentication endpoints failed");
}

// ***** Unified social login function *****
export async function socialLogin(
  data: SocialLoginRequest,
): Promise<LoginResponse> {
  console.log("socialLogin called with:", {
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
        email: data.email || "",
        name: data.name || "",
        photoURL: data.photoURL || "",
      },
    },
    {
      endpoint: "auth-users/firebase-auth",
      payload: {
        idToken: data.idToken,
        provider: data.provider,
        email: data.email || "",
        name: data.name || "",
        profilePicture: data.photoURL || "",
        firebaseUid: "",
      },
    },
    {
      endpoint: "auth-users/auth-login",
      payload: {
        Email: data.email || "",
        ExternalId: "",
        FirstName: data.name?.split(" ")[0] || "",
        LastName: data.name?.split(" ").slice(1).join(" ") || "",
        ProfilePicture: data.photoURL || "",
        idToken: data.idToken,
      },
    },
  ];

  let lastError: Error | null = null;

  for (const attempt of attempts) {
    try {
      console.log(`Trying social login with endpoint: ${attempt.endpoint}`);
      const response = await fetchPublic(attempt.endpoint, {
        method: "POST",
        body: JSON.stringify(attempt.payload),
      });

      const loginData = await response.json();
      console.log(`Success with endpoint: ${attempt.endpoint}`);

      setAuthData(loginData);

      return loginData;
    } catch (error) {
      console.warn(`Failed with endpoint ${attempt.endpoint}:`, error);
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }

  throw lastError || new Error("All social login attempts failed");
}

// ============ PROFILE FUNCTIONS ============

export async function updateProfile(
  data: UpdateUserRequest,
): Promise<UserProfile> {
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
  const response = await fetchWithAuth("auth-users/delivery-address", {
    method: "GET",
  });
  const data = await response.json();
  return extractArrayFromResponse(data);
}

export async function addNewAddress(
  data: AddDeliveryAddressRequest,
): Promise<UserProfile> {
  const response = await fetchWithAuth("auth-users/add-new-address", {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function updateDeliveryAddress(
  id: string,
  data: AddDeliveryAddressRequest,
): Promise<UserProfile> {
  const response = await fetchWithAuth(
    `auth-users/update-delivery-address/${id}`,
    {
      method: "PUT",
      body: JSON.stringify(data),
    },
  );
  return response.json();
}

export async function deleteDeliveryAddress(id: string): Promise<void> {
  await fetchWithAuth(`auth-users/delete-delivery-address/${id}`, {
    method: "DELETE",
  });
}

// ============ AUTH USER VERIFICATION (for regular users) ============

export async function verifyAuthUserEmail(
  data: EmailVerificationRequest,
): Promise<LoginResponse> {
  console.log("Verifying auth user email with:", {
    Token: data.Token,
    hasPassword: !!data.Password,
  });

  const response = await fetchPublic("auth-users/email-verification", {
    method: "POST",
    body: JSON.stringify({
      Token: data.Token,
      Password: data.Password,
    }),
  });
  const loginData = await response.json();
  setAuthData(loginData);
  return loginData;
}

export async function requestAuthUserEmailVerificationToken(
  email: string,
): Promise<string> {
  console.log("Requesting auth user email verification token for:", email);

  const response = await fetchPublic(
    "auth-users/email-verification-request-token",
    {
      method: "POST",
      body: JSON.stringify({ Email: email }),
    },
  );
  return response.json();
}

export async function resendAuthUserVerificationCode(email: string): Promise<string> {
  console.log("Resending auth user verification code for:", email);
  return requestAuthUserEmailVerificationToken(email);
}

// ============ COMPANY/BRAND OWNER VERIFICATION ============

/**
 * Send verification code to company/brand owner email
 * Uses admin-controller endpoints
 */
export async function sendCompanyVerificationCode(email: string): Promise<{ message: string; code?: string }> {
  try {
    console.log(`Sending company verification code to: ${email}`);
    
    const endpoints = [
      { endpoint: "admin-controller/send-verification", method: "POST" },
      { endpoint: "admin-controller/email-verification-request-token-request-token", method: "POST" },
      { endpoint: "admin-controller/verify-request", method: "POST" },
    ];
    
    let lastError: Error | null = null;
    
    for (const attempt of endpoints) {
      try {
        console.log(`Trying company verification endpoint: ${attempt.endpoint} (${attempt.method})`);
        const response = await fetchPublic(attempt.endpoint, {
          method: attempt.method,
          body: JSON.stringify({ Email: email }),
        });
        
        const data = await response.json();
        console.log(`Company verification code sent via ${attempt.endpoint}`);
        return data;
      } catch (error) {
        console.warn(`Failed with endpoint ${attempt.endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    try {
      console.log("Falling back to auth-users endpoint for company verification");
      const response = await fetchPublic("auth-users/email-verification-request-token", {
        method: "POST",
        body: JSON.stringify({ Email: email }),
      });
      const data = await response.json();
      console.log("Company verification code sent via fallback");
      return data;
    } catch (error) {
      console.warn("Fallback also failed:", error);
      if (lastError) throw lastError;
      throw error;
    }
    
  } catch (error) {
    console.error("Failed to send company verification code:", error);
    throw error;
  }
}

/**
 * Verify company/brand owner email with code
 * Uses admin-controller endpoints
 */
export async function verifyCompanyEmailWithCode(email: string, code: string): Promise<{ isValid: boolean; message?: string; token?: string }> {
  try {
    console.log(`Verifying company email: ${email} with code: ${code}`);
    
    const endpoints = [
      { endpoint: "admin-controller/verify-email", method: "POST", body: { Email: email, Code: code } },
      { endpoint: "admin-controller/email-verification-request-token", method: "POST", body: { Email: email, Token: code } },
      { endpoint: "admin-controller/verify", method: "POST", body: { Email: email, Token: code } },
      { endpoint: "admin-controller/company-verify", method: "POST", body: { Email: email, Otp: code } },
    ];
    
    let lastError: Error | null = null;
    
    for (const attempt of endpoints) {
      try {
        console.log(`Trying company verification with endpoint: ${attempt.endpoint} (${attempt.method})`);
        const response = await fetchPublic(attempt.endpoint, {
          method: attempt.method,
          body: JSON.stringify(attempt.body),
        });
        
        const data = await response.json();
        
        if (response.ok) {
          console.log("Company email verification successful");
          return { 
            isValid: true, 
            message: data.message || "Company email verified successfully",
            token: data.token || data.accessToken
          };
        }
        
        if (response.status === 405) {
          console.warn(`Method not allowed for ${attempt.endpoint}, trying next...`);
          continue;
        }
        
        lastError = new Error(data.message || data.error || `Verification failed with status ${response.status}`);
        console.warn(`Company verification attempt failed:`, lastError.message);
        
      } catch (error) {
        console.warn(`Failed with endpoint ${attempt.endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    try {
      console.log("Falling back to auth-users endpoint for company verification");
      const response = await fetchPublic("auth-users/email-verification", {
        method: "POST",
        body: JSON.stringify({ Email: email, Token: code }),
      });
      
      const data = await response.json();
      
      if (response.ok) {
        console.log("Company email verification successful via fallback");
        return { 
          isValid: true, 
          message: data.message || "Company email verified successfully",
          token: data.token
        };
      }
    } catch (error) {
      console.warn("Fallback verification failed:", error);
    }
    
    if (lastError) {
      throw lastError;
    }
    
    throw new Error("Company verification failed. Please try again.");
    
  } catch (error) {
    console.error("Company email verification failed:", error);
    throw error;
  }
}

/**
 * Resend company verification code
 */
export async function resendCompanyVerificationCode(email: string): Promise<{ message: string }> {
  return sendCompanyVerificationCode(email);
}

/**
 * Check if company email is verified
 */
export async function checkCompanyEmailVerification(email: string): Promise<{ isVerified: boolean; company?: any }> {
  try {
    const endpoints = [ 
      `admin-controller/check-verification?email=${encodeURIComponent(email)}`,
      `admin-controller/company-verification-status?email=${encodeURIComponent(email)}`,
      `admin-controller/verify-status?email=${encodeURIComponent(email)}`,
    ];
    
    for (const endpoint of endpoints) {
      try {
        const response = await fetchPublic(endpoint, {
          method: "GET",
        });
        
        if (response.ok) {
          const data = await response.json();
          return { 
            isVerified: data.isVerified || data.emailVerified || data.verified || false,
            company: data.company || data
          };
        }
      } catch (e) {
        continue;
      }
    }
    
    return { isVerified: false };
  } catch (error) {
    console.warn("Failed to check company email verification:", error);
    return { isVerified: false };
  }
}

// ============ FORGOT PASSWORD FUNCTIONS ============

export async function forgotPassword(email: string): Promise<string> {
  const response = await fetchPublic(
    "auth-users/forgot-password-request-token",
    {
      method: "POST",
      body: JSON.stringify({ Email: email }),
    },
  );
  return response.json();
}

export async function resetPassword(
  data: ResetPasswordRequest,
): Promise<string> {
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
  const response = await fetchWithAuth(`auth-users/two-factor-enabled/${id}`, {
    method: "POST",
  });
  return response.json();
}

// ============ COMPANY FUNCTIONS ============

export async function getCompanies(): Promise<Company[]> {
  try {
    const response = await fetchWithAuth("logistics-controller", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch companies:", error);
    return [];
  }
}

export async function getCompaniesPaginated(
  params: GetCompaniesParams = {},
): Promise<PaginatedCompaniesResponse> {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.append(key, String(value));
    }
  });
  try {
    const response = await fetchWithAuth(
      `logistics-controller?${query.toString()}`,
      {
        method: "GET",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch paginated companies:", error);
    return {
      data: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0,
      hasPrevious: false,
      hasNext: false,
    };
  }
}

export async function getCompanyById(id: string): Promise<Company> {
  try {
    const response = await fetchWithAuth(`logistics-controller/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch company by ID:", error);
    throw error;
  }
}

// FIXED: Enhanced addCompany with validation and better error handling
export async function addCompany(companyData: {
  companyName: string;
  companyEmail: string;
  phoneNumber: string;
  companyAddress?: string;
  state?: string;
  locality?: string;
  valueCharge: number;
  companyInfo?: string;
  noOfTrucks?: number;
  nofOfBikes?: number;
  bankName?: string;
  accountName?: string;
  accountNumber?: string;
  deliveryTypes?: string[];
  serviceAreas?: string[];
  companyStatus?: string;
  adminId?: string;
}): Promise<Company> {
  try {
    console.log("Adding company with data:", JSON.stringify(companyData, null, 2));
    
    const requiredFields = ['companyName', 'companyEmail', 'phoneNumber', 'valueCharge'];
    const missingFields = requiredFields.filter(field => !companyData[field as keyof typeof companyData]);
    
    if (missingFields.length > 0) {
      throw new Error(`Missing required fields: ${missingFields.join(', ')}`);
    }
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(companyData.companyEmail)) {
      throw new Error('Invalid email format');
    }
    
    if (!companyData.phoneNumber || companyData.phoneNumber.length < 10) {
      throw new Error('Invalid phone number');
    }
    
    const user = getStoredUser();
    console.log("Current user:", user?.userType || "Unknown");
    
    const payload = {
      companyName: companyData.companyName.trim(),
      companyEmail: companyData.companyEmail.trim().toLowerCase(),
      phoneNumber: companyData.phoneNumber.trim(),
      companyAddress: companyData.companyAddress?.trim() || "",
      state: companyData.state?.trim() || "",
      locality: companyData.locality?.trim() || "",
      valueCharge: Number(companyData.valueCharge) || 0,
      companyInfo: companyData.companyInfo?.trim() || "",
      noOfTrucks: Number(companyData.noOfTrucks) || 0,
      nofOfBikes: Number(companyData.nofOfBikes) || 0,
      bankName: companyData.bankName?.trim() || "",
      accountName: companyData.accountName?.trim() || "",
      accountNumber: companyData.accountNumber?.trim() || "",
      deliveryTypes: Array.isArray(companyData.deliveryTypes) 
        ? companyData.deliveryTypes 
        : (companyData.deliveryTypes ? [companyData.deliveryTypes] : []),
      serviceAreas: Array.isArray(companyData.serviceAreas)
        ? companyData.serviceAreas
        : (companyData.serviceAreas ? [companyData.serviceAreas] : []),
      companyStatus: companyData.companyStatus || "pending",
      adminId: companyData.adminId || user?.userId || "",
    };
    
    console.log("Sending payload:", JSON.stringify(payload, null, 2));
    
    const response = await fetchWithAuth("logistics-controller", {
      method: "POST",
      body: JSON.stringify(payload),
    });
    
    const result = await response.json();
    console.log("Company added successfully:", result);
    return result;
  } catch (error) {
    console.error("Failed to add company:", error);
    if (error instanceof Error) {
      throw new Error(`Failed to add company: ${error.message}`);
    }
    throw error;
  }
}

export async function updateCompany(
  id: string,
  companyData: any,
): Promise<Company> {
  try {
    const response = await fetchWithAuth(`logistics-controller/${id}`, {
      method: "PUT",
      body: JSON.stringify(companyData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update company:", error);
    throw error;
  }
}

export async function deleteCompany(id: string): Promise<void> {
  try {
    await fetchWithAuth(`logistics-controller/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete company:", error);
    throw error;
  }
}

export async function updateCompanyReview(
  id: string,
  data: { rating: number; review: string },
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/update-logistic-company-review/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update company review:", error);
    throw error;
  }
}

// ============ COMPANY MANAGEMENT FUNCTIONS ============

/**
 * Update company status (active/pending/suspended)
 */
export async function updateCompanyStatus(
  id: string,
  data: { CompanyStatus: string }
): Promise<any> {
  try {
    const response = await fetchWithAuth(`logistics-controller/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update company status:", error);
    throw error;
  }
}

/**
 * Get company drivers
 */
export async function getCompanyDrivers(companyId: string): Promise<any[]> {
  try {
    const endpoints = [
      `logistics-controller/${companyId}/drivers`,
      `admin-controller/companies/${companyId}/drivers`,
      `companies/${companyId}/drivers`,
    ];
    
    for (const endpoint of endpoints) {
      try {
        const response = await fetchWithAuth(endpoint, {
          method: "GET",
        });
        if (response.ok) {
          const data = await response.json();
          return extractArrayFromResponse(data);
        }
      } catch (e) {
        continue;
      }
    }
    
    return [];
  } catch (error) {
    console.warn("Failed to fetch company drivers:", error);
    return [];
  }
}

/**
 * Get company statistics
 */
export async function getCompanyStats(companyId?: string): Promise<{
  totalOrders: number;
  totalRevenue: number;
  activeDrivers: number;
  pendingBookings: number;
  totalDrivers: number;
}> {
  try {
    if (!companyId) {
      return { totalOrders: 0, totalRevenue: 0, activeDrivers: 0, pendingBookings: 0, totalDrivers: 0 };
    }
    
    const endpoints = [
      `logistics-controller/${companyId}/stats`,
      `admin-controller/companies/${companyId}/stats`,
      `companies/${companyId}/stats`,
    ];
    
    for (const endpoint of endpoints) {
      try {
        const response = await fetchWithAuth(endpoint, {
          method: "GET",
        });
        if (response.ok) {
          return response.json();
        }
      } catch (e) {
        continue;
      }
    }
    
    return { totalOrders: 0, totalRevenue: 0, activeDrivers: 0, pendingBookings: 0, totalDrivers: 0 };
  } catch (error) {
    console.warn("Failed to fetch company stats:", error);
    return { totalOrders: 0, totalRevenue: 0, activeDrivers: 0, pendingBookings: 0, totalDrivers: 0 };
  }
}

// ============ BRAND OWNER & STAFF REGISTRATION FUNCTIONS ============

/**
 * Register a Brand Owner (creates both user and company)
 * This is the correct endpoint for adding companies
 */
function mapBrandOwnerRequestToApi(data: RegisterBrandOwnerRequest) {
  return {
    StaffType: data.staffType,
    FirstName: data.firstName,
    SurName: data.surName,
    Email: data.email,
    Password: data.password,
    Sex: data.sex,
    StaffCode: data.staffCode,
    PhoneNo: data.phoneNo,
    Address: data.address,
    CompanyName: data.companyName,
    CompanyUserName: data.companyUserName,
    CompanyType: data.companyType,
    Roles: data.roles,
    Permissions: data.permissions,
    State: data.state,
    Branch: data.branch,
    Locality: data.locality,
  };
}

export async function registerBrandOwner(
  data: RegisterBrandOwnerRequest
): Promise<any> {
  try {
    console.log("Registering Brand Owner with data:", JSON.stringify(data, null, 2));
    
    // Validate required fields
    const requiredFields = ['firstName', 'surName', 'email', 'password', 'phoneNo', 'companyName'];
    const missingFields = requiredFields.filter(field => !data[field as keyof typeof data]);
    
    if (missingFields.length > 0) {
      throw new Error(`Missing required fields: ${missingFields.join(', ')}`);
    }
    
    // Validate email format
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(data.email)) {
      throw new Error('Invalid email format');
    }
    
    // Normalize and validate phone number
    const normalizedPhone = data.phoneNo?.replace(/\D/g, "") || "";
    if (!normalizedPhone || normalizedPhone.length < 10) {
      throw new Error("Invalid phone number. Please use at least 10 digits.");
    }
    data.phoneNo = normalizedPhone;
    
    // Validate password strength
    if (data.password.length < 8) {
      throw new Error('Password must be at least 8 characters long');
    }
    
    // Validate company type
    if (!data.companyType || data.companyType.length === 0) {
      throw new Error('Please select at least one company type');
    }

    const apiPayload = mapBrandOwnerRequestToApi(data);
    const endpoints = [
      "admin-controller/add-company-owner",
      "admin-controller/register",
      "admin-controller",
    ];

    let lastError: Error | null = null;
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying Brand Owner registration endpoint: ${endpoint}`);
        // Use fetchPublic instead of fetchWithAuth
        const response = await fetchPublic(endpoint, {
          method: "POST",
          body: JSON.stringify(apiPayload),
        });
        const result = await response.json();
        console.log(`Brand Owner registered successfully via ${endpoint}:`, result);
        
        // If the response contains authentication data, store it
        if (result.token) {
          setAuthData(result);
        }
        
        return result;
      } catch (error) {
        console.warn(`Failed Brand Owner registration with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
        if (error instanceof Error) {
          const message = error.message.toLowerCase();
          if (
            !message.includes("404") &&
            !message.includes("401") &&
            !message.includes("405") &&
            !message.includes("not found") &&
            !message.includes("unsupported")
          ) {
            throw error;
          }
        }
      }
    }

    console.error("All Brand Owner registration endpoints failed");
    if (lastError) {
      throw lastError;
    }
    throw new Error("Failed to register brand owner. Please contact support.");
  } catch (error) {
    console.error("Failed to register brand owner:", error);
    if (error instanceof Error) {
      throw new Error(`Failed to register brand owner: ${error.message}`);
    }
    throw error;
  }
}

/**
 * Register a Staff member for a company
 */
export async function registerStaff(
  data: RegisterStaffRequest
): Promise<any> {
  try {
    console.log("Registering Staff with data:", JSON.stringify(data, null, 2));
    
    const requiredFields = ['firstName', 'surName', 'email', 'password', 'phoneNo', 'companyId'];
    const missingFields = requiredFields.filter(field => !data[field as keyof typeof data]);
    
    if (missingFields.length > 0) {
      throw new Error(`Missing required fields: ${missingFields.join(', ')}`);
    }
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(data.email)) {
      throw new Error('Invalid email format');
    }
    
    if (!data.phoneNo || data.phoneNo.length < 10) {
      throw new Error('Invalid phone number');
    }
    
    if (data.password.length < 8) {
      throw new Error('Password must be at least 8 characters long');
    }
    
    const response = await fetchWithAuth("admin-controller", {
      method: "POST",
      body: JSON.stringify(data),
    });
    
    const result = await response.json();
    console.log("Staff registered successfully:", result);
    return result;
  } catch (error) {
    console.error("Failed to register staff:", error);
    if (error instanceof Error) {
      throw new Error(`Failed to register staff: ${error.message}`);
    }
    throw error;
  }
}

// ============ COMPANY APPLY FUNCTIONS ============

export async function applyCompany(applicationData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("admin-controller/company-apply", {
      method: "POST",
      body: JSON.stringify(applicationData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to submit company application:", error);
    throw error;
  }
}

export async function getCompanyApplications(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("admin-controller/company-apply", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch company applications:", error);
    return [];
  }
}

export async function getCompanyApplicationById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/company-apply/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch company application:", error);
    throw error;
  }
}

export async function updateCompanyApplication(
  id: string,
  data: any,
): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/company-apply/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update company application:", error);
    throw error;
  }
}

/**
 * Register a company (or brand owner) then submit an application to admin for approval.
 */
export async function registerCompanyAndSubmitForApproval(params: {
  companyData: {
    companyName: string;
    companyEmail: string;
    phoneNumber: string;
    valueCharge: number;
    companyAddress?: string;
    state?: string;
    locality?: string;
    companyInfo?: string;
    noOfTrucks?: number;
    nofOfBikes?: number;
    bankName?: string;
    accountName?: string;
    accountNumber?: string;
    deliveryTypes?: string[];
    serviceAreas?: string[];
    adminId?: string;
  };
  ownerData?: RegisterBrandOwnerRequest;
  submitApplication?: boolean;
}): Promise<{ company?: Company; registrationResult?: any; applicationResult?: any }> {
  const { companyData, ownerData, submitApplication = true } = params;

  try {
    console.log("Starting company registration + submit for approval", {
      companyName: companyData.companyName,
      hasOwnerData: !!ownerData,
      isAuthenticated: isAuthenticated(),
    });

    let company: Company | undefined;
    let registrationResult: any = null;

    if (ownerData && !isAuthenticated()) {
      try {
        console.log("Registering brand owner (will create user/company on backend)");
        registrationResult = await registerBrandOwner(ownerData);
        if (registrationResult && registrationResult.company) {
          company = registrationResult.company as Company;
        } else if (registrationResult && registrationResult.id) {
          // some endpoints return id for company/user
        }
      } catch (err) {
        console.warn("Brand owner registration failed, will fallback to authenticated creation if possible:", err);
      }
    }

    if (!company && isAuthenticated()) {
      try {
        company = await addCompany(companyData as any);
        registrationResult = company;
      } catch (err) {
        console.warn("Failed to add company as authenticated user:", err);
        throw err;
      }
    }

    if (!company && registrationResult && typeof registrationResult === "object") {
      if ((registrationResult as any).companyId || (registrationResult as any).id) {
        company = {
          id: (registrationResult as any).companyId || (registrationResult as any).id,
          adminId: (registrationResult as any).adminId || "",
          companyEmail: companyData.companyEmail,
          companyName: companyData.companyName,
          phoneNumber: companyData.phoneNumber,
          companyLogo: "",
          companyRegNo: "",
          companyInfo: companyData.companyInfo || "",
          rating: 0,
          valueCharge: companyData.valueCharge || 0,
          noOfTrucks: companyData.noOfTrucks || 0,
          nofOfBikes: companyData.nofOfBikes || 0,
          available: true,
          companyAddress: companyData.companyAddress || "",
          postCodes: "",
          locality: companyData.locality || "",
          state: companyData.state || "",
          latitude: 0,
          longitude: 0,
          bankName: companyData.bankName || "",
          accountName: companyData.accountName || "",
          accountNumber: companyData.accountNumber || "",
          deliveryTypes: companyData.deliveryTypes || [],
          serviceAreas: companyData.serviceAreas || [],
          companyReviewModels: [],
          companyStatus: (registrationResult as any).companyStatus || "pending",
          createdAt: new Date().toISOString(),
        } as Company;
      }
    }

    let applicationResult: any = null;
    if (submitApplication && company) {
      try {
        const applicationPayload = {
          companyId: company.id,
          companyName: company.companyName,
          companyEmail: company.companyEmail,
          phoneNumber: company.phoneNumber,
          companyAddress: company.companyAddress || companyData.companyAddress || "",
          state: company.state || companyData.state || "",
          locality: company.locality || companyData.locality || "",
          companyInfo: company.companyInfo || companyData.companyInfo || "",
          adminId: company.adminId || companyData.adminId || (getStoredUser() as any)?.userId || "",
          status: "pending",
        };

        console.log("Submitting company application to admin:", applicationPayload);
        applicationResult = await applyCompany(applicationPayload);
        console.log("Company application submitted:", applicationResult);
      } catch (err) {
        console.warn("Failed to submit company application:", err);
        throw err;
      }
    }

    return { company, registrationResult, applicationResult };
  } catch (error) {
    console.error("registerCompanyAndSubmitForApproval failed:", error);
    throw error;
  }
}

export async function deleteCompanyApplication(id: string): Promise<void> {
  try {
    await fetchWithAuth(`admin-controller/company-apply/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete company application:", error);
    throw error;
  }
}

// ============ RIDER/DRIVER FUNCTIONS ============

export async function getAllRiders(): Promise<Driver[]> {
  try {
    const response = await fetchWithAuth("logistics-controller/rider", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch riders:", error);
    return [];
  }
}

export async function getRiderById(id: string): Promise<Driver> {
  try {
    const response = await fetchWithAuth(`logistics-controller/rider/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch rider by ID:", error);
    throw error;
  }
}

// Alias for getRiders (backward compatibility)
export const getRiders = getAllRiders;

// FIXED: Enhanced addRider with better error handling and logging
export async function addRider(riderData: AddDriverRequest): Promise<Driver> {
  try {
    console.log("Adding rider with data:", JSON.stringify(riderData, null, 2));
    console.log("Current token present:", !!getToken());
    console.log("Current user:", getStoredUser()?.userType || "Unknown");

    const companyId =
      riderData.companyId ||
      (getStoredUser() as any)?.companyId ||
      (getStoredUser() as any)?.CompanyId ||
      (getStoredUser() as any)?.CompanyID;

    const endpoints = [
      "logistics-controller/rider",
      "admin-controller/rider",
    ];

    if (companyId) {
      endpoints.push(
        `admin-controller/companies/${companyId}/drivers`,
        `companies/${companyId}/drivers`,
      );
    }

    let lastError: Error | null = null;
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying rider add endpoint: ${endpoint}`);
        const response = await fetchWithAuth(endpoint, {
          method: "POST",
          body: JSON.stringify(riderData),
        });

        const result = await response.json();
        console.log(`Rider added successfully via ${endpoint}:`, result);
        return result;
      } catch (error) {
        console.warn(`Failed to add rider with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
        if (error instanceof Error) {
          const message = error.message.toLowerCase();
          if (
            !message.includes("404") &&
            !message.includes("401") &&
            !message.includes("403") &&
            !message.includes("405") &&
            !message.includes("not found") &&
            !message.includes("forbidden") &&
            !message.includes("permission")
          ) {
            throw error;
          }
        }
      }
    }

    console.error("All rider add endpoints failed");
    if (lastError) {
      throw lastError;
    }
    throw new Error("Failed to add rider. Please contact support.");
  } catch (error) {
    console.error("Failed to add rider:", error);
    throw error;
  }
}

export async function updateRider(
  id: string,
  riderData: any,
): Promise<Driver> {
  try {
    const response = await fetchWithAuth(`logistics-controller/rider/${id}`, {
      method: "PUT",
      body: JSON.stringify(riderData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update rider:", error);
    throw error;
  }
}

export async function deleteRider(id: string): Promise<void> {
  try {
    await fetchWithAuth(`logistics-controller/rider/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete rider:", error);
    throw error;
  }
}

export async function updateRiderReview(
  id: string,
  data: { rating: number; review: string },
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/update-riders-review/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update rider review:", error);
    throw error;
  }
}

export async function getRiderTypes(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("logistics-controller/rider-type", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch rider types:", error);
    return [];
  }
}

// ============ PACKAGE ORDER FUNCTIONS ============

export async function getPackageOrders(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("logistics-controller/package-orders", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch package orders:", error);
    return [];
  }
}

export async function getPackageOrderById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/package-orders/${id}`,
      { method: "GET" },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch package order:", error);
    throw error;
  }
}

export async function addPackageOrder(orderData: AddOrder): Promise<any> {
  try {
    const response = await fetchWithAuth("logistics-controller/package-orders", {
      method: "POST",
      body: JSON.stringify(orderData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add package order:", error);
    throw error;
  }
}

export async function updatePackageOrder(id: string, data: any): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/package-orders/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update package order:", error);
    throw error;
  }
}

export async function deletePackageOrder(id: string): Promise<void> {
  try {
    await fetchWithAuth(`logistics-controller/package-orders/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete package order:", error);
    throw error;
  }
}

export async function trackPackageOrder(
  trackingNumber: string,
): Promise<OrderTrackingResult> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/track-order?trackingNum=${encodeURIComponent(trackingNumber)}`,
      {
        method: "GET",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to track package order:", error);
    throw error;
  }
}

export async function assignOrderToRider(
  orderId: string,
  riderId: string,
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/package-orders-rider/${orderId}`,
      {
        method: "PUT",
        body: JSON.stringify({ riderId }),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to assign order to rider:", error);
    throw error;
  }
}

export async function initializePackagePaystackPayment(
  orderId: string,
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/initialize-package-orders/${orderId}`,
      {
        method: "PUT",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Paystack payment:", error);
    throw error;
  }
}

export async function initializePackageFlutterwavePayment(
  orderId: string,
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `logistics-controller/initialize-flutterwave-package-orders/${orderId}`,
      {
        method: "PUT",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Flutterwave payment:", error);
    throw error;
  }
}

// ============ ACCOMMODATION FUNCTIONS ============

export async function getAccommodations(): Promise<Accommodation[]> {
  try {
    const response = await fetchWithAuth("bookings/accomodation", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch accommodations:", error);
    return [];
  }
}

export async function getAccommodationById(id: string): Promise<Accommodation> {
  try {
    const response = await fetchWithAuth(`bookings/accomodation/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch accommodation by ID:", error);
    throw error;
  }
}

export async function addAccommodation(accommodationData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("bookings/accomodation", {
      method: "POST",
      body: JSON.stringify(accommodationData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add accommodation:", error);
    throw error;
  }
}

export async function updateAccommodation(
  id: string,
  data: any,
): Promise<any> {
  try {
    const response = await fetchWithAuth(`bookings/accomodation/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update accommodation:", error);
    throw error;
  }
}

export async function deleteAccommodation(id: string): Promise<void> {
  try {
    await fetchWithAuth(`bookings/accomodation/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete accommodation:", error);
    throw error;
  }
}

export async function updateAccommodationReview(
  id: string,
  data: { rating: number; review: string },
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `bookings/update-accomodation-review/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update accommodation review:", error);
    throw error;
  }
}

// ============ ACCOMMODATION RESERVATIONS ============

export async function getAccommodationReservations(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("bookings/accomodation-reservations", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch accommodation reservations:", error);
    return [];
  }
}

export async function getAccommodationReservationById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `bookings/accomodation-reservations/${id}`,
      { method: "GET" },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch accommodation reservation:", error);
    throw error;
  }
}

export async function addAccommodationReservation(data: any): Promise<any> {
  try {
    const response = await fetchWithAuth("bookings/accomodation-reservations", {
      method: "POST",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add accommodation reservation:", error);
    throw error;
  }
}

export async function updateAccommodationReservation(
  id: string,
  data: any,
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `bookings/accomodation-reservations/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update accommodation reservation:", error);
    throw error;
  }
}

export async function deleteAccommodationReservation(id: string): Promise<void> {
  try {
    await fetchWithAuth(`bookings/accomodation-reservations/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete accommodation reservation:", error);
    throw error;
  }
}

// Helper to fetch rooms for a specific accommodation (backwards-compatible API)
export async function getRooms(accommodationId: string): Promise<any[]> {
  try {
    const response = await fetchWithAuth(
      `bookings/accomodation/${accommodationId}/rooms`,
      { method: "GET" },
    );
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch rooms:", error);
    return [];
  }
}

// Backwards-compatible booking helper used by UI pages
export async function bookAccommodation(data: AddCustomerBookedReservation): Promise<any> {
  try {
    const response = await fetchWithAuth("bookings/accomodation-reservations-customer", {
      method: "POST",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to book accommodation:", error);
    throw error;
  }
}

// ============ ACCOMMODATION CUSTOMERS ============

export async function getCustomerReservations(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("bookings/accomodation-reservations-customer", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch customer reservations:", error);
    return [];
  }
}

export async function getCustomerReservationById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `bookings/accomodation-reservations-customer/${id}`,
      { method: "GET" },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch customer reservation:", error);
    throw error;
  }
}

export async function addCustomerReservation(data: any): Promise<any> {
  try {
    const response = await fetchWithAuth("bookings/accomodation-reservations-customer", {
      method: "POST",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add customer reservation:", error);
    throw error;
  }
}

export async function updateCustomerReservation(
  id: string,
  data: any,
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `bookings/accomodation-reservations-customer/${id}`,
      {
        method: "PUT",
        body: JSON.stringify(data),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update customer reservation:", error);
    throw error;
  }
}

export async function deleteCustomerReservation(id: string): Promise<void> {
  try {
    await fetchWithAuth(`bookings/accomodation-reservations-customer/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete customer reservation:", error);
    throw error;
  }
}

// ============ PAYMENT FUNCTIONS ============

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

export async function initializePaystackPayment(
  payload: PaystackInitializeRequest
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      "bookings/initialize-accomodation-reservations-customer",
      {
        method: "POST",
        body: JSON.stringify(payload),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Paystack payment:", error);
    throw error;
  }
}

export async function verifyPaystackPayment(reference: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`Wallet/verify?reference=${reference}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to verify Paystack payment:", error);
    throw error;
  }
}

export async function initializeFlutterwavePayment(
  payload: FlutterwaveInitializeRequest
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      "bookings/initialize-flutterwave-accomodation-reservations-customer",
      {
        method: "POST",
        body: JSON.stringify(payload),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Flutterwave payment:", error);
    throw error;
  }
}

export async function verifyFlutterwavePayment(reference: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `Wallet/verify-flutterwave?reference=${encodeURIComponent(reference)}`,
      { method: "GET" },
    );
    return response.json();
  } catch (err) {
    const fallback = await fetchWithAuth(
      `Wallet/verify?reference=${encodeURIComponent(reference)}`,
      { method: "GET" },
    );
    return fallback.json();
  }
}

export async function trackBooking(
  ticketRef: string,
): Promise<BookingTrackingResult> {
  try {
    const response = await fetchWithAuth(
      `bookings/track-booking?ticketRef=${encodeURIComponent(ticketRef)}`,
      {
        method: "GET",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to track booking:", error);
    throw error;
  }
}

// Unified tracker: try order tracking first, then booking tracking
export async function trackParcelOrBooking(id: string): Promise<{
  type: "order" | "booking";
  data: OrderTrackingResult | BookingTrackingResult;
}> {
  try {
    const order = await trackPackageOrder(id);
    return { type: "order", data: order };
  } catch (orderErr) {
    try {
      const booking = await trackBooking(id);
      return { type: "booking", data: booking };
    } catch (bookingErr) {
      const msg = (orderErr as Error)?.message || (bookingErr as Error)?.message || "Tracking failed";
      throw new Error(msg);
    }
  }
}

export async function getReservationDates(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("bookings/all-reservations-dates", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch reservation dates:", error);
    return [];
  }
}

// ============ ALIASES FOR BACKWARD COMPATIBILITY ============

// Reservation aliases
export const getAllReservations = getAccommodationReservations;
export const getReservationById = getAccommodationReservationById;
export const addReservation = addAccommodationReservation;
export const updateReservation = updateAccommodationReservation;
export const deleteReservation = deleteAccommodationReservation;

// Customer reservation aliases
export const getAllCustomerReservations = getCustomerReservations;
export const getCustomerBookings = getCustomerReservations;
export const cancelCustomerBooking = deleteCustomerReservation;

// Orders / package aliases
export const getCustomerOrders = getPackageOrders;

// Payment aliases
export const initializeCustomerPaystackPayment = initializePaystackPayment;
export const initializeCustomerFlutterwavePayment = initializeFlutterwavePayment;

// Driver aliases (using rider endpoints)
export const getAllDrivers = getAllRiders;
export const getDriverById = getRiderById;
export const addDriver = addRider;
export const updateDriver = updateRider;
export const deleteDriver = deleteRider;
export const getAvailableDrivers = async (): Promise<Driver[]> => {
  const riders = await getAllRiders();
  return riders.filter((r) => r.status === "Available");
};
export const updateDriverStatus = updateRider;
export const getDriverStats = async () => {
  const riders = await getAllRiders();
  return {
    total: riders.length,
    available: riders.filter((r) => r.status === "Available").length,
    onDelivery: riders.filter((r) => r.status === "On Delivery").length,
    offDuty: riders.filter((r) => r.status === "Off Duty").length,
  };
};
export const assignDriverToOrder = assignOrderToRider;
export const getDriverTracking = async (id: string) => {
  const rider = await getRiderById(id);
  return {
    latitude: rider.latitude || 0,
    longitude: rider.longitude || 0,
    status: rider.status || "Unknown",
    lastUpdated: rider.updatedAt || new Date().toISOString(),
  };
};

// Order aliases
export const getAllOrders = getPackageOrders;
export const getOrderById = getPackageOrderById;
export const createOrder = addPackageOrder;
export const updateOrderStatus = updatePackageOrder;
export const deleteOrder = deletePackageOrder;
export const trackOrder = trackPackageOrder;

// Notification aliases
export const getNotifications = async (): Promise<Notification[]> => {
  try {
    const response = await fetchWithAuth("notifications", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch notifications:", error);
    return [];
  }
};

export const getNotificationById = async (id: string): Promise<Notification> => {
  try {
    const response = await fetchWithAuth(`notifications/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch notification:", error);
    throw error;
  }
};

// In lib/api.ts - Update the markNotificationAsRead function

export async function markNotificationAsRead(id: string): Promise<any> {
  // Try both possible endpoints
  const endpoints = [
    `notifications/${id}`,
    `notifications/read/${id}`,
    `notifications/mark-read/${id}`,
  ];
  
  let lastError: Error | null = null;
  
  for (const endpoint of endpoints) {
    try {
      // Try with PATCH method first
      let response = await fetchWithAuth(endpoint, { 
        method: "PATCH",
        body: JSON.stringify({ isRead: true }),
      });
      
      if (response.ok) {
        return response.json();
      }
      
      // If PATCH fails, try PUT
      response = await fetchWithAuth(endpoint, {
        method: "PUT",
        body: JSON.stringify({ isRead: true }),
      });
      
      if (response.ok) {
        return response.json();
      }
    } catch (error) {
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }
  
  // Fallback: try the updateNotification function
  try {
    return await updateNotification(id, { isRead: true });
  } catch (error) {
    // If all fail, throw the last error
    throw lastError || new Error("Failed to mark notification as read");
  }
}

// ============ NOTIFICATION FUNCTIONS ============

export async function addNotification(notificationData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("notifications", {
      method: "POST",
      body: JSON.stringify(notificationData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add notification:", error);
    throw error;
  }
}

export async function sendNotification(notificationData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("notifications/send-notification", {
      method: "POST",
      body: JSON.stringify(notificationData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to send notification:", error);
    throw error;
  }
}

export async function updateNotification(
  id: string,
  data: any,
): Promise<any> {
  try {
    const response = await fetchWithAuth(`notifications/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update notification:", error);
    throw error;
  }
}

export async function deleteNotification(id: string): Promise<any> {
  const endpoints = [`notifications/${id}`, `notifications/delete/${id}`];

  let lastError: Error | null = null;

  for (const endpoint of endpoints) {
    try {
      const response = await fetchWithAuth(endpoint, { method: "DELETE" });
      if (response.ok) {
        return response.status === 204 ? { success: true } : response.json();
      }
    } catch (error) {
      lastError = error instanceof Error ? error : new Error(String(error));
    }
  }

  throw lastError || new Error("Failed to delete notification.");
}



// ============ INFO/FEEDBACK FUNCTIONS ============

export async function getFeedback(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("info-controller/feedback", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch feedback:", error);
    return [];
  }
}

export async function getFeedbackById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`info-controller/feedback/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch feedback:", error);
    throw error;
  }
}

export async function sendFeedback(feedbackData: AddFeedbackRequest): Promise<any> {
  try {
    const response = await fetchWithAuth("info-controller/feedback", {
      method: "POST",
      body: JSON.stringify(feedbackData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to send feedback:", error);
    throw error;
  }
}

export async function sendEmail(emailData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("info-controller/send-mail", {
      method: "POST",
      body: JSON.stringify(emailData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to send email:", error);
    throw error;
  }
}

export async function sendEmailToAll(emailData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("info-controller/send-all-mail", {
      method: "POST",
      body: JSON.stringify(emailData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to send email to all:", error);
    throw error;
  }
}

export const submitFeedback = sendFeedback;

// ============ WALLET/PAYOUT FUNCTIONS ============

export async function getPayouts(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("Wallet/payout-transfer", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch payouts:", error);
    return [];
  }
}

export async function getPayoutById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`Wallet/payout-transfer/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch payout:", error);
    throw error;
  }
}

export async function updatePayout(id: string, data: any): Promise<any> {
  try {
    const response = await fetchWithAuth(`Wallet/payout-transfer/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update payout:", error);
    throw error;
  }
}

export async function deletePayout(id: string): Promise<void> {
  try {
    await fetchWithAuth(`Wallet/payout-transfer/${id}`, {
      method: "DELETE",
    });
  } catch (error) {
    console.warn("Failed to delete payout:", error);
    throw error;
  }
}

export async function addRiderPayout(payoutData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("Wallet/payout-rider-transfer", {
      method: "POST",
      body: JSON.stringify(payoutData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add rider payout:", error);
    throw error;
  }
}

export async function addGasStationPayout(payoutData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("Wallet/payout-gas-station-transfer", {
      method: "POST",
      body: JSON.stringify(payoutData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add gas station payout:", error);
    throw error;
  }
}

export async function filterPayouts(params: {
  State?: string;
  City?: string;
  AnyItem?: string;
}): Promise<any[]> {
  try {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value) query.append(key, value);
    });
    const response = await fetchWithAuth(`Wallet/payout-filter?${query.toString()}`, {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to filter payouts:", error);
    return [];
  }
}

export async function getPayoutStatistics(): Promise<any> {
  try {
    const response = await fetchWithAuth("Wallet/payout-statistic", {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch payout statistics:", error);
    throw error;
  }
}

export const getPayoutStats = getPayoutStatistics;

// ============ UPLOAD FUNCTIONS ============

export async function uploadImage(file: File): Promise<any> {
  try {
    const formData = new FormData();
    formData.append("file", file);
    const response = await fetchWithAuth("upload-file/upload-image", {
      method: "POST",
      body: formData,
      headers: {} as any,
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to upload image:", error);
    throw error;
  }
}

export async function uploadImages(files: File[]): Promise<any> {
  try {
    const formData = new FormData();
    files.forEach((file) => {
      formData.append("files", file);
    });
    const response = await fetchWithAuth("upload-file/upload-image-list", {
      method: "POST",
      body: formData,
      headers: {} as any,
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to upload images:", error);
    throw error;
  }
}

export async function uploadToWebServer(file: File): Promise<any> {
  try {
    const formData = new FormData();
    formData.append("file", file);
    const response = await fetchWithAuth("upload-file/upload-web-server", {
      method: "POST",
      body: formData,
      headers: {} as any,
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to upload to web server:", error);
    throw error;
  }
}

// ============ UPDATED ADMIN FUNCTIONS ============

export async function adminLogin(
  credentials: LoginRequest,
): Promise<LoginResponse> {
  try {
    console.log("Admin login attempt with:", { email: credentials.Email_PhoneNo });
    
    const response = await fetchPublic("admin-controller/login", {
      method: "POST",
      body: JSON.stringify(credentials),
    });
    
    const data = await response.json();
    console.log("Admin login response received");
    
    // Normalize the response data
    const normalizedData = {
      ...data,
      userType: data.userType || data.adminType || "Admin",
      staffType: data.staffType || data.adminType || "Admin",
      userId: data.userId || data.id,
      token: data.token || data.accessToken,
      refreshToken: data.refreshToken || data.refresh_token,
    };
    
    setAuthData(normalizedData);
    return normalizedData;
  } catch (error) {
    console.error("Admin login error:", error);
    throw error;
  }
}

export async function adminGetProfile(): Promise<any> {
  try {
    const response = await fetchWithAuth("admin-controller/profile", {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch admin profile:", error);
    throw error;
  }
}

export async function adminForgotPassword(email: string): Promise<any> {
  try {
    const response = await fetchPublic(
      "admin-controller/forgot-password-request-token",
      {
        method: "POST",
        body: JSON.stringify({ Email: email }),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to send forgot password request:", error);
    throw error;
  }
}

export async function adminResetPassword(data: {
  Token: string;
  Password: string;
}): Promise<any> {
  try {
    const response = await fetchPublic("admin-controller/reset-password", {
      method: "POST",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to reset password:", error);
    throw error;
  }
}

export async function adminLogout(): Promise<void> {
  try {
    await fetchWithAuth("admin-controller/logout", {
      method: "POST",
    }).catch(() => {});
  } finally {
    clearAuthData();
  }
}

// ============ UPDATED SUPER ADMIN FUNCTIONS ============

export async function createAdmin(adminData: {
  adminType?: string;
  firstName: string;
  surName?: string;
  lastName?: string;
  email: string;
  password: string;
  sex?: string;
  staffCode?: string;
  phoneNo?: string;
  phoneNumber?: string;
  state?: string;
  locality?: string;
  address?: string;
  branch?: string;
  roles?: string[];
  permissions?: string[];
}): Promise<any> {
  try {
    console.log("Creating admin account with data:", JSON.stringify(adminData, null, 2));
    
    // Normalize field names
    const adminType = adminData.adminType || "Super_Admin";
    const firstName = adminData.firstName || "";
    const surName = adminData.surName || adminData.lastName || "";
    const email = adminData.email || "";
    const password = adminData.password || "";
    const sex = adminData.sex || "Male";
    const staffCode = adminData.staffCode || `ADMIN${Math.floor(1000 + Math.random() * 9000)}`;
    const phoneNo = adminData.phoneNo || adminData.phoneNumber || "";
    const state = adminData.state || "Lagos";
    const locality = adminData.locality || "Ikeja";
    const address = adminData.address || "N/A";
    const branch = adminData.branch || "Main";
    const roles = adminData.roles || ["Super_Admin"];
    
    const defaultPermissions = [
      "CanAccessAllData",
      "CanDeleteAllData",
      "CanAssignRoles",
      "CanViewAdmin",
      "CanDeleteAdmin",
      "CanUpdateAdmin",
      "CanCreateAdmin",
      "CanManageUsers",
      "CanDeleteUsers",
      "CanCreateStaff",
      "CanManageStaff",
      "CanViewStaff",
      "CanDeleteStaff",
      "CanViewBrands",
      "CanManageBrands",
      "CanDeleteBrands",
      "CanCreateProduct",
      "CanViewProduct",
      "CanManageProduct",
      "CanDeleteProduct",
      "CanViewWallet",
      "CanManageWallet",
      "CanViewOrders",
      "CanManageOrders",
      "CanDeleteOrders",
      "CanViewBookings",
      "CanManageBookings",
      "CanDeleteBookings"
    ];
    
    const permissions = adminData.permissions && adminData.permissions.length > 0 
      ? adminData.permissions 
      : defaultPermissions;
    
    // Validate required fields
    const requiredFields: string[] = [];
    if (!firstName) requiredFields.push('firstName');
    if (!surName) requiredFields.push('surName');
    if (!email) requiredFields.push('email');
    if (!password) requiredFields.push('password');
    if (!phoneNo) requiredFields.push('phoneNo');
    
    if (requiredFields.length > 0) {
      throw new Error(`Missing required fields: ${requiredFields.join(', ')}`);
    }
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      throw new Error('Invalid email format');
    }
    
    if (password.length < 8) {
      throw new Error('Password must be at least 8 characters long');
    }
    
    const payload = {
      adminType: adminType,
      firstName: firstName,
      surName: surName,
      email: email,
      password: password,
      sex: sex,
      staffCode: staffCode,
      phoneNo: phoneNo,
      state: state,
      locality: locality,
      address: address,
      branch: branch,
      roles: roles,
      permissions: permissions,
    };
    
    console.log("Sending admin payload:", JSON.stringify(payload, null, 2));
    
    // Try multiple endpoints for admin creation
    const endpoints = [
      "admin-controller",
      "admin-controller/admins",
      "admin-controller/create",
    ];
    
    let lastError: Error | null = null;
    
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying admin creation with endpoint: ${endpoint}`);
        const response = await fetchWithAuth(endpoint, {
          method: "POST",
          body: JSON.stringify(payload),
        });
        
        if (response.ok) {
          const result = await response.json();
          console.log(`Admin created successfully with endpoint: ${endpoint}`, result);
          return result;
        }
        
        // Handle validation errors
        if (response.status === 400) {
          const errorData = await response.clone().json();
          if (errorData.errors) {
            const errorMessages = Object.entries(errorData.errors)
              .map(([key, value]) => `${key}: ${Array.isArray(value) ? value.join(', ') : value}`)
              .join('; ');
            throw new Error(`Validation failed: ${errorMessages}`);
          }
          if (errorData.title) {
            throw new Error(errorData.title);
          }
        }
        
        lastError = new Error(`Failed with status ${response.status}`);
      } catch (error) {
        console.warn(`Failed with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
        // If it's a validation error, stop trying other endpoints
        if (error instanceof Error && error.message.includes('Validation failed')) {
          throw error;
        }
      }
    }
    
    throw lastError || new Error("Failed to create admin. Please try again.");
  } catch (error) {
    console.error("Failed to create admin:", error);
    throw error;
  }
}

export const addAdmin = createAdmin;

export async function getAllAdmins(params?: any): Promise<any[]> {
  try {
    let endpoint = "admin-controller";
    if (params) {
      const query = new URLSearchParams(params).toString();
      endpoint += `?${query}`;
    }
    
    const response = await fetchWithAuth(endpoint, { method: "GET" });
    const data = await response.json();
    const admins = extractArrayFromResponse(data);
    
    console.log(`Fetched ${admins.length} admins`);
    return admins;
  } catch (error) {
    console.error("Failed to fetch admins:", error);
    return [];
  }
}

export const getAdmins = getAllAdmins;

export async function getAdminById(id: string): Promise<any> {
  try {
    if (!id) {
      throw new Error("Admin ID is required");
    }
    
    const endpoints = [
      `admin-controller/profile/${id}`,
      `admin-controller/${id}`,
      `admin-controller/admins/${id}`,
    ];
    
    let lastError: Error | null = null;
    
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying admin fetch with endpoint: ${endpoint}`);
        const response = await fetchWithAuth(endpoint, { method: "GET" });
        
        if (response.ok) {
          const data = await response.json();
          console.log(`Admin found with endpoint: ${endpoint}`);
          return data;
        }
        
        if (response.status === 404) {
          console.warn(`Admin not found at ${endpoint}`);
          continue;
        }
      } catch (error) {
        console.warn(`Failed with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    throw lastError || new Error(`Admin with ID ${id} not found`);
  } catch (error) {
    console.error(`Failed to fetch admin ${id}:`, error);
    throw error;
  }
}

export async function updateAdmin(id: string, adminData: any): Promise<any> {
  try {
    const endpoints = [
      `admin-controller/profile/${id}`,
      `admin-controller/admins/${id}`,
      `admin-controller/admin/${id}`,
      `admin-controller/users/${id}`,
    ];
    
    let lastError: Error | null = null;
    
    for (const endpoint of endpoints) {
      try {
        const response = await fetchWithAuth(endpoint, {
          method: "PUT",
          body: JSON.stringify(adminData),
        });
        
        if (response.ok) {
          const data = await response.json();
          return data;
        }
      } catch (error) {
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    throw lastError || new Error(`Failed to update admin ${id}`);
    
  } catch (error) {
    console.error(`Failed to update admin ${id}:`, error);
    throw error;
  }
}

export async function deleteAdmin(id: string): Promise<void> {
  await fetchWithAuth(`admin-controller/${id}`, { method: "DELETE" });
}

export async function assignRolesToUser(
  userId: string,
  roles: string[],
  permissions?: string[]
): Promise<any> {
  try {
    if (!userId) throw new Error("User ID is required");
    if (!roles || roles.length === 0) throw new Error("At least one role is required");
    
    const payload: any = { roles };
    if (permissions) payload.permissions = permissions;
    
    const response = await fetchWithAuth(`admin-controller/user/${userId}/access`, {
      method: "POST",
      body: JSON.stringify(payload),
    });
    
    const result = await response.json();
    return result;
  } catch (error) {
    console.error("Failed to assign roles:", error);
    throw error;
  }
}

export const assignRoleToUser = assignRolesToUser;

export async function removeRolesFromUser(
  userId: string,
  roles: string[]
): Promise<any> {
  try {
    if (!userId) throw new Error("User ID is required");
    if (!roles || roles.length === 0) throw new Error("At least one role is required to remove");
    
    const payload = { roles };
    
    const response = await fetchWithAuth(`admin-controller/user/${userId}/roles`, {
      method: "DELETE",
      body: JSON.stringify(payload),
    });
    
    return response;
  } catch (error) {
    console.error("Failed to remove roles:", error);
    throw error;
  }
}

export async function getUserRolesAndPermissions(userId: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/user/${userId}/access`, {
      method: "GET",
    });
    
    const result = await response.json();
    return result;
  } catch (error) {
    console.error("Failed to get user roles:", error);
    throw error;
  }
}

// ============ UPDATED BRAND OWNER FUNCTIONS ============

export async function brandOwnerLogin(credentials: LoginRequest): Promise<LoginResponse> {
  try {
    console.log("Brand owner login attempt with:", { email: credentials.Email_PhoneNo });
    
    const endpoints = [
      "admin-controller/login-manager-staff",
      "admin-controller/brand-owner/login",
      "admin-controller/login",
    ];
    
    let lastError: Error | null = null;
    
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying brand owner login with endpoint: ${endpoint}`);
        const response = await fetchPublic(endpoint, {
          method: "POST",
          body: JSON.stringify(credentials),
        });
        
        if (!response.ok) {
          console.warn(`Endpoint ${endpoint} returned ${response.status}`);
          continue;
        }
        
        const data = await response.json();
        console.log(`Success with endpoint: ${endpoint}`);
        
        // Normalize user data
        const userData = data.user || data;
        const normalizedData = {
          ...data,
          ...userData,
          userType: userData.userType || userData.staffType || userData.role || "BrandOwner",
          staffType: userData.staffType || userData.userType || "BrandOwner",
          userId: userData.userId || userData.id,
          companyName: userData.companyName || userData.company?.name,
          companyId: userData.companyId || userData.company?.id,
          token: data.token || data.accessToken,
          refreshToken: data.refreshToken || data.refresh_token,
        };
        
        setAuthData(normalizedData);
        return normalizedData;
      } catch (error) {
        console.warn(`Failed with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    console.error("All brand owner login endpoints failed");
    throw lastError || new Error("Failed to login as brand owner. Please check your credentials.");
  } catch (error) {
    console.error("Brand owner login error:", error);
    throw error;
  }
}

export async function getBrandOwners(params?: any): Promise<any[]> {
  try {
    let endpoint = "admin-controller/company-manager-staff";
    if (params) {
      const query = new URLSearchParams(params).toString();
      endpoint += `?${query}`;
    }
    
    const response = await fetchWithAuth(endpoint, { method: "GET" });
    const data = await response.json();
    const owners = extractArrayFromResponse(data);
    
    console.log(`Fetched ${owners.length} brand owners`);
    return owners;
  } catch (error) {
    console.warn("Failed to fetch brand owners:", error);
    return [];
  }
}

export async function getBrandOwnerById(id: string): Promise<any> {
  try {
    console.log(`Fetching brand owner by ID: ${id}`);
    
    if (!id) {
      throw new Error("Brand owner ID is required");
    }
    
    const endpoints = [
      `admin-controller/company-manager-staff-profile/${id}`,
      `admin-controller/brand-owner/${id}`,
      `admin-controller/${id}`,
    ];
    
    let lastError: Error | null = null;
    
    for (const endpoint of endpoints) {
      try {
        console.log(`Trying brand owner fetch with endpoint: ${endpoint}`);
        const response = await fetchWithAuth(endpoint, { method: "GET" });
        
        if (response.ok) {
          const data = await response.json();
          console.log(`Brand owner found with endpoint: ${endpoint}`);
          return data;
        }
        
        if (response.status === 404) {
          console.warn(`Brand owner not found at ${endpoint}`);
          continue;
        }
      } catch (error) {
        console.warn(`Failed with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
      }
    }
    
    throw lastError || new Error(`Brand owner with ID ${id} not found`);
  } catch (error) {
    console.error(`Failed to fetch brand owner ${id}:`, error);
    throw error;
  }
}

/**
 * Login as a manager/staff user
 */
export async function loginManager(
  credentials: LoginRequest,
): Promise<LoginResponse> {
  try {
    const endpoints = [
      "admin-controller/login-manager-staff",
      "admin-controller/login-manager",
      "admin-controller/staff-login",
      "admin-controller/login",
    ];

    let lastError: Error | null = null;

    for (const endpoint of endpoints) {
      try {
        console.log(`Trying manager login with endpoint: ${endpoint}`);
        const response = await fetchPublic(endpoint, {
          method: "POST",
          body: JSON.stringify(credentials),
        });

        const data = await response.json();
        console.log(`Manager login successful with endpoint: ${endpoint}`);
        setAuthData(data);
        return data;
      } catch (error) {
        console.warn(`Failed with endpoint ${endpoint}:`, error);
        lastError = error instanceof Error ? error : new Error(String(error));
        if (error instanceof Error) {
          if (!error.message.includes("404") && !error.message.includes("405")) {
            throw error;
          }
        }
      }
    }

    throw lastError || new Error("Failed to login as manager. Please check your credentials.");
  } catch (error) {
    console.warn("Failed to login as manager:", error);
    throw error;
  }
}

// ============ STAFF MANAGEMENT FUNCTIONS ============

export async function getStaff(params?: any): Promise<any[]> {
  return getBrandOwners(params);
}

export async function getStaffById(id: string): Promise<any> {
  return getBrandOwnerById(id);
}

export async function addStaff(data: any): Promise<any> {
  try {
    const requiredFields = ['staffType', 'firstName', 'surName', 'email', 'password', 'phoneNo', 'companyName'];
    const missingFields = requiredFields.filter(field => !data[field as keyof typeof data]);
    
    if (missingFields.length > 0) {
      throw new Error(`Missing required fields: ${missingFields.join(', ')}`);
    }
    
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(data.email)) {
      throw new Error('Invalid email format');
    }
    
    const normalizedPhone = data.phoneNo?.replace(/\s/g, "") || "";
    if (!normalizedPhone || normalizedPhone.length < 10) {
      throw new Error("Invalid phone number. Please use at least 10 digits.");
    }
    data.phoneNo = normalizedPhone;
    
    if (data.password.length < 8) {
      throw new Error('Password must be at least 8 characters long');
    }
    
    const payload = {
      staffType: data.staffType,
      firstName: data.firstName.trim(),
      surName: data.surName.trim(),
      email: data.email.trim().toLowerCase(),
      password: data.password,
      sex: data.sex || "Male",
      staffCode: data.staffCode || `${data.firstName.slice(0, 3).toUpperCase()}${Math.floor(1000 + Math.random() * 9000)}`,
      phoneNo: data.phoneNo,
      address: data.address?.trim() || "N/A",
      companyName: data.companyName.trim(),
      companyUserName: data.companyUserName || data.companyName.toLowerCase().replace(/\s+/g, "_"),
      companyType: data.companyType || [],
      roles: data.roles || ["Staff"],
      permissions: data.permissions || [],
    };
    
    const response = await fetchWithAuth("admin-controller/add-staff-manager", {
      method: "POST",
      body: JSON.stringify(payload),
    });
    
    if (!response.ok) {
      const errorData = await response.json().catch(() => ({}));
      
      if (errorData.errors) {
        const errorMessages = Object.values(errorData.errors).flat().join(" ");
        throw new Error(errorMessages);
      }
      if (errorData.title) {
        throw new Error(errorData.title);
      }
      throw new Error(`Failed to add staff: ${response.status}`);
    }
    
    const result = await response.json();
    return result;
    
  } catch (error) {
    console.error("Failed to add staff:", error);
    if (error instanceof Error) {
      throw error;
    }
    throw new Error("Failed to add staff. Please try again.");
  }
}

export async function updateStaff(data: any): Promise<any> {
  const response = await fetchWithAuth("admin-controller/company-manager-staff/update", {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function deleteStaff(id: string): Promise<void> {
  await fetchWithAuth(`admin-controller/company-manager-staff/${id}`, { method: "DELETE" });
}

export async function getAllStaff(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("admin-controller", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch staff:", error);
    return [];
  }
}

export async function updateStaffStatus(
  id: string,
  data: { UserStatus: boolean; RejectionReason?: string },
): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update staff status:", error);
    throw error;
  }
}

export async function registerManager(
  data: RegisterManagerRequest,
): Promise<any> {
  if (!isAuthenticated()) {
    throw new Error(
      "You must be logged in as an admin to register a new manager. Please log in first.",
    );
  }

  const endpoints = [
    "admin-controller",
    "admin-controller/add-company-owner",
    "admin-controller/register",
    "admin-controller/add-manager",
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
        if (!error.message.includes("404") && !error.message.includes("401")) {
          throw error;
        }
      }
    }
  }

  console.error("All registration endpoints failed");
  if (lastError) {
    throw lastError;
  }
  throw new Error("Failed to register manager. Please contact support.");
}

// ============ ADVERT FUNCTIONS ============

export async function getAllAdverts(): Promise<any[]> {
  try {
    const response = await fetchWithAuth("admin-controller/advert", {
      method: "GET",
    });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch adverts:", error);
    return [];
  }
}

export const getAdverts = getAllAdverts;

export async function getAdvertById(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/advert/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch advert:", error);
    throw error;
  }
}

export async function addAdvert(advertData: any): Promise<any> {
  try {
    const response = await fetchWithAuth("admin-controller/advert", {
      method: "POST",
      body: JSON.stringify(advertData),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to add advert:", error);
    throw error;
  }
}

export async function updateAdvert(id: string, data: any): Promise<any> {
  try {
    const response = await fetchWithAuth(`admin-controller/advert/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update advert:", error);
    throw error;
  }
}

export async function deleteAdvert(id: string): Promise<void> {
  await fetchWithAuth(`admin-controller/advert/${id}`, { method: "DELETE" });
}

export async function initializeAdvertPaystackPayment(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `admin-controller/initialize-paystack-advert-payment/${id}`,
      {
        method: "PUT",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Paystack payment:", error);
    throw error;
  }
}

export async function initializeAdvertFlutterwavePayment(id: string): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `admin-controller/initialize-flutterwave-advert-payment/${id}`,
      {
        method: "PUT",
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to initialize Flutterwave payment:", error);
    throw error;
  }
}

// ============ ADMIN DATA FUNCTIONS ============

export async function getAllUsers(params?: any): Promise<any[]> {
  try {
    const response = await fetchWithAuth("auth-users", { method: "GET" });
    const data = await response.json();
    return extractArrayFromResponse(data);
  } catch (error) {
    console.warn("Failed to fetch users:", error);
    return [];
  }
}

export async function getUsersPaginated(
  params: GetUsersParams = {},
): Promise<PaginatedUsersResponse> {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.append(key, String(value));
    }
  });
  try {
    const response = await fetchWithAuth(`auth-users?${query.toString()}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch paginated users:", error);
    return {
      data: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0,
      hasPrevious: false,
      hasNext: false,
    };
  }
}

export async function getUserById(id: string): Promise<UserProfile> {
  try {
    const response = await fetchWithAuth(`auth-users/for-admin/${id}`, {
      method: "GET",
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to fetch user by ID:", error);
    throw error;
  }
}

export async function deleteUser(id: string): Promise<void> {
  try {
    await fetchWithAuth(`auth-users/${id}`, { method: "DELETE" });
  } catch (error) {
    console.warn("Failed to delete user:", error);
    throw error;
  }
}

// ============ USER MANAGEMENT FUNCTIONS ============

export async function updateUserStatus(
  id: string,
  data: { UserStatus: boolean },
): Promise<any> {
  try {
    const response = await fetchWithAuth(`auth-users/status/${id}`, {
      method: "PATCH",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update user status:", error);
    throw error;
  }
}

export async function updateUserVerification(
  id: string,
  data: { EmailVerified?: boolean; PhoneVerified?: boolean },
): Promise<any> {
  try {
    const response = await fetchWithAuth(`auth-users/verify/${id}`, {
      method: "PATCH",
      body: JSON.stringify(data),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to update user verification:", error);
    throw error;
  }
}

export async function updateUserMoneyBox(
  userId: string,
  amount: number,
  method: "adding" | "deducting",
): Promise<any> {
  try {
    const response = await fetchWithAuth(
      `auth-users/update-user-money-box?method=${method}`,
      {
        method: "PUT",
        body: JSON.stringify({ userId, amount }),
      },
    );
    return response.json();
  } catch (error) {
    console.warn("Failed to update user money box:", error);
    throw error;
  }
}

// ============ BULK OPERATIONS ============

export async function bulkUpdateUserStatus(
  userIds: string[],
  status: boolean,
): Promise<any> {
  try {
    const response = await fetchWithAuth("auth-users/bulk-status", {
      method: "PATCH",
      body: JSON.stringify({ userIds, UserStatus: status }),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to bulk update user status:", error);
    throw error;
  }
}

export async function bulkDeleteUsers(userIds: string[]): Promise<any> {
  try {
    const response = await fetchWithAuth("auth-users/bulk-delete", {
      method: "POST",
      body: JSON.stringify({ userIds }),
    });
    return response.json();
  } catch (error) {
    console.warn("Failed to bulk delete users:", error);
    throw error;
  }
}

// ============ ROLES FUNCTIONS ============

export async function getRoles(): Promise<any[]> {
  try {
    const endpoints = [
      "admin-controller/roles",
      "admin-controller/user-roles",
      "admin-controller/role",
    ];

    for (const endpoint of endpoints) {
      try {
        const response = await fetchWithAuth(endpoint, { method: "GET" });
        if (!response.ok) continue;
        const data = await response.json();
        const items = extractArrayFromResponse(data);
        if (items.length > 0 || endpoint === endpoints[endpoints.length - 1]) {
          return items;
        }
        return items;
      } catch {
        // try next endpoint
      }
    }
    return [];
  } catch {
    return [];
  }
}

export async function createRole(data: any): Promise<any> {
  const response = await fetchWithAuth("admin-controller/roles", {
    method: "POST",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function updateRole(id: string, data: any): Promise<any> {
  const response = await fetchWithAuth(`admin-controller/roles/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });
  return response.json();
}

export async function deleteRole(id: string): Promise<void> {
  await fetchWithAuth(`admin-controller/roles/${id}`, { method: "DELETE" });
}

// ============ EXPORT ALL FUNCTIONS ============

export { API_URL };