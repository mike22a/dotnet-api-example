# Level 3: Hotel Reservation System API

An ASP.NET Core 8 Web API for a **Hotel Reservation System** implementing the strictest Clean Architecture at this level. This is the **Hard** level (Level 3) in the series.

## What Makes This Level Hard

| Feature | Details |
|---------|---------|
| **State Machine** | Reservations flow: `Pending → Confirmed → CheckedIn → CheckedOut / Cancelled` |
| **Date-Range Availability** | Rooms are only available if no active reservations overlap the requested dates |
| **6 Entities** | User, RoomType, Room, Guest, Reservation, Payment |
| **3 Roles** | Admin, Receptionist |
| **Revenue & Occupancy Reports** | Admin-only reporting endpoints |
| **PATCH Endpoints** | State transitions use `PATCH` (not PUT) per REST best practices |

---

## Seed Accounts

| Account | Email | Password | Role |
|---------|-------|----------|------|
| System Admin | `admin@hotel.com` | `password123` | Admin |
| Front Desk | `receptionist@hotel.com` | `password123` | Receptionist |

---

## Endpoints

### Auth (`/api/auth`)
- `POST /api/auth/register` — Create account
- `POST /api/auth/login` — Login, receive JWT
- `GET /api/auth/me` — Current user profile

### Room Types (`/api/room-types`)
- `GET` / `GET {id}` — Read (all roles)
- `POST` / `PUT {id}` / `DELETE {id}` — Admin only

### Rooms (`/api/rooms`)
- `GET` / `GET {id}` — Paginated list with roomTypeId/status filters
- `GET /available` — Available rooms for given `roomTypeId`, `checkInDate`, `checkOutDate`
- `POST` / `PUT {id}` / `DELETE {id}` — Admin only

### Guests (`/api/guests`)
- `GET` / `GET {id}` — Paginated, searchable
- `POST` / `PUT {id}` — All authenticated
- `DELETE {id}` — Admin only

### Reservations (`/api/reservations`)
- `GET` / `GET {id}` — Filter by `status`, `guestId`
- `POST` — Create reservation (validates date conflict)
- `PATCH {id}/confirm` — Pending → Confirmed
- `PATCH {id}/check-in` — Confirmed → CheckedIn (room becomes Occupied)
- `PATCH {id}/check-out` — CheckedIn → CheckedOut (room becomes Available)
- `PATCH {id}/cancel` — Cancel + auto-refund if paid

### Payments (`/api/payments`)
- `POST` — Record payment for a reservation
- `GET /reservation/{id}` — Get payment for reservation

### Reports (`/api/reports`) — Admin Only
- `GET /occupancy` — Current room occupancy statistics
- `GET /revenue?from=&to=` — Revenue by date range, grouped by room type

---

## How to Run

```bash
dotnet run --project src/HotelApi.API/HotelApi.API.csproj
```

👉 **[http://localhost:5003/swagger](http://localhost:5003/swagger)**
