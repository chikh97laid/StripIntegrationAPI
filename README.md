# StripIntegrationAPI

[![.NET](https://img.shields.io/badge/.NET-8.0-blue)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Postgres-PostgreSQL-green)](https://www.postgresql.org/)
[![Stripe](https://img.shields.io/badge/Stripe-Payments-orange)](https://stripe.com/)

**StripIntegrationAPI** — a compact ASP.NET Core 8 Web API that demonstrates Stripe Checkout integration, order & product management, and PostgreSQL persistence. Includes a small static checkout page for quick testing.

---

## Live demo
https://stripintegrationapi.onrender.com/checkout.html

---

## Key features
- Create Stripe Checkout sessions and handle webhooks  
- Orders, OrderItems and Product persistence (Postgres via EF Core)  
- Seeded demo data + simple frontend (checkout.html)  
- Docker-ready for easy deployment

---

## Tech stack
- Backend: ASP.NET Core 8 (C#)  
- ORM: Entity Framework Core (Npgsql / PostgreSQL)  
- Payments: Stripe (Stripe.net)  
- Frontend demo: static HTML/CSS/JS in `wwwroot`  
- Container: Docker

---

## Quick start (local)

1. Clone:
```bash
git clone https://github.com/chikh97laid/StripIntegrationAPI.git
cd StripIntegrationAPI
Add environment variables (or update appsettings.json):

bash
نسخ الكود
# example env names read by .NET configuration
ConnectionStrings__DefaultConnection="Host=HOST;Database=DB;Username=USER;Password=PASS;SSL Mode=Require;Trust Server Certificate=true"
Stripe__SecretKey="sk_test_..."
Stripe__WebhookSecret="whsec_..."
Create / apply migrations (local dev):

bash
نسخ الكود
dotnet ef database update
Run:

bash
نسخ الكود
dotnet run
# open http://localhost:5000/checkout.html  (or the URL shown in the console)
Important: Commit and push your Migrations/ folder to GitHub so your host (Render/Azure) can apply them during deployment.

Docker (build & run)
bash
نسخ الكود
docker build -t stripintegrationapi .
docker run -e ConnectionStrings__DefaultConnection="Host=...;Database=...;Username=...;Password=...;SSL Mode=Require;Trust Server Certificate=true" -e Stripe__SecretKey="sk_test_..." -p 8080:80 stripintegrationapi
# open http://localhost:8080/checkout.html
Environment variables (summary)
ConnectionStrings__DefaultConnection — Postgres connection string (key=value;... format)
Example:

pgsql
نسخ الكود
Host=your-host.render.com;Database=stripintegrationdb;Username=stripuser;Password=secret;SSL Mode=Require;Trust Server Certificate=true
Stripe__SecretKey — Stripe secret key (sk_test_...)

Stripe__WebhookSecret — Stripe webhook signing secret (whsec_...)

Notes
Use Stripe Test keys and Stripe test cards while developing.

If you see relation "Products" does not exist make sure migrations were created locally and pushed to the repository, then re-deploy so the host can apply them.

wwwroot/checkout.html is the demo checkout page — change fetch URLs if using a different host/domain.

Links
Repo: https://github.com/chikh97laid/StripIntegrationAPI

LinkedIn: Your LinkedIn profile here ← replace with your real LinkedIn URL
