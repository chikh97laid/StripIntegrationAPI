# StripIntegrationAPI

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Postgres-PostgreSQL-green)](https://www.postgresql.org/)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-orange)](https://stripe.com/)

**StripIntegrationAPI** — a compact ASP.NET Core 8 Web API demonstrating Stripe Checkout integration, order & product management, and PostgreSQL persistence. Includes a simple static checkout page for testing.

---

## 🌐 Live demo
[Checkout Page](https://stripintegrationapi.onrender.com/checkout.html)

---

## ✨ Key Features
- Stripe Checkout session creation and webhook handling  
- Orders, OrderItems, and Product persistence (Postgres via EF Core)  
- Seeded demo data + simple frontend (`checkout.html`)  
- RESTful API for payments and order management  

---

## 🛠 Tech Stack
- **Backend:** ASP.NET Core 8 (C#)  
- **ORM:** Entity Framework Core (Npgsql/PostgreSQL)  
- **Payments:** Stripe (Stripe.net)  
- **Frontend Demo:** static HTML/CSS/JS (`wwwroot`)  

---

## 💻 Local Development (Run Locally)

### 1️⃣ Clone the repository
``
# 1️⃣ Clone the repository
git clone https://github.com/chikh97laid/StripIntegrationAPI.git
cd StripIntegrationAPI

# 2️⃣ Register on Stripe & Add Environment Variables
# Create a Stripe account to get API keys, then create a .env file or set system environment variables

export ConnectionStrings__DefaultConnection="Host=HOST;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true"
export Stripe__SecretKey="sk_test_..."
export Stripe__WebhookSecret="whsec_..."

# 3️⃣ Apply Database Migrations
dotnet ef database update

# 4️⃣ Run the Project
dotnet run

# Open in browser
# http://localhost:5000/checkout.html

# 🚀 Deployment (Hosting)
# 1. Set environment variables on the host (PostgreSQL connection string, Stripe keys)
# 2. Run database migrations on the host if needed
# 3. Update Stripe webhook URL to match your domain

# Example Postgres connection string for host
export ConnectionStrings__DefaultConnection="Host=your-host.render.com;Database=stripintegrationdb;Username=stripuser;Password=secret;SSL Mode=Require;Trust Server Certificate=true"

# 🔗 Useful Links
# GitHub: https://github.com/chikh97laid
# LinkedIn: https://linkedin.com/in/chikhouladlaid

# 📝 Notes
# - Use Stripe test keys and test cards during development
# - Ensure database migrations exist locally before deploying
# - wwwroot/checkout.html is a demo checkout page — update fetch URLs if using a different host/domainنسخ الكود
