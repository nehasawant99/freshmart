# FreshMart Architecture

## 1. Overview

FreshMart is an ASP.NET Core Razor Pages application using Entity Framework Core for database access and ASP.NET Core Identity for authentication and role-based authorization.

The application is organized into:

```text
FreshMart
│
├── Pages/          → Razor Pages and request handling
├── Models/         → Application domain models
├── Services/       → Application services
├── Data/           → EF Core DbContext and Identity seeding
├── Migrations/     → Database schema history
└── wwwroot/        → Static files
```

---

## 2. High-Level Architecture

```text
                    FreshMart
                       │
                ASP.NET Core
                       │
              ┌────────┴────────┐
              │                 │
        Razor Pages          Identity
              │                 │
              └────────┬────────┘
                       │
                Application Logic
                       │
             ┌─────────┴─────────┐
             │                   │
        CartService          EF Core
             │                   │
             │          ApplicationDbContext
             │                   │
             └──────────┬────────┘
                        │
                        ▼
                   SQL Server
```

---

## 3. Application Layers

### Pages

The `Pages/` directory contains the Razor Pages used by customers and administrators.

Examples:

```text
Pages/
├── Account/
├── Admin/
├── Payment/
├── Cart
├── Categories
├── Checkout
├── Orders
├── OrderConfirmation
└── Products
```

Razor Page model classes handle HTTP requests and coordinate application operations.

---

### Models

The `Models/` directory contains the application's domain models.

```text
Models/
├── CartItem.cs
├── Category.cs
├── Product.cs
├── Order.cs
├── OrderItem.cs
└── Payment.cs
```

These models represent the core grocery-shopping domain.

---

### Services

The application currently contains:

```text
Services/
└── CartService.cs
```

`CartService` manages the session-based shopping cart.

It provides operations such as:

* Get cart
* Increase quantity
* Decrease quantity
* Remove item
* Calculate subtotal
* Calculate item count
* Clear cart

The service also validates product availability and stock when increasing cart quantities.

---

## 4. Data Access

Database access is handled through:

```text
Data/ApplicationDbContext.cs
```

`ApplicationDbContext` inherits from:

```csharp
IdentityDbContext<IdentityUser, IdentityRole, string>
```

This means application entities and ASP.NET Core Identity entities are managed through the same Entity Framework Core database context.

The context exposes:

```csharp
DbSet<Category>
DbSet<Product>
DbSet<Order>
DbSet<OrderItem>
DbSet<Payment>
```

---

## 5. Database Provider

FreshMart is configured to use SQL Server through Entity Framework Core:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));
```

The actual connection string is provided through application configuration and should not be committed with credentials.

---

## 6. Entity Relationships

The application currently models the following primary relationships:

```text
Category
   │
   └──< Product


Order
   │
   ├──< OrderItem
   │       │
   │       └── Product
   │
   └── Payment
```

### Order → Payment

An order has a one-to-one relationship with payment.

The payment uses `OrderId` as its foreign key.

The relationship is configured with cascade delete:

```csharp
modelBuilder.Entity<Order>()
    .HasOne(o => o.Payment)
    .WithOne(p => p.Order)
    .HasForeignKey<Payment>(p => p.OrderId)
    .OnDelete(DeleteBehavior.Cascade);
```

---

## 7. Authentication

FreshMart uses ASP.NET Core Identity.

Identity is configured with:

* Password policy
* Account lockout
* User management
* Role management
* Entity Framework Core storage

Current password requirements include:

```text
Minimum length: 8
Digit: Required
Uppercase: Required
Lowercase: Required
Non-alphanumeric: Required
```

Account lockout is configured for:

```text
Maximum failed attempts: 5
Lockout duration: 15 minutes
```

---

## 8. Authorization

FreshMart uses role-based authorization.

Two application roles are seeded:

```text
Admin
Customer
```

Customer-accessible protected pages use:

```csharp
[Authorize]
```

Administrative pages use:

```csharp
[Authorize(Roles = "Admin")]
```

This separates administrative functionality from normal customer functionality.

---

## 9. Identity Seeding

`IdentitySeeder` is responsible for creating the application's initial roles and administrator account.

Startup performs:

```text
Application Start
      ↓
Create Service Scope
      ↓
Seed Roles
      ↓
Seed Admin Account
      ↓
Start Application
```

The administrator credentials are read from configuration:

```text
AdminAccount:Email
AdminAccount:Password
```

The password itself should not be stored directly in source control.

---

## 10. Session-Based Cart

FreshMart uses ASP.NET Core Session for cart storage.

The required services are registered during application startup:

```text
HttpContextAccessor
        ↓
Distributed Memory Cache
        ↓
Session
        ↓
CartService
```

The cart is therefore maintained separately from the persistent order database until checkout.

At checkout, cart information is converted into persistent `Order` and `OrderItem` records.

---

## 11. Checkout Architecture

The checkout process combines cart data, database validation, order creation, payment creation, and inventory updates.

```text
Customer
   ↓
Checkout Page
   ↓
Validate Customer Information
   ↓
Calculate Order Total
   ↓
Create Order
   ↓
Create Payment
   ↓
Begin Database Transaction
   ↓
Validate Products
   ↓
Validate Availability
   ↓
Validate Stock
   ↓
Deduct Stock
   ↓
Save Changes
   ↓
Commit Transaction
   ↓
Clear Cart
   ↓
Payment Page
```

---

## 12. Transaction Handling

Checkout uses an explicit database transaction:

```csharp
await using var transaction =
    await _context.Database.BeginTransactionAsync();
```

Order creation and stock deduction are saved together.

If validation fails or an exception occurs:

```text
Rollback
   ↓
No partial checkout should be committed
```

Successful operations follow:

```text
SaveChanges
   ↓
Commit
```

This is an important part of FreshMart's data-consistency design.

---

## 13. Product Concurrency

Products contain a `RowVersion` property configured as an EF Core row-version concurrency token:

```csharp
modelBuilder.Entity<Product>()
    .Property(p => p.RowVersion)
    .IsRowVersion();
```

During checkout, the application catches:

```csharp
DbUpdateConcurrencyException
```

This allows the application to detect a conflicting product update, such as another customer purchasing stock at the same time.

The concurrency behavior will be tested separately in:

```text
docs/STOCK-CONCURRENCY.md
```

---

## 14. Price Precision

Product prices are configured with two decimal places:

```csharp
modelBuilder.Entity<Product>()
    .Property(p => p.Price)
    .HasPrecision(18, 2);
```

This prevents the database from relying on an unspecified decimal precision for product prices.

---

## 15. HTTP Request Pipeline

The application configures the following request pipeline:

```text
HTTPS Redirection
       ↓
Routing
       ↓
Authentication
       ↓
Authorization
       ↓
Session
       ↓
Static Assets
       ↓
Razor Pages
```

Authentication is intentionally registered before authorization:

```csharp
app.UseAuthentication();
app.UseAuthorization();
```

This allows authorization checks to operate against the authenticated user.

---

## 16. Static Files

Frontend static resources are stored under:

```text
wwwroot/
├── css/
├── js/
└── images/
```

These resources support the Razor Pages user interface.

---

## 17. Migrations

Entity Framework Core migrations are maintained under:

```text
Migrations/
```

The migration history currently includes changes for:

```text
Categories and Products
Product Seed Data
ASP.NET Identity
Orders
Product Concurrency
Payment
Payment Expiration
```

Migrations provide the version history for the database schema.

---

## 18. Architecture Principles

The current architecture follows these principles:

* Razor Pages for server-rendered application pages
* Entity Framework Core for database access
* ASP.NET Core Identity for authentication
* Role-based authorization for administrative access
* Session-based cart management
* Database transactions for checkout consistency
* Optimistic concurrency handling for product stock
* Configuration-based administrator credentials
* EF Core migrations for schema management

---

## 19. Validation Status

The architecture is implemented, but implementation does not automatically mean production-ready.

The following areas require explicit validation:

```text
Authentication
Authorization
Cart behavior
Checkout
Order lifecycle
Payment lifecycle
Payment idempotency
Stock concurrency
Cancellation concurrency
Transaction consistency
Database constraints
Failure recovery
```

These validations are documented separately under `docs/`.
