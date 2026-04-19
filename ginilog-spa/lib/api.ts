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
