# ShopHub — Online Marketplace (CSE325 Team 4)

A .NET Blazor web application for buying and selling products online. Built for the CSE325 group project using the **.NET 10** stack (Blazor Server, EF Core, SQLite) and deployed to **Azure App Service**.

## Team Members

- RayVen Shellhart
- Kolawole Uthman
- Moffat Katopola
- Natanael da Matta

## Features

- **Browse & search products** — category filter, search, sort by name / price / rating
- **Product details** — individual product pages
- **Shopping cart** — add, update quantity, remove; persisted in `localStorage`
- **Checkout** — shipping form with validation; order is saved to the database and a confirmation number is shown
- **User authentication** — sign up and log in (passwords hashed with ASP.NET Core `PasswordHasher`)
- **Seller product management (CRUD)** — sellers can create, edit, and delete products
- **Responsive design** — works on desktop, tablet, and mobile

## Tech Stack

| Layer      | Technology                              |
|------------|-----------------------------------------|
| Framework  | ASP.NET Core Blazor (interactive server) |
| Language   | C# (.NET 10)                            |
| Data       | Entity Framework Core + SQLite          |
| Front-end  | Bootstrap 5 + custom CSS                |
| Testing    | xUnit + Moq                             |
| CI/CD      | GitHub Actions → Azure App Service      |
| Project Mgmt | Trello board                          |

## Getting Started

### Prerequisites

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download/dotnet/10.0)
- A code editor (Visual Studio, Rider, or VS Code)

### Run Locally

```bash
# 1. Clone the repository
git clone <repo-url>
cd CSE325-Team4-GroupProject

# 2. Restore packages
dotnet restore

# 3. Run the application (SQLite DB is created and seeded automatically)
dotnet run
```

Open `http://localhost:5000` (see the console output / `launchSettings.json` for the exact URL, e.g. `http://localhost:5127`).

The SQLite database (`shop.db`) is created in the project root and seeded with sample products on first launch.

### Run Tests

```bash
dotnet test
```

## User Guide

1. **Browse** — from the home page, browse featured products, click a category, or use the search bar in the header to jump straight to matching products. Click **Products** to filter, search, and sort.
2. **Add to cart** — click **Add to Cart** on any product card or product page.
3. **Checkout** — open the **Cart** in the header, click **Proceed to Checkout**, fill in the shipping details, and place the order. You'll receive an order confirmation number.
4. **Create an account** — click **Sign Up** and fill in your details. Passwords are stored as hashes.
5. **Sell products** — sign up (or log in) as a **Seller** (user type "Seller" or "Both"). A **Manage Products** link appears in the header, where you can add, edit, and delete products.

## Project Structure

```
Components/
  Layout/        Main layout, header, footer
  Pages/         Home, Products, Product detail, Cart, Checkout,
                 Login, Signup, Manage Products, Error, NotFound
  Shared/        ProductCard
Models/          Product, User, Order, OrderItem
Data/            ShopDbContext (EF Core)
Services/        ProductService, UserService, CartService, OrderService, AuthStateService
Migrations/      EF Core migrations
Tests/           xUnit test project
```

## Deployment

The app is deployed to Azure App Service (`Shophub`) via the GitHub Actions workflow in `.github/workflows/main_shophub.yml`. Any push to `main` builds, runs the tests, publishes, and deploys automatically.

> **Note:** The SQLite database is stored in the app's local filesystem. On Azure, it is recreated and re-seeded on each fresh deployment. For a production app this would be replaced with a managed database such as Azure SQL.

## Project Board

Task management is tracked on Trello: [link to Trello board]

## License

University coursework project — no license.
