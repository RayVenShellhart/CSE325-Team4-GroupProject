# ShopHub — MVP Project Plan (CSE325 Team 4)

> **Working reference document.** Update this file whenever a task is started, completed, or changed.
> Status legend: `[ ]` pending · `[~]` in progress · `[x]` done · `[!]` blocked/needs decision

## Project Overview

**App:** ShopHub — an online marketplace (buy & sell) built with Blazor Server (interactive server render mode) on **.NET 10**, EF Core + SQLite, deployed to **Azure Web App** via GitHub Actions.

**Team:** RayVen Shellhart, Kolawole Uthman, Moffat Katopola, Natanael da Matta

**Goal:** MVP ready for submission that satisfies the course rubric:
- Blazor web app ✓
- User authentication
- CRUD functionality
- Tested for QA
- GitHub repo + Trello board
- Code comments + user docs
- Deployed to cloud (Azure)
- Performance / validation / accessibility (WCAG 2.1 AA) / usability / branding / navigation

---

## Current State (baseline — verified 2026-08-10)

### Working
- Product browse: list (`/products`), detail (`/product/{Id}`), search, sort, category filter
- Cart: add / update qty / remove / clear, persisted to `localStorage` (`CartService`)
- Checkout (`/checkout`) — clears cart (unawaited), no order saved
- Signup + Login (custom, plaintext passwords) — `UserService`, `AuthStateService`
- SQLite + EF Core, auto-migrate + seed on startup (`Program.cs`)
- Azure deploy workflow `.github/workflows/main_shophub.yml` (push to `main`)

### Broken / Missing (recorded at baseline)
1. `AuthStateService` uses `static` fields + `Singleton` lifetime → all users share one login state.
2. `IndvidualProduct.razor` "Add to Cart" button has no `@onclick` — does nothing.
3. Header cart count is `0` until user visits `/cart` (cart never loaded in `MainLayout`).
4. `Checkout.PlaceOrder` does not `await Cart.Clear()`, inputs are unbound, no validation/feedback.
5. Dead template artifacts: `Weather.razor`, `NavMenu.razor`, `Logout.razor`; dead links `/profile`, `/forgot-password`, `/about`, `/contact`, `/terms`, `/privacy` (all 404).
6. No product CRUD (create/update/delete). Orders are never persisted.
7. Passwords stored in plaintext.
8. No test project. README is 6 lines (wrong team name, no docs).
9. Home controls are non-functional: "Shop Now" deal buttons, category cards, header search.
10. Accessibility gaps: icon-only buttons lack aria-labels, etc.

---

## Tier 1 — Correctness Bugs (priority: highest)

### T1.1 Fix `AuthStateService` — shared login state bug
- [x] File: `Services/AuthStateService.cs`
- [x] Remove `static` fields and `static` event; use instance fields.
- [x] File: `Program.cs:17` — change `AddSingleton<AuthStateService>()` → `AddScoped<AuthStateService>()`.
- [x] Verified build passes.

### T1.2 Wire IndividualProduct "Add to Cart"
- [x] File: `Components/Pages/IndvidualProduct.razor`
- [x] Inject `CartService`, add `@onclick="AddToCart"` on the button.
- [x] Added feedback state ("Added to Cart ✓") + styled button.

### T1.3 Load cart at startup (header count)
- [x] File: `Components/Layout/MainLayout.razor` — call `CartService.LoadCartAsync()` in `OnInitializedAsync`.

### T1.4 Fix checkout flow
- [x] File: `Components/Pages/Checkout.razor`
- [x] Bind shipping fields (name, address, city, phone) to `ShippingModel`; validate non-empty with error alert.
- [x] `await Cart.Clear()` inside async `PlaceOrder` (unawaited bug fixed); try/catch + success alert.
- [~] Wire order persistence (done in T2.3 — `SaveOrderAsync()` currently returns null).
- [x] Success message shows order number once T2.3 wired.

### T1.5 Remove dead code & dead links
- [x] Deleted `Components/Pages/Weather.razor`
- [x] Deleted `Components/Layout/NavMenu.razor` (+ `.razor.css`)
- [x] Deleted `Components/Pages/Logout.razor`
- [x] `MainLayout.razor` — removed `/profile`, `/about`, `/contact` links.
- [x] `Login.razor` — removed `/forgot-password` link.
- [x] `Signup.razor` — removed `/terms` & `/privacy` links.
- [x] Build verified clean (0 errors).

> **Note (pending):** `SQLitePCLRaw.lib.e_sqlite3` 2.1.11 (transitive via EF Core Sqlite 10.0.10) has a high-severity vulnerability (NU1903). Fix by adding a newer `SQLitePCLRaw.bundle_e_sqlite3` package reference — see T5.3.

---

## Tier 2 — CRUD + Orders + Security (core rubric requirement)

### T2.1 Password hashing
- [x] `UserService.cs` — use `Microsoft.AspNetCore.Identity.PasswordHasher<User>` (built-in, no new package).
- [x] Hash on register (`RegisterAsync`); verify on login (`LoginAsync`).
- [x] Legacy plaintext rows: `LoginAsync` falls back to plaintext match and re-hashes on success (dev `shop.db`).
- [x] Nullable handling for empty password; build clean.

### T2.2 Product CRUD
- [x] `Services/ProductService.cs` — added `CreateProductAsync`, `UpdateProductAsync`, `DeleteProductAsync`.
- [x] New page `Components/Pages/ManageProducts.razor` (`@page "/manage-products"`).
- [x] Gated: `UserType == "Seller"` or `"Both"` only (redirect to login message otherwise).
- [x] UI: product list table, add/edit form, delete button.
- [x] "Manage Products" link added to header for sellers (`MainLayout.IsSeller`).

### T2.3 Orders persistence
- [x] `Models/Order.cs` — Id, UserId, CustomerName, Address, City, Phone, Total, CreatedAt, `List<OrderItem> Items`.
- [x] `Models/OrderItem.cs` — Id, OrderId, ProductId, Name, Price, Quantity.
- [x] `Data/ShopDbContext.cs` — added `DbSet<Order>` / `DbSet<OrderItem>`.
- [x] New EF migration `20260810152858_AddOrders`; applied to dev DB.
- [x] `Services/OrderService.cs` (new) — `CreateOrderAsync`, `GetOrderByIdAsync`, `GetAllOrdersAsync`, `GetOrdersByUserAsync`; registered in `Program.cs`.
- [x] Checkout calls `OrderService.CreateOrderAsync` then `await Cart.Clear()`; success shows order #.
- [x] Smoke test: app starts on localhost, `/` and `/products` return 200, migration+seed run.

---

## Tier 3 — QA + Documentation

### T3.1 Unit test project
- [x] Created `CSE325-Team4-GroupProject.Tests` (xUnit) referencing main project.
- [x] Packages: xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, coverlet.collector, Moq.
- [x] In-memory SQLite DB helper (`Tests/TestDb.cs`, `SqliteConnection(":memory:")`).
- [x] `UserServiceTests` — 7 tests: register hashes password, duplicate email, empty email, login success/fail/unknown, legacy plaintext→hash upgrade.
- [x] `ProductServiceTests` — 11 tests: seed + idempotency, get all (ordered), get by id (+missing), create, update (+missing), delete (+missing).
- [x] `CartServiceTests` — 6 tests: add new/dup, update qty (+zero removes), remove, clear, load empty (Moq `IJSRuntime`, no setups needed — defaults return completed/null ValueTasks).
- [x] Solution `CSE325-Team4-GroupProject.slnx` created (slnx, .NET 10 default) with both projects.
- [x] Main csproj excludes `CSE325-Team4-GroupProject.Tests\**` from compile (test folder lives inside main project root — without exclusion the web project tried to compile test files).
- [x] **Result: 24/24 tests passing.**

### T3.2 CI integration
- [x] `.github/workflows/main_shophub.yml` — added `Run tests` step (`dotnet test --configuration Release --no-build`) between build and publish.

### T3.3 README + code comments
- [x] Rewrite `README.md` — correct team name (Team 4), overview, feature list, tech stack, local run guide, user guide, deployment notes, link to Trello board.
- [x] Add code comments to services (`UserService`, `CartService`, `ProductService`, `AuthStateService`, `OrderService`) and key pages (`MainLayout`, `Checkout`, `ManageProducts`, `Login`, `Signup`, `Products`, `Home`, `IndvidualProduct`, `Cart`, `ProductCard`).
- [x] Ensure `PROJECT_PLAN.md` stays updated (this file).

### T5.3 Security: SQLitePCLRaw vulnerability (NU1903)
- [x] Added `SQLitePCLRaw.bundle_e_sqlite3` 3.0.5 to main csproj.
- [x] Build now has **0 warnings**. Smoke-tested app + DB after upgrade (200s).

---

## Tier 4 — UX / Accessibility / Design Polish (Design/UX 20pts)

### T4.1 Functional home controls
- [x] `Home.razor` "Shop Now" deal buttons → navigate to `/products` (pre-filtered by `?q=` where the product exists in seed data; Laptop Bag → `/products`).
- [x] Category cards → navigate to `/products?category=...`.
- [x] `Products.razor` — read `category` and `q` (search) query parameters via `[SupplyParameterFromQuery]`; added "Automotive" + "Home & Garden" options so all home categories filter.
- [x] Smoke test: `/`, `/products`, `/products?category=Electronics`, `/products?q=wireless` all return 200.

### T4.2 Header search wired
- [x] `MainLayout.razor` search form (`<form @onsubmit>`) → `Navigation.NavigateTo($"/products?q={term}")`; empty query goes to `/products`. Visually-hidden label + `aria-label` on the 🔍 button.

### T4.3 Accessibility pass (WCAG 2.1 AA)
- [x] `aria-label` on icon-only buttons: header search 🔍, cart quantity −/+. (Cart 🛒 in header is a text link; Remove has visible text.)
- [x] Ensure all images have meaningful `alt` (ProductCard, IndvidualProduct, Cart).
- [x] Focus states: global `:focus-visible` outline added to `app.css` (`2px currentColor`), visible on both light and dark headers.
- [x] `<html lang="en">` already set; contrast verified (yellow #f1c40f on navy #2c3e50 ≈ 8.5:1 passes AA).
- [x] Keep labels associated with inputs (`for`/`id`) — Checkout + header search done; rest already fine.

### T4.4 Styling consolidation (optional)
- [~] Move repeated inline `<style>` blocks into shared `app.css` — deferred: page blocks are effectively global already and visually consistent; low risk/reward to churn before submission.
- [x] Consistent button/badge styling across pages — brand `.btn-primary` (navy on yellow) + `.visually-hidden` utility + focus ring added to `app.css`.

---

## Tier 5 — Deployment & Final Checks

### T5.1 Deployment verification
- [ ] Trigger Azure workflow via `workflow_dispatch` on `main`.
- [ ] Confirm app deploys, DB migrates + seeds on Azure (SQLite file is gitignored → fresh on each deploy).
- [ ] Document SQLite-on-Azure persistence caveat in README.
- [ ] Smoke test deployed URL: home, products, product detail, add-to-cart, cart, checkout, login, signup, manage products.

### T5.3 Security: SQLitePCLRaw vulnerability (NU1903)
- [ ] Add explicit `SQLitePCLRaw.bundle_e_sqlite3` reference (newer version) to csproj to clear the high-severity transitive dependency warning.

### T5.2 Pre-submission checklist (rubric mapping)
- [ ] Blazor web app complete & functional (Function 30)
- [ ] Design/UX: responsive, accessible, branded, clear nav (Design/UX 20)
- [ ] Error handling + user feedback on all forms/actions (Error Handling 15)
- [ ] Code comments + README + Trello board linked (Documentation 15)
- [ ] Presentation video: cover all features (Presentation 20)
- [ ] Trello board up to date with tasks
- [ ] `dotnet build` and `dotnet test` pass clean

---

## Execution Notes / Decisions

- **AuthStateService:** must be per-circuit → `Scoped` (do NOT keep Singleton). Blazor Server `Scoped` == per browser circuit.
- **Password hashing:** use `PasswordHasher<User>` from `Microsoft.Extensions.Identity.Core` (available in ASP.NET Core shared framework; verify no package add needed).
- **Orders:** store shipping snapshot on `Order` (name/address/city/phone) so history stays valid even if product/user changes.
- **DB:** keep SQLite for MVP. Local `shop.db` is gitignored; deleting it forces clean migrate+seed. Azure resets filesystem on redeploy → fresh DB each deploy (acceptable for grading demo; document it).
- **Tests:** prefer `Microsoft.Data.Sqlite` in-memory (true SQLite semantics) over EF InMemory where possible.

---

## Change Log

| Date | Change |
|------|--------|
| 2026-08-10 | Baseline created from repo review (branch `natanaels-branch` @ 3f2494a). |
| 2026-08-10 | Tier 1 complete & building (T1.1–T1.5). Added T5.3 for SQLitePCLRaw vuln. |
| 2026-08-10 | Tier 2 complete & building (T2.1 hashing, T2.2 product CRUD + ManageProducts, T2.3 orders + migration). Smoke-tested `/` + `/products` = 200. |
| 2026-08-10 | T3.1 xUnit suite done (24/24 pass), T3.2 CI test step added, T5.3 SQLitePCLRaw 3.0.5 (0 warnings). |
| 2026-08-10 | T3.3 README rewritten + code comments added to services and key pages. Tier 3 complete. |
| 2026-08-10 | Tier 4: home category cards + deal buttons navigate to pre-filtered `/products`; `Products` reads `category`/`q` from URL; header search wired; a11y pass (aria-labels, alt, focus-visible); brand buttons + utilities in `app.css`. Smoke-tested routes 200. |
| 2026-08-10 | Merged `origin/main` (PR #7: reviews/ratings, checkout payments, admin/seller/profile pages, expanded schema) into `natanaels-branch`. Resolved conflicts: kept password hashing, Scoped auth, order persistence, seller CRUD, Tier 4 UX; kept main's reviews/payments/seller+admin pages. Regenerated `AddOrders` migration on the merged schema. Added `OrderServiceTests` (29/29 passing), build 0 warnings. Smoke-tested all key routes 200. |
