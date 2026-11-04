# Dev Seed — Dhana Family Fixture

**Version:** 1.0  
**Last Updated:** June 2026  
**Purpose:** Local development and integration testing for multi-member household behavior

---

## 1. Overview

The dev seed creates a **4-user Dhana Family** household with shared sample data. Use this to verify that all members see the same recipes, meal plan, pantry, and grocery lists while todos remain private per user.

**Invocation (when implemented):**

```bash
cd src/SmartMealPlanner.Api
dotnet run -- --seed
```

Or via Docker Compose:

```bash
docker compose exec api dotnet SmartMealPlanner.Api.dll --seed
```

**Idempotency:** Seed should check for existing `dad@dhana.test` user and skip or reset based on `--seed --force` flag.

---

## 2. Users

All passwords: `DhanaFamily1!` (meets MVP password rules)

| Display name | Email | Role | display_label | User ID (fixed for seed) |
|--------------|-------|------|---------------|--------------------------|
| Bob Dhana | dad@dhana.test | owner | Dad | `11111111-1111-1111-1111-111111111111` |
| Alice Dhana | mom@dhana.test | member | Mom | `22222222-2222-2222-2222-222222222222` |
| Emma Dhana | kid1@dhana.test | member | Kid 1 | `33333333-3333-3333-3333-333333333333` |
| Liam Dhana | kid2@dhana.test | member | Kid 2 | `44444444-4444-4444-4444-444444444444` |

Each user gets:

- `AspNetUsers` row (Identity)
- `user_profiles` row (`household_id` → Dhana Family)
- `household_members` row

---

## 3. Household

| Field | Value |
|-------|-------|
| id | `550e8400-e29b-41d4-a716-446655440000` |
| name | Dhana Family |
| created_by_user_id | `11111111-1111-1111-1111-111111111111` (Dad) |

---

## 4. Shared Sample Data

All rows use `household_id = 550e8400-e29b-41d4-a716-446655440000`.

### 4.1 Recipe — Spaghetti Bolognese

| Field | Value |
|-------|-------|
| id | `a0000001-0000-0000-0000-000000000001` |
| title | Spaghetti Bolognese |
| servings | 4 |
| prep_time_minutes | 15 |
| cook_time_minutes | 45 |
| created_by_user_id | Dad |

**Ingredients:**

| name | quantity | unit |
|------|----------|------|
| Spaghetti | 400 | g |
| Ground beef | 500 | g |
| Canned tomatoes | 400 | g |
| Onion | 1 | each |
| Garlic | 3 | clove |

**Steps:**

1. Dice onion and mince garlic.
2. Brown ground beef in a large pan.
3. Add onion, garlic, and tomatoes; simmer 30 minutes.
4. Cook spaghetti per package directions; serve with sauce.

**Tags:** `dinner`, `kid-friendly`

### 4.2 Pantry items

| name | category | quantity | unit | low_stock_threshold |
|------|----------|----------|------|---------------------|
| Spaghetti | Pantry Staples | 200 | g | 100 |
| Ground beef | Meat | 0 | g | 250 |
| Olive oil | Pantry Staples | 500 | ml | 100 |

Ground beef is intentionally low stock to trigger inventory alert in dev.

### 4.3 Meal plan (current week)

- `week_start_date`: Monday of current ISO week (computed at seed time)
- One entry: **Wednesday Dinner** → Spaghetti Bolognese recipe

### 4.4 Grocery list

| Field | Value |
|-------|-------|
| name | Grocery List – Dhana Family |
| items | Ground beef (500 g), Canned tomatoes (400 g) — unchecked |

### 4.5 Per-user todos (private)

| User | Todo title | Status |
|------|------------|--------|
| Dad | Thaw ground beef | Pending |
| Mom | Buy groceries Saturday | Pending |
| Kid 1 | Set table for dinner | Pending |
| Emma only sees Emma's todo; Dad does not see Mom's |

---

## 5. Implementation Sketch

Place seed logic in `SmartMealPlanner.Infrastructure/Persistence/SeedData/DevSeedRunner.cs`:

```csharp
public static class DevSeedRunner
{
    public static async Task RunAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        if (await db.UserProfiles.AnyAsync(u => u.UserId == DhanaFamilyIds.Dad))
            return; // already seeded

        await using var tx = await db.Database.BeginTransactionAsync();

        // 1. Create users + profiles
        // 2. Create household + household_members (1 owner, 3 members)
        // 3. Create recipe + ingredients + steps + tags
        // 4. Create pantry items
        // 5. Create meal plan + entry for current week
        // 6. Create grocery list + items
        // 7. Create per-user todos

        await db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}
```

Register in `Program.cs`:

```csharp
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    await DevSeedRunner.RunAsync(
        scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(),
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>());
    return;
}
```

Use fixed UUID constants in `DhanaFamilyIds.cs` for reproducible integration tests.

---

## 6. Verification Steps

After seeding, log in as each user and confirm:

1. **Dad (Owner):** Sees Dhana Family, 4 members, can access Settings → Family with edit controls
2. **Mom (Member):** Sees same recipes, meal plan, pantry, grocery; cannot rename household (403)
3. **Shared data:** Dad adds a pantry item → Mom sees it on refresh
4. **Private todos:** Dad's "Thaw ground beef" not visible when logged in as Mom
5. **Alerts:** Low-stock alert for ground beef visible to all members
6. **Cross-household:** Register a new solo user → cannot see Dhana Family recipes

---

## 7. SQL Reference (manual seed alternative)

For bootstrapping before EF seed runner exists, run after migrations:

```sql
-- Fixed UUIDs; run in transaction
-- See docs/database-design.md section 14.3 for entity summary
-- Full INSERT statements generated by initial migration seed or DevSeedRunner
```

Prefer the C# seed runner for password hashing via Identity `UserManager.CreateAsync`.

---

## 8. Document History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | June 2026 | Initial Dhana Family 4-user dev seed specification |
