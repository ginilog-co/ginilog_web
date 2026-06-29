// lib/firebase.ts

import { initializeApp, getApps, getApp } from "firebase/app";
import { 
  getAuth, 
  GoogleAuthProvider, 
  OAuthProvider,
  signInWithPopup,
  signInWithRedirect,
  getRedirectResult,
  signInWithEmailAndPassword,
  User
} from "firebase/auth";
import { setAuthData } from "@/lib/api";

// Firebase configuration
const firebaseConfig = {
  apiKey: process.env.NEXT_PUBLIC_FIREBASE_API_KEY,
  authDomain: process.env.NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN,
  projectId: process.env.NEXT_PUBLIC_FIREBASE_PROJECT_ID,
  storageBucket: process.env.NEXT_PUBLIC_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: process.env.NEXT_PUBLIC_FIREBASE_MESSAGING_SENDER_ID,
  appId: process.env.NEXT_PUBLIC_FIREBASE_APP_ID,
  measurementId: process.env.NEXT_PUBLIC_FIREBASE_MEASUREMENT_ID,
};

// Validate configuration
const requiredConfig = ['apiKey', 'authDomain', 'projectId', 'appId'];
const missingConfig = requiredConfig.filter(key => !firebaseConfig[key as keyof typeof firebaseConfig]);

if (missingConfig.length > 0) {
  console.error('❌ Missing Firebase config:', missingConfig);
  throw new Error(`Missing Firebase configuration: ${missingConfig.join(', ')}`);
}

// Initialize Firebase
let app;
try {
  if (!getApps().length) {
    app = initializeApp(firebaseConfig);
    console.log('✅ Firebase initialized successfully');
  } else {
    app = getApp();
    console.log('✅ Using existing Firebase instance');
  }
} catch (error) {
  console.error('❌ Failed to initialize Firebase:', error);
  throw error;
}

export const auth = getAuth(app);

// Providers
export const googleProvider = new GoogleAuthProvider();
googleProvider.setCustomParameters({
  prompt: 'select_account'
});

export const appleProvider = new OAuthProvider('apple.com');
appleProvider.setCustomParameters({
  locale: 'en'
});

// ============ GOOGLE SIGN-IN ============
export const signInWithGoogle = async () => {
  try {
    console.log('🔄 Starting Google sign-in...');
    const provider = new GoogleAuthProvider();
    const result = await signInWithPopup(auth, provider);
    const user = result.user;
    console.log('✅ Google sign-in successful:', {
      email: user.email,
      uid: user.uid,
      displayName: user.displayName,
    });

    
    // Get Firebase ID token
    // const idToken = await user.getIdToken();
    // console.log('✅ Got Firebase ID token (length:', idToken.length, ')');

    // Prepare payload - try multiple field name formats
    const payloads = [
      // Format 1: Your original format
      {
        email: user.email,
        idToken: user.uid,
        externalId: user.uid,
        firstName: user.displayName?.split(" ")[0] ?? "",
        lastName: user.displayName?.split(" ").slice(1).join(" ") ?? "",
        profilePicture: user.photoURL,
        phoneNo: user.phoneNumber ?? "",
      },
      // Format 2: With Email_PhoneNo
      {
        Email_PhoneNo: user.email,
        idToken: user.uid,
        externalId: user.uid,
        firstName: user.displayName?.split(" ")[0] ?? "",
        lastName: user.displayName?.split(" ").slice(1).join(" ") ?? "",
        profilePicture: user.photoURL,
        phoneNo: user.phoneNumber ?? "",
      },
      // Format 3: Minimal
      {
        Email: user.email,
        ExternalId: user.uid,
        idToken: user.uid,
      }
    ];

    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'https://api-data-connection.ginilog.org';
    
    // Try all endpoints with all payload formats
    const endpoints = [
      `${apiUrl}/api/auth-users/auth-login`,
      `${apiUrl}/api/auth-users/auth-login`,
      `${apiUrl}/api/auth-users/auth-login`,
    ];

    let lastError: Error | null = null;
    let responseData: any = null;

    for (const endpoint of endpoints) {
      for (const payload of payloads) {
        try {
          console.log(`📡 Trying: ${endpoint} with payload keys:`, Object.keys(payload));
          
          const response = await fetch(endpoint, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "Accept": "application/json",
            },
            body: JSON.stringify(payload),
          });

          const responseText = await response.text();
          console.log(`📥 Response from ${endpoint}:`, {
            status: response.status,
            bodyPreview: responseText.substring(0, 200),
          });

          if (response.ok) {
            try {
              responseData = JSON.parse(responseText);
              console.log(`✅ Success with endpoint: ${endpoint}`);
              console.log('✅ Response data:', responseData);
              break;
            } catch (e) {
              console.warn('Could not parse successful response:', e);
              continue;
            }
          } else if (response.status === 400) {
            // Try to parse validation error
            try {
              const errorData = JSON.parse(responseText);
              if (errorData.errors) {
                const errorMessages = Object.entries(errorData.errors)
                  .map(([key, value]) => `${key}: ${Array.isArray(value) ? value.join(', ') : value}`)
                  .join('; ');
                console.warn(`Validation error: ${errorMessages}`);
                // Continue to next payload if validation fails
                continue;
              }
            } catch (e) {
              // Ignore parsing error
            }
          } else if (response.status === 401) {
            console.warn(`❌ Authentication failed (401) - This is a backend issue.`);
            console.warn(`Response: ${responseText}`);
            // Don't continue trying - 401 means the token is invalid
            throw new Error(`Authentication failed: ${responseText}`);
          }
        } catch (error) {
          console.warn(`❌ Error with ${endpoint}:`, error);
          lastError = error instanceof Error ? error : new Error(String(error));
        }
      }
      if (responseData) break;
    }

    if (!responseData) {
      throw lastError || new Error('All endpoints and payload formats failed');
    }

    // Store the token
    if (responseData.token) {
      if (responseData.refreshToken) {
        setAuthData(responseData);
        console.log('✅ Auth data stored (LoginResponse format)');
      } else {
        localStorage.setItem("token", responseData.token);
        console.log('✅ Token stored successfully');
        
        if (responseData.user) {
          localStorage.setItem("user", JSON.stringify(responseData.user));
        }
      }
    } else {
      console.warn('⚠️ No token in response:', responseData);
      throw new Error('No authentication token received from server');
    }

    return responseData;
  } catch (error: any) {
    console.error("❌ Google sign-in error:", error);
    throw error;
  }
};

// ============ APPLE SIGN-IN ============
export const signInWithApple = async () => {
  try {
    console.log('🔄 Starting Apple sign-in...');
    const result = await signInWithPopup(auth, appleProvider);
    const user = result.user;
    console.log('✅ Apple sign-in successful:', {
      email: user.email,
      uid: user.uid,
    });

    // const idToken = await user.getIdToken();
    // console.log('✅ Got Firebase ID token (length:', idToken.length, ')');

    const payloads = [
      {
        email: user.email,
        idToken: user.uid,
        externalId: user.uid,
        firstName: user.displayName?.split(" ")[0] ?? "",
        lastName: user.displayName?.split(" ").slice(1).join(" ") ?? "",
        profilePicture: user.photoURL,
        phoneNo: user.phoneNumber ?? "",
      },
      {
        Email_PhoneNo: user.email,
        idToken: user.uid,
        externalId: user.uid,
        firstName: user.displayName?.split(" ")[0] ?? "",
        lastName: user.displayName?.split(" ").slice(1).join(" ") ?? "",
        profilePicture: user.photoURL,
        phoneNo: user.phoneNumber ?? "",
      },
      {
        Email: user.email,
        ExternalId: user.uid,
        idToken: user.uid,
      }
    ];

    const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'https://api-data-connection.ginilog.org';
    
    const endpoints = [
      `${apiUrl}/api/auth-users/auth-login`,
      `${apiUrl}/api/auth-users/auth-login`,
      `${apiUrl}/api/auth-users/auth-login`,
    ];

    let lastError: Error | null = null;
    let responseData: any = null;

    for (const endpoint of endpoints) {
      for (const payload of payloads) {
        try {
          console.log(`Trying: ${endpoint} with payload keys:`, Object.keys(payload));
          
          const response = await fetch(endpoint, {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              "Accept": "application/json",
            },
            body: JSON.stringify(payload),
          });

          const responseText = await response.text();
          console.log(`Response from ${endpoint}:`, {
            status: response.status,
            bodyPreview: responseText.substring(0, 200),
          });

          if (response.ok) {
            try {
              responseData = JSON.parse(responseText);
              console.log(`Success with endpoint: ${endpoint}`);
              console.log('Response data:', responseData);
              break;
            } catch (e) {
              console.warn('Could not parse successful response:', e);
              continue;
            }
          } else if (response.status === 401) {
            console.warn(` Authentication failed (401) - This is a backend issue.`);
            console.warn(`Response: ${responseText}`);
            throw new Error(`Authentication failed: ${responseText}`);
          }
        } catch (error) {
          console.warn(` Error with ${endpoint}:`, error);
          lastError = error instanceof Error ? error : new Error(String(error));
        }
      }
      if (responseData) break;
    }

    if (!responseData) {
      throw lastError || new Error('All endpoints and payload formats failed');
    }

    if (responseData.token) {
      if (responseData.refreshToken) {
        setAuthData(responseData);
        console.log('Auth data stored (LoginResponse format)');
      } else {
        localStorage.setItem("token", responseData.token);
        console.log('Token stored successfully');
      }
    }

    return responseData;
  } catch (error: any) {
    console.error("Apple sign-in error:", error);
    throw error;
  }
};

// ============ OTHER AUTH FUNCTIONS ============

export const loginWithEmail = async (email: string, password: string) => {
  try {
    const result = await signInWithEmailAndPassword(auth, email, password);
    return result.user;
  } catch (error: any) {
    console.error('Email login error:', error);
    throw error;
  }
};

export const getCurrentUser = () => {
  return auth.currentUser;
};

export const handleRedirectResult = async (): Promise<User | null> => {
  try {
    const result = await getRedirectResult(auth);
    return result?.user || null;
  } catch (error: any) {
    console.error('Redirect result error:', error);
    throw new Error(error.message);
  }
};