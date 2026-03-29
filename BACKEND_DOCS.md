# GeniLog Backend & API Documentation

## Production Environment Setup

The backend is configured to use a production database hosted on `site4now.net`.

### 1. Database Configuration
The production database connection is already configured in `Genilog_WebApi/appsettings.json`:
- **Server**: `SQL6033.site4now.net`
- **Database**: `db_ab177f_ginilog`
- **User**: `db_ab177f_ginilog_admin`

### 2. API Production URL
The production API is intended to be accessible at:
- **Primary**: `https://api-data.ginilog.com`
- **Alternative**: `https://localhost:7001` (Development/Local Testing)

### 3. Frontend Integration
The frontend `ginilog-spa` uses the `NEXT_PUBLIC_API_URL` environment variable to connect to the backend. In production, this should be set to `https://api-data.ginilog.com`.

---

## Frontend Integration Status
- **Login Page**: Updated to call `POST /api/AuthUsers/login`.
- **Register Page**: Updated to call `POST /api/AuthUsers`.
- **Base URL**: Currently hardcoded to `https://localhost:7001` in the fetch calls.

---

GeniLog is a comprehensive logistics and service management platform built with .NET 8. The backend provides a robust API for managing users, bookings (logistics, airlines, accommodations), wallets, and notifications. It uses a clean architecture with Controllers, Repositories, and Data Contexts.

---

## Technical Stack
- **Framework**: .NET 8 Web API
- **Database**: Microsoft SQL Server (via Entity Framework Core)
- **Authentication**: JWT (JSON Web Tokens) & Firebase Admin SDK
- **Real-time**: SignalR & WebSockets
- **Object Mapping**: AutoMapper
- **Documentation**: Swagger/OpenAPI

---

## Core Modules

### 1. Authentication & User Management (`AuthUsersController`)
Handles user lifecycle, security, and profile management.
- **Login**: Supports standard email/password and External (Google) authentication.
- **Registration**: New user onboarding with email verification.
- **Roles**: RBAC (Role-Based Access Control) with roles like `User`, `Admin`, and `Super_Admin`.
- **Device Management**: Tracks device tokens for push notifications.
- **Wallet Integration**: Manages "Money Box" balances for users.

### 2. Logistics & Delivery (`LogisticsController`)
Manages the core logistics engine.
- **Company & Riders**: Management of logistics companies and individual riders.
- **Order Flow**: Tracks the lifecycle of a delivery from creation to completion.
- **Reviews**: Feedback system for both companies and riders.

### 3. Bookings (`BookingsController`)
A multi-service booking engine.
- **Accommodations**: Hotel/Stay reservations with daily availability management (Monday-Sunday).
- **Airlines**: Flight ticket bookings and aircraft management.
- **Service Locations**: Geographic filtering for available services.

### 4. Wallet & Transactions (`WalletController`)
Financial layer for the platform.
- **Transactions**: Records all financial movements.
- **Balance Management**: CRUD operations on user and provider wallets.

### 5. Notifications & Communication (`NotificationsController`)
- **Push Notifications**: Integration with Firebase for real-time alerts.
- **WebSocket/SignalR**: Real-time updates for order tracking and chats.

---

## API Endpoints (Key Examples)

### Authentication
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| POST | `/api/AuthUsers/login` | Authenticate user and return JWT |
| POST | `/api/AuthUsers/auth-login` | Google External Authentication |
| GET | `/api/AuthUsers/profile` | Get current user profile (Authorized) |
| PUT | `/api/AuthUsers/update-user` | Update profile information |

### Logistics
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| GET | `/api/Logistics/companies` | List available logistics companies |
| POST | `/api/Logistics/orders` | Create a new delivery order |
| GET | `/api/Logistics/track/{id}` | Real-time order tracking |

### Bookings
| Method | Endpoint | Description |
| :--- | :--- | :--- |
| GET | `/api/Bookings/accommodations` | Search for stays |
| POST | `/api/Bookings/flight-tickets` | Book a flight |

---

## Data Schema (Highlights)
- **GeneralUsers**: Core identity table (Identity-like).
- **UsersDataModelTables**: Extended profile data for app users.
- **OrderModelDatas**: Central table for all logistics transactions.
- **AccomodationDataModels**: Complex structure with one-to-one relations for daily schedules.
- **TransactionDataModel**: Audit log for all financial activities.

---

## Development & Configuration
- **Connection String**: Configured in `appsettings.json` under `Genilog_Data_Context`.
- **JWT**: Settings for Issuer, Audience, and Key are required in configuration.
- **Firebase**: Requires `ginilog-e3c8a-firebase-adminsdk-*.json` for notification services.
