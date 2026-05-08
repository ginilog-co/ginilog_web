# Genilog API Documentation

This document provides a comprehensive overview of the Genilog Web API, including its structure, authentication, and endpoints for frontend integration.

## Project Overview
Genilog is a logistics and booking platform providing services for package delivery, airline ticketing, accommodation reservations, and wallet management.

## Technical Stack
- **Framework:** ASP.NET Core Web API
- **Database:** SQL Server (Entity Framework Core)
- **Authentication:** JWT (JSON Web Tokens)
- **Real-time:** SignalR & WebSockets
- **Third-party Services:** 
  - Firebase (Push Notifications)
  - Paystack & Flutterwave (Payments)
  - AutoMapper (DTO Mapping)

---

## Authentication
Most endpoints require a JWT Bearer token.
- **Header:** `Authorization: Bearer <your_token>`
- **Login Endpoint:** `POST /api/AuthUsers/login` or `POST /api/Admin/login`

---

## API Endpoints

### 1. Authentication & Users (`AuthUsersController`)
Manage user accounts, profiles, and delivery addresses.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/AuthUsers/login` | User login | No |
| `POST` | `/api/AuthUsers/auth-login` | External (Google) login | No |
| `POST` | `/api/AuthUsers` | Register new user | No |
| `GET` | `/api/AuthUsers/profile` | Get current user profile | User |
| `PUT` | `/api/AuthUsers/update-user` | Update user details | User |
| `GET` | `/api/AuthUsers/delivery-address` | Get user delivery addresses | User |
| `PUT` | `/api/AuthUsers/add-new-address` | Add new delivery address | User |
| `POST` | `/api/AuthUsers/email-verification` | Verify email with OTP | No |
| `POST` | `/api/AuthUsers/forgot-password-request-token` | Request password reset token | No |
| `POST` | `/api/AuthUsers/reset-password` | Reset password using token | No |

### 2. Admin Management (`AdminController`)
Administrative controls for managing staff, adverts, and applications.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Admin/login` | Admin login | No |
| `GET` | `/api/Admin/staff-users` | List all staff members | SuperAdmin/Manager |
| `POST` | `/api/Admin/add-manager` | Create a new manager | SuperAdmin |
| `POST` | `/api/Admin/advert` | Create a new advertisement | Admin/Staff |
| `PUT` | `/api/Admin/initialize-paystack-advert-payment/{id}` | Pay for advert (Paystack) | User |
| `GET` | `/api/Admin/company-apply` | View company applications | Admin/SuperAdmin |

### 3. Logistics & Riders (`LogisticsController`)
Handles companies, riders, and package orders.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Logistics` | Get all companies | No |
| `GET` | `/api/Logistics/rider` | Get all available riders | User |
| `POST` | `/api/Logistics/rider` | Register a new rider | User/Admin |
| `POST` | `/api/Logistics/package-orders` | Create a new package order | User/Admin |
| `GET` | `/api/Logistics/track-order` | Track order by tracking number | No |
| `PUT` | `/api/Logistics/initialize-paystack-package-orders/{id}` | Pay for order (Paystack) | User |

### 4. Bookings (`BookingsController`)
Flight tickets and accommodation reservations.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Bookings/airline` | List all airlines | User |
| `POST` | `/api/Bookings/airline-flight-ticket` | Book a flight ticket | Admin/Manager |
| `GET` | `/api/Bookings/accomodation` | List all accommodations | No |
| `POST` | `/api/Bookings/accomodation-reservations` | Create a reservation | Staff/Admin |
| `GET` | `/api/Bookings/accomodation-reservations-customer` | Get customer reservations | User |

### 5. Wallet & Payments (`WalletController`)
Direct wallet funding and payment verification.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Wallet/initialize` | Initialize Paystack payment | No |
| `GET` | `/api/Wallet/verify` | Verify Paystack payment | No |
| `POST` | `/api/Wallet/initialize-flutterwave` | Initialize Flutterwave payment | No |
| `GET` | `/api/Wallet/verify-flutterwave` | Verify Flutterwave payment | No |

### 6. Notifications (`NotificationsController`)
Push notifications and alerts.

| Method | Route | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/Notifications` | Get user notifications | User |
| `POST` | `/api/Notifications` | Send/Add notification | User |
| `PUT` | `/api/Notifications/{id}` | Mark notification as read | User |

---

## Data Models (DTOs)
Common objects used in requests/responses:

- **Auth:** `LoginRequset`, `AddUserRequest`, `UpdateUserRequest`, `EmailVerification`
- **Logistics:** `AddOrder`, `UpdateOrder`, `AddRiders`, `AddCompany`
- **Bookings:** `AddFlightTicket`, `AddAccomodation`, `AddBookAccomodationReservation`
- **Wallet:** `PaymentRequest`

---

## WebSocket / Real-time
- **SignalR Hub:** Configured in `Program.cs`
- **Custom WebSocket:** `ws://<server>/ws` handled by `WebSocketHandler`.

## Global Configurations
- **CORS:** Allowed for `localhost:3000`, `ginilog-web.onrender.com`, and `ginilog-web.vercel.app`.
- **JSON:** Configured to ignore reference cycles.
