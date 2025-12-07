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
```bash
git clone https://github.com/chikh97laid/StripIntegrationAPI.git
cd StripIntegrationAPI
2️⃣ Register on Stripe & Add environment variables
You must create a Stripe account to get API keys. Then create a .env file or set system environment variables:

env
نسخ الكود
ConnectionStrings__DefaultConnection="Host=HOST;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true"
Stripe__SecretKey="sk_test_..."
Stripe__WebhookSecret="whsec_..."
3️⃣ Apply database migrations
bash
نسخ الكود
dotnet ef database update
4️⃣ Run the project
bash
نسخ الكود
dotnet run
Open in browser:

bash
نسخ الكود
http://localhost:5000/checkout.html
Make sure Migrations/ folder is committed to GitHub if you plan to deploy later.

🚀 Deployment (Hosting)
Set your environment variables on the host (PostgreSQL connection string, Stripe keys)

Run database migrations on the host if needed

Update Stripe webhook URL to match your domain

Example Postgres connection string for host:

env
نسخ الكود
Host=your-host.render.com;Database=stripintegrationdb;Username=stripuser;Password=secret;SSL Mode=Require;Trust Server Certificate=true
🔗 Useful Links
GitHub: https://github.com/chikh97laid

LinkedIn: https://linkedin.com/in/chikhouladlaid

📝 Notes
Use Stripe test keys and test cards during development

Ensure database migrations exist locally before deploying, or you may see relation "Products" does not exist

wwwroot/checkout.html is a demo checkout page — update fetch URLs if using a different host/domain

yaml
نسخ الكود
