# FreshMart

A grocery shopping web application built with **ASP.NET Core Razor Pages** and **Entity Framework Core**.

FreshMart provides customer shopping, cart, checkout, order, payment, inventory, and admin management functionality.

## Features

* Customer registration and login
* Product and category browsing
* Shopping cart
* Checkout and delivery validation
* Order management and cancellation
* Payment flow with failure, retry, and expiry handling
* Inventory and stock validation
* Admin dashboard and order management
* Role-based authorization
* Database transactions and concurrency handling

## Tech Stack

* **C# / ASP.NET Core Razor Pages**
* **Entity Framework Core**
* **ASP.NET Core Identity**
* **SQL Database**
* **HTML / CSS / JavaScript**
* **Git**

## Project Structure

```text
GroceryShopping/
├── Data/
├── Models/
├── Services/
├── Pages/
├── Migrations/
├── wwwroot/
├── docs/
├── Program.cs
├── appsettings.json
└── GroceryShopping.csproj
```

## Run Locally

```bash
dotnet restore
dotnet build
dotnet run
```

The application URL will be shown in the terminal after starting the application.

## Documentation

Detailed technical documentation is available in [`docs/`](docs/):

* [Architecture](docs/ARCHITECTURE.md)
* [Features](docs/FEATURES.md)
* [Database](docs/DATABASE.md)
* [User Flow](docs/USER-FLOW.md)
* [Admin Flow](docs/ADMIN-FLOW.md)
* [Security](docs/SECURITY.md)
* [Stock & Concurrency](docs/STOCK-CONCURRENCY.md)
* [Payment](docs/PAYMENT.md)
* [Data Consistency](docs/DATA-CONSISTENCY.md)
* [Testing](docs/TESTING.md)

## Status

**Development / Validation Phase**

Core shopping, order, payment, inventory, and admin functionality has been implemented. Final validation and testing are in progress.

## Author

**Neha Sawant**

GitHub: [nehasawant99](https://github.com/nehasawant99)
