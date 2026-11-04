# Household Module — API Contracts and Authorization

**Version:** 1.0  
**Last Updated:** June 2026  
**Status:** Specification for implementation (build after Auth + Profile, before Recipes)

---

## 1. Overview

The Household module manages the family group boundary: member listing, household rename, member add/remove, and ownership transfer. It does **not** own shared kitchen data (recipes, meal plans, etc.) — those modules filter by `household_id` from `IHouseholdContext`.

**Roles:**

| Role | `household_members.role` | Permissions |
|------|--------------------------|-------------|
| Owner | `owner` | Membership management + full shared-data access |
| Member | `member` | Full shared-data access only |

Exactly one Owner per household (database partial unique index).

---

## 2. Authorization Guards

### 2.1 `IHouseholdContext`

```csharp
public enum HouseholdRole
{
    Owner,
    Member
}

public interface IHouseholdContext
{
    Guid UserId { get; }
    Guid HouseholdId { get; }
    HouseholdRole Role { get; }
    bool IsOwner { get; }

    /// <summary>
    /// Verifies the current user has a household_members row for HouseholdId.
    /// Throws ForbiddenException if not a member.
    /// </summary>
    Task EnsureMemberAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the current user is Owner of HouseholdId.
    /// Throws ForbiddenException if not Owner.
    /// </summary>
    Task EnsureOwnerAsync(CancellationToken cancellationToken = default);
}
```

### 2.2 Guard application by endpoint

| Endpoint | Guard | Notes |
|----------|-------|-------|
| `GET /api/households/me` | `EnsureMemberAsync` | Any member |
| `PUT /api/households/me` | `EnsureOwnerAsync` | Owner only |
| `POST /api/households/me/members` | `EnsureOwnerAsync` | Owner only |
| `DELETE /api/households/me/members/{userId}` | `EnsureOwnerAsync` | Owner only |
| `POST /api/households/me/transfer-ownership` | `EnsureOwnerAsync` | Owner only |

### 2.3 Shared-resource modules (not in Household module)

Recipes, Meal Plans, Pantry, Grocery, Inventory Alerts use `EnsureMemberAsync` only. Owner and Member have identical access.

### 2.4 Error responses

| Condition | HTTP | Problem title |
|-----------|------|---------------|
| No JWT / invalid token | 401 | Unauthorized |
| Authenticated but not household member | 403 | Forbidden |
| Member calls Owner-only endpoint | 403 | Forbidden |
| Target user not found | 404 | Not Found |
| Target user already in another household | 409 | Conflict |
| Owner tries to remove sole Owner (self) | 409 | Conflict |
| Validation failure (empty name, invalid email) | 400 | Validation Error |

---

## 3. API Contracts

Base path: `/api/households`  
All endpoints require `Authorization: Bearer {token}`.

### 3.1 GET `/api/households/me`

Returns the current user's household and member list.

**Authorization:** `EnsureMemberAsync`

**Response 200:**

```json
{
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Dhana Family",
    "createdAt": "2026-06-01T10:00:00Z",
    "members": [
      {
        "userId": "11111111-1111-1111-1111-111111111111",
        "displayName": "Bob Dhana",
        "displayLabel": "Dad",
        "role": "owner",
        "joinedAt": "2026-06-01T10:00:00Z"
      },
      {
        "userId": "22222222-2222-2222-2222-222222222222",
        "displayName": "Alice Dhana",
        "displayLabel": "Mom",
        "role": "member",
        "joinedAt": "2026-06-01T10:05:00Z"
      }
    ],
    "currentUserRole": "owner"
  },
  "success": true
}
```

---

### 3.2 PUT `/api/households/me`

Renames the household.

**Authorization:** `EnsureOwnerAsync`

**Request body:**

```json
{
  "name": "Dhana Family"
}
```

| Field | Type | Rules |
|-------|------|-------|
| name | string | Required, 1–100 characters |

**Response 200:** Same shape as `GET /api/households/me` with updated name.

**Response 403:** Current user is Member, not Owner.

---

### 3.3 POST `/api/households/me/members`

Adds an already-registered user to the household as Member.

**Authorization:** `EnsureOwnerAsync`

**Request body:**

```json
{
  "email": "mom@dhana.test",
  "displayLabel": "Mom"
}
```

| Field | Type | Rules |
|-------|------|-------|
| email | string | Required, valid email, must match existing AspNetUsers |
| displayLabel | string | Optional, 1–50 characters (e.g. "Mom", "Kid 1") |

**Business logic:**

1. Look up user by normalized email
2. If user not found → 404
3. If user already has `household_members` row for a different household → 409
4. If user already member of this household → 409
5. Create `household_members` row (`role = member`)
6. Update `user_profiles.household_id` to current household
7. Return updated household (via `GET` shape)

**Response 201:** Household DTO with new member in list.

---

### 3.4 DELETE `/api/households/me/members/{userId}`

Removes a member from the household.

**Authorization:** `EnsureOwnerAsync`

**Path parameter:** `userId` — UUID of member to remove

**Business logic:**

1. Target must be a member of current household → else 404
2. Cannot remove the sole Owner (if `userId` is Owner and no other Owner exists) → 409
3. Delete `household_members` row
4. Clear `user_profiles.household_id` for removed user (set to NULL or leave orphaned — **MVP:** set NULL; user cannot access shared data until re-added or re-seeded)

**Response 204:** No content.

**Note:** Removed users retain their account and private todos. They lose access to household-shared data until assigned to a household again.

---

### 3.5 POST `/api/households/me/transfer-ownership`

Transfers Owner role to another member atomically.

**Authorization:** `EnsureOwnerAsync`

**Request body:**

```json
{
  "newOwnerUserId": "22222222-2222-2222-2222-222222222222"
}
```

| Field | Type | Rules |
|-------|------|-------|
| newOwnerUserId | UUID | Required, must be existing Member of this household (not Owner) |

**Business logic (single transaction):**

1. Verify `newOwnerUserId` is Member of current household → else 404
2. Set current Owner's `household_members.role` → `member`
3. Set target's `household_members.role` → `owner`
4. Update `households.created_by_user_id` → `newOwnerUserId` (optional audit)
5. Commit transaction

**Response 200:** Household DTO with updated roles.

**JWT note:** `household_role` claim updates on next login/register token refresh in MVP.

---

## 4. DTOs (Application Layer)

```
HouseholdDto
├── Id (Guid)
├── Name (string)
├── CreatedAt (DateTimeOffset)
├── Members (List<HouseholdMemberDto>)
└── CurrentUserRole (string: owner | member)

HouseholdMemberDto
├── UserId (Guid)
├── DisplayName (string)
├── DisplayLabel (string?)
├── Role (string: owner | member)
└── JoinedAt (DateTimeOffset)

UpdateHouseholdRequest
└── Name (string)

AddHouseholdMemberRequest
├── Email (string)
└── DisplayLabel (string?)

TransferOwnershipRequest
└── NewOwnerUserId (Guid)
```

---

## 5. Service Interface

```csharp
public interface IHouseholdService
{
    Task<HouseholdDto> GetMyHouseholdAsync(CancellationToken cancellationToken = default);
    Task<HouseholdDto> UpdateHouseholdNameAsync(UpdateHouseholdRequest request, CancellationToken cancellationToken = default);
    Task<HouseholdDto> AddMemberAsync(AddHouseholdMemberRequest request, CancellationToken cancellationToken = default);
    Task RemoveMemberAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<HouseholdDto> TransferOwnershipAsync(TransferOwnershipRequest request, CancellationToken cancellationToken = default);
}
```

---

## 6. Controller

```csharp
[ApiController]
[Route("api/households")]
[Authorize]
public class HouseholdsController : ControllerBase
{
    // GET me, PUT me, POST me/members, DELETE me/members/{userId}, POST me/transfer-ownership
}
```

No business logic in controller — delegate to `IHouseholdService`.

---

## 7. Validators (FluentValidation)

| Request | Rules |
|---------|-------|
| `UpdateHouseholdRequest` | `Name`: NotEmpty, MaxLength(100) |
| `AddHouseholdMemberRequest` | `Email`: NotEmpty, EmailAddress; `DisplayLabel`: MaxLength(50) when present |
| `TransferOwnershipRequest` | `NewOwnerUserId`: NotEmpty, NotEqual(current user id) |

---

## 8. Frontend Integration

| Concern | Approach |
|---------|----------|
| Household name in header | From `GET /api/households/me` or `GET /api/users/me` |
| Settings → Family page | `HouseholdPage.tsx`; Owner sees edit controls |
| Owner-only buttons | Hide/disable when `currentUserRole !== 'owner'` |
| After membership change | Invalidate household + user queries; prompt re-login if role changed |

---

## 9. Security Test Checklist

- [ ] Member cannot call `PUT /api/households/me` (403)
- [ ] Member cannot add/remove members (403)
- [ ] Member cannot transfer ownership (403)
- [ ] Cannot add user who belongs to another household (409)
- [ ] Cannot remove sole Owner (409)
- [ ] Transfer leaves exactly one Owner
- [ ] User from household A cannot read household B shared data (403/empty)

---

## 10. Document History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | June 2026 | Initial household module API spec and Owner guard rules |
